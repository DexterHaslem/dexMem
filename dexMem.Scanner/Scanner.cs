/*
 * dexMem 
 * Dexter Haslem 2017
 * see the LICENSE file for licensing details
*/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static DexMem.Scanner.NativeMethods;

namespace DexMem.Scanner
{
    public class Scanner
    {
        private static readonly long MinApplicationAddress;

        private static readonly long MaxApplicationAddress;
        //private static uint _pageSize;

        static Scanner()
        {
            GetSystemInfo(out SYSTEM_INFO systemInfo);
            MinApplicationAddress = systemInfo.minimumApplicationAddress.ToInt64();
            MaxApplicationAddress = systemInfo.maximumApplicationAddress.ToInt64();
            //_pageSize = systemInfo.pageSize;
        }

        public static Dictionary<IntPtr, MemoryChunk> GetMemory(Debugee target)
        {
            if (!target.IsOpen)
                throw new InvalidOperationException("Debugee is not opened");

            // two step process, first we need to get all chunks of similar pages,
            // which is inherently linear, once that is done we can read em all at once
            var chunks = GetPageChunks(target);
            return ReadPageChunkContents(target, chunks);
        }

        private static Dictionary<IntPtr, MemoryChunk> GetPageChunks(Debugee target)
        {
            // get all consecuitive page(s) chunks in memory for this process that have the same attributes,
            // *and* are readable, otherwise we dont care
            var chunks = new Dictionary<IntPtr, MemoryChunk>();
            var addr = MinApplicationAddress;

            while (addr <= MaxApplicationAddress)
            {
                var pAddr = new IntPtr(addr);
                VirtualQueryEx(target.DebugHandle, pAddr, out MEMORY_BASIC_INFORMATION memInfo, 28);
                // NOTE: if you only get NOACCESS ensure you're running as admin.
                // wew lawd this makes CI builds fun!
                if (memInfo.Protect == AllocationProtectEnum.PAGE_READWRITE
                    && memInfo.MemoryState == MemoryStateEnum.MEM_COMMIT)
                {
                    // we have a committed page, save this chunk to be read
                    chunks[memInfo.BaseAddress] = new MemoryChunk(memInfo.BaseAddress, memInfo.RegionSize);
                }
                addr += memInfo.RegionSize;
            }

            return chunks;
        }

        private static Dictionary<IntPtr, MemoryChunk> ReadPageChunkContents(Debugee target,
            Dictionary<IntPtr, MemoryChunk> scans)
        {
            Parallel.ForEach(scans, mc =>
            {
                var memInfo = mc.Value;
                var buf = new byte[memInfo.RegionSize];

                if (!ReadProcessMemory(target.DebugHandle,
                    memInfo.BaseAddress, buf, memInfo.RegionSize, out int bytesRead))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
                }

                mc.Value.Contents = bytesRead > 0 ? buf : new byte[0];
            });
            return scans;
        }
    }
}