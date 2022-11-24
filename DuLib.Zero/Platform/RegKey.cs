using Microsoft.Win32;

namespace Du.Platform;

/// <summary>
/// 레지스트리
/// </summary>
[SupportedOSPlatform("windows")]
public class RegKey : IDisposable
{
	private readonly string s_base_key = "Software";

	private RegistryKey? _rk;

	/// <summary>
	/// 열기
	/// </summary>
	/// <param name="keyName"></param>
	/// <param name="createNew"></param>
	public RegKey(string keyName, bool createNew = false)
		: this(keyName, Registry.CurrentUser, createNew)
	{
	}

	/// <summary>
	/// 열기
	/// </summary>
	/// <param name="keyName"></param>
	/// <param name="highKey"></param>
	/// <param name="createNew"></param>
	public RegKey(string keyName, RegistryKey highKey, bool createNew = false)
	{
		OpenKey(keyName, highKey, createNew);
	}

	/// <summary>
	/// 열기
	/// </summary>
	/// <param name="baseKey"></param>
	/// <param name="keyName"></param>
	/// <param name="createNew"></param>
	public RegKey(string baseKey, string keyName, bool createNew = false)
		: this(baseKey, keyName, Registry.CurrentUser, createNew)
	{
	}

	/// <summary>
	/// 열기
	/// </summary>
	/// <param name="baseKey"></param>
	/// <param name="keyName"></param>
	/// <param name="highKey"></param>
	/// <param name="createNew"></param>
	public RegKey(string baseKey, string keyName, RegistryKey highKey, bool createNew = false)
	{
		s_base_key = baseKey;
		OpenKey(keyName, highKey, createNew);
	}

	//
	private RegKey(RegistryKey rk)
	{
		_rk = rk;
	}

	//
	private void OpenKey(string keyName, RegistryKey highKey, bool createNew)
	{
		var key = s_base_key + "\\" + keyName;

		_rk = highKey.OpenSubKey(key, true);
		if (_rk == null && createNew)
			_rk = highKey.CreateSubKey(key);
	}

	/// <summary>
	/// 제거
	/// </summary>
	public void Dispose()
	{
		Close();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// 닫기
	/// </summary>
	public void Close()
	{
		if (_rk != null)
		{
			_rk.Close();
			_rk = null;
		}
	}

	/// <summary>
	/// 문자열로
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return _rk != null ? _rk.ToString() : "[closed]";
	}

	/// <summary>
	/// 열려 있으면 참
	/// </summary>
	public bool IsOpen => _rk != null;

	/// <summary>
	/// 하부키를 만듭니다
	/// </summary>
	/// <param name="keyName"></param>
	/// <returns></returns>
	public RegKey? CreateKey(string keyName)
	{
		return _rk == null ? null : new RegKey(_rk.CreateSubKey(keyName));
	}

	/// <summary>
	/// 임의의 값 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public object? GetValue(string name)
	{
		return _rk?.GetValue(name);
	}

	/// <summary>
	/// 문자열 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="failRet"></param>
	/// <returns></returns>
	public string? GetString(string name, string? failRet = null)
	{
		return _rk?.GetValue(name) as string ?? failRet;
	}

	/// <summary>
	/// 정수 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="failRet"></param>
	/// <returns></returns>
	public int GetInt(string name, int failRet = -1)
	{
		return _rk?.GetValue(name) is int value ? value : failRet;
	}

	/// <summary>
	/// 긴정수 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="failRet"></param>
	/// <returns></returns>
	public long GetLong(string name, long failRet = -1)
	{
		return _rk?.GetValue(name) is long value ? value : failRet;
	}

	/// <summary>
	/// 불 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="failRet"></param>
	/// <returns></returns>
	public bool GetBool(string name, bool failRet = false)
	{
		return _rk?.GetValue(name) is int value ? value != 0 : failRet;
	}

	/// <summary>
	/// 바이트 배열 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public byte[]? GetBytes(string name)
	{
		return _rk?.GetValue(name) as byte[];
	}

	/// <summary>
	/// 디코딩한 문자열 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="failRet"></param>
	/// <returns></returns>
	public string? GetDecodingString(string name, string? failRet = null)
	{
		if (_rk?.GetValue(name) is not string value) 
			return failRet;

		var s = Converter.DecodingString(value);
		return !string.IsNullOrEmpty(s) ? s : failRet;
	}

