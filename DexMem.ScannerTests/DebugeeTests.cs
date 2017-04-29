using System;
using System.Diagnostics;
using DexMem.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DexMem.ScannerTests
{
    [TestClass]
    public class DebugeeTests
    {
        [TestMethod]
        public void OpenCloseTest()
        {
            Process process = Process.GetCurrentProcess();
            using (Debugee debugee = new Debugee(process))
            {
                debugee.Open();
                debugee.Close();
            }

            using (Debugee debugee = new Debugee(process))
            {
                debugee.Open();
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<AccessViolationException>(() => debugee.Open());

                debugee.Close();
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<AccessViolationException>(() => debugee.Close());
            }
        }
    }
}