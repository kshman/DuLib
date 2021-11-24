using System.Security.Principal;

namespace Du.Platform
{
	public static class TestEnv
	{
		public static bool IsAdministrator
		{
			get
			{
#if false
				if (OperatingSystem.IsWindows())
					return WindowsIsAdministrator();
				else
					return false;
#else
				return WindowsIsAdministrator();
#endif
			}
		}

		private static bool WindowsIsAdministrator()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}
	}
}
