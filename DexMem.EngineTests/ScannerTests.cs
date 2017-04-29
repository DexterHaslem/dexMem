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
    public class ScannerTests
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
                var memScan = Scanner.GetMemory(debugee);
                Assert.IsTrue(memScan.Count > 0);
                Assert.IsTrue(memScan.All(m => m.Value.Contents.Length > 0));
            }
            notepad.Kill();
        }
    }
}