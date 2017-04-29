/*
 * dexMem 
 * Dexter Haslem 2017
 * see the LICENSE file for licensing details
*/

using System;

namespace DexMem.Scanner
{
    /// <summary>
    /// MemoryChunk is a region of consecutive pages taht share the same attributes
    /// </summary>
    public class MemoryChunk
    {
        public IntPtr BaseAddress { get; }
        public uint RegionSize { get; }
        public byte[] Contents { get; set; }

        public MemoryChunk(IntPtr baseAddr, uint regionSize)
        {
            BaseAddress = baseAddr;
            RegionSize = regionSize;
            //Contents = contents;
        }
    }
}