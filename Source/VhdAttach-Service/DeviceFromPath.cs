using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace VhdAttachService {
    internal static class DeviceFromPath {

        public static string GetDevice(string path) {
            var device = FindCdRom(path);
            if (device == null) { device = FindPhysicalDrive(path); }
            return device;
        }


        #region PhysicalDrive

        private static string FindPhysicalDrive(string path) {
            FileSystemInfo iDirectory = null;
            var wmiQuery = new ObjectQuery("SELECT Antecedent, Dependent FROM Win32_LogicalDiskToPartition");
            var wmiSearcher = new ManagementObjectSearcher(wmiQuery);

            var iFile = new FileInfo(path);
            try {
                if ((iFile.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
                    iDirectory = new DirectoryInfo(iFile.FullName);
                } else {
                    iDirectory = iFile;
                    throw new FormatException("Argument is not a directory.");
                }
            } catch (IOException) {
                iDirectory = new DirectoryInfo(iFile.FullName);
            }


            var wmiPhysicalDiskNumber = -1;
            foreach (var iReturn in wmiSearcher.Get()) {
                var disk = GetSubsubstring((string)iReturn["Antecedent"], "Win32_DiskPartition.DeviceID", "Disk #", ",");
                var partition = GetSubsubstring((string)iReturn["Dependent"], "Win32_LogicalDisk.DeviceID", "", "");
                if (iDirectory.Name.StartsWith(partition, StringComparison.InvariantCultureIgnoreCase)) {
                    if (int.TryParse(disk, NumberStyles.Integer, CultureInfo.InvariantCulture, out wmiPhysicalDiskNumber)) {
                        return @"\\?\PHYSICALDRIVE" + wmiPhysicalDiskNumber.ToString(CultureInfo.InvariantCulture);
                    } else {
                        throw new FormatException("Cannot retrieve physical disk number.");
                    }
                }
            }
            return null;
        }

        private static string GetSubsubstring(string value, string type, string start, string end) {
            var xStart0 = value.IndexOf(":" + type + "=\"");
            if (xStart0 < 0) { return null; }
            var xStart1 = value.IndexOf("\"", xStart0 + 1);
            if (xStart1 < 0) { return null; }
            var xEnd1 = value.IndexOf("\"", xStart1 + 1);
            if (xEnd1 < 0) { return null; }
            var extract = value.Substring(xStart1 + 1, xEnd1 - xStart1 - 1);

            int xStart2 = 0;
            if (!string.IsNullOrEmpty(start)) { xStart2 = extract.IndexOf(start); }
            if (xStart2 < 0) { return null; }

            int xEnd2 = extract.Length;
            if (!string.IsNullOrEmpty(end)) { xEnd2 = extract.IndexOf(end); }
            if (xEnd2 < 0) { return null; }

            return extract.Substring(xStart2 + start.Length, xEnd2 - xStart2 - start.Length);
        }

        #endregion


        #region CdRom

        private static string FindCdRom(string path) {
            var dosDevice = path[0] + ":";
            var sb = new StringBuilder(64);
            if (NativeMethods.QueryDosDeviceW(dosDevice, sb, (uint)sb.Capacity) > 0) {
                var dosPath = sb.ToString();
                Debug.WriteLine(sb.ToString() + " is at " + dosDevice);
                if (dosPath.StartsWith(@"\Device\CdRom", StringComparison.OrdinalIgnoreCase)) {
                    int cdromNumber = 0;
                    if (int.TryParse(dosPath.Substring(13), NumberStyles.Integer, CultureInfo.InvariantCulture, out cdromNumber)) {
                        return @"\\?\CDROM" + cdromNumber.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            return null;
        }


        private static class NativeMethods {

            [DllImportAttribute("kernel32.dll", EntryPoint = "QueryDosDeviceW")]
            public static extern UInt32 QueryDosDeviceW(
                [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpDeviceName,
                [OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpTargetPath,
                UInt32 ucchMax);

        }

        #endregion

    }
}
