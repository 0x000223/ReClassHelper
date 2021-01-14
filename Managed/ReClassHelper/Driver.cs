using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using ReClassHelper.Native;
using ReClassHelper.Native.Types;
using ReClassNET.Core;

namespace ReClassHelper
{
   public partial class Driver
    {
        private const string SymLink = "\\\\.\\ReClassHelper";

        private const string ServiceName = "ReClassHelper";

        private static IntPtr handle;

        public Driver(string path)
        {
            var state = ServiceManager.GetServiceState(ServiceName);

            switch (state)
            {
                case EServiceState.NotFound:
                {
                    var serviceHandle = ServiceManager.CreateService(ServiceName, ServiceName, path + ServiceName + ".sys");

                    var status = ServiceManager.StartService(serviceHandle);

                    if (status is false)
                    {
                        ServiceManager.DeleteService(serviceHandle);

                        throw new ApplicationException($"Failed to start ReClassHelper service - {Marshal.GetLastWin32Error()}");
                    }

                    Advapi32.CloseServiceHandle(serviceHandle);

                    break;
                }
                case EServiceState.Stopped:
                {
                    var serviceHandle = ServiceManager.OpenService(ServiceName);

                    var status = ServiceManager.StartService(serviceHandle);

                    if (status is false)
                    {
                        ServiceManager.DeleteService(serviceHandle);

                        throw new ApplicationException($"Failed to start ReClassHelper service - {Marshal.GetLastWin32Error()}");
                    }

                    Advapi32.CloseServiceHandle(serviceHandle);

                    break;
                }
            }

            handle = 
                Kernel32.CreateFile(SymLink, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero,
                    FileMode.Open, 0, IntPtr.Zero);
        }

        ~Driver()
        {
            var serviceHandle = ServiceManager.OpenService(ServiceName);

            var status = new ServiceStatus();

            Advapi32.ControlService(serviceHandle, EServiceControl.Stop, status);

            ServiceManager.DeleteService(serviceHandle);

            Advapi32.CloseServiceHandle(serviceHandle);
        }

        public bool Read(int processId, IntPtr address, out byte[] buffer, int size)
        {
            buffer = new byte[size];
            var gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            var request = new ReadRequest()
            {
                ProcessId = processId,
                Address = address,
                Buffer =  gcHandle.AddrOfPinnedObject(),
                Size = size
            };

            var requestSize = Marshal.SizeOf(request);

            var pRequest = Marshal.AllocHGlobal(requestSize);
            Marshal.StructureToPtr(request, pRequest, false);

            var status =
                Kernel32.DeviceIoControl(
                    handle,         // Driver handle
                    IOCTL_READ,     // Control code
                    pRequest,       // Request struct
                    requestSize,    // Request size
                    IntPtr.Zero,    // Output buffer
                    0,              // Output size
                    out _,          // Returned bytes count
                    IntPtr.Zero);   // Async struct

            gcHandle.Free();
            Marshal.FreeHGlobal(pRequest);

            return status;
        }

        public T Read<T>(int processId, IntPtr address)
        {
            var type = typeof(T);
            var size = Marshal.SizeOf(type);
            var buffer = new byte[size];

            var gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            var request = new ReadRequest()
            {
                ProcessId = processId,
                Address = address,
                Buffer = gcHandle.AddrOfPinnedObject(),
                Size = size
            };

            var requestSize = Marshal.SizeOf(request);

            var pRequest = Marshal.AllocHGlobal(requestSize);
            Marshal.StructureToPtr(request, pRequest, false);

            var status =
                Kernel32.DeviceIoControl(
                    handle,         // Driver handle
                    IOCTL_READ,     // Control code
                    pRequest,       // Request struct
                    requestSize,    // Request size
                    IntPtr.Zero,    // Output
                    0,              // Output size
                    out _,          // Returned bytes count
                    IntPtr.Zero);   // Async struct

            var ret = (T) Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), type);

            gcHandle.Free();
            Marshal.FreeHGlobal(pRequest);

