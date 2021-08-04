using System;
using System.Runtime.InteropServices;

namespace WikiUpload
{

    internal class NativeMethods
    {
        internal const int WM_SYSCOMMAND = 0x112;
        internal const int MF_SEPARATOR = 0x800;
        internal const int MF_BYPOSITION = 0x400;
        internal const int MF_STRING = 0x0;

        internal const int SwShownormal = 1;
        internal const int SwShowminimized = 2;

        private NativeMethods() { }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

    }
}
