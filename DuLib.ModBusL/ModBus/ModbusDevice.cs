using Du.ModBus.Supp;

namespace Du.ModBus;

/// <summary>
/// 모드버스 디바이스
/// </summary>
public class ModBusDevice
{
	private readonly ReaderWriterLockSlim _lock_coils = new();
	private readonly ReaderWriterLockSlim _lock_discrete_inputs = new();
	private readonly ReaderWriterLockSlim _lock_input_registers = new();
	private readonly ReaderWriterLockSlim _lock_holding_registers = new();

	private readonly List<int> _coils = new();
	private readonly List<int> _discrete_inputs = new();
	private readonly Dictionary<int, ushort> _input_registers = new();
	private readonly Dictionary<int, ushort> _holding_registers = new();

	public byte DeviceId { get; }

	public ModBusDevice(byte id)
	{
		DeviceId = id;
	}

	public ModBusCoil GetCoil(int address)
	{
		using (_lock_coils.GetReadLock())
		{
			return new ModBusCoil
			{
				Address = address,
				AsBool = _coils.Contains(address),
			};
		}
	}

	public void SetCoil(int address, bool value)
	{
		using (_lock_coils.GetWriteLock())
		{
			switch (value)
			{
				case true when !_coils.Contains(address):
					_coils.Add(address);
					break;
				case false when _coils.Contains(address):
					_coils.Remove(address);
					break;
			}
		}
	}

	public ModBusDiscreteInput GetInput(int address)
	{
		using (_lock_discrete_inputs.GetReadLock())
		{
			return new ModBusDiscreteInput
			{
				Address = address,
				AsBool = _discrete_inputs.Contains(address),
			};
		}
	}

	public void SetInput(int address, bool value)
	{
		using (_lock_discrete_inputs.GetWriteLock())
		{
			switch (value)
			{
				case true when !_discrete_inputs.Contains(address):
					_discrete_inputs.Add(address);
					break;
				case false when _discrete_inputs.Contains(address):
					_discrete_inputs.Remove(address);
					break;
			}
		}
	}

	public ModBusRegister GetInputRegister(int address)
	{
		using (_lock_input_registers.GetReadLock())
		{
			if (_input_registers.TryGetValue(address, out var value))
			{
				return new ModBusRegister
				{
					Type = ModBusType.InputRegister,
					Address = address,
					Register = value,
				};
			}
		}

		return new ModBusRegister
		{
			Type = ModBusType.InputRegister,
			Address = address,
		};
	}

	public void SetInputRegister(int address, ushort value)
	{
		using (_lock_input_registers.GetWriteLock())
		{
			if (value > 0)
				_input_registers[address] = value;
			else
				_input_registers.Remove(address);
		}
	}

	public ModBusRegister GetHoldingRegister(int address)
	{
		using (_lock_holding_registers.GetReadLock())
		{
			if (_holding_registers.TryGetValue(address, out ushort value))
			{
				return new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address,
					Register = value,
				};
			}
		}

		return new ModBusRegister
		{
			Type = ModBusType.HoldingRegister,
			Address = address
		};
	}

	public void SetHoldingRegister(int address, ushort value)
	{
		using (_lock_holding_registers.GetWriteLock())
		{
			if (value > 0)
				_holding_registers[address] = value;
			else
				_holding_registers.Remove(address);
		}
	}
}
