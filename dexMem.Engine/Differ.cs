﻿/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System.Collections.Generic;
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
        // TODO: consider a lightweight, flat memory address list version of this
        public static List<MemoryDiff> Diff(MemorySnapshot left, MemorySnapshot right)
        {
            var diffs = new List<MemoryDiff>();
            Parallel.ForEach(left.ByChunks, lm =>
            {
                if (!right.ByChunks.TryGetValue(lm.Key, out MemoryChunk rm))
                {
                    // right side didnt have it, could have been freed, etc
                    return;
                }

                for (var offset = 0; offset < lm.Value.RegionSize; ++offset)
                {
                    var leftVal = lm.Value.Contents[offset];
                    // if right size changed on us, just bail early. dont think this is possible.. but..
                    if (rm.Contents.Length <= offset)
                        break;

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