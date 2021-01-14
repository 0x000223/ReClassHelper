using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PEB_LDR_DATA32
    {
        public uint Length;
        public bool Initialized;
        public uint SsHandle; //ULONG SsHandle;
        public LIST_ENTRY32 InLoadOrderModuleList;
        public LIST_ENTRY32 InMemoryOrderModuleList;
        public LIST_ENTRY32 InInitializationOrderModuleList;
    }
}
