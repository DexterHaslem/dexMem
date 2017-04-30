/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Collections.Generic;

namespace DexMem.Engine
{
    public class MemorySnapshot
    {
        public Dictionary<IntPtr, MemoryChunk> ByChunks { get; private set; }
        public IntPtr BaseAddress { get; private set; }
        public long TotalCommittedSize { get; private set; }
        public DateTime TakenAt { get; private set; }

        public MemorySnapshot(Dictionary<IntPtr, MemoryChunk> byChunks, IntPtr baseAddress, long totalCommittedSize,
            DateTime takenAt)
        {
            ByChunks = byChunks;
            BaseAddress = baseAddress;
            TotalCommittedSize = totalCommittedSize;
            TakenAt = takenAt;
        }
    }
}