	/// <summary>
	/// 압축 해제한 문자열 얻기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="failRet"></param>
	/// <returns></returns>
	public string? GetDecompressString(string name, string? failRet = null)
	{
		if (_rk?.GetValue(name) is not string value) 
			return failRet;

		var s = Converter.DecompressString(value);
		return !string.IsNullOrEmpty(s) ? s : failRet;
	}

	/// <summary>
	/// 값 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetValue(string? name, object value)
	{
		_rk?.SetValue(name, value);
	}

	/// <summary>
	/// 문자열 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetString(string? name, string value)
	{
		_rk?.SetValue(name, value, RegistryValueKind.String);
	}

	/// <summary>
	/// 정수 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetInt(string? name, int value)
	{
		_rk?.SetValue(name, value, RegistryValueKind.DWord);
	}

	/// <summary>
	/// 긴정수 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetLong(string? name, long value)
	{
		_rk?.SetValue(name, value, RegistryValueKind.QWord);
	}

	/// <summary>
	/// 불 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetBool(string? name, bool value)
	{
		_rk?.SetValue(name, value ? 1 : 0, RegistryValueKind.DWord);
	}

	/// <summary>
	/// 바이트 문자열 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetBytes(string? name, byte[] value)
	{
		_rk?.SetValue(name, value, RegistryValueKind.Binary);
	}

	/// <summary>
	/// 문자열을 인코딩하여 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetEncodingString(string? name, string value)
	{
		if (_rk == null) 
			return;

		var s = Converter.EncodingString(value);
		_rk.SetValue(name, s, RegistryValueKind.String);
	}

	/// <summary>
	/// 문자열을 압축하여 넣기
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public void SetCompressString(string? name, string value)
	{
		if (_rk == null) 
			return;

		var s = Converter.CompressString(value);
		_rk.SetValue(name, s, RegistryValueKind.String);
	}

	/// <summary>
	/// 지정한 키를 지우기
	/// </summary>
	/// <param name="keyName"></param>
	/// <param name="alsoDeleteTree">안에 있는 키 전부 지우려면 참</param>
	/// <returns></returns>
	public bool DeleteKey(string keyName, bool alsoDeleteTree = false)
	{
		if (_rk == null)
			return false;

		try
		{
			if (alsoDeleteTree)
				_rk.DeleteSubKeyTree(keyName);
			else
				_rk.DeleteSubKey(keyName);
			return true;
		}
		catch { return false; }
	}

	/// <summary>
	/// 값 지우기
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public bool DeleteValue(string name)
	{
		if (_rk == null)
			return false;

		try
		{
			_rk.DeleteValue(name);
			return true;
		}
		catch { return false; }
	}

	/// <summary>
	/// 확장자 등록하기
	/// </summary>
	/// <param name="extension">확장자 (".testext")</param>
	/// <param name="type">확장자 형식 ("Test.testext")</param>
	/// <param name="description">확장자 설명</param>
	/// <param name="executePath">이 확장자로 실행할 프로그램 전체경로</param>
	/// <param name="friendlyName">그냥 부를 이름 (없어도됨)</param>
	/// <returns></returns>
	public static bool RegisterExtension(string extension, string type, string description, string executePath, string? friendlyName = null)
	{
		// (".testext", "Test.testext", "Test extension register", "c:\test.exe", "테스트프로그램")

		try
		{
			using (var rc = new RegKey("Classes"))
			{
				using (var re = rc.CreateKey(extension))
					re?.SetString(null, type);

				using var rt = rc.CreateKey(type);
				rt?.SetString(null, description);

				using var rs = rt?.CreateKey("shell");

				using var ro = rs?.CreateKey("open");
				if (!string.IsNullOrEmpty(friendlyName))
					rc.SetString("FriendlyAppName", friendlyName);

				using var rn = ro?.CreateKey("command");
				rn?.SetString(null, $"\"{executePath}\" \"%1\"");
			}

			return true;
		}
		catch { return false; }
	}

	/// <summary>
	/// 확장자를 지웁니다
	/// </summary>
	/// <param name="extension">지울 확장자</param>
	/// <param name="type">형식 이름</param>
	/// <returns></returns>
	public static bool UnregisterExtension(string extension, string type)
	{
		// (".testext", "Test.testext")

		try
		{
			using var rc = new RegKey("Classes");
			rc.DeleteKey(extension);
			rc.DeleteKey(type, true);

			return true;
		}
		catch { return false; }
	}
}

