using System;
using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PebLdrData
    {
        public uint Length;
        public bool Initialized;
        public IntPtr SsHandle;
        public ListEntry InLoadOrderModuleList;
        public ListEntry InMemoryOrderModuleList;
        public ListEntry InInitializationOrderModuleList;
    }
}
