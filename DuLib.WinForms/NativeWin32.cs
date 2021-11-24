using System.Runtime.InteropServices;

namespace Du;
internal class NativeWin32
{
	internal const int WM_COPYDATA = 0x004A;
	internal const int WM_WINDOWPOSCHANGING = 0x0046;

	internal const int SW_RESTORE = 9;

	[DllImport("user32.dll")]
	internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref WmCopyData lParam);

	[DllImport("user32.dll")]
	internal static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	internal static extern bool IsIconic(IntPtr hWnd);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr LocalAlloc(int flag, int size);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr LocalFree(IntPtr p);

	//
	internal struct WmCopyData : IDisposable
	{
		public IntPtr dwData;
		public int cbData;
		public IntPtr lpData;

		public void Dispose()
		{
			if (lpData != IntPtr.Zero)
			{
				LocalFree(lpData);
				lpData = IntPtr.Zero;
			}
		}

		public void SetString(string value)
		{
			Dispose();

			cbData = (value.Length + 1) * 2;
			lpData = LocalAlloc(0x40, cbData);
			Marshal.Copy(value.ToCharArray(), 0, lpData, value.Length);
			dwData = (IntPtr)1;
		}

		public void Send(IntPtr handle)
		{
			_ = SendMessage(handle, WM_COPYDATA, IntPtr.Zero, ref this);
		}

		public static WmCopyData FromLParam(IntPtr lparam)
		{
			return Marshal.PtrToStructure<WmCopyData>(lparam);
		}

		public static string? Receive(IntPtr lparam)
		{
			var s = FromLParam(lparam);
			var v = Marshal.PtrToStringUni(s.lpData);
			return v;
		}
	}
}
