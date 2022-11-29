using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Du.ModBus.Interface;
using Du.ModBus.Protocol;
using Du.ModBus.Supp;
using Microsoft.Extensions.Logging;

namespace Du.ModBus;

/// <summary>
/// 모드버스 TCP 서버
/// </summary>
public class ModBusTcpServer : IModBusServer
{
	private readonly ILogger? _lg;

	private readonly ConcurrentDictionary<byte, ModBusDevice> _devices = new();

	private TcpListener? _listener;
	private readonly ConcurrentDictionary<TcpClient, bool> _clients = new();
	private Task _task_conn = Task.CompletedTask;
	private readonly List<Task> _task_client = new();

	private readonly CancellationTokenSource _cts_stop = new();
	private readonly ModBusTcpRequestHandler? _req_handler;

	public ModBusTcpServer(int port = 502, IPAddress? listenAddress = null,
		ILogger? lg = null, ModBusTcpRequestHandler? requestHandler = null)
	{
		ListenAddress = listenAddress ?? IPAddress.Any;

		if (port is < 0 or > 65535)
			throw new ArgumentOutOfRangeException(nameof(port));

		try
		{
			var listener = new TcpListener(ListenAddress, port);
			listener.Start(10);
			Port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
		}
		catch (Exception ex)
		{
			throw new ArgumentException(Cpr.ex_argument, nameof(port), ex);
		}

		_lg = lg;
		_req_handler = requestHandler ?? HandleRequest;

		Initialization = Task.Run(Initialize);
	}

	public event EventHandler<ClientEventArgs>? ClientConnected;
	public event EventHandler<ClientEventArgs>? ClientDisconnected;
	public event EventHandler<ModBusWriteEventArgs>? InputWritten;
	public event EventHandler<ModBusWriteEventArgs>? RegisterWritten;

	public Task Initialization { get; }
	public DateTime StartTime { get; private set; } = DateTime.MinValue;
	public bool IsRunning { get; private set; }
	public IPAddress ListenAddress { get; }
	public int Port { get; }
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(1);
	public List<byte> DeviceIds => _devices.Keys.ToList();

