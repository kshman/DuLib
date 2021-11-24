using System.Drawing.Text;

namespace Du.WinForms;

/// <summary>
/// 글꼴 점검
/// </summary>
public static class TestFont
{
	/// <summary>
	/// 지정 글꼴이 있는가 본다
	/// </summary>
	/// <param name="fontname"></param>
	/// <returns></returns>
	public static bool IsInstalled(string fontname)
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

	/// <summary>
	/// 지정 글꼴들이 있는가 본다
	/// </summary>
	/// <param name="fontnames"></param>
	/// <returns></returns>
	public static int IsInstalled(string[] fontnames)
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

