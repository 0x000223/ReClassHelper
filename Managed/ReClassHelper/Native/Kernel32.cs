using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace ReClassHelper.Native
{
    public static class Kernel32
    {
        //[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint ioControlCode,
        //    [MarshalAs(UnmanagedType.AsAny)][In] object inBuffer,
        //    uint nInBufferSize,
        //    [MarshalAs(UnmanagedType.AsAny)][Out] object outBuffer,
        //    uint nOutBufferSize,
        //    ref int pBytesReturned, IntPtr overlapped);

        //[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern SafeFileHandle CreateFile(string fileName,
        //    [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
        //    [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
        //    IntPtr securityAttributes,
        //    [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        //    [MarshalAs(UnmanagedType.U4)] FileAttributes flags,
        //    IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string filename, FileAccess access, FileShare sharing, 
            IntPtr SecurityAttributes, FileMode mode, FileOptions options, IntPtr template);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeviceIoControl(
                                        IntPtr hDevice,
                                        uint dwIoControlCode,
                                        IntPtr InBuffer,
                                        int nInBufferSize,
                                        IntPtr OutBuffer,
                                        int nOutBufferSize,
                                        out int pBytesReturned,
                                        IntPtr lpOverlapped);
    }
}
