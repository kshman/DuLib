using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Du.ModBus.Interface;
using Du.ModBus.Protocol;
using Du.ModBus.Supp;

namespace Du.ModBus;

/// <summary>
/// 모드버스 TCP 클라이언트
/// </summary>
public class ModBusTcpClient : IModBusClient
{
	private readonly ILogger? _lg;

	private readonly object _lock_recon = new();

	private readonly object _lock_trx_id = new();
	private readonly SemaphoreSlim _lock_send = new(1, 1);
	private readonly SemaphoreSlim _lock_queue = new(1, 1);
	private readonly List<QueuedRequest> _awaiting_responses = new();

	private TimeSpan _keep_alive = TimeSpan.FromSeconds(30);

	private CancellationTokenSource _cts_stop = new();
	private CancellationTokenSource _cts_recv = new();
	private TaskCompletionSource<bool>? _tcs_recon;
	private Task _task_recv = Task.CompletedTask;

	private TcpClient? _tcp;
	private NetworkStream? _nst;

	private bool _is_work;
	private bool _is_recon;
	private bool _was_conn;

	private ushort _transaction_id;

	public ModBusTcpClient(string host, int port = 502, ILogger? logger = null)
	{
		if (string.IsNullOrWhiteSpace(host))
			throw new ArgumentNullException(nameof(host));

		if (port is < 1 or > ushort.MaxValue)
			throw new ArgumentOutOfRangeException(nameof(port));

		_lg = logger;
		Host = host;
		Port = port;
	}

	public ModBusTcpClient(string host, ILogger logger)
		: this(host, 502, logger) { }

	public ModBusTcpClient(IPAddress address, int port = 502, ILogger? logger = null)
		: this(address.ToString(), port, logger) { }

	public ModBusTcpClient(IPAddress address, ILogger logger)
		: this(address, 502, logger) { }

	public event EventHandler? Connected;
	public event EventHandler? Disconnected;

