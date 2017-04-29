/*
 * dexMem 
 * Dexter Haslem 2017
 * see the LICENSE file for licensing details
*/
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
            var process = Process.GetCurrentProcess();
            using (var debugee = new Debugee(process))
            {
                debugee.Open();
                debugee.Close();
            }

            using (var debugee = new Debugee(process))
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