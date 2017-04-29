/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DexMem.Engine
{
    /// <summary>
    /// Differ will create a snapshot of differences between two memory scans. 
    /// Unchanged memory is not included
    /// </summary>
    public class Differ
    {
        // take in the dicts directly instead of lists to avoid constant address lookups
        public static List<MemoryDiff> Diff(Dictionary<IntPtr, MemoryChunk> left, Dictionary<IntPtr, MemoryChunk> right)
        {
            var diffs = new List<MemoryDiff>();
            Parallel.ForEach(left, lm =>
            {
                if (!right.TryGetValue(lm.Key, out MemoryChunk rm))
                {
                    // right side didnt have it, could have been freed, etc
                    return;
                }

                // ensure size didnt change, it shouldnt but.. if they dont match dont try anything
                // tricky
                if (lm.Value.RegionSize != rm.RegionSize)
                {
                    return;
                }

                for (var offset = 0; offset < lm.Value.RegionSize; ++offset)
                {
                    var leftVal = lm.Value.Contents[offset];
                    var rightVal = rm.Contents[offset];

                    if (leftVal == rightVal)
                        continue;

                    lock (diffs)
                        diffs.Add(new MemoryDiff(rm.BaseAddress + offset, leftVal, rightVal));
                }
            });

            return diffs;
        }
    }
}
