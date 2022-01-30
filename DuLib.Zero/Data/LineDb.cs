namespace Du.Data;

internal static class ParseLineDb
{
	private static readonly char[] s_separates = { '\n', '\r' };

	internal static string[] SplitLines(string context)
	{
		return context.Split(s_separates, StringSplitOptions.RemoveEmptyEntries);
	}
}

/// <summary>
/// 문자열 키를 가진 줄 디비
/// </summary>
/// <typeparam name="T"></typeparam>
public class LineStringDb<T> : Generic.LineDb<string, T>
//, IEnumerable<KeyValuePair<string, T>>, IEnumerable, IReadOnlyCollection<KeyValuePair<string, T>>
{
	/// <summary>
	/// 생성자
	/// </summary>
	protected LineStringDb()
	{
	}

	/// <summary>
	/// 빈거 만들기
	/// </summary>
	/// <returns></returns>
	public static new LineStringDb<T> Empty()
	{
		return new LineStringDb<T>();
	}

	/// <summary>
	/// 문자열에서 만들기
	/// </summary>
	/// <param name="context"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public static LineStringDb<T> FromContext(string context, Generic.IStringConverter<T> converter)
	{
		var l = new LineStringDb<T>();
		l.AddFromContext(context, converter);
		return l;
	}

	/// <summary>
	/// 파일에서 만들기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public static LineStringDb<T>? FromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
	{
		var l = new LineStringDb<T>();
		return l.AddFromFile(filename, encoding, converter) ? l : null;
	}

	private void InternalParseLines(string ctx, Generic.IStringConverter<T> converter)
	{
		var ss = ParseLineDb.SplitLines(ctx);
		foreach (var v in ss)
		{
			var l = v.TrimStart();
			if (LineToKeyValue(l, out string key, out string value))
				Db[key] = converter.StringConvert(value);
		}
	}

	/// <summary>
	/// 문자열에서 추가하기
	/// </summary>
	/// <param name="context"></param>
	/// <param name="converter"></param>
	public void AddFromContext(string context, Generic.IStringConverter<T> converter)
	{
		InternalParseLines(context, converter);
	}

	/// <summary>
	/// 파일에서 추가하기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public bool AddFromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
	{
		try
		{
			if (File.Exists(filename))
			{
				var context = File.ReadAllText(filename, encoding);
				InternalParseLines(context, converter);
				return true;
			}
		}
		catch { }

		return false;
	}
}

/// <summary>
/// 정수 키를 가진 줄 디비
/// </summary>
/// <typeparam name="T"></typeparam>
public class LineIntDb<T> : Generic.LineDb<int, T>
{
	/// <summary>
	/// 생성자
	/// </summary>
	protected LineIntDb()
	{
	}

	/// <summary>
	/// 빈거 만들기
	/// </summary>
	/// <returns></returns>
	public static new LineIntDb<T> Empty()
	{
		return new LineIntDb<T>();
	}

	/// <summary>
	/// 문자열에서 만들기
	/// </summary>
	/// <param name="context"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public static LineIntDb<T> FromContext(string context, Generic.IStringConverter<T> converter)
	{
		var l = new LineIntDb<T>();
		l.AddFromContext(context, converter);
		return l;
	}

	/// <summary>
	/// 파일에서 만들기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public static LineIntDb<T>? FromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
	{
		var l = new LineIntDb<T>();
		return l.AddFromFile(filename, encoding, converter) ? l : null;
	}

	private void InternalParseLines(string ctx, Generic.IStringConverter<T> converter)
	{
		var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var v in ss)
		{
			string l = v.TrimStart();
			if (LineToKeyValue(l, out string key, out string value) && int.TryParse(key, out var nkey))
				Db[nkey] = converter.StringConvert(value);
		}
	}

	/// <summary>
	/// 문자열에서 추가하기
	/// </summary>
	/// <param name="context"></param>
	/// <param name="converter"></param>
	public void AddFromContext(string context, Generic.IStringConverter<T> converter)
	{
		InternalParseLines(context, converter);
	}

	/// <summary>
	/// 파일에서 추가하기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public bool AddFromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
	{
		try
		{
			if (File.Exists(filename))
			{
				var context = File.ReadAllText(filename, encoding);
				InternalParseLines(context, converter);
				return true;
			}
		}
		catch { }

		return false;
	}
}

