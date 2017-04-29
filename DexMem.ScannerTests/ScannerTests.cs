using System.Diagnostics;
using System.Linq;
using DexMem.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DexMem.ScannerTests
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
                var memScan = Scanner.Scanner.GetMemory(debugee);
                Assert.IsTrue(memScan.Count > 0);
                Assert.IsTrue(memScan.All(m => m.Value.Contents.Length > 0));
            }
            notepad.Kill();
        }
    }
}