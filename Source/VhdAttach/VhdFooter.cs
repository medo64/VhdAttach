using System;
using System.Text;

namespace VhdAttach {
    internal class VhdFooter {

        public VhdFooter(byte[] bytes) {
            if (bytes.Length != 512) { throw new FormatException("Footer must be 512 bytes long."); }

            this.Cookie = ASCIIEncoding.ASCII.GetString(bytes, 0, 8);

            this.Features = (VhdFeature)GetInt32(bytes, 8);

            var fileFormatVersionMajor = GetUInt16(bytes, 12);
            var fileFormatVersionMinor = GetUInt16(bytes, 14);
            this.FileFormatVersion = new Version(fileFormatVersionMajor, fileFormatVersionMinor);

            this.DataOffset = GetUInt64(bytes, 16);

            this.TimeStamp = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(GetInt32(bytes, 24));

            this.CreatorApplication = ASCIIEncoding.ASCII.GetString(bytes, 28, 4);

            var creatorVersionMajor = GetUInt16(bytes, 32);
            var creatorVersionMinor = GetUInt16(bytes, 34);
            this.CreatorVersion = new Version(creatorVersionMajor, creatorVersionMinor);

            this.CreatorHostOs = (VhdCreatorHostOs)GetInt32(bytes, 36);

            this.OriginalSize = GetUInt64(bytes, 40);

            this.CurrentSize = GetUInt64(bytes, 48);

            this.DiskGeometryCylinders = GetUInt16(bytes, 56);
            this.DiskGeometryHeads = bytes[58];
            this.DiskGeometrySectors = bytes[59];

            this.DiskType = (VhdDiskType)GetInt32(bytes, 60);

            this.Checksum = GetFooterChecksum(bytes);
            this.IsChecksumCorrect = (this.Checksum[0] == bytes[64]) && (this.Checksum[1] == bytes[65]) && (this.Checksum[2] == bytes[66]) && (this.Checksum[3] == bytes[67]);

            var uniqueIdBytes = new byte[16];
            Buffer.BlockCopy(bytes, 68, uniqueIdBytes, 0, 16);
            this.UniqueId = new Guid(uniqueIdBytes);

            this.SavedState = (bytes[84] == 1);
        }


        /// <summary>
        /// Cookies are used to uniquely identify the original creator of the hard disk image. 
        /// The values are case-sensitive. Microsoft uses the “conectix” string to identify this file as a hard disk image created by Microsoft Virtual Server, Virtual PC, and predecessor products. The cookie is stored as an eight-character ASCII string with the “c” in the first byte, the “o” in the second byte, and so on.
        /// </summary>
        public String Cookie { get; private set; }

        /// <summary>
        /// This is a bit field used to indicate specific feature support. The following table displays the list of features. Any fields not listed are reserved.
        /// Feature  Value:
        ///     No features enabled  (0x00000000): The hard disk image has no special features enabled in it. 
        ///     Temporary (0x00000001): This bit is set if the current disk is a temporary disk. A temporary disk designation indicates to an application that this disk is a candidate for deletion on shutdown.
        ///     Reserved  (0x00000002): This bit must always be set to 1.  All other bits are also reserved and should be set to 0.
        /// </summary>
        public VhdFeature Features { get; private set; }

        /// <summary>
        /// This field is divided into a major/minor version and matches the version of the specification used in creating the file. The most-significant two bytes are for the  major version. The least-significant two bytes are the minor version. This must match the file format specification. For the current specification, this field must  be initialized to 0x00010000. The major version will be incremented only when the file format is modified in such a way that it is no longer compatible with older versions of the file format. 
        /// </summary>
        public Version FileFormatVersion { get; private set; }

        /// <summary>
        /// This field holds the absolute byte offset, from the beginning of the file, to the next structure. This field is used for dynamic disks and differencing disks, but not fixed disks. For fixed disks, this field should be set to 0xFFFFFFFF.
        /// </summary>
        public UInt64 DataOffset { get; private set; }

        /// <summary>
        /// This field stores the creation time of a hard disk image. This is the number of seconds since January 1, 2000 12:00:00 AM in UTC/GMT.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// This field is used to document which application created the hard disk. The field is a left-justified text field. It uses a single-byte character set. If the hard disk is created by Microsoft Virtual PC, "vpc " is written in this field. If the hard disk image is created by Microsoft Virtual Server, then "vs  " is written in this field. Other applications should use their own unique identifiers.
        /// </summary>
        public String CreatorApplication { get; private set; }

        /// <summary>
        /// This field holds the major/minor version of the application that created the hard disk image. Virtual Server 2004 sets this value to 0x00010000 and Virtual PC 2004 sets this to 0x00050000. 
        /// </summary>
        public Version CreatorVersion { get; private set; }

