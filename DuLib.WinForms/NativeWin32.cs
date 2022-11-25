using System.Runtime.InteropServices;

namespace Du;
internal partial class NativeWin32
{
	internal const int WM_COPYDATA = 0x004A;
	internal const int WM_WINDOWPOSCHANGING = 0x0046;

	internal const int SW_RESTORE = 9;

	[LibraryImport("user32.dll")]
	internal static partial int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref WmCopyData lParam);

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
		public IntPtr DwData;
		public int CbData;
		public IntPtr LpData;

		public void Dispose()
		{
			if (LpData == IntPtr.Zero) 
				return;

			LocalFree(LpData);
			LpData = IntPtr.Zero;
		}

		public void SetString(string value)
		{
			Dispose();

			CbData = (value.Length + 1) * 2;
			LpData = LocalAlloc(0x40, CbData);
			Marshal.Copy(value.ToCharArray(), 0, LpData, value.Length);
			DwData = (IntPtr)1;
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
			var v = Marshal.PtrToStringUni(s.LpData);
			return v;
		}
	}
}