            return status ? ret : default;
        }

        public string ReadUnicode(int processId, IntPtr address, int length)
        {
            var status = Read(processId, address, out var buffer, length);

            return status ? Encoding.Unicode.GetString(buffer, 0, length) : string.Empty;
        }

        public bool Write(int processId, IntPtr address, byte[] buffer, int size)
        {
            // Check if driver is running

            var request = new WriteRequest()
            {
                ProcessId = processId,
                Address = address,
                Size = size,
                Buffer = Marshal.AllocHGlobal(size)
            };

            Marshal.Copy(buffer, 0, request.Buffer, size);

            var requestSize = Marshal.SizeOf(request);

            var pRequest = Marshal.AllocHGlobal(requestSize);
            Marshal.StructureToPtr(request, pRequest, false);

            var status =
                Kernel32.DeviceIoControl(
                    handle,         // Driver handle
                    IOCTL_WRITE,    // Control code
                    pRequest,       // Request struct
                    requestSize,    // Request size
                    IntPtr.Zero,    // Output buffer
                    0,              // Output size
                    out _,          // Returned bytes count
                    IntPtr.Zero);   // Async struct


            Marshal.FreeHGlobal(request.Buffer);
            Marshal.FreeHGlobal(pRequest);

            return status;
        }

        public bool GetProcessInfo(int processId, ref EnumerateProcessData data)
        {
            var request = new ProcessInfo()
            {
                ProcessId = processId,

                PebAddress = new IntPtr(),

                IsWow64 = false
            };

            var requestSize = Marshal.SizeOf(request);

            var pRequest = Marshal.AllocHGlobal(requestSize);
            Marshal.StructureToPtr(request, pRequest, false);

            var status =
                Kernel32.DeviceIoControl(
                    handle,             // Driver handle
                    IOCTL_PROCESS_INFO, // Control code
                    pRequest,           // Request struct
                    requestSize,        // Requst size
                    pRequest,           // Output buffer
                    requestSize,        // Output size
                    out _,              // Returned bytes count
                    IntPtr.Zero);       // Async struct

            if (status is false)
            {
                return false;
            }

            var result = (ProcessInfo) Marshal.PtrToStructure(pRequest, typeof(ProcessInfo));

            if (result.IsWow64)
            {
                // 32 bit

                var peb32 = Read<PEB32>(processId, result.PebAddress);

                var ldr = Read<PEB_LDR_DATA32>(processId, new IntPtr(peb32.Ldr));

                var head = new IntPtr(ldr.InLoadOrderModuleList.Flink);

                var entry = Read<LDR_DATA_TABLE_ENTRY32>(processId, head);

                data.Name = ReadUnicode(processId, new IntPtr(entry.BaseDllName.Buffer), entry.BaseDllName.Length);
                data.Path = ReadUnicode(processId, new IntPtr(entry.FullDllName.Buffer), entry.FullDllName.Length);
            }
            else
            {
                // 64 bit

                var peb = Read<PEB>(processId, result.PebAddress);

                var ldr = Read<PEB_LDR_DATA>(processId, peb.Ldr);

                var head = ldr.InLoadOrderModuleList.Flink;

                var entry = Read<LDR_DATA_TABLE_ENTRY>(processId, head);

                data.Name = ReadUnicode(processId, entry.BaseDllName.Buffer, entry.BaseDllName.Length);
                data.Path = ReadUnicode(processId, entry.FullDllName.Buffer, entry.FullDllName.Length);
            }

            Marshal.FreeHGlobal(pRequest);

            return status;
        }

        public bool GetProcessModules(int processId, ref EnumerateRemoteModuleCallback callback)
        {
            var request = new ProcessInfo()
            {
                ProcessId = processId
            };

            var requestSize = Marshal.SizeOf(request);

            var pRequest = Marshal.AllocHGlobal(requestSize);
            Marshal.StructureToPtr(request, pRequest, false);

            var status =
                Kernel32.DeviceIoControl(
                    handle,             // Driver handle
                    IOCTL_PROCESS_INFO, // Control code
                    pRequest,           // Request struct
                    requestSize,        // Request size
                    pRequest,           // Output buffer
                    requestSize,        // Output size
                    out _,              // Bytes returned count
                    IntPtr.Zero);       // Async struct 

            if (status is false)
            {
                return false;
            }

            var result = (ProcessInfo) Marshal.PtrToStructure(pRequest, typeof(ProcessInfo));

            if (result.IsWow64)
            {
                // 32 bit

                var peb32 = Read<PEB32>(processId, result.PebAddress);

                var ldr = Read<PEB_LDR_DATA32>(processId, new IntPtr(peb32.Ldr));

                var head = new IntPtr(ldr.InLoadOrderModuleList.Flink);
                var next = head;

                do
                {
                    var entry = Read<LDR_DATA_TABLE_ENTRY32>(processId, next);

                    next = new IntPtr(entry.InLoadOrderLinks.Flink);

                    if (entry.SizeOfImage == 0 || entry.DllBase == 0)
                    {
                        continue;
                    }

                    var data = new EnumerateRemoteModuleData()
                    {
                        BaseAddress = new IntPtr(entry.DllBase),
                        Path = ReadUnicode(processId, new IntPtr(entry.FullDllName.Buffer), entry.FullDllName.Length),
                        Size = new IntPtr(entry.SizeOfImage)
                    };

                    callback.Invoke(ref data);

                } while (next != head);
            }
            else
            {
                // 64 bit

                var peb = Read<PEB>(processId, result.PebAddress);

                var ldr = Read<PEB_LDR_DATA>(processId, peb.Ldr);

                var head = ldr.InLoadOrderModuleList.Flink;
                var next = head;

                do
                {
                    var entry = Read<LDR_DATA_TABLE_ENTRY>(processId, next);

                    next = entry.InLoadOrderLinks.Flink;

                    if (entry.SizeOfImage == 0 || entry.DllBase == IntPtr.Zero)
                    {
                        continue;
                    }

                    var data = new EnumerateRemoteModuleData()
                    {
                        BaseAddress = entry.DllBase,
                        Path = ReadUnicode(processId, entry.FullDllName.Buffer, entry.FullDllName.Length),
                        Size = new IntPtr(entry.SizeOfImage)
                    };

                    callback.Invoke(ref data);

                } while (next != head);
            }

            Marshal.FreeHGlobal(pRequest);

            return status;
        }
    } 
}
