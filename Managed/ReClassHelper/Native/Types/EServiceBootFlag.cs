using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    public enum EServiceBootFlag
    {
        Start       = 0x00000000,
        SystemStart = 0x00000001,
        AutoStart   = 0x00000002,
        DemandStart = 0x00000003,
        Disabled    = 0x00000004
    }
}
