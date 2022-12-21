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
		return (fld?.GetCustomAttributes(typeof(TA), false) as TA[])?.FirstOrDefault();
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
		return attr?.Description ?? type.ToString();
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
	/// <param name="throwException"></param>
	public static void TaskAwait(this Task task, bool throwException = false)
	{
		try
		{
			task.Wait();
		}
		catch (AggregateException ex)
		{
			// 이게 온다 원래
			if (throwException)
			{
				if (ex.InnerException != null)
					throw ex.InnerException;
				throw;
			}
		}
		catch
		{
			if (throwException)
				throw;
		}
	}

	/// <summary>
	/// ValueTask 기다리기
	/// </summary>
	/// <param name="valueTask"></param>
	public static void ValueTaskAwait(this ValueTask valueTask)
	{
		/*
		try
		{
			await valueTask;
		}
		catch
		{
			// 무시
		}
		*/
		throw new NotImplementedException();
	}

	/// <summary>
	/// ValueTask 기다리기
	/// </summary>
	/// <param name="valueTask"></param>
	/// <param name="continueOnCapturedContext"></param>
	public static void ValueTaskAwait(this ValueTask valueTask, bool continueOnCapturedContext)
	{
		/*
		try
		{
			await valueTask.ConfigureAwait(continueOnCapturedContext);
		}
		catch
		{
			// 무시
		}
		*/
		throw new NotImplementedException();
	}

	/// <summary>
	/// 배열 복수
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="index"></param>
	/// <param name="length"></param>
	/// <returns></returns>
	public static T[] SubArray<T>(this T[] array, int index, int length)
	{
		// 오류 검사 넣어야 함
		T[] ret = new T[length];
		Array.Copy(array, index, ret, 0, length);
		return ret;
	}
}
