using System.Drawing.Text;
using System.Security.Principal;

namespace DuLib.Platform
{
	public static class TestPlatform
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

		public static int IsFontInstalled(string[] fontnames)
		{
			// 아 이거 select any 쓰면 되는데 까먹엇따
			InstalledFontCollection collection = new InstalledFontCollection();
			foreach (var family in collection.Families)
			{
				for (var i = 0; i < fontnames.Length; i++)
				{
					if (family.Name == fontnames[i])
						return i;
				}
			}
			return -1;
		}
	}
}
