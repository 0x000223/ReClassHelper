using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LDR_DATA_TABLE_ENTRY32
    {
        public LIST_ENTRY32 InLoadOrderLinks;
        public LIST_ENTRY32 InMemoryOrderLinks;
        public LIST_ENTRY32 InInitializationOrderLinks;
        public uint DllBase;
        public uint EntryPoint;
        public uint SizeOfImage;
        public UNICODE_STRING32 FullDllName;
        public UNICODE_STRING32 BaseDllName;
        public uint Flags;
        public ushort LoadCount;
        public ushort TlsIndex;
        public LIST_ENTRY32 HashLinks;
        public uint TimeDateStamp;
    }
}
