using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LdrDataTableEntry32
    {
        public ListEntry32 InLoadOrderLinks;
        public ListEntry32 InMemoryOrderLinks;
        public ListEntry32 InInitializationOrderLinks;
        public uint DllBase;
        public uint EntryPoint;
        public uint SizeOfImage;
        public UnicodeString32 FullDllName;
        public UnicodeString32 BaseDllName;
        public uint Flags;
        public ushort LoadCount;
        public ushort TlsIndex;
        public ListEntry32 HashLinks;
        public uint TimeDateStamp;
    }
}
