using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper
{
    public partial class Driver
    {
        private static readonly uint HELPER_DEVICE_TYPE    = 0x00222222;

        private static readonly uint IOCTL_READ            = CTL_CODE(HELPER_DEVICE_TYPE, 0x800, 0, 1 | 2);
        private static readonly uint IOCTL_WRITE           = CTL_CODE(HELPER_DEVICE_TYPE, 0x801, 0, 1 | 2);
        private static readonly uint IOCTL_PROCESS_INFO    = CTL_CODE(HELPER_DEVICE_TYPE, 0x802, 0, 1 | 2);

        private static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return ((deviceType << 16) | (access << 14) | (function << 2) | method);
        }

        private struct ReadRequest
        {
            public int    ProcessId;
            public IntPtr Address;
            public IntPtr Buffer;
            public int    Size;

            public ReadRequest(int processId, IntPtr address, IntPtr buffer, int size)
            {
                ProcessId = processId;
                Address   = address;
                Buffer    = buffer;
                Size      = size;
            }
        }

        private struct WriteRequest
        {
            public int    ProcessId;
            public IntPtr Address;
            public IntPtr Buffer;
            public int    Size;

            public WriteRequest(int processId, IntPtr address, IntPtr buffer, int size)
            {
                ProcessId = processId;
                Address   = address;
                Buffer    = buffer;
                Size      = size;
            }
        }

        private struct ProcessInfo
        {
            public int    ProcessId;
            public IntPtr PebAddress;
            public bool IsWow64;

            public ProcessInfo(int processId, IntPtr pebAddress, bool isWow64)
            {
                ProcessId  = processId;
                PebAddress = pebAddress;
                IsWow64    = isWow64;
            }
        }
    }
}
