using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ReClassHelper.Native.Types;
using ReClassNET.Core;

namespace ReClassHelper.Tests
{
    [TestFixture]
    public class DriverTest
    {
        [Test]
        public void ConstructorTest()
        {
            var path = "C:\\Users\\Max\\Documents\\Projects\\ReClassHelper\\bin\\Debug\\x64\\WDM Driver\\";

            var driver = new Driver(path);

            Assert.NotNull(driver);
        }

        [Test]
        public void GetProcessInfoTest()
        {
            var path = "C:\\Users\\Max\\Documents\\Projects\\ReClassHelper\\bin\\Debug\\x64\\WDM Driver\\";

            var driver = new Driver(path);

            var data = new EnumerateProcessData();

            var status = driver.GetProcessInfo(18288, ref data);

            Assert.IsTrue(status);
            Assert.NotNull(data);
        }

        [Test]
        public void ReadTest()
        {
            var path = Environment.CurrentDirectory + "\\bin\\Debug\\x64\\WDM Driver\\";

            var driver = new Driver(path);

            var result = driver.Read<PEB32>(18288,new IntPtr(0x2D6000));

            Assert.NotNull(result);
        }

        [Test]
        public void GetProcessModulesTest()
        {
            var driver = new Driver("C:\\Users\\Max\\Documents\\Projects\\ReClassHelper\\bin\\Debug\\x64\\WDM Driver\\");

            EnumerateRemoteModuleCallback cb = null;

            var result = driver.GetProcessModules(18288, ref cb);

            Assert.True(result);
        }


    }
}