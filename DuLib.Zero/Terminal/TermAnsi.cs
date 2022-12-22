using System.ComponentModel;

namespace Du.Terminal;

/// <summary>
/// 콘솔용 안시 코드
/// </summary>
public static class TermAnsi
{
    #region 상태
    /// <summary>상태 리셋</summary>
    [Description("상태 리셋")]
    public const string Reset = "\x1b[0m";
    /// <summary>두껍게</summary>
    [Description("두껍게")]
    public const string Bold = "\x1b[1m";
    /// <summary>밑줄</summary>
    [Description("밑줄")]
    public const string Underline = "\x1b[4m";
    /// <summary>반전</summary>
    [Description("반전")]
    public const string Reverse = "\x1b[7m";
    #endregion

    #region 글자색
    /// <summary>검정</summary>
    [Description("검정")]
    public const string Black = "\x1b[30m";
    /// <summary>빨강</summary>
    [Description("빨강")]
    public const string DarkRed = "\x1b[31m";
    /// <summary>초록</summary>
    [Description("초록")]
    public const string DarkGreen = "\x1b[32m";
    /// <summary>노랑</summary>
    [Description("노랑")]
    public const string DarkYellow = "\x1b[33m";
    /// <summary>파랑</summary>
    [Description("파랑")]
    public const string DarkBlue = "\x1b[34m";
    /// <summary>자홍</summary>
    [Description("자홍")]
    public const string DarkMagenta = "\x1b[35m";
    /// <summary>청록</summary>
    [Description("청록")]
    public const string DarkCyan = "\x1b[36m";
    /// <summary>회색</summary>
    [Description("밝은 회색")]
    public const string Gray = "\x1b[37m";
    /// <summary>밝은 회색</summary>
    [Description("회색")]
    public const string DarkGray = "\x1b[90m";
    /// <summary>밝은 빨강</summary>
    [Description("밝은 빨강")]
    public const string Red = "\x1b[91m";
    /// <summary>밝은 초록</summary>
    [Description("밝은 초록")]
    public const string Green = "\x1b[92m";
    /// <summary>밝은 노랑</summary>
    [Description("밝은 노랑")]
    public const string Yellow = "\x1b[93m";
    /// <summary>밝은 파랑</summary>
    [Description("밝은 파랑")]
    public const string Blue = "\x1b[94m";
    /// <summary>밝은 자홍</summary>
    [Description("밝은 자홍")]
    public const string Magenta = "\x1b[95m";
    /// <summary>밝은 청록</summary>
    [Description("밝은 청록")]
    public const string Cyan = "\x1b[96m";
    /// <summary>하양</summary>
    [Description("하양")]
    public const string White = "\x1b[97m";
    #endregion

    #region 배경색
    /// <summary>검정</summary>
    [Description("검정 배경")]
    public const string BgBlack = "\x1b[40m";
    /// <summary>빨강</summary>
    [Description("빨강 배경")]
    public const string BgDarkRed = "\x1b[41m";
    /// <summary>초록</summary>
    [Description("초록 배경")]
    public const string BgDarkGreen = "\x1b[42m";
    /// <summary>노랑</summary>
    [Description("노랑 배경")]
    public const string BgDarkYellow = "\x1b[43m";
    /// <summary>파랑</summary>
    [Description("파랑 배경")]
    public const string BgDarkBlue = "\x1b[44m";
    /// <summary>자홍</summary>
    [Description("자홍 배경")]
    public const string BgDarkMagenta = "\x1b[45m";
    /// <summary>청록</summary>
    [Description("청록 배경")]
    public const string BgDarkCyan = "\x1b[46m";
    /// <summary>하양</summary>
    [Description("밝은 회색 배경")]
    public const string BgGray = "\x1b[47m";
    /// <summary>검정</summary>
    [Description("회색 배경")]
    public const string BgDarkGray = "\x1b[100m";
    /// <summary>빨강</summary>
    [Description("밝은 빨강 배경")]
    public const string BgRed = "\x1b[101m";
    /// <summary>초록</summary>
    [Description("밝은 초록 배경")]
    public const string BgGreen = "\x1b[102m";
    /// <summary>노랑</summary>
    [Description("밝은 노랑 배경")]
    public const string BgYellow = "\x1b[103m";
    /// <summary>파랑</summary>
    [Description("밝은 파랑 배경")]
    public const string BgBlue = "\x1b[104m";
    /// <summary>자홍</summary>
    [Description("밝은 자홍 배경")]
    public const string BgMagenta = "\x1b[105m";
    /// <summary>청록</summary>
    [Description("밝은 청록 배경")]
    public const string BgCyan = "\x1b[106m";
    /// <summary>하양</summary>
    [Description("하양 배경")]
    public const string BgWhite = "\x1b[107m";
    #endregion
}

