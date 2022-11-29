using System.ComponentModel;

namespace Du.Data;

/// <summary>
/// this에 대한 조작 기능
/// </summary>
public static class ThisType
{
	/// <summary>
	/// 타입의 어트리뷰트를 가져온다
	/// </summary>
	/// <typeparam name="TA"></typeparam>
	/// <typeparam name="TT"></typeparam>
	/// <param name="type"></param>
	/// <returns></returns>
	public static TA? GetAttribute<TA, TT>(this TT type)
		where TA : Attribute
		where TT : class, Enum
	{
		var fld = type.GetType().GetField(type.ToString());
		var attr = fld?.GetCustomAttributes(typeof(TA), false) as TA[];
		return attr?.FirstOrDefault();
	}

	/// <summary>
	/// 타입에서 설명 어트리뷰트를 가져온다
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="type"></param>
	/// <returns></returns>
	public static string GetDescription<T>(this T type) where T : class, Enum
	{
		var attr = type.GetAttribute<DescriptionAttribute, T>();
		var desc = attr?.Description;
		return desc ?? type.ToString();
	}

	/// <summary>
	/// 열거형에서 설명 어트리뷰트를 가져온다
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public static string GetDescription(this Enum e)
		=> GetDescription<Enum>(e);

	/// <summary>
	/// 예외에서 InnerException이나 그냥 메시지를 가져온다
	/// </summary>
	/// <param name="exception"></param>
	/// <returns></returns>
	public static string GetMessage(this Exception exception)
		=> exception.InnerException?.Message ?? exception.Message;

	/// <summary>
	/// Task 기다리기
	/// </summary>
	/// <param name="task"></param>
	public static async void TaskAwait(this Task task)
	{
		try
		{
			await task;
		}
		catch
		{
			// 무시
		}
	}

	/// <summary>
	/// ValueTask 기다리기
	/// </summary>
	/// <param name="valueTask"></param>
	public static async void ValueTaskAwait(this ValueTask valueTask)
	{
		try
		{
			await valueTask;
		}
		catch
		{
			// 무시
		}
	}

	/// <summary>
	/// ValueTask 기다리기
	/// </summary>
	/// <param name="valueTask"></param>
	/// <param name="continueOnCapturedContext"></param>
	public static async void ValueTaskAwait(this ValueTask valueTask, bool continueOnCapturedContext)
	{
		try
		{
			await valueTask.ConfigureAwait(continueOnCapturedContext);
		}
		catch
		{
			// 무시
		}
	}
}
