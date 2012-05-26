using System;
using System.Text;

namespace VirtualHardDiskImage {

    internal class DynamicDiskHeader {

        public DynamicDiskHeader() {
            this.Bytes = new Byte[1024];
            this.BeginUpdate();
            this.Cookie = "cxsparse";
            this.DataOffset = 0xFFFFFFFFFFFFFFFF;
            this.TableOffset = 0;
            this.HeaderVersion = new Version(1, 0);
            this.MaxTableEntries = 0;
            this.BlockSize = 2097152; //2MB
            //this.ParentUniqueId = Guid.Empty;
            //this.ParentTimeStamp = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.EndUpdate();
        }

        /// <summary>
        /// Create new instance from existing footer bytes.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        public DynamicDiskHeader(byte[] bytes) {
            if (bytes == null) { throw new ArgumentNullException("bytes", "Argument bytes cannot be null."); }
            if (bytes.Length != 1024) { throw new FormatException("Dynamic disk header must be 1024 bytes long."); }

            this.Bytes = bytes;
        }


        /// <summary>
        /// This field holds the value "cxsparse". This field identifies the header.
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
        /// This field contains the absolute byte offset to the next structure in the hard disk image. It is currently unused by existing formats and should be set to 0xFFFFFFFF.
        /// </summary>
        public UInt64 DataOffset {
            get {
                return GetUInt64(this.Bytes, 8);
            }
            set {
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 8, 8);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the absolute byte offset of the Block Allocation Table (BAT) in the file.
        /// </summary>
        public UInt64 TableOffset {
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
        /// This field stores the version of the dynamic disk header. The field is divided into Major/Minor version. The least-significant two bytes represent the minor version, and the most-significant two bytes represent the major version. This must match with the file format specification. For this specification, this field must be initialized to 0x00010000.  The major version will be incremented only when the header format is modified in such a way that it is no longer compatible with older versions of the product.
        /// </summary>
        public Version HeaderVersion {
            get {
                var major = GetUInt16(this.Bytes, 24);
                var minor = GetUInt16(this.Bytes, 26);
                return new Version(major, minor);

            }
            set {
                var major = GetBytes((UInt16)value.Major);
                var minor = GetBytes((UInt16)value.Minor);
                Buffer.BlockCopy(major, 0, this.Bytes, 24, 2);
                Buffer.BlockCopy(minor, 0, this.Bytes, 26, 2);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field holds the maximum entries present in the BAT. This should be equal to the number of blocks in the disk (that is, the disk size divided by the block size).
        /// </summary>
        public UInt32 MaxTableEntries {
            get {
                return GetUInt32(this.Bytes, 28);
            }
            set {
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 28, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// A block is a unit of expansion for dynamic and differencing hard disks. It is stored in bytes. This size does not include the size of the block bitmap. It is only the size of the data section of the block. The sectors per block must always be a power of two. The default value is 0x00200000 (indicating a block size of 2 MB).
        /// </summary>
        public UInt32 BlockSize {
            get {
                return GetUInt32(this.Bytes, 32);
            }
            set {
                //throw new ArgumentException("Argument must be power of two.", "value"); }
                var buffer = GetBytes(value);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 32, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field holds a basic checksum of the dynamic header. It is a one’s complement of the sum of all the bytes in the header without the checksum field.
        /// If the checksum verification fails the file should be assumed to be corrupt.
        /// </summary>
        public Byte[] Checksum {
            get {
                var buffer = new Byte[4];
                Buffer.BlockCopy(this.Bytes, 36, buffer, 0, 4);
                return buffer;
            }
            set {
                if (value == null) { throw new ArgumentNullException("value", "Value cannot be null."); }
                if (value.Length != 4) { throw new ArgumentException("Value must be 4 bytes in length.", "value"); }
                Buffer.BlockCopy(value, 0, this.Bytes, 36, 4);
            }
        }

        /// <summary>
        /// Gets whether checksum is correct.
        /// </summary>
        public Boolean IsChecksumCorrect {
            get {
                var curr = this.Checksum;
                var valid = GetChecksum(this.Bytes);
                return (curr[0] == valid[0]) && (curr[1] == valid[1]) && (curr[2] == valid[2]) && (curr[3] == valid[3]);
            }
        }

        /// <summary>
        /// This field is used for differencing hard disks. A differencing hard disk stores a 128-bit UUID of the parent hard disk.
        /// </summary>
        public Guid ParentUniqueId {
            get {
                var buffer = new byte[16];
                Buffer.BlockCopy(this.Bytes, 40, buffer, 0, 16);
                return new Guid(buffer);
            }
            set {
                var buffer = value.ToByteArray();
                Buffer.BlockCopy(buffer, 0, this.Bytes, 40, 16);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field contains a Unicode string (UTF-16) of the parent hard disk filename.
        /// </summary>
        public String ParentUnicodeName {
            get {
                return UnicodeEncoding.Unicode.GetString(this.Bytes, 64, 512);
            }
            set {
                if (value == null) { throw new ArgumentNullException("value", "Value cannot be null."); }
                var buffer = UnicodeEncoding.Unicode.GetBytes(value);
                if (buffer.Length > 512) { throw new ArgumentException("File name must be 512 or less characters.", "value"); }
                Array.Resize(ref buffer, 512);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 64, 512);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }

        /// <summary>
        /// This field stores the modification time stamp of the parent hard disk. This is the number of seconds since January 1, 2000 12:00:00 AM in UTC/GMT.
        /// </summary>
        public DateTime ParentTimeStamp {
            get {
                return new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(GetUInt32(this.Bytes, 56));
            }
            set {
                var origin = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                if (value < origin) { value = origin; }
                var diff = (uint)(value - origin).TotalSeconds;
                var buffer = GetBytes(diff);
                Buffer.BlockCopy(buffer, 0, this.Bytes, 56, 4);
                if (this.UpdateCounter == 0) { this.UpdateChecksum(); }
            }
        }


        /// <summary>
        /// Bytes of header structure.
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
            var buffer = GetChecksum(this.Bytes);
            Buffer.BlockCopy(buffer, 0, this.Bytes, 36, 4);
        }

        private static Byte[] GetChecksum(byte[] bytes) {
            uint checksum = 0;
            for (int i = 0; i < bytes.Length; i++) {
                if ((i >= 36) && (i < 40)) { continue; }
                checksum += bytes[i];
            }
            checksum = ~checksum;
            return GetBytes(checksum);
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

}
