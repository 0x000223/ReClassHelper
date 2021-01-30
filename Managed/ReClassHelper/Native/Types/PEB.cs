using System;
using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Peb
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
