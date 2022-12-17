namespace Du.Terminal;

/// <summary>
/// 콘솔 아웃풋 전용
/// </summary>
public static class Consout
{
	#region 잠금
	private static readonly object s_lock = new();

	/// <summary>
	/// 클래스 잠금 오브젝트
	/// </summary>
	public static object LockObject => s_lock;
	#endregion

	#region 출력
	/// <summary>
	/// 시간 색깔
	/// </summary>
	public static string TimedTermColor { get; set; } = TermAnsi.Yellow;
	/// <summary>
	/// 열기 브라켓
	/// </summary>
	public static string OpenBracket { get; set; } = "[";
	/// <summary>
	/// 닫기 브라켓
	/// </summary>
	public static string CloseBracket { get; set; } = "]";

	/// <summary>
	/// 콘솔 출력
	/// </summary>
	/// <param name="message"></param>
	public static void Write(string message)
	{
		lock (s_lock)
			Console.Write(message);
	}

	/// <summary>
	/// 콘솔 출력 + 새줄
	/// </summary>
	/// <param name="message"></param>
	public static void WriteLine(string message)
	{
		lock (s_lock)
			Console.WriteLine(message);
	}

	/// <summary>
	/// 콘솔 새줄
	/// </summary>
	public static void WriteLine()
	{
		lock (s_lock)
			Console.WriteLine();
	}

	/// <summary>
	/// 콘솔 출력: 시간 + 메시지
	/// </summary>
	/// <param name="message"></param>
	public static void TimedWrite(string message)
	{
		lock (s_lock)
			Console.Write($"{OpenBracket}{TimedTermColor}{DateTime.Now:O}{TermAnsi.Reset}{CloseBracket} {message}");
	}

	/// <summary>
	/// 콘솔 출력 + 새줄: 시간 + 메시지
	/// </summary>
	/// <param name="message"></param>
	public static void TimedWriteLine(string message)
	{
		lock (s_lock)
			Console.WriteLine($"{OpenBracket}{TimedTermColor}{DateTime.Now:O}{TermAnsi.Reset}{CloseBracket} {message}");
	}
	#endregion

	#region 탑 텍스트
	/// <summary>
	/// 리포트 활성 색깔
	/// </summary>
	public static string ReportActive { get; set; } = $"{TermAnsi.Yellow}{TermAnsi.BgDarkCyan}";
	/// <summary>
	/// 리포트 남은 색깔
	/// </summary>
	public static string ReportLeft { get; set; } = $"{TermAnsi.Gray}{TermAnsi.BgDarkGray}";
	/// <summary>
	/// 리포트 위치
	/// 이 값이 0보다 작으면 화면 아래부터 시작한다
	/// </summary>
	public static int ReportLocation { get; set; } = 0;
	/// <summary>
	/// 리포트에 퍼센트를 붙여요
	/// </summary>
	public static bool ReportShowPercent { get; set; } = true;

	/// <summary>
	/// 진행바 표시
	/// </summary>
	/// <param name="value"></param>
	/// <param name="message"></param>
	public static void Report(double value, string? message = null) =>
		InternalReport(value, message ?? string.Empty);

	/// <summary>
	/// 진행바 표시
	/// </summary>
	/// <param name="value"></param>
	/// <param name="max"></param>
	/// <param name="message"></param>
	public static void Report(int value, int max, string? message = null) =>
		InternalReport((double)value / max, message ?? string.Empty);

