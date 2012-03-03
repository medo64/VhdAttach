using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace VhdAttach {
    internal static class LettersFromDrivePath {

        public static string[] GetLetters(string drivePath) {
            int driveNumber;
            if (drivePath.StartsWith(@"\\.\PHYSICALDRIVE", StringComparison.InvariantCulture) && int.TryParse(drivePath.Substring(17), NumberStyles.Integer, CultureInfo.InvariantCulture, out driveNumber)) {
                throw new NotSupportedException();
            } else if (drivePath.StartsWith(@"\\.\CDROM", StringComparison.InvariantCulture) && int.TryParse(drivePath.Substring(9), NumberStyles.Integer, CultureInfo.InvariantCulture, out driveNumber)) {
                return GetLettersFromCdrom(driveNumber);
            } else {
                return null;
            }
        }


        private static string[] GetLettersFromCdrom(int driveNumber) {
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


        private static class NativeMethods {

            [DllImportAttribute("kernel32.dll", EntryPoint = "QueryDosDeviceW")]
            public static extern UInt32 QueryDosDeviceW(
                [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpDeviceName,
                [OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpTargetPath,
                UInt32 ucchMax);

        }

    }
}
