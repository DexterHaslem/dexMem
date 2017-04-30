/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DexMem.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DexMem.EngineTests
{
    [TestClass()]
    public class DifferTests
    {
        [TestMethod()]
        public void DiffTest()
        {
            var testAddr = new IntPtr(1234);
            var size = 8;
            var shift = 4;
            var testValLeft = new MemoryChunk(testAddr, (uint) size)
            {
                Contents = new byte[] {1, 2, 3, 4, 5, 6, 7, 8}
            };

            var testValRight = testValLeft.DeepClone();
            testValRight.Contents = testValLeft.Contents.Select(v => v + shift)
                .Select(i => (byte) i) // not sure why, but no matter what prev select returns, must cast
                .ToArray();

            var left = new MemorySnapshot(new Dictionary<IntPtr, MemoryChunk>(), IntPtr.Zero, 1, DateTime.Now);
            var right = new MemorySnapshot(new Dictionary<IntPtr, MemoryChunk>(), IntPtr.Zero, 1, DateTime.Now);

            left.ByChunks[testAddr] = testValLeft;
            right.ByChunks[testAddr] = testValRight;
            var diffs1 = Differ.Diff(left, right);
            Assert.IsTrue(diffs1.All(d => d.Diff == shift));

            // set them nearly same and ensure diff only has actual diffs
            testValRight.Contents = new byte[] {1, 2, 3, 4, 10, 6, 7, 8};
            var diffs2 = Differ.Diff(left, right);
            Assert.IsTrue(diffs2.Count == 1);
            Assert.IsTrue(diffs2[0].Diff == 5);

            testValLeft.Contents = testValRight.Contents;
            var diffs3 = Differ.Diff(left, right);
            Assert.IsTrue(diffs3.Count == 0);
        }
    }
}