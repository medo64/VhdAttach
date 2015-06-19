using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace VhdAttachCommon {
    internal static class ServiceSettings {

        private static readonly string RootSubkeyPath = @"Software\Josip Medved\VHD Attach";
        private static readonly RegistryKey RootRegistryKey = Registry.LocalMachine;
        private static readonly string pathToVhdAttach = Path.Combine((new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName, "VhdAttach.exe");


        public static FileWithOptions[] AutoAttachVhdList {
            get {
                var lines = new List<string>();
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(RootSubkeyPath, false)) {
                    if (rk != null) {
                        var fileArray = rk.GetValue("AutoAttachVhdList", null) as string[];
                        if (fileArray != null) { lines.AddRange(fileArray); }
                    }
                }
                var files = new List<FileWithOptions>();
                foreach (var line in lines) {
                    if (!string.IsNullOrEmpty(line.Trim())) {
                        files.Add(new FileWithOptions(line));
                    }
                }
                return files.ToArray();
            }
            set {
                var lines = new List<string>();
                foreach (var file in value) {
                    lines.Add(file.ToString());
                }
                using (RegistryKey rk = RootRegistryKey.CreateSubKey(RootSubkeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                    if (rk != null) {
                        rk.SetValue("AutoAttachVhdList", lines.ToArray(), RegistryValueKind.MultiString);
                    }
                }
            }
        }


        /// <summary>
        /// Returns true if VHD Attach is handling extension.
        /// </summary>
        public static bool ContextMenuVhd {
            get {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@".vhd\OpenWithProgids", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey)) {
                    if (rk != null) {
                        var text = rk.GetValue("Windows.VhdFile", null) as string;
                        if (text != null) { return true; }
                    }
                    return false;
                }
            }
            set {
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@".vhd", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey)) {
                        RegistryKey subKey = null;
                        try {
                            subKey = Registry.ClassesRoot.OpenSubKey(@".vhd\OpenWithProgids", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey);
                            if (subKey == null) { subKey = Registry.ClassesRoot.CreateSubKey(@".vhd\OpenWithProgids", RegistryKeyPermissionCheck.ReadWriteSubTree); }
                            subKey.SetValue("Windows.VhdFile", "", RegistryValueKind.String);
                        } finally {
                            if (subKey != null) { subKey.Close(); }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if VHD Attach is handling extension.
        /// </summary>
        public static bool ContextMenuIso {
            get {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@".iso\OpenWithProgids", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey)) {
                    if (rk != null) {
                        var text = rk.GetValue("Windows.IsoFile", null) as string;
                        if (text != null) { return true; }
                    }
                    return false;
                }
            }
            set {
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@".iso\OpenWithProgids", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey | RegistryRights.SetValue)) {
                        RegistryKey subKey = null;
                        try {
                            subKey = Registry.ClassesRoot.OpenSubKey(@".iso\OpenWithProgids", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey);
                            if (subKey == null) { subKey = Registry.ClassesRoot.CreateSubKey(@".iso\OpenWithProgids", RegistryKeyPermissionCheck.ReadWriteSubTree); }
                            subKey.SetValue("Windows.IsoFile", "", RegistryValueKind.String);
                        } finally {
                            if (subKey != null) { subKey.Close(); }
                        }
                    }
                }
            }
        }


        public static bool ContextMenuVhdOpen {
            get {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell\VhdAttach-Open", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-Open")) {
                            keyMain.SetValue("", "Open with VHD Attach", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" ""%1""", pathToVhdAttach), RegistryValueKind.String);
                            }
                        }
                        rk.SetValue("", "VhdAttach-Open");
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-Open");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                        try {
                            rk.DeleteValue("");
                        } catch (ArgumentException) { } //for cases when there is no such value.
                    }
                }
            }
        }

        public static bool ContextMenuVhdAttach {
            get {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell\VhdAttach-Attach", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-Attach")) {
                            keyMain.SetValue("", "Attach", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/attach"), RegistryValueKind.String);
                            }
                        }
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-Attach");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                    }
                }
            }
        }

        public static bool ContextMenuVhdAttachReadOnly {
            get {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell\VhdAttach-AttachReadOnly", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-AttachReadOnly")) {
                            keyMain.SetValue("", "Attach (read-only)", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/readonly /attach"), RegistryValueKind.String);
                            }
                        }
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-AttachReadOnly");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                    }
                }
            }
        }

        public static bool ContextMenuVhdDetach {
            get {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell\VhdAttach-Detach", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-Detach")) {
                            keyMain.SetValue("", "Detach", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/detach"), RegistryValueKind.String);
                            }
                        }
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.VhdFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-Detach");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                    }
                }
            }
        }

        public static bool ContextMenuVhdDetachDrive {
            get {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) >= 6000002) { return false; } //if Windows 8 or higher, ignore
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Drive\shell\VhdAttach-DetachDrive", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) >= 6000002) { return; } //if Windows 8 or higher, ignore
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Drive\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-DetachDrive")) {
                            keyMain.SetValue("", "Detach drive", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Single", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/detachdrive"), RegistryValueKind.String);
                            }
                        }
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Drive\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-DetachDrive");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                    }
                }
            }
        }


        public static bool ContextMenuIsoOpen {
            get {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { return false; } //if lower than Windows 8, ignore
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell\VhdAttach-Open", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { return; } //if lower than Windows 8, ignore
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-Open")) {
                            keyMain.SetValue("", "Open with VHD Attach", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" ""%1""", pathToVhdAttach), RegistryValueKind.String);
                            }
                        }
                        rk.SetValue("", "VhdAttach-Open");
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-Open");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                        try {
                            rk.SetValue("", "mount");  //default for Windows 8
                        } catch (ArgumentException) { } //for cases when there is no such value.
                    }
                }
            }
        }

        public static bool ContextMenuIsoAttachReadOnly {
            get {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { return false; } //if lower than Windows 8, ignore
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell\VhdAttach-AttachReadOnly", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { return; } //if lower than Windows 8, ignore
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-AttachReadOnly")) {
                            keyMain.SetValue("", "Attach", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/readonly /attach"), RegistryValueKind.String);
                            }
                        }
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-AttachReadOnly");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                    }
                }
            }
        }

        public static bool ContextMenuIsoDetach {
            get {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { return false; } //if lower than Windows 8, ignore
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell\VhdAttach-Detach", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                    return (rk != null);
                }
            }
            set {
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { return; } //if lower than Windows 8, ignore
                if (value == true) {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        using (var keyMain = rk.CreateSubKey("VhdAttach-Detach")) {
                            keyMain.SetValue("", "Detach", RegistryValueKind.String);
                            keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                            keyMain.SetValue("MultiSelectModel", "Document", RegistryValueKind.String);
                            using (var keyCommand = keyMain.CreateSubKey("command")) {
                                keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/detach"), RegistryValueKind.String);
                            }
                        }
                    }
                } else {
                    using (var rk = Registry.ClassesRoot.OpenSubKey(@"Windows.IsoFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                        try {
                            rk.DeleteSubKeyTree("VhdAttach-Detach");
                        } catch (ArgumentException) { } //for cases when there is no such subkey.
                    }
                }
            }
        }

    }
}