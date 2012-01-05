using VhdAttach;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace VhdAttachTest {

    [TestClass()]
    public class VhdFooterTest {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void VhdFooter_Parse1() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-FF-FF-FF-FF-FF-FF-FF-FF-15-DC-BD-20-77-69-6E-20-00-06-00-01-57-69-32-6B-00-00-00-00-C0-00-00-00-00-00-00-00-C0-00-00-00-18-61-10-3F-00-00-00-02-FF-FF-E5-66-3A-E3-A9-5E-DD-1D-A3-49-BD-6D-3C-D7-90-5A-B6-70-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual((UInt64)0xFFFFFFFFFFFFFFFF, target.DataOffset);
            Assert.AreEqual(new DateTime(2011, 8, 16, 5, 31, 12, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("win ", target.CreatorApplication);
            Assert.AreEqual(new Version(6, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)3221225472, target.OriginalSize);
            Assert.AreEqual((UInt64)3221225472, target.CurrentSize);
            Assert.AreEqual(6241, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(63, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.FixedHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-E5-66", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("5ea9e33a-1ddd-49a3-bd6d-3cd7905ab670"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }

        [TestMethod()]
        public void VhdFooter_Parse2() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-FF-FF-FF-FF-FF-FF-FF-FF-15-74-6E-C8-77-69-6E-20-00-06-00-01-57-69-32-6B-00-00-00-08-00-00-00-00-00-00-00-08-00-00-00-00-40-40-10-FF-00-00-00-02-FF-FF-E5-9D-48-A5-FD-A9-74-81-23-4D-B3-89-E9-41-D2-33-7E-F7-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual((UInt64)0xFFFFFFFFFFFFFFFF, target.DataOffset);
            Assert.AreEqual(new DateTime(2011, 5, 29, 2, 41, 12, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("win ", target.CreatorApplication);
            Assert.AreEqual(new Version(6, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)34359738368, target.OriginalSize);
            Assert.AreEqual((UInt64)34359738368, target.CurrentSize);
            Assert.AreEqual(16448, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(255, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.FixedHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-E5-9D", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("a9fda548-8174-4d23-b389-e941d2337ef7"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }

        [TestMethod()]
        public void VhdFooter_Parse3() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-FF-FF-FF-FF-FF-FF-FF-FF-16-70-DF-D8-77-69-6E-20-00-06-00-01-57-69-32-6B-00-00-00-19-00-00-00-00-00-00-00-19-00-00-00-00-C8-C8-10-FF-00-00-00-02-FF-FF-E2-A5-BD-C1-AF-B7-E1-AE-AC-47-B7-F2-8B-C5-26-9F-0A-F2-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, target.DataOffset);
            Assert.AreEqual(new DateTime(2011, 12, 6, 14, 14, 48, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("win ", target.CreatorApplication);
            Assert.AreEqual(new Version(6, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)107374182400, target.OriginalSize);
            Assert.AreEqual((UInt64)107374182400, target.CurrentSize);
            Assert.AreEqual(51400, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(255, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.FixedHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-E2-A5", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("b7afc1bd-aee1-47ac-b7f2-8bc5269f0af2"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }

        [TestMethod()]
        public void VhdFooter_Parse4() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-00-00-00-00-00-00-02-00-16-7B-95-F6-76-62-6F-78-00-04-00-01-57-69-32-6B-00-00-00-05-00-00-00-00-00-00-00-05-00-00-00-00-A2-8A-10-3F-00-00-00-03-FF-FF-EE-A0-D1-AA-42-85-60-1B-FE-44-88-05-14-7A-D1-88-B5-10-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual((UInt64)512, target.DataOffset);
            Assert.AreEqual(new DateTime(2011, 12, 14, 17, 14, 30, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("vbox", target.CreatorApplication);
            Assert.AreEqual(new Version(4, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)21474836480, target.OriginalSize);
            Assert.AreEqual((UInt64)21474836480, target.CurrentSize);
            Assert.AreEqual(41610, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(63, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.DynamicHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-EE-A0", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("8542aad1-1b60-44fe-8805-147ad188b510"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }

        [TestMethod()]
        public void VhdFooter_Parse5() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-00-00-00-00-00-00-02-00-14-DB-F4-2D-64-32-76-00-00-01-00-00-57-69-32-6B-00-00-00-25-43-3D-60-00-00-00-00-25-43-3D-60-00-FF-FF-10-FF-00-00-00-03-FF-FF-EA-E4-52-1F-CD-53-39-A4-E5-4B-92-F2-88-99-48-B7-BC-27-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual((UInt64)512, target.DataOffset);
            Assert.AreEqual(new DateTime(2011, 2, 2, 10, 53, 33, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("d2v\0", target.CreatorApplication);
            Assert.AreEqual(new Version(1, 0), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)160041885696, target.OriginalSize);
            Assert.AreEqual((UInt64)160041885696, target.CurrentSize);
            Assert.AreEqual(65535, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(255, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.DynamicHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-EA-E4", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("53cd1f52-a439-4be5-92f2-889948b7bc27"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }

        [TestMethod()]
        public void VhdFooter_Parse6() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-00-00-00-00-00-00-02-00-16-98-8B-19-77-69-6E-20-00-06-00-01-57-69-32-6B-00-00-00-EE-2F-8D-30-00-00-00-00-EE-2F-8D-30-00-FF-FF-10-FF-00-00-00-03-FF-FF-E8-44-7D-D8-77-50-FA-9B-A6-42-90-33-C3-97-96-A6-9A-E5-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual((UInt64)512, target.DataOffset);
            Assert.AreEqual(new DateTime(2012, 1, 5, 16, 23, 53, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("win ", target.CreatorApplication);
            Assert.AreEqual(new Version(6, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)1022999998464, target.OriginalSize);
            Assert.AreEqual((UInt64)1022999998464, target.CurrentSize);
            Assert.AreEqual(65535, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(255, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.DynamicHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-E8-44", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("5077d87d-9bfa-42a6-9033-c39796a69ae5"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(true, target.IsChecksumCorrect);
        }

        [TestMethod()]
        public void VhdFooter_Parse7() {
            byte[] bytes = GetBytesFromHex("63-6F-6E-65-63-74-69-78-00-00-00-02-00-01-00-00-00-00-00-00-00-00-02-00-16-98-8B-19-77-69-6E-20-00-06-00-01-57-69-32-6B-00-00-00-EE-2F-8D-30-00-00-00-00-EE-2F-8D-30-00-FF-FF-10-FF-00-00-00-03-FF-FF-E8-44-7D-D8-77-50-FA-9B-A6-42-90-33-C3-97-96-A6-9A-E5-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01");
            VhdFooter target = new VhdFooter(bytes);
            Assert.AreEqual("conectix", target.Cookie);
            Assert.AreEqual(VhdFeature.Reserved, target.Features);
            Assert.AreEqual(new Version(1, 0), target.FileFormatVersion);
            Assert.AreEqual((UInt64)512, target.DataOffset);
            Assert.AreEqual(new DateTime(2012, 1, 5, 16, 23, 53, DateTimeKind.Utc), target.TimeStamp);
            Assert.AreEqual("win ", target.CreatorApplication);
            Assert.AreEqual(new Version(6, 1), target.CreatorVersion);
            Assert.AreEqual(VhdCreatorHostOs.Windows, target.CreatorHostOs);
            Assert.AreEqual((UInt64)1022999998464, target.OriginalSize);
            Assert.AreEqual((UInt64)1022999998464, target.CurrentSize);
            Assert.AreEqual(65535, target.DiskGeometryCylinders);
            Assert.AreEqual(16, target.DiskGeometryHeads);
            Assert.AreEqual(255, target.DiskGeometrySectors);
            Assert.AreEqual(VhdDiskType.DynamicHardDisk, target.DiskType);
            Assert.AreEqual("FF-FF-E8-43", BitConverter.ToString(target.Checksum));
            Assert.AreEqual(new Guid("5077d87d-9bfa-42a6-9033-c39796a69ae5"), target.UniqueId);
            Assert.AreEqual(false, target.SavedState);
            Assert.AreEqual(false, target.IsChecksumCorrect);
        }



        private static byte[] GetBytesFromHex(string hex) {
            hex = hex.Trim().Replace("-", "");
            if (hex.Length % 2 != 0) { throw new FormatException("Must have leading 0."); }
            var result = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2) {
                result[i / 2] = byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return result;
        }

    }
}
