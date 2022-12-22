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
    /// <param name="failRet">실패시 반환값</param>
    /// <returns></returns>
    public static long ToLong(string? s, long failRet = 0)
    {
        return long.TryParse(s, out var ret) ? ret : failRet;
    }

    /// <summary>
    /// 문자열을 정수로
    /// </summary>
    /// <param name="s"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static int ToInt(string? s, int failRet = 0)
    {
        return int.TryParse(s, out var ret) ? ret : failRet;
    }

    /// <summary>
    /// 문자열을 짧은정수로
    /// </summary>
    /// <param name="s"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static short ToShort(string? s, short failRet = 0)
    {
        return short.TryParse(s, out var ret) ? ret : failRet;
    }

    /// <summary>
    /// 문자열을 부호없는 짧은정수로
    /// </summary>
    /// <param name="s"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static ushort ToUshort(string? s, ushort failRet = 0)
    {
        return ushort.TryParse(s, out var ret) ? ret : failRet;
    }

    /// <summary>
    /// 문자열을 불로
    /// </summary>
    /// <param name="s"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static bool ToBool(string? s, bool failRet = false)
    {
        return string.IsNullOrEmpty(s) ? failRet : s.ToUpper().Equals("TRUE");
    }

    /// <summary>
    /// 문자열을 단실수로
    /// </summary>
    /// <param name="s"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static float ToFloat(string? s, float failRet = 0.0f)
    {
        return float.TryParse(s, out float v) ? v : failRet;
    }

    /// <summary>
    /// AARRGGBB 문자열을 색깔로
    /// </summary>
    /// <param name="s"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static Color ToColorArgb(string? s, Color failRet)
    {
        try
        {
            var i = Convert.ToInt32(s, 16);
            var r = Color.FromArgb(i);
            return r;
        }
        catch
        {
            return failRet;
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
    public static IPAddress ToIpAddressFromIpv4(string? ipstr)
    {
        if (string.IsNullOrEmpty(ipstr))
            return IPAddress.None;

        try
        {
            var sa = ipstr.Trim().Split('.');
            if (sa.Length == 4)
            {
                if (sa[3].Contains(':'))
                    sa[3] = sa[3][..sa[3].IndexOf(":", StringComparison.Ordinal)];

                var ivs = new byte[4];
                for (var i = 0; i < 4; i++)
                    ivs[i] = byte.Parse(sa[i]);

                return new IPAddress(ivs);
            }
        }
        catch
        {
            // 무시
        }

        return IPAddress.None;
    }

    /// <summary>
    /// 문자열을 포인트로
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static Point ToPoint(string? x, string? y, Point failRet)
    {
        var pt = new Point(
            ToInt(x, failRet.X),
            ToInt(y, failRet.Y));
        return pt;
    }

    /// <summary>
    /// 문자열을 사이즈로
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="failRet"></param>
    /// <returns></returns>
    public static Size ToSize(string? width, string? height, Size failRet)
    {
        var sz = new Size(
            ToInt(width, failRet.Width),
            ToInt(height, failRet.Height));
        return sz;
    }

    /// <summary>
    /// 문자열을 수치로 바꾼 문자열로
    /// </summary>
    /// <param name="readableString"></param>
    /// <returns></returns>
    public static string? EncodingString(string readableString)
    {
        if (string.IsNullOrEmpty(readableString))
            return null;

        var bs = Encoding.UTF8.GetBytes(readableString);

        var sb = new StringBuilder();
        foreach (var b in bs)
            sb.Append($"{b:X2}");

        return sb.ToString();
    }

    private static byte HexCharToByte(char ch)
    {
        var b = ch - '0';
        if (b is >= 0 and <= 9)
            return (byte)b;
        b = ch - 'A' + 10;
        if (b is >= 10 and <= 15)
            return (byte)b;
        return 0;
    }

    /// <summary>
    /// 수치로 바꾼 문자열을 문자열로
    /// </summary>
    /// <param name="rawString"></param>
    /// <returns></returns>
    public static string? DecodingString(string rawString)
    {
        if (string.IsNullOrEmpty(rawString) || (rawString.Length % 2) != 0)
            return null;

        var bs = new byte[rawString.Length / 2];

        for (int i = 0, u = 0; i < rawString.Length; i += 2, u++)
        {
            var b = HexCharToByte(rawString[i]) * 16 + HexCharToByte(rawString[i + 1]);
            bs[u] = (byte)b;
        }

        return Encoding.UTF8.GetString(bs);
    }

    /// <summary>
    /// BASE64로 인코딩
    /// </summary>
    /// <param name="rawString">원본</param>
    /// <returns>BASE64로 바뀐 문자열</returns>
    public static string? EncodingBase64(string rawString)
    {
        if (string.IsNullOrEmpty(rawString))
            return null;

        var bytes = Encoding.UTF8.GetBytes(rawString);
        var base64 = Convert.ToBase64String(bytes);
        return base64;
    }

    /// <summary>
    /// BASE64를 디코딩
    /// </summary>
    /// <param name="base64String">BASE64 문자열</param>
    /// <returns>변환된 원본 문자열</returns>
    public static string? DecodingBase64(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
            return null;

        var bytes = Convert.FromBase64String(base64String);
        var raw_string = Encoding.UTF8.GetString(bytes);
        return raw_string;
    }

    /// <summary>
    /// GZIP 으로 압축한 문자열
    /// </summary>
    /// <param name="rawString"></param>
    /// <returns></returns>
    public static string? CompressString(string rawString)
    {
        if (string.IsNullOrEmpty(rawString))
            return null;

        var raw = Encoding.UTF8.GetBytes(rawString);
        var mst = new MemoryStream();

        using (var gzip = new GZipStream(mst, CompressionMode.Compress, true))
            gzip.Write(raw, 0, raw.Length);

        mst.Position = 0;

        var buf = new byte[mst.Length];
        _ = mst.Read(buf, 0, buf.Length);

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
    /// <param name="compressedString"></param>
    /// <returns></returns>
    public static string? DecompressString(string compressedString)
    {
        if (string.IsNullOrEmpty(compressedString) || (compressedString.Length % 2) != 0)
            return null;

        var bs = new byte[compressedString.Length / 2];

        for (int i = 0, u = 0; i < compressedString.Length; i += 2, u++)
        {
            var b = HexCharToByte(compressedString[i]) * 16 + HexCharToByte(compressedString[i + 1]);
            bs[u] = (byte)b;
        }

        using var mst = new MemoryStream();

        var len = BitConverter.ToInt32(bs, 0);
        mst.Write(bs, 4, bs.Length - 4);

        bs = new byte[len];
        mst.Position = 0;

        using (var gzip = new GZipStream(mst, CompressionMode.Decompress))
            _ = gzip.Read(bs, 0, bs.Length);

        return Encoding.UTF8.GetString(bs);
    }
}
