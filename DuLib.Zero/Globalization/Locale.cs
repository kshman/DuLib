using Du.Data;

namespace Du.Globalization;

/// <summary>
/// 로캘 데이터를 처리
/// </summary>
public static class Locale
{
    private static readonly LineDbV3 s_db = LineDbV3.Empty();
    private static readonly Dictionary<string, string> s_dic = new();

    /// <summary>
    /// 현재 로캘을 얻는다
    /// </summary>
    public static string CurrentLocale { get; private set; } = string.Empty;

    /// <summary>
    /// 로캘 목록을 얻는다
    /// </summary>
    /// <returns>로켈 목록</returns>
    public static IEnumerable<string> GetLocaleList()
    {
        return s_dic.Keys.ToArray();
    }

    /// <summary>
    /// 로캘을 추가한다
    /// </summary>
    /// <param name="name"></param>
    /// <param name="contextOrFilename"></param>
    /// <param name="isfile"></param>
    public static void AddLocale(string name, string contextOrFilename, bool isfile = false)
    {
        if (!isfile)
            s_dic[name] = contextOrFilename;
        else
        {
            var s = $"FILE:{contextOrFilename}";
            s_dic[name] = s;
        }
    }

    /// <summary>
    /// 로캘을 설정
    /// </summary>
    /// <param name="name"></param>
    public static bool SetLocale(string name)
    {
        if (name == CurrentLocale) 
            return true;
        if (!s_dic.TryGetValue(name, out var context))
            return false;

        InternalSetLocale(name, context);

        return true;
    }

    /// <summary>
    /// 기본 로캘
    /// </summary>
    /// <returns></returns>
    public static bool SetDefaultLocale()
    {
        if (s_dic.Count == 0)
            return false;

        var first = s_dic.First();
        if (first.Key != CurrentLocale)
            InternalSetLocale(first.Key, first.Value);

        return true;
    }

    //
    private static void InternalSetLocale(string name, string context)
    {
        if (context.StartsWith("FILE:"))
        {
            // 이건 파일
            var filename = context[5..];
            s_db.AddFromFile(filename, Encoding.UTF8, true);
        }
        else
        {
            // 이건 컨텍스트
            s_db.AddFromContext(context, true);
        }

        CurrentLocale = name;
    }

    /// <summary>
    /// 로캘이 있나 확인
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool HasLocale(string name)
    {
        return s_dic.ContainsKey(name);
    }

    /// <summary>
    /// 값 얻기
    /// </summary>
    /// <param name="key">키</param>
    /// <returns>얻은 값</returns>
    public static string Text(string key)
    {
        return s_db.Try(key, out string? v) && v != null ? v : $"<{key}>";
    }

    /// <summary>
    /// 값 얻고 문자열 포맷
    /// </summary>
    /// <param name="key">키</param>
    /// <param name="prms">인수들</param>
    /// <returns>만들어진 값</returns>
    public static string Text(string key, params object[] prms)
    {
        return s_db.Try(key, out string? v) && v != null ? string.Format(v, prms) : $"<{key}>";
    }

    /// <summary>
    /// 값 얻기
    /// </summary>
    /// <param name="key">키</param>
    /// <returns>얻은 값</returns>
    public static string Text(int key)
    {
        return s_db.Try(key, out var v) && v != null ? v : $"{{{key}}}";
    }

    /// <summary>
    /// 값 얻고 문자열 포맷
    /// </summary>
    /// <param name="key">키</param>
    /// <param name="prms">인수들</param>
    /// <returns>만들어진 값</returns>
    public static string Text(int key, params object[] prms)
    {
        return s_db.Try(key, out var v) && v != null ? string.Format(v, prms) : $"{{{key}}}";
    }

    /// <summary>
    /// 값 얻기
    /// </summary>
    /// <param name="key">문자열로 된 숫자 키</param>
    /// <returns>얻은값</returns>
    public static string TextAsInt(string key)
    {
        return int.TryParse(key, out var n) ? Text(n) : key;
    }
}

/// <summary>
/// 로케일 바꾸기 인터페이스
/// </summary>
public interface ILocaleTranspose
{
    /// <summary>
    /// 로케일 바꿈
    /// </summary>
    void LocaleTranspose();
}

// 폼데이터 일괄 변환
// Text = "\{(\d{4})\}";
// Text = "\{(\d{1,4})\}";
// Text = Locale.Text($1);
