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
        private const int SERVICE_KERNEL_DRIVER = 1; // Service type

        private static IntPtr OpenServiceManager(EServiceControlManagerAccessRights accessRight)
        {
            var manager = Advapi32.OpenSCManager(null, null, accessRight);
            
            return (manager != IntPtr.Zero) ? manager : 
                throw new ApplicationException("Failed to connect to Service Manager - please run as administrator");
        }

        public static IntPtr CreateService(string serviceName, string pathName)
        {
            var manager = OpenServiceManager(EServiceControlManagerAccessRights.AllAccess);
            
            var service = 
                Advapi32.CreateService(
                    manager,                        // Handle service control manager
                    serviceName,                    // Service name
                    serviceName,                    // Display name
                    EServiceAccessRights.AllAccess, // Access rights
                    SERVICE_KERNEL_DRIVER,          // Type
                    EServiceBootFlag.DemandStart,   // Boot flag
                    EServiceError.Normal,           // Error
                    pathName,                       // Full path
                    null,                           // Load group name
                    IntPtr.Zero,                    // Unique group tag
                    null,                           // Dependencies
                    null,                           // Account name
                    null);                          // Account password

            Advapi32.CloseServiceHandle(manager);

            return service;
        }

        public static bool DeleteService(IntPtr service)
        {
            // Deleting a Service (https://docs.microsoft.com/en-us/windows/win32/services/deleting-a-service)

            Advapi32.ControlService(service, EServiceControl.Stop, new ServiceStatus());

            var result = Advapi32.DeleteService(service);

            Advapi32.CloseServiceHandle(service);

            return result;
        }

        public static bool StartService(IntPtr service)
        {
            // Starting a Service (https://docs.microsoft.com/en-us/windows/win32/services/starting-a-service)

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
    }
}
