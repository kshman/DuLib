namespace Du.Data;

/// <summary>
/// 문자열을 그대로 
/// </summary>
public class StringToStringConverter : Generic.IStringConverter<string>
{
	/// <summary>
	/// 문자열을 그대로
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public string StringConvert(string? s)
	{
		return s ?? string.Empty;
	}
}

/// <summary>
/// 문자열을 정수로 변환
/// </summary>
public class StringToIntConverter : Generic.IStringConverter<int>
{
	/// <summary>
	/// 문자열을 정수로
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public int StringConvert(string? s)
	{
		return int.TryParse(s, out var n) ? n : 0;
	}
}

/// <summary>
/// 문자열을 실수로 변환
/// </summary>
public class StringToDoubleConverter : Generic.IStringConverter<double>
{
	/// <summary>
	/// 문자열을 실수로
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public double StringConvert(string? s)
	{
		return double.TryParse(s, out var n) ? n : 0;
	}
}