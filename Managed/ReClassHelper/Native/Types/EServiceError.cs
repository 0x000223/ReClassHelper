using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    public enum EServiceError
    {
        Ignore   = 0x00000000,
        Normal   = 0x00000001,
        Severe   = 0x00000002,
        Critical = 0x00000003
    }
}
