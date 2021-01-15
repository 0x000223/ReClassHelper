using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReClassHelper.Native;
using ReClassHelper.Native.Types;

namespace ReClassHelper
{
    public class ServiceManager
    {
        private const int STANDARD_RIGHTS_REQUIRED  = 0xF0000;
        private const int SERVICE_KERNEL_DRIVER     = 0x00000001;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;

        private static IntPtr OpenServiceManager(EServiceControlManagerAccessRights accessRight)
        {
            var manager = Advapi32.OpenSCManager(null, null, accessRight);
            if (manager == IntPtr.Zero)
            {
                throw new ApplicationException($"Failed to connect to Service Manager: please run as administrator");
            }

            return manager;
        }

        public static IntPtr OpenService(string serviceName)
        {
            var manager = OpenServiceManager(EServiceControlManagerAccessRights.AllAccess);

            var service = Advapi32.OpenService(manager, serviceName, EServiceAccessRights.AllAccess);

            Advapi32.CloseServiceHandle(manager);

            return service;
        }

        public static IntPtr CreateService(string serviceName, string displayName, string pathName)
        {
            var manager = OpenServiceManager(EServiceControlManagerAccessRights.AllAccess);
            
            var service = 
                Advapi32.CreateService(
                    manager,                        // Handle service control manager
                    serviceName,                    // Service name
                    displayName,                    // Display name
                    EServiceAccessRights.AllAccess, // Access rights
                    SERVICE_KERNEL_DRIVER,          // Type
                    EServiceBootFlag.DemandStart,   // Boot flag
                    EServiceError.Normal,           // Error
                    pathName,                       // Full path
                    null,                           //
                    IntPtr.Zero,                    //
                    null,                           //
                    null,                           //
                    null);                          //

            Advapi32.CloseServiceHandle(manager);

            return service;
        }

        public static bool DeleteService(IntPtr service)
        {
            Advapi32.ControlService(service, EServiceControl.Stop, new ServiceStatus());

            var result = Advapi32.DeleteService(service);

            Advapi32.CloseServiceHandle(service);

            return result;
        }

        public static bool StartService(IntPtr service)
        {
            Advapi32.StartService(service, 0, null);

            var status = new ServiceStatus();

            Advapi32.QueryServiceStatus(service, status);

            while (status.dwCurrentState == EServiceState.StartPending)
            {
                var waitTime = status.dwWaitHint / 10;

                if (waitTime < 1000)
                {
                    waitTime = 1000;
                }
                else if (waitTime > 10000)
                {
                    waitTime = 10000;
                }

                Thread.Sleep(waitTime);

                Advapi32.QueryServiceStatus(service, status);
            }

            return status.dwCurrentState == EServiceState.Running;
        }

        public static void StopService(string serviceName)
        {
            var manager = OpenServiceManager(EServiceControlManagerAccessRights.AllAccess);

            var service = 
                Advapi32.OpenService(manager, serviceName, EServiceAccessRights.QueryStatus | EServiceAccessRights.Stop);

            if (service == IntPtr.Zero)
            {
                // * Handle failed open service *
            }

            var status = new ServiceStatus();

            Advapi32.ControlService(service, EServiceControl.Stop, status);

            // Parse status for change ? 

            Advapi32.CloseServiceHandle(manager);
            Advapi32.CloseServiceHandle(service);
        }

        public static EServiceState GetServiceState(IntPtr service)
        {
            var status = new ServiceStatus();

            Advapi32.QueryServiceStatus(service, status);

            return status.dwCurrentState;
        }

        public static EServiceState GetServiceState(string serviceName)
        {
            var manager = OpenServiceManager(EServiceControlManagerAccessRights.Connect);

            var service = 
                Advapi32.OpenService(manager, serviceName, EServiceAccessRights.QueryStatus);

            if (service == IntPtr.Zero)
            {
                return EServiceState.NotFound;
            }

            var state = GetServiceState(service);

            Advapi32.CloseServiceHandle(manager);
            Advapi32.CloseServiceHandle(service);

            return state;
        }
    }
}
