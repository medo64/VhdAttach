using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace VhdAttach {
    internal static class LettersFromDrivePath {

        public static string[] GetLetters(string drivePath) {
            int driveNumber;
            if (drivePath.StartsWith(@"\\.\PHYSICALDRIVE", StringComparison.InvariantCulture) && int.TryParse(drivePath.Substring(17), NumberStyles.Integer, CultureInfo.InvariantCulture, out driveNumber)) {
                return GetLettersFromPhysicalDrive(driveNumber);
            } else if (drivePath.StartsWith(@"\\.\CDROM", StringComparison.InvariantCulture) && int.TryParse(drivePath.Substring(9), NumberStyles.Integer, CultureInfo.InvariantCulture, out driveNumber)) {
                return GetLettersFromCdRom(driveNumber);
            } else {
                return null;
            }
        }


        #region PhysicalDrive

        private static string[] GetLettersFromPhysicalDrive(int driveNumber) {
            var list = new List<string>();

            var wmiQuery = new ObjectQuery("SELECT Antecedent, Dependent FROM Win32_LogicalDiskToPartition");
            using (var wmiSearcher = new ManagementObjectSearcher(wmiQuery)) {
                foreach (var iReturn in wmiSearcher.Get()) {
                    var device = GetSubsubstring((string)iReturn["Antecedent"], "Win32_DiskPartition.DeviceID", "Disk #", ",");
                    var letter = GetSubsubstring((string)iReturn["Dependent"], "Win32_LogicalDisk.DeviceID", "", "");
                    int diskNumber;
                    if (int.TryParse(device, NumberStyles.Integer, CultureInfo.InvariantCulture, out diskNumber)) {
                        if (driveNumber == diskNumber) {
                            if ((letter.Length == 2) && (letter.EndsWith(":", StringComparison.OrdinalIgnoreCase))) {
                                list.Add(letter + @"\");
                            } else {
                                list.Add(letter);
                            }
                        }
                    }
                }
            }

            return list.ToArray();
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

        private static string[] GetLettersFromCdRom(int driveNumber) {
            for (char letter = 'A'; letter <= 'Z'; letter++) {
                var dosDevice = letter + ":";
                var sb = new StringBuilder(64);
                if (NativeMethods.QueryDosDeviceW(dosDevice, sb, (uint)sb.Capacity) > 0) {
                    var dosPath = sb.ToString();
                    Debug.WriteLine(sb.ToString() + " is at " + dosDevice);
                    if (dosPath.StartsWith(@"\Device\CdRom", StringComparison.OrdinalIgnoreCase)) {
                        int cdromNumber = 0;
                        if (int.TryParse(dosPath.Substring(13), NumberStyles.Integer, CultureInfo.InvariantCulture, out cdromNumber)) {
                            if (cdromNumber == driveNumber) {
                                return new string[] { dosDevice + @"\" };
                            }
                        }
                    }
                }
            }
            return null;
        }

        #endregion


        private static class NativeMethods {

            [DllImportAttribute("kernel32.dll", EntryPoint = "QueryDosDeviceW")]
            public static extern UInt32 QueryDosDeviceW(
                [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpDeviceName,
                [OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpTargetPath,
                UInt32 ucchMax);

        }

    }
}
