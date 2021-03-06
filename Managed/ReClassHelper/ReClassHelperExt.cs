﻿using System;
using System.Diagnostics;
using System.Drawing;
using ReClassHelper.Properties;
using ReClassNET.Core;
using ReClassNET.Debugger;
using ReClassNET.Plugins;

namespace ReClassHelper
{
    public class ReClassHelperExt : Plugin, ICoreProcessFunctions
    {
        public override Image Icon => Resources.Icon;

        public override bool Initialize(IPluginHost host)
        {
            host = host ?? throw new ArgumentNullException(nameof(host));

            Driver.Initialize();

            host.Process.CoreFunctions.RegisterFunctions("ReClassHelper", this);

            return true;
        }

        public override void Terminate()
        {
            Driver.Terminate();
        }

        public void EnumerateProcesses(EnumerateProcessCallback callbackProcess)
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                // Skipping system processes
                if (process.Id == 0) 
                {
                    continue;
                }

                var data = new EnumerateProcessData();

                try
                {
                    data.Id   = new IntPtr(process.Id);
                    data.Name = process.MainModule.ModuleName;
                    data.Path = process.MainModule.FileName;
                }
                catch
                {
                    // If protected process

                    Driver.GetProcessInfo(process.Id, ref data);
                }

                callbackProcess.Invoke(ref data);
            }
        }

        public void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection,
            EnumerateRemoteModuleCallback callbackModule)
        {
            Driver.GetProcessModules(process.ToInt32(), ref callbackModule);

            // TODO - Add section enumeration
        }

        public IntPtr OpenRemoteProcess(IntPtr pid, ProcessAccess desiredAccess) => pid;

        public bool IsProcessValid(IntPtr process) => process != IntPtr.Zero;

        public void CloseRemoteProcess(IntPtr process) { }

        public bool ReadRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size)
        {
            var status = Driver.Read(process.ToInt32(), address, out var output, size);

            output.CopyTo(buffer, offset);

            return status;
        }

        public bool WriteRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size)
        {
            var tempBuffer = new byte[size];

            Buffer.BlockCopy(buffer, offset, tempBuffer, 0, size);

            var status = 
                Driver.Write(process.ToInt32(), address, tempBuffer, size);

            return status;
        }

        public void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action) { }

        public bool AttachDebuggerToProcess(IntPtr id) => false;

        public void DetachDebuggerFromProcess(IntPtr id) { }

        public bool AwaitDebugEvent(ref DebugEvent evt, int timeoutInMilliseconds) => false;

        public void HandleDebugEvent(ref DebugEvent evt) { }

        public bool SetHardwareBreakpoint(IntPtr id, IntPtr address, HardwareBreakpointRegister register,
            HardwareBreakpointTrigger trigger, HardwareBreakpointSize size, bool set) => false;
    }
}