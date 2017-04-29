using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DexMem.Scanner
{
    public class Debugee : IDisposable
    {
        private const int ProcessPermissionFlags = NativeMethods.PROCESS_QUERY_INFORMATION | NativeMethods.PROCESS_WM_READ;

        public Process Process { get; }
        public IntPtr DebugHandle { get; private set; }
        public bool IsOpen => DebugHandle != IntPtr.Zero;

        #region IDisposable native
        private void ReleaseUnmanagedResources()
        {
            if (IsOpen)
            {
                // dont bother checking the return value here, always zero
                NativeMethods.CloseHandle(DebugHandle);
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
            DebugHandle = NativeMethods.OpenProcess(ProcessPermissionFlags, false, Process.Id);
            if (DebugHandle == IntPtr.Zero)
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        }

        public void Close()
        {
            var closed = NativeMethods.CloseHandle(DebugHandle);
            if (!closed)
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        }
    }
}
