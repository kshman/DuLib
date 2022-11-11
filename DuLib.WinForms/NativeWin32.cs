using System.Runtime.InteropServices;

namespace Du;
internal partial class NativeWin32
{
	internal const int WM_COPYDATA = 0x004A;
	internal const int WM_WINDOWPOSCHANGING = 0x0046;

	internal const int SW_RESTORE = 9;

	[LibraryImport("user32.dll")]
	internal static partial int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref WmCopyData lParam);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool SetForegroundWindow(IntPtr hWnd);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool IsIconic(IntPtr hWnd);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	internal static partial IntPtr LocalAlloc(int flag, int size);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	internal static partial IntPtr LocalFree(IntPtr p);

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
