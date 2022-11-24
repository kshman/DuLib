namespace Du.ModBus;

/// <summary>
/// 모드버스 기본값
/// </summary>
public static class ModBusConfig
{
	public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
	public static ModBusObjectType DefaultObject { get; set; } = ModBusObjectType.HoldingRegister;
}

