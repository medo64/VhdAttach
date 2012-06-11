using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VhdAttachCommon {

    [DebuggerDisplay("{VolumeName} at {DriveLetter}")]
    internal class Volume {

        internal Volume(string volumeName) {
            this.VolumeName = volumeName;
        }

        public string VolumeName { get; private set; }

        public string DriveLetter {
            get {
                var volumePaths = new StringBuilder(4096);
                int volumePathsLength = 0;
                if (NativeMethods.GetVolumePathNamesForVolumeName(this.VolumeName, volumePaths, volumePaths.Capacity, out volumePathsLength)) {
                    foreach (var volume in volumePaths.ToString().Split('\0')) {
                        if (volume.Length == 3) { return volume; }
                    }
                }
                return null;
            }
        }

        public string DriveLetter2 {
            get {
                var letter = this.DriveLetter;
                return (letter != null) ? letter.Substring(0, 2) : null;
            }
        }

        public void ChangeLetter(string newLetter) {
            newLetter = ParseDriveLetter(newLetter);
            if (newLetter == this.DriveLetter) { return; } //nothing to do

            this.RemoveLetter();
            if (NativeMethods.SetVolumeMountPoint(newLetter, this.VolumeName) == false) {
                throw new Win32Exception();
            }
        }

        public void RemoveLetter() {
            if (this.DriveLetter != null) {
                if (NativeMethods.DeleteVolumeMountPoint(this.DriveLetter) == false) {
                    throw new Win32Exception();
                }
            }
        }


        public static Volume GetFromLetter(string driveLetter) {
            if (driveLetter == null) { throw new ArgumentNullException("driveLetter", "Argument cannot be null."); }
            driveLetter = ParseDriveLetter(driveLetter);
            if (driveLetter == null) { throw new ArgumentOutOfRangeException("driveLetter", "Drive letter expected."); }

            StringBuilder volumeName = new StringBuilder(50);
            if (NativeMethods.GetVolumeNameForVolumeMountPoint(driveLetter, volumeName, volumeName.Capacity)) {
                return new Volume(volumeName.ToString());
            } else {
                return null;
            }
        }

        private static string ParseDriveLetter(string driveLetter) {
            if (driveLetter == null) { return null; }
            driveLetter = driveLetter.Trim().ToUpperInvariant();
            if (!(driveLetter.EndsWith("\\", StringComparison.Ordinal))) { driveLetter += "\\"; }
            if ((driveLetter.Length != 3) || (driveLetter[0] < 'A') || (driveLetter[0] > 'Z') || (driveLetter[1] != ':') || (driveLetter[2] != '\\')) { return null; }
            return driveLetter;
        }


        private class NativeMethods {

            [DllImportAttribute("kernel32.dll", EntryPoint = "DeleteVolumeMountPointW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean DeleteVolumeMountPoint([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] String lpszVolumeMountPoint);

            [DllImportAttribute("kernel32.dll", EntryPoint = "GetVolumeNameForVolumeMountPointW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean GetVolumeNameForVolumeMountPoint([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] String lpszVolumeMountPoint, [OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName, Int32 cchBufferLength);

            [DllImportAttribute("kernel32.dll", EntryPoint = "GetVolumePathNamesForVolumeNameW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean GetVolumePathNamesForVolumeName([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] String lpszVolumeName, [OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpszVolumePathNames, Int32 cchBufferLength, [OutAttribute()] out Int32 lpcchReturnLength);

            [DllImportAttribute("kernel32.dll", EntryPoint = "SetVolumeMountPointW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean SetVolumeMountPoint([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] String lpszVolumeMountPoint, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] String lpszVolumeName);

        }

    }
}
