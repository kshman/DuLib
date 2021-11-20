using Du.PlatForm;
using System;
using System.Reflection;
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
		public static bool ReceiveCopyDataString(int msg, IntPtr lparam, out string value)
		{
			if (msg != NativeWin32.WM_COPYDATA)
			{
				value = null;
				return false;
			}
			else
			{
				value = NativeWin32.WmCopyData.Receive(lparam);
				return true;
			}
		}
	}
}