        /// <summary>
        /// This field stores the type of host operating system this disk image is created on. 
        ///     Windows  0x5769326B (Wi2k)
        ///     Macintosh  0x4D616320 (Mac )
        /// </summary>
        public VhdCreatorHostOs CreatorHostOs { get; private set; }

        /// <summary>
        /// This field stores the size of the hard disk in bytes, from the perspective of the virtual machine, at creation time. This field is for informational purposes.
        /// </summary>
        public UInt64 OriginalSize { get; private set; }

        /// <summary>
        /// This field stores the current size of the hard disk, in bytes, from the perspective of the virtual machine. This value is same as the original size when the hard disk is created. This value can change depending on whether the hard disk is expanded.
        /// </summary>
        public UInt64 CurrentSize { get; private set; }

        /// <summary>
        /// This field stores the cylinder value for the hard disk.
        /// </summary>
        public UInt16 DiskGeometryCylinders { get; private set; }

        /// <summary>
        /// This field stores the heads value for the hard disk.
        /// </summary>
        public Byte DiskGeometryHeads { get; private set; }

        /// <summary>
        /// This field stores the sectors per track value for the hard disk.
        /// </summary>
        public Byte DiskGeometrySectors { get; private set; }

        /// <summary>
        /// Type of virtual disk.
        /// </summary>
        public VhdDiskType DiskType { get; private set; }

        /// <summary>
        /// This field holds a basic checksum of the hard disk footer. It is just a one’s complement of the sum of all the bytes in the footer without the checksum field.
        /// If the checksum verification fails, the Virtual PC and Virtual Server products will instead use the header. If the checksum in the header also fails, the file should be assumed to be corrupt. The pseudo-code for the algorithm used to determine the checksum can be found in the appendix of this document.
        /// </summary>
        public Byte[] Checksum { get; private set; }

        /// <summary>
        /// Every hard disk has a unique ID stored in the hard disk. This is used to identify the hard disk. This is a 128-bit universally unique identifier (UUID). This field is used to associate a parent hard disk image with its differencing hard disk image(s).
        /// </summary>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// This field holds a one-byte flag that describes whether the system is in saved state. If the hard disk is in the saved state the value is set to 1. Operations such as compaction and expansion cannot be performed on a hard disk in a saved state. 
        /// </summary>
        public Boolean SavedState { get; private set; }


        /// <summary>
        /// Gets whether checksum is correct.
        /// </summary>
        public Boolean IsChecksumCorrect { get; private set; }


        /// <summary>
        /// Returns bytes for given structure.
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBytes() {
            return null;
        }


        private static Byte[] GetFooterChecksum(byte[] footer) {
            int checksum = 0;
            for (int i = 0; i < footer.Length; i++) {
                if ((i >= 64) && (i < 68)) { continue; }
                checksum += footer[i];
            }
            checksum = ~checksum;
            return GetBytes(checksum);
        }


        private static Byte[] GetBytes(int value) {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            } else {
                return bytes;
            }
        }

        private static UInt16 GetUInt16(byte[] bytes, int offset) {
            if (BitConverter.IsLittleEndian) {
                return BitConverter.ToUInt16(new byte[] { bytes[offset + 1], bytes[offset + 0] }, 0);
            } else {
                return BitConverter.ToUInt16(bytes, offset);
            }
        }

        private static Int32 GetInt32(byte[] bytes, int offset) {
            if (BitConverter.IsLittleEndian) {
                return BitConverter.ToInt32(new byte[] { bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset + 0] }, 0);
            } else {
                return BitConverter.ToInt32(bytes, offset);
            }
        }

        private static UInt64 GetUInt64(byte[] bytes, int offset) {
            if (BitConverter.IsLittleEndian) {
                return BitConverter.ToUInt64(new byte[] { bytes[offset + 7], bytes[offset + 6], bytes[offset + 5], bytes[offset + 4], bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset + 0] }, 0);
            } else {
                return BitConverter.ToUInt64(bytes, offset);
            }
        }

    }


    internal enum VhdCreatorHostOs {
        Windows = 0x5769326B, //"Wi2k"
        Macintosh = 0x4D616320, //"Mac "
    }

    internal enum VhdDiskType {
        None = 0,
        FixedHardDisk = 2,
        DynamicHardDisk = 3,
        DifferencingHardDisk = 4,
    }

    internal enum VhdFeature {
        NoFeaturesEnabled = 0x00000000,
        Temporary = 0x00000001,
        Reserved = 0x00000002
    }

}
