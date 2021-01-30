using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PebLdrData32
    {
        public uint Length;
        public bool Initialized;
        public uint SsHandle;
        public ListEntry32 InLoadOrderModuleList;
        public ListEntry32 InMemoryOrderModuleList;
        public ListEntry32 InInitializationOrderModuleList;
    }
}