	public string Host { get; }
	public int Port { get; }
	public Task ConnectingTask { get; private set; } = Task.CompletedTask;
	public bool IsConnected { get; private set; }
	public TimeSpan ReconnectTimeSpan { get; set; } = TimeSpan.MaxValue;
	public TimeSpan MaxConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
	public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(1);
	public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(1);
	public TimeSpan KeepAliveTimeSpan
	{
		get => _keep_alive;
		set
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				_lg?.LogWarning(Cpr.log_windows_only);

			_keep_alive = value.TotalMilliseconds < 0 ? TimeSpan.Zero : value;
			InternalSetKeepAlive();
		}
	}

	public bool EnableTransactionId { get; set; }

	public async Task Open(CancellationToken cancellationToken = default)
	{
		var cancelTask = ReconnectTimeSpan == TimeSpan.MaxValue ?
			Task.Delay(Timeout.Infinite, cancellationToken) :
			Task.Delay(ReconnectTimeSpan, cancellationToken);

		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.Open");
			InternalCheckDisposed();

			if (_is_work)
			{
				await Task.WhenAny(ConnectingTask, cancelTask);
				return;
			}
			_is_work = true;
			_cts_stop = new CancellationTokenSource();

			_lg?.LogInformation(Cpr.log_now_starting, Cpr.modbus_client);

			lock (_lock_recon)
			{
				_transaction_id = 0;
				IsConnected = false;
				_was_conn = false;
				ConnectingTask = InternalGetReconnectTask(true);
			}

			_lg?.LogInformation(Cpr.log_started, Cpr.modbus_client);
			await Task.WhenAny(ConnectingTask, cancelTask);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{ }
		finally
		{
			if (cancelTask.Status != TaskStatus.WaitingForActivation)
				cancelTask.Dispose();
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.Open");
		}
	}

	public async Task Close(CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.Close");
			InternalCheckDisposed();

			if (!_is_work)
				return;

			_lg?.LogInformation(Cpr.log_now_stopping, Cpr.modbus_client);

			_cts_stop.Cancel();
			_cts_recv.Cancel();

			bool connected;
			lock (_lock_recon)
			{
				connected = IsConnected;
				IsConnected = false;
				_was_conn = false;
			}

			await Task.WhenAny(ConnectingTask, Task.Delay(Timeout.Infinite, cancellationToken));

			_nst?.DisposeAsync().ValueTaskAwait();
			_tcp?.Dispose();

			_is_work = false;
			_lg?.LogInformation(Cpr.log_stopped, Cpr.modbus_client);

			if (connected)
				Task.Run(() => Disconnected?.Invoke(this, EventArgs.Empty), cancellationToken).TaskAwait();
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{ }
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.Close");
		}
	}

	public async Task<List<ModBusCoil>?> ReadCoils(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.ReadCoils");

			if (startAddress < ModBusObject.MinAddress || ModBusObject.MaxAddress < startAddress + count)
				throw new ArgumentOutOfRangeException(nameof(startAddress));
			if (count is < ModBusObject.MinCount or > ModBusObject.MaxCoilCountRead)
				throw new ArgumentOutOfRangeException(nameof(count));

			_lg?.LogDebug(Cpr.log_dev_rd_coil_cnt, deviceId, startAddress, count);

			try
			{
				Request request = new()
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.ReadCoils,
					Address = startAddress,
					Count = count
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				List<ModBusCoil> list = new();
				for (var i = 0; i < count; i++)
				{
					var posByte = i / 8;
					var posBit = i % 8;

					var val = response.Data[posByte] & (byte)Math.Pow(2, posBit);

					list.Add(new ModBusCoil
					{
						Address = startAddress + i,
						AsBool = val > 0
					});
				}
				return list;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_coil, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_coil, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return null;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.ReadCoils");
		}
	}

	public async Task<List<ModBusDiscreteInput>?> ReadDiscreteInputs(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.ReadDiscreteInputs");

			if (startAddress < ModBusObject.MinAddress || ModBusObject.MaxAddress < startAddress + count)
				throw new ArgumentOutOfRangeException(nameof(startAddress));
			if (count is < ModBusObject.MinCount or > ModBusObject.MaxCoilCountRead)
				throw new ArgumentOutOfRangeException(nameof(count));

			_lg?.LogDebug(Cpr.log_dev_rd_discrete_input_cnt, deviceId, startAddress, count);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.ReadDiscreteInputs,
					Address = startAddress,
					Count = count
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				List<ModBusDiscreteInput> list = new();
				for (var i = 0; i < count; i++)
				{
					var posByte = i / 8;
					var posBit = i % 8;

					var val = response.Data[posByte] & (byte)Math.Pow(2, posBit);

					list.Add(new ModBusDiscreteInput
					{
						Address = startAddress + i,
						AsBool = val > 0
					});
				}
				return list;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_discrete_input, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_discrete_input, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return null;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.ReadDiscreteInputs");
		}
	}

	public async Task<List<ModBusRegister>?> ReadHoldingRegisters(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.ReadHoldingRegisters");

			if (startAddress < ModBusObject.MinAddress || ModBusObject.MaxAddress < startAddress + count)
				throw new ArgumentOutOfRangeException(nameof(startAddress));
			if (count is < ModBusObject.MinCount or > ModBusObject.MaxRegisterCountRead)
				throw new ArgumentOutOfRangeException(nameof(count));

			_lg?.LogDebug(Cpr.log_dev_rd_hold_regi_cnt, deviceId, startAddress, count);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.ReadHoldingRegisters,
					Address = startAddress,
					Count = count
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				List<ModBusRegister> list = new();
				for (var i = 0; i < count; i++)
				{
					list.Add(new ModBusRegister
					{
						Address = startAddress + i,
						ByteHi = response.Data[i * 2],
						ByteLo = response.Data[i * 2 + 1]
					});
				}
				return list;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_hold_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_hold_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return null;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.ReadHoldingRegisters");
		}
	}

	public async Task<List<ModBusRegister>?> ReadInputRegisters(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.ReadInputRegisters");

			if (startAddress < ModBusObject.MinAddress || ModBusObject.MaxAddress < startAddress + count)
				throw new ArgumentOutOfRangeException(nameof(startAddress));
			if (count is < ModBusObject.MinCount or > ModBusObject.MaxRegisterCountRead)
				throw new ArgumentOutOfRangeException(nameof(count));

			_lg?.LogDebug(Cpr.log_dev_rd_in_regi_cnt, deviceId, startAddress, count);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.ReadInputRegisters,
					Address = startAddress,
					Count = count
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				List<ModBusRegister> list = new();
				for (var i = 0; i < count; i++)
				{
					list.Add(new ModBusRegister
					{
						Address = startAddress + i,
						ByteHi = response.Data[i * 2],
						ByteLo = response.Data[i * 2 + 1]
					});
				}
				return list;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_in_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_in_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return null;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.ReadInputRegisters");
		}
	}

	public async Task<Dictionary<ModBusDevIdObject, string>?> ReadDeviceInformation(byte deviceId, ModBusDevIdCategory categoryId, ModBusDevIdObject objectId = ModBusDevIdObject.VendorName, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.ReadDeviceInformation");

			var raw = await ReadDeviceInformationRaw(deviceId, categoryId, objectId, cancellationToken);

			return raw?.ToDictionary(
				kvp => (ModBusDevIdObject)kvp.Key,
				kvp => Encoding.ASCII.GetString(kvp.Value));
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.ReadDeviceInformation");
		}
	}

	public async Task<Dictionary<byte, byte[]>?> ReadDeviceInformationRaw(byte deviceId, ModBusDevIdCategory categoryId, ModBusDevIdObject objectId = ModBusDevIdObject.VendorName, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.ReadDeviceInformationRaw");
			_lg?.LogDebug(Cpr.log_dev_rd_devinfo_raw, deviceId, categoryId, objectId);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.EncapsulatedInterface,
					Mei = ModBusMei.ReadDeviceInformation,
					MeiCategory = categoryId,
					MeiObject = objectId
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				var dict = new Dictionary<byte, byte[]>();
				for (int i = 0, idx = 0; i < response.ObjectCount && idx < response.Data.Length; i++)
				{
					var objId = response.Data.GetByte(idx);
					idx++;
					var len = response.Data.GetByte(idx);
					idx++;
					var bytes = response.Data.GetBytes(idx, len);
					idx += len;

					dict.Add(objId, bytes);
				}

				if (response.MoreRequestsNeeded)
				{
					var transDict = await ReadDeviceInformationRaw(deviceId, categoryId, (ModBusDevIdObject)response.NextObjectId, cancellationToken);
					if (transDict != null)
					{
						foreach (var kvp in transDict)
							dict.Add(kvp.Key, kvp.Value);
					}
				}

				return dict;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_devinfo_raw, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_read_devinfo_raw, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return null;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.ReadDeviceInformationRaw");
		}
	}

	public async Task<bool> WriteSingleCoil(byte deviceId, ModBusObject coil, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.WriteSingleCoil");

			if (coil.Type != ModBusType.Coil)
				throw new ArgumentException(Cpr.ex_invalid_coil_type);
			if (coil.Address is < ModBusObject.MinAddress or > ModBusObject.MaxAddress)
				throw new ArgumentOutOfRangeException(nameof(coil), nameof(coil.Address));

			_lg?.LogDebug(Cpr.log_dev_wr_coil, deviceId, coil);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.WriteSingleCoil,
					Address = coil.Address,
					Data = new DataBuffer(2)
				};
				var value = (ushort)(coil.AsBool ? 0xFF00 : 0x0000);
				request.Data.SetUInt16(0, value);
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				return request.TransactionId == response.TransactionId &&
					request.DeviceId == response.DeviceId &&
					request.Function == response.Function &&
					request.Address == response.Address &&
					request.Data.Equals(response.Data);
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_coil, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_coil, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return false;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.WriteSingleCoil");
		}
	}

	public async Task<bool> WriteSingleRegister(byte deviceId, ModBusObject register, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.WriteSingleRegister");

			if (register.Type != ModBusType.HoldingRegister)
				throw new ArgumentException(Cpr.ex_invalid_register_type);
			if (register.Address is < ModBusObject.MinAddress or > ModBusObject.MaxAddress)
				throw new ArgumentOutOfRangeException(nameof(register), nameof(register.Address));

			_lg?.LogDebug(Cpr.log_dev_wr_regi, deviceId, register);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.WriteSingleRegister,
					Address = register.Address,
					Data = new DataBuffer(new[] { register.ByteHi, register.ByteLo })
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				return request.TransactionId == response.TransactionId &&
					request.DeviceId == response.DeviceId &&
					request.Function == response.Function &&
					request.Address == response.Address &&
					request.Data.Equals(response.Data);
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return false;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.WriteSingleRegister");
		}
	}

	public async Task<bool> WriteCoils(byte deviceId, IEnumerable<ModBusObject> coilsArray, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.WriteCoils");

			var coils = coilsArray as ModBusObject[] ?? coilsArray.ToArray();

			if (coils.Any() != true)
				throw new ArgumentNullException(nameof(coilsArray));
			if (coils.Any(c => c.Type != ModBusType.Coil))
				throw new ArgumentException(Cpr.ex_invalid_coil_type);

			var orderedList = coils.OrderBy(c => c.Address).ToList();
			if (orderedList.Count is < ModBusObject.MinCount or > ModBusObject.MaxCoilCountWrite)
				throw new IndexOutOfRangeException(Cpr.count);

			var firstAddress = orderedList.First().Address;
			var lastAddress = orderedList.Last().Address;

			if (firstAddress + orderedList.Count - 1 != lastAddress)
				throw new ArgumentException(Cpr.ex_no_address_gap_within_request);
			if (firstAddress < ModBusObject.MinAddress || ModBusObject.MaxAddress < lastAddress)
				throw new IndexOutOfRangeException(Cpr.address);

			_lg?.LogDebug(Cpr.log_dev_wr_coil_cnt, deviceId, firstAddress, orderedList.Count);

			var numBytes = (int)Math.Ceiling(orderedList.Count / 8.0);
			var coilBytes = new byte[numBytes];
			for (var i = 0; i < orderedList.Count; i++)
			{
				if (!orderedList[i].AsBool)
					continue;

				var posByte = i / 8;
				var posBit = i % 8;
				var mask = (byte)Math.Pow(2, posBit);
				coilBytes[posByte] = (byte)(coilBytes[posByte] | mask);
			}

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.WriteMultipleCoils,
					Address = firstAddress,
					Count = (ushort)orderedList.Count,
					Data = new DataBuffer(coilBytes)
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}

				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				return request.TransactionId == response.TransactionId &&
					request.Address == response.Address &&
					request.Count == response.Count;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_coil, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_coil, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return false;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.WriteCoils");
		}
	}

	public async Task<bool> WriteRegisters(byte deviceId, IEnumerable<ModBusObject> registersArray, CancellationToken cancellationToken = default)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.WriteRegisters");

			var registers = registersArray as ModBusObject[] ?? registersArray.ToArray();

			if (registers.Any() != true)
				throw new ArgumentNullException(nameof(registersArray));
			if (registers.Any(r => r.Type != ModBusType.HoldingRegister))
				throw new ArgumentException(Cpr.ex_invalid_register_type);

			var orderedList = registers.OrderBy(c => c.Address).ToList();
			if (orderedList.Count is < ModBusObject.MinCount or > ModBusObject.MaxRegisterCountWrite)
				throw new ArgumentOutOfRangeException(nameof(registersArray), Cpr.count);

			var firstAddress = orderedList.First().Address;
			var lastAddress = orderedList.Last().Address;

			if (firstAddress + orderedList.Count - 1 != lastAddress)
				throw new ArgumentException(Cpr.ex_no_address_gap_within_request);
			if (firstAddress < ModBusObject.MinAddress || ModBusObject.MaxAddress < lastAddress)
				throw new IndexOutOfRangeException(Cpr.address);

			_lg?.LogDebug(Cpr.log_dev_wr_regi_cnt, deviceId, firstAddress, orderedList.Count);

			var data = new DataBuffer(orderedList.Count * 2);
			for (var i = 0; i < orderedList.Count; i++)
				data.SetUInt16(i * 2, orderedList[i].Register);

			try
			{
				var request = new Request
				{
					DeviceId = deviceId,
					Function = ModBusFunctionCode.WriteMultipleRegisters,
					Address = firstAddress,
					Count = (ushort)orderedList.Count,
					Data = data
				};
				var response = await InternalSendRequest(request, cancellationToken);
				if (response.IsTimeout)
					throw new SocketException((int)SocketError.TimedOut);

				if (response.IsError)
				{
					throw new ModBusException(response.ErrorMessage)
					{
						ErrorCode = response.ErrorCode
					};
				}
				if (request.TransactionId != response.TransactionId)
					throw new ModBusException(Cpr.ex_not_match_with + nameof(response.TransactionId));

				return request.TransactionId == response.TransactionId &&
					request.Address == response.Address &&
					request.Count == response.Count;
			}
			catch (SocketException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}
			catch (IOException ex)
			{
				_lg?.LogWarning(ex, Cpr.log_conn_task_ex, Cpr.task_write_regi, ex.GetMessage());
				if (!_is_recon)
					ConnectingTask = InternalGetReconnectTask();
			}

			return false;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.WriteRegisters");
		}
	}

	private async Task InternalReconnect()
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.InternalReconnect");
			lock (_lock_recon)
			{
				if (_is_recon || _cts_stop.IsCancellationRequested)
					return;

				_is_recon = true;
				IsConnected = false;
			}

			_lg?.LogInformation(Cpr.log_starting_conn, Cpr.GetConnDesc(_was_conn));
			if (_was_conn)
			{
				_cts_recv.Cancel();
				_task_recv.TaskAwait();
				_cts_recv = new CancellationTokenSource();
				_task_recv = Task.CompletedTask;
				Task.Run(() => Disconnected?.Invoke(this, EventArgs.Empty)).TaskAwait();
			}

			var timeout = TimeSpan.FromSeconds(2);
			var startTime = DateTime.UtcNow;

			var address = StaticResolveHost(Host);
			while (!_cts_stop.IsCancellationRequested)
			{
				try
				{
					_nst?.DisposeAsync().ValueTaskAwait();
					_nst = null;

					_tcp?.Dispose();
					_tcp = new TcpClient(address.AddressFamily);

					var connectTask = _tcp.ConnectAsync(address, Port);
					if (await Task.WhenAny(connectTask, Task.Delay(timeout, _cts_stop.Token)) == connectTask && _tcp.Connected)
					{
						InternalSetKeepAlive();
						_nst = _tcp.GetStream();

						_cts_recv = new CancellationTokenSource();
						_task_recv = Task.Run(async () => await InternalReceiveLoop());

						lock (_lock_recon)
						{
							IsConnected = true;
							_was_conn = true;

							_tcs_recon?.TrySetResult(true);
							Task.Run(() => Connected?.Invoke(this, EventArgs.Empty)).TaskAwait();
						}

						_lg?.LogInformation(Cpr.log_conn_success_with, Cpr.GetConnDesc(_was_conn));
						return;
					}
					else
					{
						if (timeout < MaxConnectTimeout)
						{
							_lg?.LogWarning(Cpr.log_conn_timeout,
								Cpr.GetConnDesc(_was_conn), timeout);
							timeout = timeout.Add(TimeSpan.FromSeconds(2));
							if (timeout > MaxConnectTimeout)
								timeout = MaxConnectTimeout;
						}
						throw new SocketException((int)SocketError.TimedOut);
					}
				}
				catch (SocketException) when (ReconnectTimeSpan == TimeSpan.MaxValue || DateTime.UtcNow - startTime <= ReconnectTimeSpan)
				{
					await Task.Delay(1000, _cts_stop.Token);
				}
				catch (OperationCanceledException) when (_cts_stop.IsCancellationRequested)
				{
					throw;
				}
				catch (Exception ex)
				{
					_lg?.LogError(ex, Cpr.log_conn_try_again,
						Cpr.GetConnDesc(_was_conn), ex.GetMessage());
				}
			}
		}
		catch (OperationCanceledException) when (_cts_stop.IsCancellationRequested)
		{
			_tcs_recon?.TrySetCanceled();
		}
		catch (Exception ex)
		{
			_lg?.LogError(ex, Cpr.log_conn_fail_ex, Cpr.GetConnDesc(_was_conn), ex.GetMessage());
			_tcs_recon?.TrySetException(ex);
		}
		finally
		{
			lock (_lock_recon)
			{
				_is_recon = false;
				_tcs_recon = null;
			}
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.InternalReconnect");
		}
	}

	private async Task InternalReceiveLoop()
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.InternalReceiveLoop");
			_lg?.LogInformation(Cpr.log_start_recv);

			while (!_cts_recv.IsCancellationRequested)
			{
				try
				{
					if (_nst == null)
					{
						await Task.Delay(200, _cts_recv.Token);
						continue;
					}

					while (!_cts_recv.IsCancellationRequested)
					{
						using var rst = new MemoryStream();

						using (var timeCts = new CancellationTokenSource(ReceiveTimeout))
						using (var cts = CancellationTokenSource.CreateLinkedTokenSource(timeCts.Token, _cts_recv.Token))
						{
							try
							{
								var hdr = await _nst.ReadExpectedBytes(6, cts.Token);
								await rst.WriteAsync(hdr, 0, hdr.Length, cts.Token);

								var bs = hdr.Skip(4).Take(2).ToArray();
								if (BitConverter.IsLittleEndian)
									Array.Reverse(bs);

								var following = BitConverter.ToUInt16(bs, 0);
								var payload = await _nst.ReadExpectedBytes(following, cts.Token);
								await rst.WriteAsync(payload, 0, payload.Length, cts.Token);
							}
							catch (OperationCanceledException) when (timeCts.IsCancellationRequested)
							{
								continue;
							}
						}

						try
						{
							var response = new Response(rst.GetBuffer());

							QueuedRequest? qreq;
							await _lock_queue.WaitAsync(_cts_recv.Token);
							try
							{
								if (!EnableTransactionId)
								{
									qreq = _awaiting_responses.FirstOrDefault();
								}
								else
								{
									qreq = _awaiting_responses
										.FirstOrDefault(i =>
											i.TransactionId == response.TransactionId);
									if (qreq == null)
										_lg?.LogWarning(Cpr.log_tr_no_match, response.TransactionId);
								}

								if (qreq != null)
								{
									qreq.Registration.DisposeAsync().ValueTaskAwait();
									_awaiting_responses.Remove(qreq);
								}
							}
							finally
							{
								_lock_queue.Release();
							}

							if (qreq != null)
							{
								if (EnableTransactionId)
									_lg?.LogDebug(Cpr.log_tr_resp, response.TransactionId);

								qreq.CancellationTokenSource?.Dispose();
								qreq.TaskCompletionSource?.TrySetResult(response);
								qreq.TimeoutCancellationTokenSource?.Dispose();
							}
						}
						catch (ArgumentException ex)
						{
							_lg?.LogError(ex, Cpr.log_recv_invalid_data, ex.Message);
						}
						catch (NotImplementedException ex)
						{
							_lg?.LogError(ex, Cpr.log_recv_invalid_data, ex.Message);
						}
					}
				}
				catch (OperationCanceledException) when (_cts_recv.IsCancellationRequested)
				{
					// Receive loop stopping
					throw;
				}
				catch (IOException)
				{
					if (!_is_recon)
						ConnectingTask = InternalGetReconnectTask();

					await Task.Delay(1, _cts_recv.Token);   // make sure the reconnect task has time to start.
				}
				catch (Exception ex)
				{
					_lg?.LogError(ex, Cpr.log_recv_error_ex, ex.GetType().Name, ex.GetMessage());
				}
			}
		}
		catch (OperationCanceledException) when (_cts_recv.IsCancellationRequested)
		{
			// Receive loop stopping
			//var ex = new SocketException((int)SocketError.TimedOut);

			await _lock_queue.WaitAsync(_cts_stop.Token);
			try
			{
				foreach (var queuedItem in _awaiting_responses)
				{
					queuedItem.Registration.DisposeAsync().ValueTaskAwait();
					queuedItem.CancellationTokenSource?.Dispose();
					queuedItem.TaskCompletionSource?.TrySetCanceled();
					queuedItem.TimeoutCancellationTokenSource?.Dispose();
				}

				_awaiting_responses.Clear();
			}
			catch
			{
				// 오우
			}
			finally
			{
				_lock_queue.Release();
			}
			_lg?.LogInformation(Cpr.log_stop_resp);
		}
		catch (Exception ex)
		{
			_lg?.LogError(ex, Cpr.log_recv_error_ex, ex.GetType().Name, ex.GetMessage());
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.InternalReceiveLoop");
		}
	}

	private async Task<Response> InternalSendRequest(Request request, CancellationToken cancellationToken)
	{
		try
		{
			_lg?.LogTrace(Cpr.log_method_enter, "ModBusTcpClient.InternalSendRequest");
			InternalCheckDisposed();

			lock (_lock_recon)
			{
				if (!IsConnected)
				{
					if (!_is_recon)
						ConnectingTask = InternalGetReconnectTask(true);

					throw new InvalidOperationException(Cpr.ex_client_no_conn);
				}
			}

			if (_nst == null)
				throw new InvalidOperationException(Cpr.ex_client_cant_open_stream);

			using var scts = CancellationTokenSource.CreateLinkedTokenSource(_cts_stop.Token, cancellationToken);

			var qreq = new QueuedRequest
			{
				TaskCompletionSource = new TaskCompletionSource<Response>()
			};

			lock (_lock_trx_id)
				qreq.TransactionId = _transaction_id++;

			await _lock_queue.WaitAsync(scts.Token);
			try
			{
				_awaiting_responses.Add(qreq);
				_lg?.LogDebug(Cpr.log_tr_add_recv_que, qreq.TransactionId);
			}
			finally
			{
				_lock_queue.Release();
			}

			await _lock_send.WaitAsync(scts.Token);
			try
			{
				request.TransactionId = qreq.TransactionId;
				_lg?.LogDebug(Cpr.log_sending_req, request);

				var bytes = request.Serialize();
				using var timeCts = new CancellationTokenSource(SendTimeout);
				using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts_stop.Token, timeCts.Token, cancellationToken);
				try
				{
					await _nst.WriteAsync(bytes, 0, bytes.Length, cts.Token);
					_lg?.LogDebug(Cpr.log_tr_send_req, request.TransactionId);

					qreq.TimeoutCancellationTokenSource = new CancellationTokenSource(ReceiveTimeout);
					qreq.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts_stop.Token, cancellationToken, qreq.TimeoutCancellationTokenSource.Token);
					qreq.Registration = qreq.CancellationTokenSource.Token.Register(() => InternalRemoveQueuedItem(qreq));
				}
				catch (OperationCanceledException) when (timeCts.IsCancellationRequested)
				{
					qreq.TaskCompletionSource.TrySetCanceled();
					await _lock_queue.WaitAsync(_cts_stop.Token);
					try
					{
						_awaiting_responses.Remove(qreq);
					}
					finally
					{
						_lock_queue.Release();
					}
				}
			}
			catch (Exception ex)
			{
				_lg?.LogError(ex, Cpr.log_send_error_ex, ex.GetType().Name, ex.GetMessage());
				qreq.TaskCompletionSource.TrySetException(ex);
				await _lock_queue.WaitAsync(_cts_stop.Token);
				try
				{
					_awaiting_responses.Remove(qreq);
				}
				finally
				{
					_lock_queue.Release();
				}
			}
			finally
			{
				_lock_send.Release();
			}

			return await qreq.TaskCompletionSource.Task;
		}
		finally
		{
			_lg?.LogTrace(Cpr.log_method_leave, "ModBusTcpClient.InternalSendRequest");
		}
	}

	private async void InternalRemoveQueuedItem(QueuedRequest item)
	{
		try
		{
			await _lock_queue.WaitAsync(_cts_stop.Token);
			try
			{
				_awaiting_responses.Remove(item);
				item.CancellationTokenSource?.Dispose();
				item.Registration.DisposeAsync().ValueTaskAwait();
				item.TaskCompletionSource?.TrySetCanceled();
				item.TimeoutCancellationTokenSource?.Dispose();
			}
			finally
			{
				_lock_queue.Release();
			}
		}
		catch
		{
			// 오우
		}
	}

	private Task InternalGetReconnectTask(bool alreadyLocked = false)
	{
		Task task;
		if (alreadyLocked)
		{
			_tcs_recon ??= new TaskCompletionSource<bool>();
			task = _tcs_recon.Task;
		}
		else
		{
			lock (_lock_recon)
			{
				_tcs_recon ??= new TaskCompletionSource<bool>();
				task = _tcs_recon.Task;
			}
		}

		Task.Run(async () => await InternalReconnect()).TaskAwait();
		return task;
	}

	private static IPAddress StaticResolveHost(string host)
	{
		if (IPAddress.TryParse(host, out var address))
			return address;

		return Dns
				   .GetHostAddresses(host)
				   .OrderBy(ip =>
					   ip.AddressFamily)
				   .FirstOrDefault(ip => ip.AddressFamily is
					   AddressFamily.InterNetwork or AddressFamily.InterNetworkV6) ??
			   throw new ArgumentException(Cpr.ex_no_host_found, nameof(host));
	}

	private void InternalSetKeepAlive()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			return;

		var enabled = _keep_alive.TotalMilliseconds > 0;
		var interval = _keep_alive.TotalMilliseconds > uint.MaxValue ? uint.MaxValue : (uint)_keep_alive.TotalMilliseconds;

		const int sizeuint = sizeof(uint);
		var config = new byte[sizeuint * 3];

		Array.Copy(BitConverter.GetBytes(enabled ? 1U : 0U), 0, config, 0, sizeuint);
		Array.Copy(BitConverter.GetBytes(interval), 0, config, sizeuint * 1, sizeuint);
		Array.Copy(BitConverter.GetBytes(interval), 0, config, sizeuint * 2, sizeuint);
		_tcp?.Client.IOControl(IOControlCode.KeepAliveValues, config, null);
	}

	private bool _is_disposed;

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (!_is_disposed)
			{
				Close().
					ConfigureAwait(false).
					GetAwaiter().
					GetResult();
				_is_disposed = true;
			}
		}
	}

	private void InternalCheckDisposed()
	{
		if (_is_disposed)
			throw new ObjectDisposedException(GetType().FullName);
	}

	public override string ToString()
	{
		InternalCheckDisposed();

		StringBuilder sb = new();
		sb.Append($"[{Host}:{Port}]");
		if (IsConnected) sb.Append(' ').Append(Cpr.connected);
		return sb.ToString();
	}
}
