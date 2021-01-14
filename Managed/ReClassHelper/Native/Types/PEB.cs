using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PEB
    {
        public byte InheritedAddressSpace;
        public byte ReadImageFileExecOptions;
        public byte BeingDebugged;
        public byte BitField;
        public IntPtr Mutant;
        public IntPtr ImageBaseAddress;
        public IntPtr Ldr;
        public IntPtr ProcessParameters;
        public IntPtr SubSystemData;
        public IntPtr ProcessHeap;
        public IntPtr FastPebLock;
        public IntPtr AtlThunkSListPtr;
        public IntPtr IFEOKey;
        public IntPtr CrossProcessFlags;
        public IntPtr KernelCallbackTable;
        public uint SystemReserved;
        public uint AtlThunkSListPtr32;
        public IntPtr ApiSetMap;
    }
}
