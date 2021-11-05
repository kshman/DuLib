using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuLib.Data
{
	public static class TimeSupp
	{
		// 유닉스 시간(epoch)
		public static long UnixTimeNow
		{
			get
			{
				var timespan = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
				return (long)timespan.TotalMilliseconds;
			}
		}

		// 에오르제아 시간(ET Tick)
		public static double EorzeaTimeNow
		{
			get
			{
				double now = ((ConvertEorzeaTimeTick(DateTime.UtcNow) - _et_epoch) / 1000 - 90000) * (1440.0 / 70.0);
				return now;
			}
		}

		// 에오르제아 시간 (ET 시/분)
		public static (int Hour, int Minute) CurrentEorzeaTime()
		{
			var now = EorzeaTimeNow;
			var h = (int)((now % (3600 * 24)) / 3600.0);
			var m = (int)((now % 3600) / 60.0);
			return (h, m);
		}

		private static readonly DateTime _et_date = new DateTime(2010, 6 + 1, 12, 0, 0, 0, DateTimeKind.Utc);
		private static readonly double _et_epoch = ConvertEorzeaTimeTick(_et_date) - 9/*KST(UTC-9)*/ * 60 * 60 * 1000;

		public static long ConvertEorzeaTimeTick(DateTime dt)
		{
			return (dt.Ticks - 621355968000000000) / TimeSpan.TicksPerMillisecond;
		}
	}
}
