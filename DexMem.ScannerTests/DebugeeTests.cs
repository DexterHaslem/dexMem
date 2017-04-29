using Microsoft.VisualStudio.TestTools.UnitTesting;
using DexMem.Scanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DexMem.Scanner.Tests
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
        }
    }
}