	//
	private static void InternalReport(double value, string message)
	{
		value = Math.Clamp(value, 0, 1);
		StringBuilder sb;

		if (Console.IsOutputRedirected)
		{
			if (!ReportShowPercent)
				WriteLine(message);
			else
			{
				sb = new StringBuilder(message);
				sb.Append((int)(value * 100)).Append('%');
				WriteLine(sb.ToString());
			}
			return;
		}

		var width = Console.WindowWidth;
		sb = new StringBuilder(message.Length < width ? message : message[..width]); // 2바이트 문자는... 계산 안됨

		var progress = Math.Clamp((int)(value * width), 0, width);
		var (length, left) = InternalStringLength(message, progress); // 2바이트 문자 계산용

		if (!ReportShowPercent)
		{
			if (width > length)
				sb.Append(' ', width - length);
		}
		else
		{
			if (width > length - 5)
			{
				sb.Append(' ', width - length - 5);
				sb.AppendFormat("{0,3}% ", (int)(value * 100));
			}
			else
			{
				// 아니 이건 %붙일 자리가 없네
				if (width > length)
					sb.Append(' ', width - length);
			}
		}

		sb.Insert(progress - left, ReportLeft);
		sb.Insert(0, ReportActive);
		sb.Append(TermAnsi.Reset);

		lock (s_lock)
		{
			Console.CursorVisible = false;
			var (sx, sy) = Console.GetCursorPosition();

			Console.SetCursorPosition(0, InternalRealReportLocation);
			Console.Write(sb.ToString());

			Console.SetCursorPosition(sx, sy);
			Console.CursorVisible = true;
		}
	}

	/// <summary>
	/// 리포트, 메시지만
	/// </summary>
	/// <param name="message"></param>
	/// <param name="align"></param>
	public static void Report(string message, ConsAlign align = ConsAlign.Left) =>
		Report(message, ReportActive, align);

	/// <summary>
	/// 리포트, 메시만 +데코
	/// </summary>
	/// <param name="message"></param>
	/// <param name="decoration"></param>
	/// <param name="align"></param>
	public static void Report(string message, string decoration, ConsAlign align = ConsAlign.Left)
	{
		if (Console.IsOutputRedirected)
		{
			WriteLine(message);
			return;
		}

		var width = Console.WindowWidth;
		var length = InternalStringLength(message); // 2바이트 문자 계산용

		var sb = new StringBuilder(message.Length < width ? message : message[..width]); // 2바이트 문자는... 계산 안됨

		if (width > length)
		{
			var left = width - length;
			if (align == ConsAlign.Center)
			{
				var half = left / 2;
				sb.Append(' ', half);
				sb.Insert(0, new string(' ', left - half));
			}
			else
			{
				if (align == ConsAlign.Left)
					sb.Append(' ', left);
				else
					sb.Insert(0, new string(' ', left));
			}
		}

		sb.Insert(0, decoration);
		sb.Append(TermAnsi.Reset);

		lock (s_lock)
		{
			Console.CursorVisible = false;
			var (sx, sy) = Console.GetCursorPosition();

			Console.SetCursorPosition(0, InternalRealReportLocation);
			Console.Write(sb.ToString());

			Console.SetCursorPosition(sx, sy);
			Console.CursorVisible = true;
		}
	}

	/// <summary>
	/// 리포트 영역을 지운다
	/// </summary>
	public static void Report()
	{
		if (Console.IsOutputRedirected)
			return;

		var sb = new StringBuilder(TermAnsi.Reset);
		sb.Append(' ', Console.WindowWidth);

		lock (s_lock)
		{
			Console.CursorVisible = false;
			var (sx, sy) = Console.GetCursorPosition();

			Console.SetCursorPosition(0, InternalRealReportLocation);
			Console.Write(sb.ToString());

			Console.SetCursorPosition(sx, sy);
			Console.CursorVisible = true;
		}
	}

	//
	private static (int len, int left) InternalStringLength(string input, int progress)
	{
		var left = 0;
		var len = 0;
		for (var i = 0; i < input.Length; i++)
		{
			if (char.IsAscii(input[i]))
				len++;
			else
			{
				len += 2;
				if (i < progress)
					left++;
			}
		}
		return (len, left);
	}

	//
	private static int InternalStringLength(string input)
	{
		var len = 0;
		for (var i = 0; i < input.Length; i++)
		{
			if (char.IsAscii(input[i]))
				len++;
			else
				len += 2;
		}
		return len;
	}

	//
	private static int InternalRealReportLocation
	{
		get
		{
			if (ReportLocation >= 0)
				return ReportLocation;

			int height = Console.WindowHeight;
			return height + ReportLocation;
		}
	}
	#endregion
}

/// <summary>
/// 콘솔 정렬
/// </summary>
public enum ConsAlign
{
	/// <summary>
	/// 왼쪽
	/// </summary>
	Left,
	/// <summary>
	/// 가운데
	/// </summary>
	Center,
	/// <summary>
	/// 오른쪽
	/// </summary>
	Right,
}
