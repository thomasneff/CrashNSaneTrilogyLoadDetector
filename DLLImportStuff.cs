using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrashNSaneLoadDetector
{
	class DLLImportStuff
	{

	
		public enum TernaryRasterOperations : uint
		{
			SRCCOPY = 0x00CC0020,
			SRCPAINT = 0x00EE0086,
			SRCAND = 0x008800C6,
			SRCINVERT = 0x00660046,
			SRCERASE = 0x00440328,
			NOTSRCCOPY = 0x00330008,
			NOTSRCERASE = 0x001100A6,
			MERGECOPY = 0x00C000CA,
			MERGEPAINT = 0x00BB0226,
			PATCOPY = 0x00F00021,
			PATPAINT = 0x00FB0A09,
			PATINVERT = 0x005A0049,
			DSTINVERT = 0x00550009,
			BLACKNESS = 0x00000042,
			WHITENESS = 0x00FF0062,
			CAPTUREBLT = 0x40000000
		}

		public enum DPI_AWARENESS_CONTEXT : int
		{
			DPI_AWARENESS_CONTEXT_UNAWARE =  -1,
			DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2,
			DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
			DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4,
		}

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr DC, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr SrcDC, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteDC(IntPtr hDC);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		public static bool SetThreadDpiAwarenessContextAvaliable = false;
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT dpiContext);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetThreadDpiAwarenessContext();


		//LoadLibrary / GetProcAddress is used to check if SetThreadDpiAwarenessContext exists (Win10 only)
		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

		public static bool DoesThreadAwarenessContextExist()
		{

			IntPtr dll_handle = LoadLibrary("user32.dll");
			uint proc_address = GetProcAddress(dll_handle, "SetThreadDpiAwarenessContext").ToUInt32();
			SetThreadDpiAwarenessContextAvaliable = proc_address != 0;
			return SetThreadDpiAwarenessContextAvaliable;
		}
	}
}
