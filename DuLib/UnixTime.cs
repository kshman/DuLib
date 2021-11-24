using System;

namespace Du
{
	public static class UnixTime
	{
		private static readonly DateTime _base_datetime = new DateTime(1970, 1, 1, 0, 0, 0);

		public static DateTime BaseDateTime => _base_datetime;

		// 유닉스 시간(epoch)
		public static long Tick
		{
			get
			{
				var timespan = (DateTime.Now - _base_datetime);
				return (long)timespan.TotalMilliseconds;
			}
		}
	}
}
