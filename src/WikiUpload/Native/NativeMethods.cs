using System;
using System.Runtime.InteropServices;

namespace WikiUpload
{
    internal class NativeMethods
    {
        internal const Int32 WM_SYSCOMMAND = 0x112;
        internal const Int32 MF_SEPARATOR = 0x800;
        internal const Int32 MF_BYPOSITION = 0x400;
        internal const Int32 MF_STRING = 0x0;

        private NativeMethods() { }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);
    }
}
