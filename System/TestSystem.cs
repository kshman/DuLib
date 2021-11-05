using System.Drawing.Text;
using System.Security.Principal;

namespace DuLib.System
{
	public static class TestSystem
	{
		public static bool IsAdministrator
		{
			get
			{
				WindowsIdentity identity = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		public static bool IsFontInstalled(string fontname)
		{
			InstalledFontCollection collection = new InstalledFontCollection();
			foreach (var family in collection.Families)
			{
				if (family.Name == fontname)
					return true;
			}
			return false;
		}
	}
}
