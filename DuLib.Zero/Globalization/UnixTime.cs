namespace Du.Globalization;

/// <summary>
/// 유닉스시간 계산
/// </summary>
public static class UnixTime
{
    /// <summary>
    /// 유닉스시간 기준값 (1970년 1월 1일 0시)
    /// </summary>
    public static DateTime Epoch => DateTime.UnixEpoch;

    /// <summary>
    /// 유닉스 현재 시간 틱 (=epoch)
    /// </summary>
    public static long Tick
    {
        get
        {
            var timespan = (DateTime.UtcNow - Epoch);
            return (long)timespan.TotalMilliseconds;
        }
    }
}
