﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClassHelper.Native.Types
{
    [Flags]
    public enum EServiceControlManagerAccessRights
    {
        Connect                = 0x0001,
        CreateService          = 0x0002,
        EnumerateService       = 0x0004,
        Lock                   = 0x0008,
        QueryLockStatus        = 0x0010,
        ModifyBootConfig       = 0x0020,
        StandardRightsRequired = 0xF0000,

        AllAccess = (StandardRightsRequired | Connect | CreateService |
                     EnumerateService | Lock | QueryLockStatus | ModifyBootConfig)
    }
}
