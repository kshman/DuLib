using System.Collections;

namespace Du.Data.Generic;

/// <summary>
/// 줄단위 디비
/// </summary>
/// <typeparam name="TKey">키형식</typeparam>
/// <typeparam name="TValue">값형식</typeparam>
public class LineDb<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>> where TKey : notnull
{
	/// <summary>
	/// 데이터 사전
	/// </summary>
	protected readonly Dictionary<TKey, TValue> Db = new();

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
	public static LineDb<TKey, TValue> Empty()
	{
		return new LineDb<TKey, TValue>();
	}

	/// <summary>
	/// 문자열에서 만들기
	/// </summary>
	/// <param name="context"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public static LineDb<TKey, TValue> FromContext(string context, IKeyValueStringConverter<TKey, TValue> converter)
	{
		var l = new LineDb<TKey, TValue>();
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
	public static LineDb<TKey, TValue>? FromFile(string filename, Encoding encoding, IKeyValueStringConverter<TKey, TValue> converter)
	{
		var l = new LineDb<TKey, TValue>();
		return l.AddFromFile(filename, encoding, converter) ? l : null;
	}

	/// <summary>
	/// 항목 갯수
	/// </summary>
	public int Count => Db.Count;

	/// <summary>
	/// 키로 얻기
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public TValue this[TKey key]
	{
		get
		{
			return Db[key];
		}
		set
		{
			Db[key] = value;
		}
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return Db.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return Db.GetEnumerator();
	}

	/// <summary>
	/// 값 쓰기
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public void Set(TKey key, TValue value)
	{
		Db[key] = value;
	}

	/// <summary>
	/// 값 얻기
	/// </summary>
	/// <param name="key"></param>
	/// <param name="defvalue"></param>
	/// <returns></returns>
	public TValue Get(TKey key, TValue defvalue)
	{
		return Db.TryGetValue(key, out TValue? value) ? value : defvalue;
	}

	/// <summary>
	/// 값 얻기
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public TValue? Get(TKey key)
	{
		return Db.TryGetValue(key, out TValue? value) ? value : default;
	}

	/// <summary>
	/// 값 있나 확인하고 있으면 얻기
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool Try(TKey key, out TValue? value)
	{
		return Db.TryGetValue(key, out value);
	}

	/// <summary>
	/// 문자열 값이 있나 확인
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool TryParse(TKey key, out string? value)
	{
		if (!Db.TryGetValue(key, out var v))
		{
			value = null;
			return false;
		}
		else
		{
			value = v?.ToString();
			return true;
		}
	}

	/// <summary>
	/// 정수 값이 있나 확인
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool TryParse(TKey key, out int value)
	{
		if (!Db.TryGetValue(key, out var v))
		{
			value = 0;
			return false;
		}
		else
		{
			if (!int.TryParse(v?.ToString(), out value))
				return false;
			return true;
		}
	}

	/// <summary>
	/// 짧은정수 값이 있나 확인
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool TryParse(TKey key, out ushort value)
	{
		if (!Db.TryGetValue(key, out var v))
		{
			value = 0;
			return false;
		}
		else
		{
			if (!ushort.TryParse(v?.ToString(), out value))
				return false;
			return true;
		}
	}

	/// <summary>
	/// 키 지우기
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public bool Remove(TKey key)
	{
		return Db.Remove(key);
	}

	/// <summary>
	/// 한 줄에서 키와 값을 얻는다
	/// </summary>
	/// <param name="l"></param>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	protected bool LineToKeyValue(string l, out string key, out string value)
	{
		key = string.Empty;
		value = string.Empty;

		if (l[0] == '#' || l.StartsWith("//"))
			return false;

		if (l[0] == '"')
		{
			var qt = l.IndexOf('"', 1);
			if (qt < 0)
				return false;   // no end quote. probably

			var t = l[(qt + 1)..].TrimStart();
			if (t.Length == 0 || t[0] != '=')
				return false;   // no value

			key = l[1..qt].Trim();
			value = t[1..].Trim();
		}
		else
		{
			var div = l.IndexOf('=');
			if (div <= 0)
				return false;   // not valid line

			key = l[..div].Trim();
			value = l[(div + 1)..].Trim();
		}

		return true;
	}

	private void InternalParseLines(string ctx, IKeyValueStringConverter<TKey, TValue> converter)
	{
		var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var v in ss)
		{
			var l = v.TrimStart();
			if (LineToKeyValue(l, out string? key, out string? value))
				Db[converter.KeyStringConvert(key)] = converter.ValueStringConvert(value);
		}
	}

	/// <summary>
	/// 문자열에서 항목을 추가
	/// </summary>
	/// <param name="context"></param>
	/// <param name="converter"></param>
	public void AddFromContext(string context, IKeyValueStringConverter<TKey, TValue> converter)
	{
		InternalParseLines(context, converter);
	}

	/// <summary>
	/// 파일에서 항목을 추가
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="converter"></param>
	/// <returns></returns>
	public bool AddFromFile(string filename, Encoding encoding, IKeyValueStringConverter<TKey, TValue> converter)
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

	/// <summary>
	/// 파일로 저장합니다
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="encoding"></param>
	/// <param name="headers"></param>
	public void Save(string filename, Encoding encoding, string[]? headers = null)
	{
		using var sw = new StreamWriter(filename, false, encoding);
		Save(sw, headers);
	}

	/// <summary>
	/// 쓰개를 써서 저장
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="headers"></param>
	public void Save(TextWriter stream, string[]? headers = null)
	{
		if (headers != null)
		{
			foreach (var l in headers)
				stream.WriteLine($"# {l}");
			stream.WriteLine();
		}

		foreach (var l in Db)
		{
			var t = l.Key.ToString();
			if (t?.IndexOf('=') != -1)
				stream.WriteLine($"\"{t}\"={l.Value}");
			else
				stream.WriteLine($"{t}={l.Value}");
		}
	}
}
