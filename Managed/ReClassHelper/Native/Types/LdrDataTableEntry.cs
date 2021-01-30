using System;
using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct LdrDataTableEntry
    {
        public ListEntry InLoadOrderLinks;
        public ListEntry InMemoryOrderLinks;
        public ListEntry InInitializationOrderLinks;
        public IntPtr DllBase;
        public IntPtr EntryPoint;
        public uint SizeOfImage;
        public UnicodeString FullDllName;
        public UnicodeString BaseDllName;
        public uint Flags;
        public ushort LoadCount;
        public ushort TlsIndex;
        public ListEntry HashLinks;
        public uint TimeDateStamp;
    }
}
