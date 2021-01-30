﻿using System.Runtime.InteropServices;

namespace ReClassHelper.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public class ServiceStatus
    {
        public int dwServiceType             = 0;
        public EServiceState dwCurrentState  = 0;
        public int dwControlsAccepted        = 0;
        public int dwWin32ExitCode           = 0;
        public int dwServiceSpecificExitCode = 0;
        public int dwCheckPoint              = 0;
        public int dwWaitHint                = 0;
    }
}
