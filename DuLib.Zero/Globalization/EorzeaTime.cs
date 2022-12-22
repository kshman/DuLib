namespace Du.Globalization;

/// <summary>
/// FFXIV 에오르제아 시간 제어
/// </summary>
public class EorzeaTime
{
    /// <summary>
    /// 에오르제아 기준값 (2010년 7월 12일 UTC)
    /// </summary>
    public static DateTime Epoch => new(2010, 6 + 1, 12, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 시간
    /// </summary>
    public int Hour { get; set; }

    /// <summary>
    /// 분
    /// </summary>
    public int Minute { get; set; }

    /// <summary>
    /// 0시 0분으로 하여 만듦
    /// </summary>
    public EorzeaTime()
        : this(0, 0)
    {
    }

    /// <summary>
    /// 시/분을 지정하여 만듦
    /// </summary>
    /// <param name="hour">시</param>
    /// <param name="minute">분</param>
    public EorzeaTime(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
    }

    /// <summary>
    /// 현재 시간 
    /// </summary>
    public static EorzeaTime Now
    {
        get
        {
            var now = Current;
            var h = (int)((now % (3600 * 24)) / 3600.0);
            var m = (int)((now % 3600) / 60.0);
            return new EorzeaTime(h, m);
        }
    }

    /// <summary>
    /// 현재시간 틱 (=epoch)
    /// </summary>
    public static double Current
    {
        get
        {
            // 기준
            const long org = 1278860400000; // GetTick(Epoch) + (-9/*개발사 타임존, 즉 JST*/) * 60 * 60 * 1000;
            // 현재
            return ((GetTick(DateTime.UtcNow) - org) / 1000 - 90000) * (1440.0 / 70.0);
        }
    }

    //
    private static long GetTick(DateTime dt)
        => (dt.Ticks - 621355968000000000) / TimeSpan.TicksPerMillisecond;
}
