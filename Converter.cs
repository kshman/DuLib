using System;
using System.Drawing;
using System.Net;
using System.Text;

namespace DuLib
{
	public static class Converter
	{
		public static long ToLong(string s, long failret = 0)
		{
			return long.TryParse(s, out var ret) ? ret : failret;
		}

		public static int ToInt(string s, int failret = 0)
		{
			return int.TryParse(s, out var ret) ? ret : failret;
		}

		public static short ToShort(string s, short failret = 0)
		{
			return short.TryParse(s, out var ret) ? ret : failret;
		}

		public static ushort ToUshort(string s, ushort failret = 0)
		{
			return ushort.TryParse(s, out var ret) ? ret : failret;
		}

		public static bool ToBool(string s, bool failret = false)
		{
			return string.IsNullOrEmpty(s) ? failret : s.ToUpper().Equals("TRUE");
		}

		public static float ToFloat(string s, float failret = 0.0f)
		{
			return float.TryParse(s, out float v) ? v : failret;
		}

		public static Color ToColorArgb(string s, Color failret)
		{
			try
			{
				var i = Convert.ToInt32(s, 16);
				var r = Color.FromArgb(i);
				return r;
			}
			catch
			{
				return failret;
			}
		}

		public static Color ToColorArgb(string s)
		{
			return ToColorArgb(s, Color.Transparent);
		}

		public static IPAddress ToIPAddressFromIPV4(string ipstr)
		{
			try
			{
				var sa = ipstr.Trim().Split('.');
				if (sa.Length == 4)
				{
					if (sa[3].Contains(":"))
						sa[3] = sa[3].Substring(0, sa[3].IndexOf(":"));

					var ivs = new byte[4];
					for (var i = 0; i < 4; i++)
						ivs[i] = byte.Parse(sa[i]);

					return new IPAddress(ivs);
				}
			}
			catch { }

			return IPAddress.None;
		}

		public static string Base64Encoding(string readblestring)
		{
			var bs = Encoding.Unicode.GetBytes(readblestring);
			return Convert.ToBase64String(bs);
		}

		public static string Base64Decoding(string rawstring)
		{
			var bs = Convert.FromBase64String(rawstring);
			return Encoding.Unicode.GetString(bs);
		}

		public static string EncodingString(string readblestring)
		{
			var bs = Encoding.UTF8.GetBytes(readblestring);

			StringBuilder sb = new StringBuilder();
			foreach (var b in bs)
				sb.Append($"{b:X2}");

			return sb.ToString();
		}

		private static byte HexCharToByte(char ch)
		{
			var b = ch - '0';
			if (b >= 0 && b <= 9)
				return (byte)b;
			b = ch - 'A' + 10;
			if (b >= 10 && b <= 15)
				return (byte)b;
			return 0;
		}

		public static string DecodingString(string rawstring)
		{
			if ((rawstring.Length % 2) != 0)
				return null;

			byte[] bs = new byte[rawstring.Length / 2];

			for (int i = 0, u = 0; i < rawstring.Length; i += 2, u++)
			{
				var b = HexCharToByte(rawstring[i]) * 16 + HexCharToByte(rawstring[i + 1]);
				bs[u] = (byte)b;
			}

			return Encoding.UTF8.GetString(bs);
		}
	}
}