/// <summary>
/// 문자열 키/값을 가지는 줄 디비 (버전3 호환형)
/// </summary>
public class LineDb : Generic.LineDb<string, string>
{
	/// <summary>
	/// 생성자
	/// </summary>
	protected LineDb()
	{
	}

	/// <summary>
	/// 빈거 만들기
	/// </summary>
	/// <returns></returns>
	public static new LineDb Empty()
	{
		return new LineDb();
	}

	/// <summary>
	/// 문자열에서 만들기
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	public static LineDb FromContext(string context)
	{
		var l = new LineDb();
		l.AddFromContext(context);
		return l;
	}

	/// <summary>
	/// 파일에서 만들기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <returns></returns>
	public static LineDb? FromFile(string filename, Encoding encoding)
	{
		var l = new LineDb();
		return l.AddFromFile(filename, encoding) ? l : null;
	}

	private void InternalParseLines(string ctx)
	{
		var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var v in ss)
		{
			string l = v.TrimStart();
			if (LineToKeyValue(l, out var key, out var value) && key != null)
				Db[key] = value;
		}
	}

	/// <summary>
	/// 문자열에서 추가하기
	/// </summary>
	/// <param name="context"></param>
	public void AddFromContext(string context)
	{
		InternalParseLines(context);
	}

	/// <summary>
	/// 파일에서 추가하기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <returns></returns>
	public bool AddFromFile(string filename, Encoding encoding)
	{
		try
		{
			if (File.Exists(filename))
			{
				var context = File.ReadAllText(filename, encoding);
				InternalParseLines(context);
				return true;
			}
		}
		catch { }

		return false;
	}
}

/// <summary>
/// 줄디비 v3
/// </summary>
public class LineDbV3
{
	/// <summary>
	/// 문자열 키 자료
	/// </summary>
	protected readonly Dictionary<string, string> StringDb = new();
	/// <summary>
	/// 정수 키 자료
	/// </summary>
	protected readonly Dictionary<int, string> IntDb = new();

	/// <summary>
	/// 생성자
	/// </summary>
	protected LineDbV3()
	{
	}

	/// <summary>
	/// 빈거 만들기
	/// </summary>
	/// <returns></returns>
	public static LineDbV3 Empty()
	{
		return new LineDbV3();
	}

	/// <summary>
	/// 문자열로 만들기
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="useintdb"></param>
	/// <returns></returns>
	public static LineDbV3 FromContext(string ctx, bool useintdb)
	{
		var l = new LineDbV3();
		l.AddFromContext(ctx, useintdb);
		return l;
	}

	/// <summary>
	/// 파일로 만들기
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="useintdb"></param>
	/// <returns></returns>
	public static LineDbV3 FromFile(string filename, Encoding encoding, bool useintdb)
	{
		var l = new LineDbV3();
		l.AddFromFile(filename, encoding, useintdb);
		return l;
	}

	/// <summary>
	/// 문자열로 추가
	/// </summary>
	/// <param name="context"></param>
	/// <param name="useintdb"></param>
	public void AddFromContext(string context, bool useintdb = false)
	{
		ParseLines(context, useintdb);
	}

	/// <summary>
	/// 파일에서 추가
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="useintdb"></param>
	public void AddFromFile(string filename, Encoding encoding, bool useintdb = false)
	{
		try
		{
			var context = File.ReadAllText(filename, encoding);
			ParseLines(context, useintdb);
		}
		catch { }
	}

	/// <summary>
	/// 파일로 저장
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="enc"></param>
	/// <param name="header"></param>
	/// <returns></returns>
	public bool Save(string filename, Encoding enc, string? header = null)
	{
		if (string.IsNullOrEmpty(filename))
			return false;

		using (var sw = new StreamWriter(filename, false, enc))
		{
			if (!string.IsNullOrEmpty(header))
			{
				sw.WriteLine(header);
				sw.WriteLine();
			}

			foreach (var l in StringDb)
				sw.WriteLine(l.Key.IndexOf('=') < 0 ? $"{l.Key}={l.Value}" : $"\"{l.Key}\"={l.Value}");

			foreach (var l in IntDb)
				sw.WriteLine($"{l.Key}={l.Value}");
		}

		return true;
	}

