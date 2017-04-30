/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/
using System;

namespace DexMem.Engine
{
    /// <summary>
    /// MemoryDiff is the calculated differences between two memory locations. It's always calculated
    /// from the lowest granularity (byte) so it can be upcast without recalculating
    /// </summary>
    public class MemoryDiff
    {
        public IntPtr Address { get; }
        public byte Left { get; }
        public byte Right { get; }
        public byte Diff => (byte) (Right - Left); // TODO: something that makes sense (this could overflow/uncheck)

        public MemoryDiff(IntPtr addr, byte left, byte right)
        {
            Address = addr;
            Left = left;
            Right = right;
        }
    }
}