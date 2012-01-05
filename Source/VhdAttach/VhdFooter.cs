using System;
using System.Text;

namespace VhdAttach {

    /// <summary>
    /// Handling VHD Footer structure.
    /// </summary>
    internal class VhdFooter {

        /// <summary>
        /// Create new instance.
        /// </summary>
        public VhdFooter() {
            this.Bytes = new Byte[512];
            this.BeginUpdate();
            this.Cookie = "conectix";
            this.Features = VhdFeature.NoFeaturesEnabled;
            this.FileFormatVersion = new Version(1, 0);
            this.DataOffset = 0;
            this.TimeStamp = DateTime.UtcNow;
            this.CreatorApplication = VhdCreatorApplication.None;
            this.CreatorVersion = new Version(0, 0);
            this.CreatorHostOs = VhdCreatorHostOs.Windows;
            this.OriginalSize = 0;
            this.CurrentSize = 0;
            this.DiskGeometryCylinders = 0;
            this.DiskGeometryHeads = 0;
            this.DiskGeometrySectors = 0;
            this.DiskType = VhdDiskType.None;
            this.UniqueId = Guid.NewGuid();
            this.SavedState = false;
            this.EndUpdate();
        }

        /// <summary>
        /// Create new instance from existing footer bytes.
        /// </summary>
        /// <param name="Bytes">Footer bytes.</param>
        public VhdFooter(byte[] bytes) {
            if (bytes == null) { throw new ArgumentNullException("bytes", "Argument bytes cannot be null."); }
            if (bytes.Length != 512) { throw new FormatException("Footer must be 512 bytes long."); }

            this.Bytes = bytes;
        }


