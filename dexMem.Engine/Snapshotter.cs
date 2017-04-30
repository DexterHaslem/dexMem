/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static DexMem.Engine.NativeMethods;

namespace DexMem.Engine
{
    public class Snapshotter
    {
        public static MemorySnapshot GetMemorySnapshot(Debugee target)
        {
            if (!target.IsOpen)
                throw new InvalidOperationException("Debugee is not opened");

            // two step process, first we need to get all chunks of similar pages,
            // which is inherently linear, once that is done we can read em all at once
            var chunks = GetPageChunks(target);
            var byChunks = ReadPageChunkContents(target, chunks);
            var totalSize = byChunks.Values.Sum(v => v.RegionSize);
            // TODO: real base address
            return new MemorySnapshot(byChunks, byChunks.Keys.FirstOrDefault(), totalSize, DateTime.Now);
        }

        private static Dictionary<IntPtr, MemoryChunk> GetPageChunks(Debugee target)
        {
            // get all consecuitive page(s) chunks in memory for this process that have the same attributes,
            // *and* are readable, otherwise we dont care
            var chunks = new Dictionary<IntPtr, MemoryChunk>();
            var addr = SystemInfo.MinApplicationAddress;

            while (addr <= SystemInfo.MaxApplicationAddress)
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