using System.IO.Compression;
using System.Net;

namespace Du;

/// <summary>
/// 변환기
/// </summary>
public static class Converter
{
	/// <summary>
	/// 문자열을 긴정수로
	/// </summary>
	/// <param name="s">문자열</param>
	/// <param name="failret">실패시 반환값</param>
	/// <returns></returns>
	public static long ToLong(string? s, long failret = 0)
	{
		return long.TryParse(s, out var ret) ? ret : failret;
	}

	/// <summary>
	/// 문자열을 정수로
	/// </summary>
	/// <param name="s"></param>
	/// <param name="failret"></param>
	/// <returns></returns>
	public static int ToInt(string? s, int failret = 0)
	{
		return int.TryParse(s, out var ret) ? ret : failret;
	}

	/// <summary>
	/// 문자열을 짧은정수로
	/// </summary>
	/// <param name="s"></param>
	/// <param name="failret"></param>
	/// <returns></returns>
	public static short ToShort(string? s, short failret = 0)
	{
		return short.TryParse(s, out var ret) ? ret : failret;
	}

	/// <summary>
	/// 문자열을 부호없는 짧은정수로
	/// </summary>
	/// <param name="s"></param>
	/// <param name="failret"></param>
	/// <returns></returns>
	public static ushort ToUshort(string? s, ushort failret = 0)
	{
		return ushort.TryParse(s, out var ret) ? ret : failret;
	}

	/// <summary>
	/// 문자열을 불로
	/// </summary>
	/// <param name="s"></param>
	/// <param name="failret"></param>
	/// <returns></returns>
	public static bool ToBool(string? s, bool failret = false)
	{
		return string.IsNullOrEmpty(s) ? failret : s.ToUpper().Equals("TRUE");
	}

	/// <summary>
	/// 문자열을 단실수로
	/// </summary>
	/// <param name="s"></param>
	/// <param name="failret"></param>
	/// <returns></returns>
	public static float ToFloat(string? s, float failret = 0.0f)
	{
		return float.TryParse(s, out float v) ? v : failret;
	}

	/// <summary>
	/// AARRGGBB 문자열을 색깔로
	/// </summary>
	/// <param name="s"></param>
	/// <param name="failret"></param>
	/// <returns></returns>
	public static Color ToColorArgb(string? s, Color failret)
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

	/// <summary>
	/// 문자열을 색깔로
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static Color ToColorArgb(string? s)
	{
		return ToColorArgb(s, Color.Transparent);
	}

	/// <summary>
	/// IPV4 주소 문자열을 IP주소로
	/// </summary>
	/// <param name="ipstr"></param>
	/// <returns></returns>
	public static IPAddress ToIPAddressFromIPV4(string? ipstr)
	{
		if (!string.IsNullOrEmpty(ipstr))
		{
			try
			{
				var sa = ipstr.Trim().Split('.');
				if (sa.Length == 4)
				{
					if (sa[3].Contains(':'))
						sa[3] = sa[3][..sa[3].IndexOf(":")];

					var ivs = new byte[4];
					for (var i = 0; i < 4; i++)
						ivs[i] = byte.Parse(sa[i]);

					return new IPAddress(ivs);
				}
			}
			catch { }
		}

		return IPAddress.None;
	}

	/// <summary>
	/// 문자열을 수치로 바꾼 문자열로
	/// </summary>
	/// <param name="readblestring"></param>
	/// <returns></returns>
	public static string EncodingString(string readblestring)
	{
		var bs = Encoding.UTF8.GetBytes(readblestring);

		var sb = new StringBuilder();
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

	/// <summary>
	/// 수치로 바꾼 문자열을 문자열로
	/// </summary>
	/// <param name="rawstring"></param>
	/// <returns></returns>
	public static string? DecodingString(string rawstring)
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

	/// <summary>
	/// GZIP 으로 압축한 문자열
	/// </summary>
	/// <param name="rawstring"></param>
	/// <returns></returns>
	public static string CompressString(string rawstring)
	{
		var raw = Encoding.UTF8.GetBytes(rawstring);
		var mst = new MemoryStream();

		using (var gzip = new GZipStream(mst, CompressionMode.Compress, true))
			gzip.Write(raw, 0, raw.Length);

		mst.Position = 0;

		var buf = new byte[mst.Length];
		mst.Read(buf, 0, buf.Length);

		var bs = new byte[buf.Length + 4];
		Buffer.BlockCopy(buf, 0, bs, 4, buf.Length);
		Buffer.BlockCopy(BitConverter.GetBytes(raw.Length), 0, bs, 0, 4);

		var sb = new StringBuilder();
		foreach (var b in bs)
			sb.Append($"{b:X2}");

		return sb.ToString();
	}

	/// <summary>
	/// GZIP 으로 압축한 문자열을 풀기
	/// </summary>
	/// <param name="compressedstring"></param>
	/// <returns></returns>
	public static string? DecompressString(string compressedstring)
	{
		if ((compressedstring.Length % 2) != 0)
			return null;

		var bs = new byte[compressedstring.Length / 2];

		for (int i = 0, u = 0; i < compressedstring.Length; i += 2, u++)
		{
			var b = HexCharToByte(compressedstring[i]) * 16 + HexCharToByte(compressedstring[i + 1]);
			bs[u] = (byte)b;
		}

		using var mst = new MemoryStream();

		int len = BitConverter.ToInt32(bs, 0);
		mst.Write(bs, 4, bs.Length - 4);

		bs = new byte[len];
		mst.Position = 0;

		using (var gzip = new GZipStream(mst, CompressionMode.Decompress))
			gzip.Read(bs, 0, bs.Length);

		return Encoding.UTF8.GetString(bs);
	}
}
