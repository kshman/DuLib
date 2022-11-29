namespace Du.ModBus.Interface;

public interface IModBusServer : IDisposable
{
	event EventHandler<ModBusWriteEventArgs> InputWritten;
	event EventHandler<ModBusWriteEventArgs> RegisterWritten;

	Task Initialization { get; }
	DateTime StartTime { get; }
	bool IsRunning { get; }
	TimeSpan Timeout { get; set; }
	List<byte> DeviceIds { get; }

	ModBusCoil GetCoil(byte deviceId, int coilNumber);
	void SetCoil(byte deviceId, int coilNumber, bool value);
	void SetCoil(byte deviceId, ModBusCoil coil);

	ModBusDiscreteInput GetDiscreteInput(byte deviceId, int inputNumber);
	void SetDiscreteInput(byte deviceId, int inputNumber, bool value);
	void SetDiscreteInput(byte deviceId, ModBusDiscreteInput discreteInput);

	ModBusRegister GetInputRegister(byte deviceId, int registerNumber);
	void SetInputRegister(byte deviceId, int registerNumber, ushort value);
	void SetInputRegister(byte deviceId, int registerNumber, byte highByte, byte lowByte);
	void SetInputRegister(byte deviceId, ModBusRegister register);

	ModBusRegister GetHoldingRegister(byte deviceId, int registerNumber);
	void SetHoldingRegister(byte deviceId, int registerNumber, ushort value);
	void SetHoldingRegister(byte deviceId, int registerNumber, byte highByte, byte lowByte);
	void SetHoldingRegister(byte deviceId, ModBusRegister register);

	bool AddDevice(byte deviceId);
	bool RemoveDevice(byte deviceId);
}

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ModBusWriteEventArgs : EventArgs
{
	public byte DeviceId { get; }
	public List<ModBusObject> Objects { get; }

	public ModBusWriteEventArgs(byte deviceId, ModBusCoil coil)
	{
		DeviceId = deviceId;
		Objects = new List<ModBusObject> { coil };
	}

	public ModBusWriteEventArgs(byte deviceId, List<ModBusCoil> coils)
	{
		DeviceId = deviceId;
		Objects = coils.ConvertAll(x => (ModBusObject)x);
	}

	public ModBusWriteEventArgs(byte deviceId, ModBusRegister register)
	{
		DeviceId = deviceId;
		Objects = new List<ModBusObject> { register };
	}

	public ModBusWriteEventArgs(byte deviceId, List<ModBusRegister> registers)
	{
		DeviceId = deviceId;
		Objects = registers.ConvertAll(x => (ModBusObject)x);
	}
}
