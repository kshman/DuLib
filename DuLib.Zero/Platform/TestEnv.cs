using System.Security.Principal;

namespace Du.Platform;

/// <summary>
/// 환경에 따른 테스트
/// </summary>
public static class TestEnv
{
	/// <summary>
	/// 관리자로 실행중인가!
	/// </summary>
	public static bool IsAdministrator => 
		OperatingSystem.IsWindows() && WindowsIsAdministrator();

	[SupportedOSPlatform("windows")]
	private static bool WindowsIsAdministrator()
	{
		var identity = WindowsIdentity.GetCurrent();
		var principal = new WindowsPrincipal(identity);
		return principal.IsInRole(WindowsBuiltInRole.Administrator);
	}
}

