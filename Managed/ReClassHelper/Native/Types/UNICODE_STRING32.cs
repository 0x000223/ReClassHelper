using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UNICODE_STRING32
    {
        public ushort Length;
        public ushort MaximumLength;
        public uint Buffer;
    }
}
