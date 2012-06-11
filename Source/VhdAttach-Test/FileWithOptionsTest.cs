using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VhdAttachCommon;

namespace VhdAttachTest {

    [TestClass()]
    public class FileWithOptionsTest {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void Test_FileWithOptions_Read_01() {
            var x = new FileWithOptions(@"/readonly/E:\Virtual Disks\Install.vhd");
            Assert.AreEqual(@"E:\Virtual Disks\Install.vhd", x.FileName );
            Assert.AreEqual(true, x.ReadOnly);
            Assert.AreEqual(false, x.NoDriveLetter);
        }

        [TestMethod()]
        public void Test_FileWithOptions_Read_02() {
            var x = new FileWithOptions(@"E:\Virtual Disks\Floppy.vhd");
            Assert.AreEqual(@"E:\Virtual Disks\Floppy.vhd", x.FileName);
            Assert.AreEqual(false, x.ReadOnly);
            Assert.AreEqual(false, x.NoDriveLetter);
        }


        [TestMethod()]
        public void Test_FileWithOptions_Write_01() {
            var x = new FileWithOptions("Test.vhd");
            x.ReadOnly = true;
            Assert.AreEqual("Test.vhd", x.FileName);
            Assert.AreEqual(true, x.ReadOnly);
            Assert.AreEqual(false, x.NoDriveLetter);
            Assert.AreEqual("/readonly/Test.vhd", x.ToString());
        }

        [TestMethod()]
        public void Test_FileWithOptions_Write_02() {
            var x = new FileWithOptions("Test.vhd");
            x.ReadOnly = true;
            x.NoDriveLetter = true;
            Assert.AreEqual("Test.vhd", x.FileName);
            Assert.AreEqual(true, x.ReadOnly);
            Assert.AreEqual(true, x.NoDriveLetter);
            Assert.AreEqual("/readonly,nodriveletter/Test.vhd", x.ToString());
        }

        [TestMethod()]
        public void Test_FileWithOptions_Write_03() {
            var x = new FileWithOptions("/readonly/Test.vhd");
            Assert.AreEqual(true, x.ReadOnly);
            x.ReadOnly = false;
            Assert.AreEqual("Test.vhd", x.FileName);
            Assert.AreEqual(false, x.ReadOnly);
            Assert.AreEqual(false, x.NoDriveLetter);
            Assert.AreEqual("Test.vhd", x.ToString());
        }

    }
}
