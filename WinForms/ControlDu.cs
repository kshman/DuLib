using Du.PlatForm;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Du.WinForms
{
	public static class ControlDu
	{
		public static void DoubleBuffered(Control control, bool enabled)
		{
			var prop = control.GetType().GetProperty(
				"DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

			prop.SetValue(control, enabled, null);
		}

		static double[] _FormEffectAppearOpacity = new double[] { 0.1d, 0.3d, 0.7d, 0.8d, 0.9d, 1.0d };

		//
		public static void FormEffectAppear(Form form)
		{
			var count = 0;

			var timer = new Timer()
			{
				Interval = 20,
			};

			form.RightToLeftLayout = false;
			form.Opacity = 0d;

			timer.Tick += (o, e) =>
			{
				if ((count + 1 > _FormEffectAppearOpacity.Length) || form == null)
				{
					timer.Stop();
					timer.Dispose();
					timer = null;
				}
				else
				{
					form.Opacity = _FormEffectAppearOpacity[count++];
				}
			};
			timer.Start();
		}

		//
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

		//
		public static bool ShowIfIconic(Form form)
		{
			return form != null && ShowIfIconic(form.Handle);
		}

		//
		public static bool IsIconic(IntPtr handle)
		{
			return handle != IntPtr.Zero && NativeWin32.IsIconic(handle);
		}

		//
		public static bool IsIconic(Form form)
		{
			return form != null && IsIconic(form.Handle);
		}

		//
		public static void SetForeground(IntPtr handle)
		{
			if (handle != IntPtr.Zero)
				NativeWin32.SetForegroundWindow(handle);
		}

		//
		public static void SetForeground(Control control)
		{
			if (control != null)
				SetForeground(control.Handle);
		}

		//
		public static void SendCopyDataString(IntPtr handle, string value)
		{
			if (handle != IntPtr.Zero)
			{
				NativeWin32.WmCopyData d = new NativeWin32.WmCopyData();
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

		//
		public static void SendCopyDataString(Control control, string value)
		{
			if (control != null)
				SendCopyDataString(control.Handle, value);
		}

		//
		public static bool ReceiveCopyDataString(ref Message msg, out string value)
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

		//
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
}
