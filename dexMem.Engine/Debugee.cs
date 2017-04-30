/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static DexMem.Engine.NativeMethods;

namespace DexMem.Engine
{
    public class Debugee : IDisposable
    {
        private const ProcessAccessFlags ProcessPermissionFlags =
            ProcessAccessFlags.QueryInformation
            | ProcessAccessFlags.VirtualMemoryRead
            | ProcessAccessFlags.VirtualMemoryWrite;

        public Process Process { get; }
        public IntPtr DebugHandle { get; private set; }
        public bool IsOpen => DebugHandle != IntPtr.Zero;
        public string Name => Process != null ? $"{Process.Id} {Process.ProcessName} - {Process.MainWindowTitle}" : "";

        #region IDisposable native

        private void ReleaseUnmanagedResources()
        {
            if (IsOpen)
            {
                // dont bother checking the return value here, always zero
                CloseHandle(DebugHandle);
            }
            DebugHandle = IntPtr.Zero;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Debugee()
        {
            ReleaseUnmanagedResources();
        }

        #endregion

        public Debugee(Process process)
        {
            Process = process;
        }

        public void Open()
        {
            if (IsOpen)
                throw new AccessViolationException("Native handle is already open for this process");

            // open the process w/ read access, this will require running this process as admin
            DebugHandle = OpenProcess(ProcessPermissionFlags, false, Process.Id);
            if (DebugHandle == IntPtr.Zero)
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        }

        public void Close()
        {
            if (!IsOpen)
                throw new AccessViolationException("Native handle is not open");

            var closed = CloseHandle(DebugHandle);
            // always clear even if failed so we dont try again
            DebugHandle = IntPtr.Zero;
            if (!closed)
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        }


        public static IEnumerable<Debugee> Available => Process.GetProcesses()
            .OrderBy(p => p.Id)
            .Select(p => new Debugee(p));
    }
}