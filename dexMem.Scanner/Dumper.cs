using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DexMem.Scanner
{
    public class Dumper
    {
        public static void DumpProcessMemory(string processName, string path)
        {
            NativeMethods.SYSTEM_INFO systemInfo;
            NativeMethods.GetSystemInfo(out systemInfo);

            var minAddr = systemInfo.minimumApplicationAddress;
            var maxAddr = systemInfo.maximumApplicationAddress;

            var foundProcesses = Process.GetProcessesByName(processName);
            if (foundProcesses.Length < 1)
            {
                throw new FileNotFoundException();
            }

            // if there is more than one running ignore it
            var process = foundProcesses[0];

            // open the process w/ read access, this will require running this process as admin
            var handle = NativeMethods.OpenProcess(
                NativeMethods.PROCESS_QUERY_INFORMATION | NativeMethods.PROCESS_WM_READ,
                false, process.Id);

            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            using (var bw = File.Create(path))
            {
                while (minAddr.ToInt64() < maxAddr.ToInt64())
                {
                    NativeMethods.MEMORY_BASIC_INFORMATION memInfo;
                    // TODO: magicnum
                    NativeMethods.VirtualQueryEx(handle, minAddr, out memInfo, 28);

                    // NOTE: if you only get NOACCESS ensure you're running as admin.
                    // wew lawd this makes CI builds fun!
                    if (memInfo.Protect == NativeMethods.AllocationProtectEnum.PAGE_READWRITE
                        && memInfo.State == NativeMethods.StateEnum.MEM_COMMIT)
                    {
                        // we have a committed page, read the whole chunk

                        var buf = new byte[memInfo.RegionSize];
                        NativeMethods.ReadProcessMemory(handle, memInfo.BaseAddress, buf,
                            memInfo.RegionSize, out int bytesRead);

                        bw.Write(buf, 0, bytesRead);
                        //const int chunkSize = 32;
                        //for (int i = 0; i < memInfo.RegionSize; i += chunkSize)
                        //{
                        //    // this is mostly nonsense
                        //    //var memStr = System.Text.Encoding.UTF8.GetString(buf, i, chunkSize);
                        //    //var chunkStr = string.Join("", buf.Select(b => (char)b));
                        //    //sw.WriteLine($"0x{memInfo.BaseAddress + i}: {chunkStr}");
                        //}
                        //foreach (var b in buf)
                        //    sw.Write(b);
                    }

                    minAddr = new IntPtr(minAddr.ToInt64() + memInfo.RegionSize);
                }
            }

        }
    }
}