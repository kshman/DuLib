using System.Runtime.InteropServices;

namespace Du.WinForms;

/// <summary>
/// 컨트롤 도움
/// </summary>
public static class ControlDu
{
	/// <summary>
	/// 컨트롤의 더블버퍼링 켬끔
	/// </summary>
	/// <param name="control"></param>
	/// <param name="enabled"></param>
	public static void DoubleBuffered(Control control, bool enabled)
	{
		var prop = control.GetType().GetProperty(
			"DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
		prop?.SetValue(control, enabled, null);
	}
}

/// <summary>
/// 폼 도움
/// </summary>
public static class FormDu
{
	private static readonly double[] _effect_appear_opacity = new double[] { 0.1, 0.3, 0.7, 0.8, 0.9, 1.0 };

	/// <summary>
	/// 스르륵 나타나기 이펙트
	/// </summary>
	/// <param name="form"></param>
	public static void EffectAppear(Form form)
	{
		var count = 0;

		var timer = new System.Windows.Forms.Timer()
		{
			Interval = 20,
		};

		form.RightToLeftLayout = false;
		form.Opacity = 0d;

		timer.Tick += (o, e) =>
		{
			if ((count + 1 > _effect_appear_opacity.Length) || form == null)
			{
				timer.Stop();
				timer.Dispose();
				timer = null;
			}
			else
			{
				form.Opacity = _effect_appear_opacity[count++];
			}
		};
		timer.Start();
	}

	/// <summary>
	/// 최상단 윈도우로
	/// </summary>
	/// <param name="handle"></param>
	public static void SetForeground(IntPtr handle)
	{
		if (handle != IntPtr.Zero)
			NativeWin32.SetForegroundWindow(handle);
	}

	/// <summary>
	/// 최상단 윈도우로
	/// </summary>
	/// <param name="control"></param>
	public static void SetForeground(Control control)
	{
		if (control != null)
			SetForeground(control.Handle);
	}

	/// <summary>
	/// 아이콘이면 보이게한다
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static bool ShowIfIconic(IntPtr handle)
	{
		if (handle == IntPtr.Zero)
			return false;

		if (!NativeWin32.IsIconic(handle))
			return false;
		else
		{
			NativeWin32.ShowWindowAsync(handle, NativeWin32.SW_RESTORE);
			return true;
		}
	}

	/// <summary>
	/// 아이콘이면 보이게한다
	/// </summary>
	/// <param name="form"></param>
	/// <returns></returns>
	public static bool ShowIfIconic(Form form)
	{
		return form != null && ShowIfIconic(form.Handle);
	}

	/// <summary>
	/// 아이콘 상태인가
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static bool IsIconic(IntPtr handle)
	{
		return handle != IntPtr.Zero && NativeWin32.IsIconic(handle);
	}

	/// <summary>
	/// 아이콘 상태인가
	/// </summary>
	/// <param name="form"></param>
	/// <returns></returns>
	public static bool IsIconic(Form form)
	{
		return form != null && IsIconic(form.Handle);
	}

	/// <summary>
	/// CopyData로 문자열을 보낸다
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="value"></param>
	public static void SendCopyDataString(IntPtr handle, string value)
	{
		if (handle != IntPtr.Zero)
		{
			var d = new NativeWin32.WmCopyData();
			try
			{
				d.SetString(value);
				d.Send(handle);
			}
			finally
			{
				d.Dispose();
			}
		}
	}

	/// <summary>
	/// CopyData로 문자열을 보낸다
	/// </summary>
	/// <param name="control"></param>
	/// <param name="value"></param>
	public static void SendCopyDataString(Control control, string value)
	{
		if (control != null)
			SendCopyDataString(control.Handle, value);
	}

	/// <summary>
	/// CopyData로 온 문자열을 받는다
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public static bool ReceiveCopyDataString(ref Message msg, out string? value)
	{
		if (msg.Msg != NativeWin32.WM_COPYDATA)
		{
			value = null;
			return false;
		}
		else
		{
			value = NativeWin32.WmCopyData.Receive(msg.LParam);
			return true;
		}
	}

	/// <summary>
	/// 자석 도킹 기능을 수행한다
	/// </summary>
	/// <param name="msg">WndProc의 메시지</param>
	/// <param name="form">수행할 폼</param>
	/// <param name="margin">자석으로 붙일 감쇠거리</param>
	public static void MagneticDockForm(ref Message msg, Form form, int margin)
	{
		if (msg.Msg != NativeWin32.WM_WINDOWPOSCHANGING)
			return;

		var desktop = (Screen.FromHandle(form.Handle)).WorkingArea;
		var pos = Marshal.PtrToStructure<WindowPos>(msg.LParam);

		// 왼쪽
		if (Math.Abs(pos.x - desktop.Left) < margin)
			pos.x = desktop.Left;

		// 오른쪽
		if (Math.Abs((pos.x + pos.cx) - (desktop.Left + desktop.Width)) < margin)
			pos.x = desktop.Right - pos.cx;

		// 위쪽
		if (Math.Abs(pos.y - desktop.Top) < margin)
			pos.y = desktop.Top;

		// 아래쪽
		if (Math.Abs((pos.y + pos.cy) - (desktop.Top + desktop.Height)) < margin)
			pos.y = desktop.Bottom - form.Bounds.Height;

		Marshal.StructureToPtr(pos, msg.LParam, false);
		msg.Result = IntPtr.Zero;
	}

	//
	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowPos
	{
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public int flags;
	}
}
