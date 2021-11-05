using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuLib.System
{
	public static class UnixTime
	{
		public static long Now
		{
			get
			{
				var timespan = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
				return (long)timespan.TotalMilliseconds;
			}
		}
	}
}