	private static readonly char[] _ParseSplitChars = new char[] { '\n', '\r' };

	/// <summary>
	/// 문자열 분석
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="useintdb"></param>
	protected void ParseLines(string ctx, bool useintdb)
	{
		var ss = ctx.Split(_ParseSplitChars, StringSplitOptions.RemoveEmptyEntries);

		foreach (var v in ss)
		{
			string name, value, l = v.TrimStart();

			if (l[0] == '#' || l.StartsWith("//"))
				continue;

			if (l[0] == '"')
			{
				var qt = l.IndexOf('"', 1);
				if (qt < 0)
				{
					// no end quote?
					continue;
				}

				value = l[(qt + 1)..].TrimStart();

				if (value.Length == 0 || value[0] != '=')
				{
					// no value
					continue;
				}

				name = l[1..qt].Trim();
				value = value[1..].Trim();
			}
			else
			{
				var div = l.IndexOf('=');
				if (div <= 0)
					continue;

				name = l[..div].Trim();
				value = l[(div + 1)..].Trim();
			}

			if (!useintdb)
				StringDb[name] = value;
			else
			{
				if (!int.TryParse(name, out var nkey))
					StringDb[name] = value;
				else
					IntDb[nkey] = value;
			}
		}
	}

	/// <summary>
	/// 설정
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void Set(string name, string value)
	{
		StringDb[name] = value;
	}

	/// <summary>
	/// 설정
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public void Set(int key, string value)
	{
		IntDb[key] = value;
	}

	/// <summary>
	/// 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public string Get(string name)
	{
		return Get(name, string.Empty);
	}

	/// <summary>
	/// 얻기
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public string Get(int key)
	{
		return Get(key, string.Empty);
	}

	/// <summary>
	/// 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="defvalue"></param>
	/// <returns></returns>
	public string Get(string name, string defvalue)
	{
		if (!StringDb.TryGetValue(name, out string? value))
			return defvalue;
		return value;
	}

	/// <summary>
	/// 얻기
	/// </summary>
	/// <param name="key"></param>
	/// <param name="defvalue"></param>
	/// <returns></returns>
	public string Get(int key, string defvalue)
	{
		if (!IntDb.TryGetValue(key, out string? value))
			return defvalue;
		return value;
	}

	/// <summary>
	/// 해보기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool Try(string name, out string? value)
	{
		return StringDb.TryGetValue(name, out value);
	}

	/// <summary>
	/// 해보기
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool Try(int key, out string? value)
	{
		return IntDb.TryGetValue(key, out value);
	}

	/// <summary>
	/// 해보기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool Try(string name, out int value)
	{
		if (!StringDb.TryGetValue(name, out string? v))
		{
			value = 0;
			return false;
		}
		else
		{
			if (!int.TryParse(v, out value))
				return false;
			return true;
		}
	}

	/// <summary>
	/// 해보기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool Try(string name, out ushort value)
	{
		if (!StringDb.TryGetValue(name, out string? v))
		{
			value = 0;
			return false;
		}
		else
		{
			if (!ushort.TryParse(v, out value))
				return false;
			return true;
		}
	}

	/// <summary>
	/// 문자열 데이터 열거기
	/// </summary>
	/// <returns></returns>
	public IEnumerator<KeyValuePair<string, string>> GetStringDb()
	{
		return StringDb.GetEnumerator();
	}

	/// <summary>
	/// 정수 데이터 열거기
	/// </summary>
	/// <returns></returns>
	public IEnumerator<KeyValuePair<int, string>> GetIntDb()
	{
		return IntDb.GetEnumerator();
	}

	/// <summary>
	///  갯수
	/// </summary>
	public int Count => StringDb.Count + IntDb.Count;

	/// <summary>
	/// 이거!
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public string this[string key] => StringDb[key];

	/// <summary>
	/// 이거!
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public string this[int key] => IntDb[key];

	/// <summary>
	/// 문자열로!
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"String={StringDb.Count} / Int={IntDb.Count}";
	}

	/// <summary>
	/// 전부 지운다
	/// </summary>
	public void Clear()
	{
		StringDb.Clear();
		IntDb.Clear();
	}
}
