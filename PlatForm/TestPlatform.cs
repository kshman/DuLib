using System;
using System.Drawing.Text;
using System.Security.Principal;

namespace Du.Platform
{
	public static class TestPlatform
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

		public static bool IsFontInstalled(string fontname)
		{
			InstalledFontCollection collection = new InstalledFontCollection();
#if false
			return Array.Find(collection.Families, f => f.Name == fontname) != null;
#else
			foreach (var family in collection.Families)
			{
				if (family.Name == fontname)
					return true;
			}
			return false;
#endif
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
