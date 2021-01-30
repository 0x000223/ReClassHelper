using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UnicodeString32
    {
        public ushort Length;
        public ushort MaximumLength;
        public uint Buffer;
    }
}