namespace Du.Globalization;

/// <summary>
/// 유닉스시간 계산
/// </summary>
public static class UnixTime
{
	/// <summary>
	/// 유닉스시간 기준값 (1970년 1월 1일 0시)
	/// </summary>
	public static DateTime BaseDateTime => new(1970, 1, 1, 0, 0, 0);

	/// <summary>
	/// 유닉스시간 기준값 (UTC기준, 1070년 1월 1일 0시)
	/// </summary>
	public static DateTime Epoch => new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	/// <summary>
	/// 유닉스 현재 시간 틱 (=epoch)
	/// </summary>
	public static long Tick
	{
		get
		{
			var timespan = (DateTime.Now - BaseDateTime);
			return (long)timespan.TotalMilliseconds;
		}
	}
}
