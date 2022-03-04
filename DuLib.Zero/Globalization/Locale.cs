using Du.Data;

namespace Du.Globalization;

#nullable disable

/// <summary>
/// 로캘 데이터를 처리
/// </summary>
public static class Locale
{
	private static readonly LineDbV3 s_db = LineDbV3.Empty();
	private static readonly Dictionary<string, string> s_langs = new();

	/// <summary>
	/// 현재 로캘을 얻는다
	/// </summary>
	public static string CurrentLocale { get; private set; } = string.Empty;

	/// <summary>
	/// 로캘을 추가한다
	/// </summary>
	/// <param name="name"></param>
	/// <param name="context_or_filename"></param>
	/// <param name="isfile"></param>
	public static void AddLocale(string name, string context_or_filename, bool isfile = false)
	{
		if (!isfile)
			s_langs[name] = context_or_filename;
		else
		{
			var s = $"FILE:{context_or_filename}";
			s_langs[name] = s;
		}
	}

	/// <summary>
	/// 로캘을 설정
	/// </summary>
	/// <param name="name"></param>
	public static bool SetLocale(string name)
	{
		if (name != CurrentLocale)
		{
			if (!s_langs.TryGetValue(name, out var context))
				return false;

			InternalSetLocale(name, context);
		}

		return true;
	}

	/// <summary>
	/// 기본 로캘
	/// </summary>
	/// <returns></returns>
	public static bool SetDefaultLocale()
	{
		if (s_langs.Count == 0)
			return false;

		var first = s_langs.First();
		if (first.Key != CurrentLocale)
			InternalSetLocale(first.Key, first.Value);

		return true;
	}

	//
	private static void InternalSetLocale(string name, string context)
	{
		if (context.StartsWith("FILE:"))
		{
			// 이건 파일
			var filename = context[5..];
			s_db.AddFromFile(filename, Encoding.UTF8, true);
		}
		else
		{
			// 이건 컨텍스트
			s_db.AddFromContext(context, true);
		}

		CurrentLocale = name;
	}

	/// <summary>
	/// 로캘이 있나 확인
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static bool HasLocale(string name)
	{
		return s_langs.ContainsKey(name);
	}

	/// <summary>
	/// 값 얻기
	/// </summary>
	/// <param name="key">키</param>
	/// <returns>얻은 값</returns>
	public static string Text(string key)
	{
		return s_db.Try(key, out string v) ? v : $"<{key}>";
	}

	/// <summary>
	/// 값 얻고 문자열 포맷
	/// </summary>
	/// <param name="key">키</param>
	/// <param name="prms">인수들</param>
	/// <returns>만들어진 값</returns>
	public static string Text(string key, params object[] prms)
	{
		return s_db.Try(key, out string v) ? string.Format(v, prms) : $"<{key}>";
	}

	/// <summary>
	/// 값 얻기
	/// </summary>
	/// <param name="key">키</param>
	/// <returns>얻은 값</returns>
	public static string Text(int key)
	{
		return s_db.Try(key, out string v) ? v : $"{{{key}}}";
	}

	/// <summary>
	/// 값 얻고 문자열 포맷
	/// </summary>
	/// <param name="key">키</param>
	/// <param name="prms">인수들</param>
	/// <returns>만들어진 값</returns>
	public static string Text(int key, params object[] prms)
	{
		return s_db.Try(key, out string v) ? string.Format(v, prms) : $"{{{key}}}";
	}
}

#nullable enable


// 폼데이터 일괄 변환
// Text = "\{(\d{4})\}";
// Text = "\{(\d{1,4})\}";
// Text = Locale.Text($1);
