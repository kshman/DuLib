namespace Du;

/// <summary>
/// 유닉스시간 계산
/// </summary>
public static class UnixTime
{
	private static readonly DateTime _base_datetime = new(1970, 1, 1, 0, 0, 0);

	/// <summary>
	/// 유닉스시간 기준값 (1970년 1월 1일 0시)
	/// </summary>
	public static DateTime BaseDateTime => _base_datetime;

	/// <summary>
	/// 유닉스 현재 시간 틱 (=epoch)
	/// </summary>
	public static long Tick
	{
		get
		{
			var timespan = (DateTime.Now - _base_datetime);
			return (long)timespan.TotalMilliseconds;
		}
	}
}
