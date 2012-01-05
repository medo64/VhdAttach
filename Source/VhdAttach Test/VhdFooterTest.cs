using VhdAttach;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace VhdAttachTest {

    [TestClass()]
    public class VhdFooterTest {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void VhdFooterConstructorTest() {
            byte[] bytes = GetBytesFromHex("636F6E65637469780000000200010000FFFFFFFFFFFFFFFF15DCBD2077696E20000600015769326B00000000C000000000000000C00000001861103F00000002FFFFE5663AE3A95EDD1DA349BD6D3CD7905AB6700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, target.DataOffset);
            Assert.AreEqual(new DateTime(2011, 8, 16, 5, 31, 12, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("win ", target.CreatorApplication);
            Assert.AreEqual(new Version(6, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual(3221225472, target.OriginalSize);
            Assert.AreEqual(3221225472, target.CurrentSize);
            Assert.AreEqual(6241, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(63, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.FixedHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-E5-66", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("5ea9e33a-1ddd-49a3-bd6d-3cd7905ab670"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }


        private static byte[] GetBytesFromHex(string hex) {
            if (hex.Length % 2 != 0) { throw new FormatException("Must have leading 0."); }
            var result = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2) {
                result[i / 2] = byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return result;
        }

    }
}
