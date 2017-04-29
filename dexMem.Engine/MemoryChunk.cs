/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Diagnostics;

namespace DexMem.Engine
{
    /// <summary>
    /// MemoryChunk is a region of consecutive pages taht share the same attributes
    /// </summary>
    [DebuggerDisplay("(<MemoryChunk> BaseAddr={BaseAddress} RegionSize={RegionSize})")]
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

        public MemoryChunk DeepClone()
        {
            var ret = new MemoryChunk(BaseAddress, RegionSize) {Contents = (byte[]) Contents.Clone()};
            return ret;
        }
    }
}