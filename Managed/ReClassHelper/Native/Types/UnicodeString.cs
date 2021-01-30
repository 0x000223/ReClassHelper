using System;
using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UnicodeString
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
    }
}
