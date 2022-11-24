namespace Du.ModBus;

/// <summary>
/// 모드버스 메시지 타입
/// </summary>
public enum ModBusMessage
{
	Unset,
	Read,
	WriteSingle,
	WriteMultiple,
}

/// <summary>
/// 모드버스 기능 코드
/// </summary>
public enum ModBusFunctionCode
{
	ReadCoils = 1,
	ReadDiscreteInputs = 2,
	ReadHoldingRegisters = 3,
	ReadInputRegisters = 4,
	WriteSingleCoil = 5,
	WriteSingleRegister = 6,
	WriteMultipleCoils = 15,
	WriteMultipleRegisters = 16,
	EncapsulatedInterface = 43,
}

/// <summary>
/// 모드버스 MEI 타입
/// </summary>
public enum ModBusMei : byte
{
	CanOpenGeneralReference = 13,
	ReadDeviceInformation = 14,
}

/// <summary>
/// 모드버스 디바이스 ID 카테고리
/// </summary>
public enum ModBusDevIdCategory : byte
{
	Basic = 1,
	Regular = 2,
	Extended = 3,
	Individual = 4,
}

/// <summary>
/// 모드버스 디바이스 ID 오브젝트
/// </summary>
public enum ModBusDevIdObject : byte
{
	VendorName = 0,
	ProductCode = 1,
	MajorMinorRevision = 2,
	VendorUrl = 3,
	ProductName = 4,
	ModelName = 5,
	UserApplicationName = 6,
}

/// <summary>
/// 모드버스 오류 코드
/// </summary>
public enum ModBusErrorCode : byte
{
	[Description("문제없음")]
	NoError = 0,
	[Description("잘못된 기능")]
	IllegalFunction = 1,
	[Description("잘못된 데이터 주소")]
	IllegalDataAddress = 2,
	[Description("잘못된 데이터 값")]
	IllegalDataValue = 3,
	[Description("슬레이브 장치 실패")]
	SlaveDeviceFailure = 4,
	[Description("승인")]
	Acknowledge = 5,
	[Description("슬레이브 장치 바쁨")]
	SlaveDeviceBusy = 6,
	[Description("승인 부정")]
	NegativeAcknowledge = 7,
	[Description("메모리 패리티 오류")]
	MemoryParityError = 8,
	[Description("게이트웨이 경로가 없음")]
	GatewayPath = 10,
	[Description("게이트웨이 대상 장치가 반응이 없음")]
	GatewayTargetDevice = 11,
}

/// <summary>
/// 모드버스 오브젝트 타입
/// </summary>
public enum ModBusType
{
	Unknown,
	Coil,
	DiscreteInput,
	HoldingRegister,
	InputRegister,
}
