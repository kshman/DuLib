namespace Du.ModBus.Supp;

internal class ProxyDevice
{
	private readonly ReaderWriterLockSlim _lock_coils = new();
	private readonly ReaderWriterLockSlim _lock_discrete_inputs = new();
	private readonly ReaderWriterLockSlim _lock_input_registers = new();
	private readonly ReaderWriterLockSlim _lock_holding_registers = new();

	private readonly Dictionary<ushort, (DateTime Timestamp, bool Value)> _coils = new();
	private readonly Dictionary<ushort, (DateTime Timestamp, bool Value)> _discrete_inputs = new();
	private readonly Dictionary<ushort, (DateTime Timestamp, ushort Value)> _input_registers = new();
	private readonly Dictionary<ushort, (DateTime Timestamp, ushort Value)> _holding_registers = new();

	public (DateTime Timestamp, bool Value) GetCoil(ushort address)
	{
		using (_lock_coils.GetReadLock())
		{
			if (_coils.TryGetValue(address, out var value))
				return value;
		}
		return (DateTime.UtcNow, false);
	}

	public void SetCoil(ushort address, bool value)
	{
		using (_lock_coils.GetWriteLock())
			_coils[address] = (DateTime.UtcNow, value);
	}

	public (DateTime Timestamp, bool Value) GetDiscreteInput(ushort address)
	{
		using (_lock_discrete_inputs.GetReadLock())
		{
			if (_discrete_inputs.TryGetValue(address, out var value))
				return value;
		}
		return (DateTime.UtcNow, false);
	}

	public void SetDiscreteInput(ushort address, bool value)
	{
		using (_lock_discrete_inputs.GetWriteLock())
			_discrete_inputs[address] = (DateTime.UtcNow, value);
	}

	public (DateTime Timestamp, ushort Value) GetInputRegister(ushort address)
	{
		using (_lock_input_registers.GetReadLock())
		{
			if (_input_registers.TryGetValue(address, out var value))
				return value;
		}
		return (DateTime.UtcNow, 0);
	}

	public void SetInputRegister(ushort address, ushort value)
	{
		using (_lock_input_registers.GetWriteLock())
			_input_registers[address] = (DateTime.UtcNow, value);
	}

	public (DateTime Timestamp, ushort Value) GetHoldingRegister(ushort address)
	{
		using (_lock_holding_registers.GetReadLock())
		{
			if (_holding_registers.TryGetValue(address, out var value))
				return value;
		}
		return (DateTime.UtcNow, 0);
	}

	public void SetHoldingRegister(ushort address, ushort value)
	{
		using (_lock_holding_registers.GetWriteLock())
			_holding_registers[address] = (DateTime.UtcNow, value);
	}
}
