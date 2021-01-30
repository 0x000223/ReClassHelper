using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ListEntry32
    {
        public uint Flink;
        public uint Blink;
    }
}
