/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System.Diagnostics;
using System.Linq;
using DexMem.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DexMem.EngineTests
{
    [TestClass()]
    public class SnapshotterTests
    {
        [TestMethod()]
        public void ScanTest()
        {
            var notepad = Process.Start("notepad", "");
            if (!notepad.WaitForInputIdle(1000))
                return;
            using (var debugee = new Debugee(notepad))
            {
                debugee.Open();
                var snapshot = Snapshotter.GetMemorySnapshot(debugee);
                Assert.IsTrue(snapshot.ByChunks.Count > 0);
                Assert.IsTrue(snapshot.ByChunks.All(m => m.Value.Contents.Length > 0));
            }
            notepad.Kill();
        }
    }
}