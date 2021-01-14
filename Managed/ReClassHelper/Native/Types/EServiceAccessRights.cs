using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [Flags]
    public enum EServiceAccessRights
    {
        QueryConfig            = 0x1,
        ChangeConfig           = 0x2,
        QueryStatus            = 0x4,
        EnumerateDependants    = 0x8,
        Start                  = 0x10,
        Stop                   = 0x20,
        PauseContinue          = 0x40,
        Interrogate            = 0x80,
        UserDefinedControl     = 0x100,
        Delete                 = 0x00010000,
        StandardRightsRequired = 0xF0000,

        AllAccess = (StandardRightsRequired | QueryConfig | ChangeConfig |
                     QueryStatus | EnumerateDependants | Start | Stop | PauseContinue |
                     Interrogate | UserDefinedControl)
    }
}
