using Microsoft.VisualStudio.TestTools.UnitTesting;
using DexMem.Scanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DexMem.Scanner.Tests
{
    [TestClass()]
    public class DumperTests
    {
        [TestMethod()]
        public void DumpProcessMemoryTest()
        {
            var notepad = Process.Start("notepad", "");
            var outFile = "notepad.mdump";
            File.Delete(outFile);

            Dumper.DumpProcessMemory("notepad", outFile);

            notepad.Kill();
            Assert.IsTrue(File.Exists(outFile));
        }
    }
}