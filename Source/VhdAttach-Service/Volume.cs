using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace VhdAttachCommon {

    [DebuggerDisplay("{VolumeName} at {DriveLetter}")]
    internal class Volume {

        internal Volume(string volumeName) {
            this.VolumeName = volumeName;
        }

        public string VolumeName { get; private set; }

        private string VolumeNameWithoutSlash {
            get {
                return RemoveLastBackslash(this.VolumeName);
            }
        }

        /// <summary>
        /// Returns drive letter with colon (:) and trailing backslash (\).
        /// </summary>
        public string DriveLetter3 {
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

        /// <summary>
        /// Returns drive letter with colon (:) but without trailing backslash (\).
        /// </summary>
        public string DriveLetter2 {
            get {
                var letter = this.DriveLetter3;
                return (letter != null) ? letter.Substring(0, 2) : null;
            }
        }

        public void ChangeLetter(string newLetter) {
            newLetter = ParseDriveLetter(newLetter);
            if (newLetter == this.DriveLetter3) { return; } //nothing to do

            this.RemoveLetter();
            if (NativeMethods.SetVolumeMountPoint(newLetter, this.VolumeName) == false) {
                throw new Win32Exception();
            }
        }

        public void RemoveLetter() {
            if (this.DriveLetter3 != null) {
                if (NativeMethods.DeleteVolumeMountPoint(this.DriveLetter3) == false) {
                    throw new Win32Exception();
                }
            }
        }


        private int? _physicalDriveNumber;
        public int? PhysicalDriveNumber {
            get {
                FillExtentInfo();
                return this._physicalDriveNumber;
            }
        }

        private long? _physicalDriveExtentOffset;
        public long? PhysicalDriveExtentOffset {
            get {
                FillExtentInfo();
                return this._physicalDriveExtentOffset;
            }
        }

        private long? _physicalDriveExtentLength;
        public long? PhysicalDriveExtentLength {
            get {
                FillExtentInfo();
                return this._physicalDriveExtentLength;
            }
        }


        private bool _hasExtentInfo = false;
        private void FillExtentInfo() {
            if (_hasExtentInfo) { return; }

            int diskNumber;
            long startingOffset;
            long extentLength;
            if (GetExtentInfo(this.VolumeNameWithoutSlash, out diskNumber, out startingOffset, out extentLength)) {
                this._physicalDriveNumber = diskNumber;
                this._physicalDriveExtentOffset = startingOffset;
                this._physicalDriveExtentLength = extentLength;
            }

            _hasExtentInfo = true;
        }

        private static bool GetExtentInfo(string volumeNameWithoutSlash, out int diskNumber, out long startingOffset, out long extentLength) {
            var volumeHandle = NativeMethods.CreateFile(volumeNameWithoutSlash, 0, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (volumeHandle.IsInvalid == false) {
                var de = new NativeMethods.VOLUME_DISK_EXTENTS();
                de.NumberOfDiskExtents = 1;
                int bytesReturned = 0;
                if (NativeMethods.DeviceIoControl(volumeHandle, NativeMethods.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, ref de, Marshal.SizeOf(de), ref bytesReturned, IntPtr.Zero)) {
                    if (bytesReturned > 0) {
                        diskNumber = de.Extents.DiskNumber;
                        startingOffset = de.Extents.StartingOffset;
                        extentLength = de.Extents.ExtentLength;
                        return true;
                    }
                }
            }

            diskNumber = 0;
            startingOffset = 0;
            extentLength = 0;
            return false;
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


        public static IList<Volume> GetVolumesOnPhysicalDrive(string physicalDrive) {
            int driveNumber;
            if (physicalDrive.StartsWith(@"\\.\PHYSICALDRIVE", StringComparison.InvariantCulture) && int.TryParse(physicalDrive.Substring(17), NumberStyles.Integer, CultureInfo.InvariantCulture, out driveNumber)) {
                return GetVolumesOnPhysicalDrive(driveNumber);
                //} else if (physicalDrive.StartsWith(@"\\.\CDROM", StringComparison.InvariantCulture) && int.TryParse(physicalDrive.Substring(9), NumberStyles.Integer, CultureInfo.InvariantCulture, out driveNumber)) {
                //    return GetVolumesOnPhysicalDrive(driveNumber);
            } else {
                return null;
            }
        }

        public static IList<Volume> GetVolumesOnPhysicalDrive(int physicalDriveNumber) {
            var volumes = new List<Volume>();

            var sb = new StringBuilder(50);
            var volumeSearchHandle = NativeMethods.FindFirstVolume(sb, sb.Capacity);
            if (volumeSearchHandle.IsInvalid == false) {
                do {
                    var volume = new Volume(sb.ToString());
                    if (volume.PhysicalDriveNumber == physicalDriveNumber) {
                        volumes.Add(volume);
                    }
                } while (NativeMethods.FindNextVolume(volumeSearchHandle, sb, sb.Capacity));
            }
            volumeSearchHandle.Close();

            volumes.Sort(
                delegate(Volume volume1, Volume volume2) {
                    if ((volume1.PhysicalDriveExtentOffset ?? -1) < (volume2.PhysicalDriveExtentOffset ?? -1)) {
                        return -1;
                    } else if ((volume1.PhysicalDriveExtentOffset ?? -1) > (volume2.PhysicalDriveExtentOffset ?? -1)) {
                        return +1;
                    } else {
                        return 0;
                    }
                }
            );

            return volumes;
        }


        private static string ParseDriveLetter(string driveLetter) {
            if (driveLetter == null) { return null; }
            driveLetter = driveLetter.Trim().ToUpperInvariant();
            if (!(driveLetter.EndsWith("\\", StringComparison.Ordinal))) { driveLetter += "\\"; }
            if ((driveLetter.Length != 3) || (driveLetter[0] < 'A') || (driveLetter[0] > 'Z') || (driveLetter[1] != ':') || (driveLetter[2] != '\\')) { return null; }
            return driveLetter;
        }

        private static string RemoveLastBackslash(string text) {
            if (text.EndsWith("\\", StringComparison.Ordinal)) {
                return text.Remove(text.Length - 1);
            } else {
                return text;
            }
        }


        private class NativeMethods {

            public const UInt32 FILE_SHARE_READ = 0x1;
            public const UInt32 FILE_SHARE_WRITE = 0x2;
            public const UInt32 OPEN_EXISTING = 0x3;
            public const UInt32 IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x560000;


            [StructLayout(LayoutKind.Sequential)]
            public struct DISK_EXTENT {
                public Int32 DiskNumber;
                public Int64 StartingOffset;
                public Int64 ExtentLength;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct VOLUME_DISK_EXTENTS {
                public Int32 NumberOfDiskExtents;
                public DISK_EXTENT Extents;
            }


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


            [DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
            public static extern VolumeSafeHandle CreateFile([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, [InAttribute()] IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, [InAttribute()] IntPtr hTemplateFile);

            [DllImportAttribute("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool DeviceIoControl([InAttribute()] VolumeSafeHandle hDevice, UInt32 dwIoControlCode, [InAttribute()] IntPtr lpInBuffer, Int32 nInBufferSize, ref VOLUME_DISK_EXTENTS lpOutBuffer, Int32 nOutBufferSize, ref Int32 lpBytesReturned, IntPtr lpOverlapped);


            [DllImportAttribute("kernel32.dll", EntryPoint = "FindFirstVolumeW", SetLastError = true)]
            public static extern SearchSafeHandle FindFirstVolume([OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName, Int32 cchBufferLength);

            [DllImportAttribute("kernel32.dll", EntryPoint = "FindNextVolumeW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean FindNextVolume(SearchSafeHandle hFindVolume, [OutAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName, Int32 cchBufferLength);


            #region SafeHandles

            [SecurityPermission(SecurityAction.Demand)]
            public class VolumeSafeHandle : SafeHandleMinusOneIsInvalid {

                public VolumeSafeHandle()
                    : base(true) { }


                protected override bool ReleaseHandle() {
                    return CloseHandle(this.handle);
                }

                public override string ToString() {
                    return this.handle.ToString();
                }

                [DllImportAttribute("kernel32.dll", SetLastError = true)]
                [return: MarshalAsAttribute(UnmanagedType.Bool)]
                public static extern Boolean CloseHandle(IntPtr hObject);

            }

            [SecurityPermission(SecurityAction.Demand)]
            public class SearchSafeHandle : SafeHandleMinusOneIsInvalid {

                public SearchSafeHandle()
                    : base(true) { }


                protected override bool ReleaseHandle() {
                    return FindVolumeClose(this.handle);
                }

                public override string ToString() {
                    return this.handle.ToString();
                }

                [DllImportAttribute("kernel32.dll", EntryPoint = "FindVolumeClose")]
                [return: MarshalAsAttribute(UnmanagedType.Bool)]
                public static extern bool FindVolumeClose([InAttribute()] IntPtr hFindVolume);

            }

            #endregion

        }

    }
}