	public ModBusCoil GetCoil(byte deviceId, int coilNumber)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.GetCoil");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			return device.GetCoil(coilNumber);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.GetCoil");
		}
	}

	public void SetCoil(byte deviceId, int coilNumber, bool value)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetCoil(byte, ushort, bool)");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			device.SetCoil(coilNumber, value);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetCoil(byte, ushort, bool)");
		}
	}

	public void SetCoil(byte deviceId, ModBusCoil coil)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetCoil(byte, ModBusObject)");
			ThrowIfDisposed();
			if (coil.Type != ModBusType.Coil)
				throw new ArgumentException(Cpr.ex_invalid_coil_type, nameof(coil));

			SetCoil(deviceId, coil.Address, coil.AsBool);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetCoil(byte, ModBusObject)");
		}
	}

	public ModBusDiscreteInput GetDiscreteInput(byte deviceId, int inputNumber)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.GetDiscreteInput");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			return device.GetInput(inputNumber);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.GetDiscreteInput");
		}
	}

	public void SetDiscreteInput(byte deviceId, int inputNumber, bool value)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetDiscreteInput(byte, ushort, bool)");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			device.SetInput(inputNumber, value);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetDiscreteInput(byte, ushort, bool)");
		}
	}

	public void SetDiscreteInput(byte deviceId, ModBusDiscreteInput discreteInput)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetDiscreteInput(byte, ModBusObject)");
			ThrowIfDisposed();
			if (discreteInput.Type != ModBusType.DiscreteInput)
				throw new ArgumentException(Cpr.ex_invalid_input_type, nameof(discreteInput));

			SetDiscreteInput(deviceId, discreteInput.Address, discreteInput.AsBool);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetDiscreteInput(byte, ModBusObject)");
		}
	}

	public ModBusRegister GetInputRegister(byte deviceId, int registerNumber)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.GetInputRegister");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			return device.GetInputRegister(registerNumber);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.GetInputRegister");
		}
	}

	public void SetInputRegister(byte deviceId, int registerNumber, ushort value)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetInputRegister(byte, ushort, ushort)");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			device.SetInputRegister(registerNumber, value);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetInputRegister(byte, ushort, ushort)");
		}
	}

	public void SetInputRegister(byte deviceId, int registerNumber, byte highByte, byte lowByte)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetInputRegister(byte, ushort, byte, byte)");
			ThrowIfDisposed();
			SetInputRegister(deviceId, new ModBusRegister
			{
				Address = registerNumber,
				ByteHi = highByte,
				ByteLo = lowByte,
				Type = ModBusType.InputRegister
			});
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetInputRegister(byte, ushort, byte, byte)");
		}
	}

	public void SetInputRegister(byte deviceId, ModBusRegister register)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetInputRegister(byte, ModBusObject)");
			ThrowIfDisposed();
			if (register.Type != ModBusType.InputRegister)
				throw new ArgumentException(Cpr.ex_invalid_register_type, nameof(register));

			SetInputRegister(deviceId, register.Address, register.Register);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetInputRegister(byte, ModBusObject)");
		}
	}

	public ModBusRegister GetHoldingRegister(byte deviceId, int registerNumber)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.GetHoldingRegister");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			return device.GetHoldingRegister(registerNumber);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.GetHoldingRegister");
		}
	}

	public void SetHoldingRegister(byte deviceId, int registerNumber, ushort value)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetHoldingRegister(byte, ushort, ushort)");
			ThrowIfDisposed();
			if (!_devices.TryGetValue(deviceId, out var device))
				throw new ArgumentException(Cpr.ex_no_device_exist, nameof(deviceId));

			device.SetHoldingRegister(registerNumber, value);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetHoldingRegister(byte, ushort, ushort)");
		}
	}

	public void SetHoldingRegister(byte deviceId, int registerNumber, byte highByte, byte lowByte)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetHoldingRegister(byte, ushort, byte, byte)");
			ThrowIfDisposed();
			SetHoldingRegister(deviceId, new ModBusRegister
			{
				Address = registerNumber,
				ByteHi = highByte,
				ByteLo = lowByte,
				Type = ModBusType.HoldingRegister
			});
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetHoldingRegister(byte, ushort, byte, byte)");
		}
	}

	public void SetHoldingRegister(byte deviceId, ModBusRegister register)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.SetHoldingRegister(byte, ModBusObject)");
			ThrowIfDisposed();
			if (register.Type != ModBusType.HoldingRegister)
				throw new ArgumentException(Cpr.ex_invalid_register_type, nameof(register));

			SetHoldingRegister(deviceId, register.Address, register.Register);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.SetHoldingRegister(byte, ModBusObject)");
		}
	}

	public bool AddDevice(byte deviceId)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.AddDevice");
			ThrowIfDisposed();
			return _devices.TryAdd(deviceId, new ModBusDevice(deviceId));
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.AddDevice");
		}
	}

	public bool RemoveDevice(byte deviceId)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.RemoveDevice");
			ThrowIfDisposed();
			return _devices.TryRemove(deviceId, out _);
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.RemoveDevice");
		}
	}

	private Task Initialize()
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.Initialize");
			ThrowIfDisposed();

			_listener?.Stop();
			_listener = null;
			_listener = new TcpListener(ListenAddress, Port);

			if (ListenAddress.AddressFamily == AddressFamily.InterNetworkV6)
				_listener.Server.DualMode = true;

			_listener.Start();
			StartTime = DateTime.UtcNow;
			IsRunning = true;

			_task_conn = Task.Run(async () => await WaitForClient());
			_lg?.LogInformation(Cpr.log_tcp_server_stared, ListenAddress, Port);

			return Task.CompletedTask;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.Initialize");
		}
	}

	private async Task WaitForClient()
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.WaitForClient");
			if (_listener == null)
				return;
			while (!_cts_stop.IsCancellationRequested)
			{
				try
				{
					var client = await _listener.AcceptTcpClientAsync();
					if (!_clients.TryAdd(client, true))
						continue;

					var clientTask = Task.Run(async () => await HandleClient(client));
					_task_client.Add(clientTask);
				}
				catch
				{
					// keep things quiet
				}
			}
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.WaitForClient");
		}
	}

	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	private async Task HandleClient(TcpClient client)
	{
		_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpServer.HandleClient");
		var endpoint = (IPEndPoint)client.Client.RemoteEndPoint!;
		try
		{
			ClientConnected?.Invoke(this, new ClientEventArgs(endpoint));
			_lg?.LogInformation(Cpr.log_tcp_client_connected, endpoint.Address);

			var stream = client.GetStream();
			while (!_cts_stop.IsCancellationRequested)
			{
				using var req_mst = new MemoryStream();

				using (var cts = new CancellationTokenSource(Timeout))
				await using (_cts_stop.Token.Register(() => { cts.Cancel(); }))
				{
					try
					{
						var header = await stream.ReadExpectedBytes(6, cts.Token);
						await req_mst.WriteAsync(header, cts.Token);

						var bytes = header.Skip(4).Take(2).ToArray();
						if (BitConverter.IsLittleEndian)
							Array.Reverse(bytes);

						int following = BitConverter.ToUInt16(bytes, 0);
						var payload = await stream.ReadExpectedBytes(following, cts.Token);
						await req_mst.WriteAsync(payload, cts.Token);
					}
					catch (OperationCanceledException) when (cts.IsCancellationRequested)
					{
						continue;
					}
				}

				try
				{
					var req = new Request(req_mst.GetBuffer());
					var rsp = _req_handler?.Invoke(req, _cts_stop.Token);
					if (rsp != null)
					{
						using var cts = new CancellationTokenSource(Timeout);
						await using var reg = _cts_stop.Token.Register(() => cts.Cancel());
						try
						{
							var bytes = rsp.Serialize();
							await stream.WriteAsync(bytes, cts.Token);
						}
						catch (OperationCanceledException) when (cts.IsCancellationRequested)
						{
							//continue;
						}
					}
				}
				catch (ArgumentException ex)
				{
					_lg?.LogWarning(ex, Cpr.log_tcp_recv_invalid_data, endpoint.Address, ex.Message);
				}
				catch (NotImplementedException ex)
				{
					_lg?.LogWarning(ex, Cpr.log_tcp_recv_invalid_data, endpoint.Address, ex.Message);
				}
			}
		}
		catch (IOException)
		{
			// client connection closed
			//return;
		}
		catch (Exception ex)
		{
			_lg?.LogError(ex, Cpr.log_unexcepted_error_on, ex.GetType().Name, ex.GetMessage());
		}
		finally
		{
			ClientDisconnected?.Invoke(this, new ClientEventArgs(endpoint));
			_lg?.LogInformation(Cpr.log_tcp_client_disconnected, endpoint.Address);

			client.Dispose();
			_clients.TryRemove(client, out _);

			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpServer.HandleClient");
		}
	}

	private Response? HandleRequest(Request request, CancellationToken cancellationToken)
	{
		// The device is not known => no response to send.
		if (!_devices.ContainsKey(request.DeviceId))
			return null;

		return request.Function switch
		{
			ModBusFunctionCode.ReadCoils => HandleReadCoils(request),
			ModBusFunctionCode.ReadDiscreteInputs => HandleReadDiscreteInputs(request),
			ModBusFunctionCode.ReadHoldingRegisters => HandleReadHoldingRegisters(request),
			ModBusFunctionCode.ReadInputRegisters => HandleReadInputRegisters(request),
			ModBusFunctionCode.WriteSingleCoil => HandleWriteSingleCoil(request),
			ModBusFunctionCode.WriteSingleRegister => HandleWritSingleRegister(request),
			ModBusFunctionCode.WriteMultipleCoils => HandleWriteMultipleCoils(request),
			ModBusFunctionCode.WriteMultipleRegisters => HandleWriteMultipleRegisters(request),
			ModBusFunctionCode.EncapsulatedInterface => HandleEncapsulatedInterface(request),
			_ => new Response(request)
			{
				ErrorCode = ModBusErrorCode.IllegalFunction
			}
		};
	}

	private Response? HandleReadCoils(Request req)
	{
		var rsp = new Response(req);

		try
		{
			if (req.Count is < ModBusObject.MinCount or > ModBusObject.MaxCoilCountRead)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address < ModBusObject.MinAddress ||
					 req.Address + req.Count > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					var len = (int)Math.Ceiling(req.Count / 8.0);
					rsp.Data = new DataBuffer(len);
					for (var i = 0; i < req.Count; i++)
					{
						var addr = (ushort)(req.Address + i);
						if (!GetCoil(req.DeviceId, addr).AsBool) 
							continue;

						var posByte = i / 8;
						var posBit = i % 8;

						var mask = (byte)Math.Pow(2, posBit);
						rsp.Data[posByte] = (byte)(rsp.Data[posByte] | mask);
					}
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}
		}
		catch
		{
			return null;
		}

		return rsp;
	}

	private Response? HandleReadDiscreteInputs(Request req)
	{
		var rsp = new Response(req);
		try
		{
			if (req.Count is < ModBusObject.MinCount or > ModBusObject.MaxCoilCountRead)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address < ModBusObject.MinAddress ||
					 req.Address + req.Count > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					var len = (int)Math.Ceiling(req.Count / 8.0);
					rsp.Data = new DataBuffer(len);
					for (var i = 0; i < req.Count; i++)
					{
						var addr = (ushort)(req.Address + i);
						if (!GetDiscreteInput(req.DeviceId, addr).AsBool) 
							continue;

						var posByte = i / 8;
						var posBit = i % 8;

						var mask = (byte)Math.Pow(2, posBit);
						rsp.Data[posByte] = (byte)(rsp.Data[posByte] | mask);
					}
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}
		}
		catch
		{
			return null;
		}

		return rsp;
	}

	private Response? HandleReadHoldingRegisters(Request req)
	{
		var rsp = new Response(req);
		try
		{
			if (req.Count is < ModBusObject.MinCount or > ModBusObject.MaxRegisterCountRead)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address < ModBusObject.MinAddress ||
					 req.Address + req.Count > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					rsp.Data = new DataBuffer(req.Count * 2);
					for (var i = 0; i < req.Count; i++)
					{
						var addr = (ushort)(req.Address + i);
						var reg = GetHoldingRegister(req.DeviceId, addr);
						rsp.Data.SetUInt16(i * 2, reg.Register);
					}
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}
		}
		catch
		{
			return null;
		}

		return rsp;
	}

	private Response? HandleReadInputRegisters(Request req)
	{
		var rsp = new Response(req);

		try
		{
			if (req.Count is < ModBusObject.MinCount or > ModBusObject.MaxRegisterCountRead)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address < ModBusObject.MinAddress ||
					 req.Address + req.Count > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					rsp.Data = new DataBuffer(req.Count * 2);
					for (var i = 0; i < req.Count; i++)
					{
						var addr = (ushort)(req.Address + i);
						var reg = GetInputRegister(req.DeviceId, addr);
						rsp.Data.SetUInt16(i * 2, reg.Register);
					}
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}
		}
		catch
		{
			return null;
		}

		return rsp;
	}

	private Response? HandleEncapsulatedInterface(Request req)
	{
		var rsp = new Response(req);
		if (req.Mei != ModBusMei.ReadDeviceInformation)
		{
			rsp.ErrorCode = ModBusErrorCode.IllegalFunction;
			return rsp;
		}

		if (((byte)req.MeiObject > 0x06 && (byte)req.MeiObject < 0x80))
		{
			rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			return rsp;
		}

		if (req.MeiCategory < ModBusDevIdCategory.Basic || req.MeiCategory > ModBusDevIdCategory.Individual)
		{
			rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			return rsp;
		}

		var attribute = Assembly.GetAssembly(GetType()) // typeof(ModBusTcpServer)
			?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		var version = attribute != null ? attribute.InformationalVersion : Cpr.unkversion;

		rsp.Mei = req.Mei;
		rsp.MeiCategory = req.MeiCategory;

		var dict = new Dictionary<ModBusDevIdObject, string>();
		switch (req.MeiCategory)
		{
			case ModBusDevIdCategory.Individual:
				switch (req.MeiObject)
				{
					case ModBusDevIdObject.VendorName:
						rsp.ConformityLevel = 0x81;
						dict.Add(ModBusDevIdObject.VendorName, "DULIB");
						break;
					case ModBusDevIdObject.ProductCode:
						rsp.ConformityLevel = 0x81;
						dict.Add(ModBusDevIdObject.ProductCode, "DULIB.MODBUS-TCP");
						break;
					case ModBusDevIdObject.MajorMinorRevision:
						rsp.ConformityLevel = 0x81;
						dict.Add(ModBusDevIdObject.MajorMinorRevision, version);
						break;
					case ModBusDevIdObject.VendorUrl:
						rsp.ConformityLevel = 0x82;
						dict.Add(ModBusDevIdObject.VendorUrl, "https://github.com/kshman/DuLib");
						break;
					case ModBusDevIdObject.ProductName:
						rsp.ConformityLevel = 0x82;
						dict.Add(ModBusDevIdObject.ProductName, "DuLib.ModBus");
						break;
					case ModBusDevIdObject.ModelName:
						rsp.ConformityLevel = 0x82;
						dict.Add(ModBusDevIdObject.ModelName, "TCP Server");
						break;
					case ModBusDevIdObject.UserApplicationName:
						rsp.ConformityLevel = 0x82;
						dict.Add(ModBusDevIdObject.UserApplicationName, "ModBus TCP Server");
						break;
					default:
						rsp.ConformityLevel = 0x83;
						dict.Add(req.MeiObject, "Custom Data for " + req.MeiObject);
						break;
				}
				break;

			case ModBusDevIdCategory.Extended:
				rsp.ConformityLevel = 0x03;
				dict.Add(ModBusDevIdObject.VendorName, "DULIB");
				dict.Add(ModBusDevIdObject.ProductCode, "DULIB.MODBUS-TCP");
				dict.Add(ModBusDevIdObject.MajorMinorRevision, version);
				dict.Add(ModBusDevIdObject.VendorUrl, "https://github.com/kshman/DuLib");
				dict.Add(ModBusDevIdObject.ProductName, "DuLib.ModBus");
				dict.Add(ModBusDevIdObject.ModelName, "TCP Server");
				dict.Add(ModBusDevIdObject.UserApplicationName, "ModBus TCP Server");
				break;

			case ModBusDevIdCategory.Regular:
				rsp.ConformityLevel = 0x02;
				dict.Add(ModBusDevIdObject.VendorName, "DULIB");
				dict.Add(ModBusDevIdObject.ProductCode, "DULIB.MODBUS-TCP");
				dict.Add(ModBusDevIdObject.MajorMinorRevision, version);
				dict.Add(ModBusDevIdObject.VendorUrl, "https://github.com/kshman/DuLib");
				dict.Add(ModBusDevIdObject.ProductName, "DuLib.ModBus");
				dict.Add(ModBusDevIdObject.ModelName, "TCP Server");
				dict.Add(ModBusDevIdObject.UserApplicationName, "ModBus TCP Server");
				break;

			case ModBusDevIdCategory.Basic:
				rsp.ConformityLevel = 0x01;
				dict.Add(ModBusDevIdObject.VendorName, "DULIB");
				dict.Add(ModBusDevIdObject.ProductCode, "DULIB.MODBUS-TCP");
				dict.Add(ModBusDevIdObject.MajorMinorRevision, version);
				break;

			default:
				return null;
		}

		rsp.MoreRequestsNeeded = false;
		rsp.NextObjectId = 0x00;
		rsp.ObjectCount = (byte)dict.Count;
		rsp.Data = new DataBuffer();

		foreach (var kvp in dict)
		{
			var bytes = Encoding.UTF8.GetBytes(kvp.Value);

			rsp.Data.AddByte((byte)kvp.Key);
			rsp.Data.AddByte((byte)bytes.Length);
			rsp.Data.AddBytes(bytes);
		}

		return rsp;
	}

	private Response? HandleWriteSingleCoil(Request req)
	{
		var rsp = new Response(req);

		try
		{
			if (req.Data == null)
				return null;

			var val = req.Data.GetUInt16(0);

			if (val != 0x0000 && val != 0xFF00)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address is < ModBusObject.MinAddress or > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					var coil = new ModBusCoil
					{
						Address = req.Address,
						AsBool = (val > 0)
					};

					SetCoil(req.DeviceId, coil);
					rsp.Data = req.Data;

					InputWritten?.Invoke(this, new ModBusWriteEventArgs(req.DeviceId, coil));
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}
		}
		catch
		{
			return null;
		}

		return rsp;
	}

	private Response? HandleWritSingleRegister(Request req)
	{
		var rsp = new Response(req);

		try
		{
			if (req.Data == null)
				return null;

			var val = req.Data.GetUInt16(0);

			if (req.Address is < ModBusObject.MinAddress or > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					var register = new ModBusRegister
					{
						Address = req.Address,
						Register = val,
						Type = ModBusType.HoldingRegister
					};

					SetHoldingRegister(req.DeviceId, register);
					rsp.Data = req.Data;

					RegisterWritten?.Invoke(this, new ModBusWriteEventArgs(req.DeviceId, register));
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}
		}
		catch
		{
			return null;
		}

		return rsp;
	}

	private Response? HandleWriteMultipleCoils(Request req)
	{
		try
		{
			var rsp = new Response(req);

			if (req.Data == null)
				return null;

			var numBytes = (int)Math.Ceiling(req.Count / 8.0);

			if (req.Count is < ModBusObject.MinCount or > ModBusObject.MaxCoilCountWrite || 
			    numBytes != req.Data.Length)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address < ModBusObject.MinAddress || 
			         req.Address + req.Count > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					var list = new List<ModBusCoil>();
					for (var i = 0; i < req.Count; i++)
					{
						var addr = (ushort)(req.Address + i);

						var posByte = i / 8;
						var posBit = i % 8;

						var mask = (byte)Math.Pow(2, posBit);
						var val = req.Data[posByte] & mask;

						var coil = new ModBusCoil
						{
							Address = addr,
							AsBool = (val > 0)
						};
						SetCoil(req.DeviceId, coil);
						list.Add(coil);
					}
					InputWritten?.Invoke(this, new ModBusWriteEventArgs(req.DeviceId, list));
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}

			return rsp;
		}
		catch
		{
			return null;
		}
	}

	private Response? HandleWriteMultipleRegisters(Request req)
	{
		try
		{
			var rsp = new Response(req);

			if (req.Data == null)
				return null;

			//req.Data contains [byte count] [data]..[data]
			if (req.Count is < ModBusObject.MinCount or > ModBusObject.MaxRegisterCountWrite ||
			    req.Count * 2 != req.Data.Length - 1)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataValue;
			else if (req.Address < ModBusObject.MinAddress || 
			         req.Address + req.Count > ModBusObject.MaxAddress)
				rsp.ErrorCode = ModBusErrorCode.IllegalDataAddress;
			else
			{
				try
				{
					var list = new List<ModBusRegister>();
					for (var i = 0; i < req.Count; i++)
					{
						var addr = (ushort)(req.Address + i);
						var val = req.Data.GetUInt16(i * 2 + 1);

						var register = new ModBusRegister()
						{
							Address = addr,
							Register = val,
							Type = ModBusType.HoldingRegister
						};
						SetHoldingRegister(req.DeviceId, register);
						list.Add(register);
					}
					RegisterWritten?.Invoke(this, new ModBusWriteEventArgs(req.DeviceId, list));
				}
				catch
				{
					rsp.ErrorCode = ModBusErrorCode.SlaveDeviceFailure;
				}
			}

			return rsp;
		}
		catch
		{
			return null;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected bool _disposed;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing)
			return;

		if (_disposed)
			return;

		_disposed = true;
		_cts_stop.Cancel();

		_listener?.Stop();
		foreach (var client in _clients.Keys)
			client.Dispose();

		Task.WaitAll(_task_client.ToArray());
		_task_conn.Wait();

		_clients.Clear();

		IsRunning = false;
	}

	protected void ThrowIfDisposed()
	{
		if (_disposed)
			throw new ObjectDisposedException(GetType().FullName);
	}
}

/// <summary>
/// 모드버스 리퀘스트 핸들러
/// </summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate Response? ModBusTcpRequestHandler(Request request, CancellationToken cancellationToken);

/// <summary>
/// 클라이언트 이벤트
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ClientEventArgs : EventArgs
{
	public IPEndPoint EndPoint { get; }

	public ClientEventArgs(IPEndPoint endpoint)
	{
		EndPoint = endpoint;
	}
}