        /// <summary>
        /// Cookies are used to uniquely identify the original creator of the hard disk image. 
        /// The values are case-sensitive. Microsoft uses the “conectix” string to identify this file as a hard disk image created by Microsoft Virtual Server, Virtual PC, and predecessor products. The cookie is stored as an eight-character ASCII string with the “c” in the first byte, the “o” in the second byte, and so on.
        /// </summary>
        public String Cookie {
            get {
                return ASCIIEncoding.ASCII.GetString(this.Bytes, 0, 8);
            }
            set {
                if (value == null) { throw new ArgumentNullException("value", "Value cannot be null."); }
                var buffer = ASCIIEncoding.ASCII.GetBytes(value);
                if (buffer.Length != 8) { throw new ArgumentException("Cookie must be 8 ASCII characters long.", "value"); }
                Buffer.BlockCopy(buffer, 0, this.Bytes, 0, 8);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This is a bit field used to indicate specific feature support. The following table displays the list of features. Any fields not listed are reserved.
        /// Feature  Value:
        ///     No features enabled  (0x00000000): The hard disk image has no special features enabled in it. 
        ///     Temporary (0x00000001): This bit is set if the current disk is a temporary disk. A temporary disk designation indicates to an application that this disk is a candidate for deletion on shutdown.
        ///     Reserved  (0x00000002): This bit must always be set to 1.  All other bits are also reserved and should be set to 0.
        /// </summary>
        public VhdFeature Features {
            get {
                return (VhdFeature)GetInt32(this.Bytes, 8);
            }
            set {
                var buffer = GetBytes((int)value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 8, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field is divided into a major/minor version and matches the version of the specification used in creating the file. The most-significant two bytes are for the  major version. The least-significant two bytes are the minor version. This must match the file format specification. For the current specification, this field must  be initialized to 0x00010000. The major version will be incremented only when the file format is modified in such a way that it is no longer compatible with older versions of the file format. 
        /// </summary>
        public Version FileFormatVersion {
            get {
                var major = GetUInt16(Bytes, 12);
                var minor = GetUInt16(Bytes, 14);
                return new Version(major, minor);
            }
            set {
                var major = GetBytes((UInt16)value.Major);
                var minor = GetBytes((UInt16)value.Minor);
                Buffer.BlockCopy(major, 0, this.Bytes, 12, 2);
                Buffer.BlockCopy(minor, 0, this.Bytes, 14, 2);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field holds the absolute byte offset, from the beginning of the file, to the next structure. This field is used for dynamic disks and differencing disks, but not fixed disks. For fixed disks, this field should be set to 0xFFFFFFFF.
        /// </summary>
        public UInt64 DataOffset {
            get {
                return GetUInt64(this.Bytes, 16);
            }
            set {
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 16, 8);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the creation time of a hard disk image. This is the number of seconds since January 1, 2000 12:00:00 AM in UTC/GMT.
        /// </summary>
        public DateTime TimeStamp {
            get {
                return new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(GetUInt32(this.Bytes, 24));
            }
            set {
                var origin = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                if (value < origin) { value = origin; }
                var diff = (uint)(value - origin).TotalSeconds;
                var buffer = GetBytes(diff);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 24, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field is used to document which application created the hard disk. The field is a left-justified text field. It uses a single-byte character set. If the hard disk is created by Microsoft Virtual PC, "vpc " is written in this field. If the hard disk image is created by Microsoft Virtual Server, then "vs  " is written in this field. Other applications should use their own unique identifiers.
        /// </summary>
        public VhdCreatorApplication CreatorApplication {
            get {
                return (VhdCreatorApplication)GetInt32(this.Bytes, 28);
            }
            set {
                var buffer = GetBytes((int)value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 28, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field holds the major/minor version of the application that created the hard disk image. Virtual Server 2004 sets this value to 0x00010000 and Virtual PC 2004 sets this to 0x00050000. 
        /// </summary>
        public Version CreatorVersion {
            get {
                var major = GetUInt16(this.Bytes, 32);
                var minor = GetUInt16(this.Bytes, 34);
                return new Version(major, minor);

            }
            set {
                var major = GetBytes((UInt16)value.Major);
                var minor = GetBytes((UInt16)value.Minor);
                Buffer.BlockCopy(major, 0, this.Bytes, 32, 2);
                Buffer.BlockCopy(minor, 0, this.Bytes, 34, 2);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the type of host operating system this disk image is created on. 
        ///     Windows  0x5769326B (Wi2k)
        ///     Macintosh  0x4D616320 (Mac )
        /// </summary>
        public VhdCreatorHostOs CreatorHostOs {
            get {
                return (VhdCreatorHostOs)GetInt32(this.Bytes, 36);
            }
            set {
                var buffer = GetBytes((int)value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 36, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the size of the hard disk in bytes, from the perspective of the virtual machine, at creation time. This field is for informational purposes.
        /// </summary>
        public UInt64 OriginalSize {
            get {
                return GetUInt64(this.Bytes, 40);
            }
            set {
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 40, 8);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the current size of the hard disk, in bytes, from the perspective of the virtual machine. This value is same as the original size when the hard disk is created. This value can change depending on whether the hard disk is expanded.
        /// </summary>
        public UInt64 CurrentSize {
            get {
                return GetUInt64(this.Bytes, 48);
            }
            set {
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 48, 8);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the cylinder value for the hard disk.
        /// </summary>
        public UInt16 DiskGeometryCylinders {
            get {
                return GetUInt16(this.Bytes, 56);
            }
            set {
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 56, 2);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the heads value for the hard disk.
        /// </summary>
        public Byte DiskGeometryHeads {
            get {
                return this.Bytes[58];
            }
            set {
                this.Bytes[58] = value;
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the sectors per track value for the hard disk.
        /// </summary>
        public Byte DiskGeometrySectors {
            get {
                return this.Bytes[59];
            }
            set {
                this.Bytes[59] = value;
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// Type of virtual disk.
        /// </summary>
        public VhdDiskType DiskType {
            get {
                return (VhdDiskType)GetInt32(this.Bytes, 60);
            }
            set {
                var buffer = GetBytes((int)value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 60, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field holds a basic checksum of the hard disk footer. It is just a one’s complement of the sum of all the bytes in the footer without the checksum field.
        /// If the checksum verification fails, the Virtual PC and Virtual Server products will instead use the header. If the checksum in the header also fails, the file should be assumed to be corrupt. The pseudo-code for the algorithm used to determine the checksum can be found in the appendix of this document.
        /// </summary>
        public Byte[] Checksum {
            get {
                var buffer = new Byte[4];
                Buffer.BlockCopy(this.Bytes, 64, buffer, 0, 4);
                return buffer;
            }
            set {
                if (value == null) { throw new ArgumentNullException("value", "Value cannot be null."); }
                if (value.Length != 4) { throw new ArgumentException("Value must be 4 bytes in length.", "value"); }
                Buffer.BlockCopy(value, 0, this.Bytes, 64, 4);
            }
        }

        /// <summary>
        /// Gets whether checksum is correct.
        /// </summary>
        public Boolean IsChecksumCorrect {
            get {
                var curr = this.Checksum;
                var valid = GetFooterChecksum(this.Bytes);
                return (curr[0] == valid[0]) && (curr[1] == valid[1]) && (curr[2] == valid[2]) && (curr[3] == valid[3]);
            }
        }


        /// <summary>
        /// Every hard disk has a unique ID stored in the hard disk. This is used to identify the hard disk. This is a 128-bit universally unique identifier (UUID). This field is used to associate a parent hard disk image with its differencing hard disk image(s).
        /// </summary>
        public Guid UniqueId {
            get {
                var buffer = new byte[16];
                Buffer.BlockCopy(this.Bytes, 68, buffer, 0, 16);
                return new Guid(buffer);
            }
            set {
                var buffer = value.ToByteArray();
                Buffer.BlockCopy(buffer, 0, this.Bytes, 68, 16);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field holds a one-byte flag that describes whether the system is in saved state. If the hard disk is in the saved state the value is set to 1. Operations such as compaction and expansion cannot be performed on a hard disk in a saved state. 
        /// </summary>
        public Boolean SavedState {
            get {
                return (this.Bytes[84] == 1);
            }
            set {
                this.Bytes[84] = (value) ? (Byte)1 : (Byte)0;
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }



        /// <summary>
        /// Bytes of footer structure.
        /// </summary>
        public Byte[] Bytes { get; private set; }


        private int UpdateCounter;

        /// <summary>
        /// Stops processing checksum updates until EndUpdate is called.
        /// </summary>
        public void BeginUpdate() {
            this.UpdateCounter += 1;
        }

        /// <summary>
        /// Recalculates fields not updated since BeginUpdate.
        /// </summary>
        public void EndUpdate() {
            this.UpdateCounter = Math.Max(this.UpdateCounter - 1, 0);
            if (this.UpdateCounter == 0) {
                UpdateChecksum();
            }
        }

        /// <summary>
        /// Updates checksum.
        /// </summary>
        public void UpdateChecksum() {
            var buffer = GetFooterChecksum(this.Bytes);
            Buffer.BlockCopy(buffer, 0, this.Bytes, 64, 4);
        }


        private static Byte[] GetFooterChecksum(byte[] footer) {
            uint checksum = 0;
            for (int i = 0; i < footer.Length; i++) {
                if ((i >= 64) && (i < 68)) { continue; }
                checksum += footer[i];
            }
            checksum = ~checksum;
            return GetBytes(checksum);
        }


        private static Byte[] GetBytes(UInt16 value) {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[1], bytes[0] };
            } else {
                return bytes;
            }
        }

        private static Byte[] GetBytes(Int32 value) {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            } else {
                return bytes;
            }
        }

        private static Byte[] GetBytes(UInt32 value) {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            } else {
                return bytes;
            }
        }

        private static Byte[] GetBytes(UInt64 value) {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0] };
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

        private static UInt32 GetUInt32(byte[] bytes, int offset) {
            if (BitConverter.IsLittleEndian) {
                return BitConverter.ToUInt32(new byte[] { bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset + 0] }, 0);
            } else {
                return BitConverter.ToUInt32(bytes, offset);
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


    internal enum VhdCreatorApplication {
        None = 0,
        JosipMedvedVhdAttach = 0x6a6d7661,          //"jmva"
        MicrosoftSysinternalsDisk2Vhd = 0x64327600, //"d2v\0"
        MicrosoftVirtualPC = 0x76706320,            //"vps "
        MicrosoftVirtualServer = 0x76732020,        //"vs  "
        MicrosoftWindows = 0x77696e20,              //"win "
        OracleVirtualBox = 0x76626f78,              //"vbox"
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
