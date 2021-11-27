namespace Du.Globalization;

/// <summary>
/// FFXIV 에오르제아 시간 제어
/// </summary>
public class EorzeaTime
{
	/// <summary>
	/// 시간
	/// </summary>
	public int Hour { get; set; }

	/// <summary>
	/// 분
	/// </summary>
	public int Minute { get; set; }

	/// <summary>
	/// 지역 시간에 맞춘 델타값 (일본표준시간기준 UTC-9)
	/// </summary>
	public static int TimeZoneDelta => _tz_delta;

	/// <summary>
	/// 에오르제아 기준값 (2010년 7월 12일 UTC)
	/// </summary>
	public static DateTime BaseDateTime => _base_datetime;

	/// <summary>
	/// 기준 계산값 (epoch)
	/// </summary>
	public static double BaseEpoch => _base_epoch;

	private static readonly int _tz_delta = -9;   // JST/KST UTC-9;
	private static readonly DateTime _base_datetime = new(2010, 6 + 1, 12, 0, 0, 0, DateTimeKind.Utc);
	private static readonly double _base_epoch = ConvertEpoch(_base_datetime) + _tz_delta * 60 * 60 * 1000;

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
			var now = Epoch;
			var h = (int)((now % (3600 * 24)) / 3600.0);
			var m = (int)((now % 3600) / 60.0);
			return new EorzeaTime(h, m);
		}
	}

	/// <summary>
	/// 현재시간 틱 (=epoch)
	/// </summary>
	public static double Epoch
	{
		get
		{
			double now = ((ConvertEpoch(DateTime.UtcNow) - _base_epoch) / 1000 - 90000) * (1440.0 / 70.0);
			return now;
		}
	}

	//
	private static long ConvertEpoch(DateTime dt)
	{
		return (dt.Ticks - 621355968000000000) / TimeSpan.TicksPerMillisecond;
	}
}
