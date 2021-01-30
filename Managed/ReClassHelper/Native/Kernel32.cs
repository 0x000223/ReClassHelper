using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ReClassHelper.Native
{
    public static class Kernel32
    {
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
