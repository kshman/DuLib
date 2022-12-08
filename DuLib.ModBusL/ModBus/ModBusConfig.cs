using System;
using System.Reflection;

namespace Du.ModBus;

/// <summary>
/// 모드버스 기본값
/// </summary>
public static class ModBusConfig
{
	private static readonly string? s_dev_id_version =
		Assembly.GetAssembly(typeof(ModBusConfig))?
		  .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

	public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
	public static ModBusType DefaultType { get; set; } = ModBusType.HoldingRegister;

	public static string DevIdVendorName { get; set; } = "kshman";
	public static string DevIdVendorUrl { get; set; } = "https://github.com/kshman/DuLib";
	public static string DevIdProductCode { get; set; } = "DULIB.MODBUS.TCP";
	public static string DevIdProductName { get; set; } = "DuLib.ModBus";
	public static string DevIdModelName { get; set; } = "TCP Server";
	public static string DevIdApplicationName { get; set; } = "ModBus TCP Server";
	public static string DevIdVersion { get; set; } = s_dev_id_version ?? "0";
}
