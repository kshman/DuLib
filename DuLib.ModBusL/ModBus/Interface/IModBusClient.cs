namespace Du.ModBus.Interface;

/// <summary>
/// 모드버스 클라이언트 인터페이스
/// </summary>
public interface IModBusClient : IDisposable
{
	Task ConnectingTask { get; }
	bool IsConnected { get; }
	bool EnableTransactionId { get; set; }
	TimeSpan ReconnectTimeSpan { get; set; }
	TimeSpan SendTimeout { get; set; }
	TimeSpan ReceiveTimeout { get; set; }

	Task Open(CancellationToken cancellationToken = default);
	Task Close(CancellationToken cancellationToken = default);

	Task<List<ModBusCoil>?> ReadCoils(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default);
	Task<List<ModBusDiscreteInput>?> ReadDiscreteInputs(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default);
	Task<List<ModBusRegister>?> ReadHoldingRegisters(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default);
	Task<List<ModBusRegister>?> ReadInputRegisters(byte deviceId, int startAddress, int count, CancellationToken cancellationToken = default);
	Task<Dictionary<ModBusDevIdObject, string>?> ReadDeviceInformation(byte deviceId, ModBusDevIdCategory categoryId, ModBusDevIdObject objectId = ModBusDevIdObject.VendorName, CancellationToken cancellationToken = default);
	Task<Dictionary<byte, byte[]>?> ReadDeviceInformationRaw(byte deviceId, ModBusDevIdCategory categoryId, ModBusDevIdObject objectId = ModBusDevIdObject.VendorName, CancellationToken cancellationToken = default);

	Task<bool> WriteSingleCoil(byte deviceId, ModBusObject coil, CancellationToken cancellationToken = default);
	Task<bool> WriteSingleRegister(byte deviceId, ModBusObject register, CancellationToken cancellationToken = default);
	Task<bool> WriteCoils(byte deviceId, IEnumerable<ModBusObject> coilsArray, CancellationToken cancellationToken = default);
	Task<bool> WriteRegisters(byte deviceId, IEnumerable<ModBusObject> registersArray, CancellationToken cancellationToken = default);
}
