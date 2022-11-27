namespace Du.Data.Generic;

/// <summary>
/// StringConvert 함수 구현
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStringConverter<out T>
{
	/// <summary>
	/// 문자열 변환
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	T StringConvert(string? s);
}

/// <summary>
/// 키/값 변환
/// </summary>
public interface IKeyValueStringConverter<out TKey, out TValue>
{
	/// <summary>
	/// 키 문자열 변환
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	TKey KeyStringConvert(string? key);

	/// <summary>
	/// 값 문자열 변환
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	TValue ValueStringConvert(string? value);
}

