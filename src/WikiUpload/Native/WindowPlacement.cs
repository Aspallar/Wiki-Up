using System;
using System.Configuration;
using System.Runtime.InteropServices;

namespace WikiUpload
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int length;
        public int flags;
        public int showCmd;
        public Native.Point minPosition;
        public Native.Point maxPosition;
        public Native.Rect normalPosition;
    }
}
