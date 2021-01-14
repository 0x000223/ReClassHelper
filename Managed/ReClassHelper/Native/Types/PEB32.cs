using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PEB32
    {
        public byte InheritedAddressSpace;
        public byte ReadImageFileExecOptions;
        public byte BeingDebugged;
        public byte BitField;
        public uint Mutant;
        public uint ImageBaseAddress;
        public uint Ldr;
        public uint ProcessParameters;
        public uint SubSystemData;
        public uint ProcessHeap;
        public uint FastPebLock;
        public uint AtlThunkSListPtr;
        public uint IFEOKey;
        public uint CrossProcessFlags;
        public uint UserSharedInfoPtr;
        public uint SystemReserved;
        public uint AtlThunkSListPtr32;
        public uint ApiSetMap;
    }
}
