using System;

namespace Du
{
	public class EorzeaTime
	{
		public int Hour { get; set; }
		public int Minute { get; set; }

		public static readonly int _tz_delta = -9;   // JST/KST UTC-9;
		public static readonly DateTime _base_datetime = new DateTime(2010, 6 + 1, 12, 0, 0, 0, DateTimeKind.Utc);
		public static readonly double _base_epoch = ConvertEpoch(_base_datetime) + _tz_delta * 60 * 60 * 1000;

		public static int TimeZoneDelta => _tz_delta;
		public static DateTime BaseDateTime => _base_datetime;
		public static double BaseEpoch => _base_epoch;

		//
		public EorzeaTime()
			: this(0, 0)
		{
		}

		//
		public EorzeaTime(int hour, int minute)
		{
			Hour = hour;
			Minute = minute;
		}

		//
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

		// 
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
}
