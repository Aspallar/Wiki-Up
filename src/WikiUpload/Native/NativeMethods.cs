using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, IntPtr wIDNewItem, string lpNewItem);

        // The constants we'll use to identify our custom system menu items
        internal static readonly IntPtr AboutSysMenuId = new IntPtr(1001);

        internal static void CreateSystemMenu(IntPtr handle)
        {
            IntPtr systemMenuHandle = GetSystemMenu(handle, false);
            InsertMenu(systemMenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, IntPtr.Zero, string.Empty);
            InsertMenu(systemMenuHandle, 7, MF_BYPOSITION, AboutSysMenuId, "About...");
        }
    }
}
