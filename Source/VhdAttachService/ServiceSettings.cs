using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using Microsoft.Win32;

internal static class ServiceSettings {

    private static readonly string RootSubkeyPath = @"Software\Josip Medved\VHD Attach";
    private static readonly RegistryKey RootRegistryKey = Registry.LocalMachine;
    private static readonly string pathToVhdAttach = Path.Combine((new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName, "VhdAttach.exe");


    public static string[] AutoAttachVhdList {
        get {
            var files = new List<string>();
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(RootSubkeyPath, false)) {
                if (rk != null) {
                    var fileArray = rk.GetValue("AutoAttachVhdList", null) as string[];
                    if (fileArray != null) { files.AddRange(fileArray); }
                }
            }
            return files.ToArray();
        }
        set {
            using (RegistryKey rk = RootRegistryKey.CreateSubKey(RootSubkeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (rk != null) {
                    rk.SetValue("AutoAttachVhdList", value, RegistryValueKind.MultiString);
                }
            }
        }
    }


    public static bool ContextMenuAttach {
        get {
            using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell\Attach", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                return (rk != null);
            }
        }
        set {
            if (value == true) {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    using (var keyMain = rk.CreateSubKey("Attach")) {
                        keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                        keyMain.SetValue("MultiSelectModel", "Player", RegistryValueKind.String);
                        using (var keyCommand = keyMain.CreateSubKey("command")) {
                            keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/attach"), RegistryValueKind.String);
                        }
                    }
                }
            } else {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    try {
                        rk.DeleteSubKeyTree("Attach");
                    } catch (ArgumentException) { } //for cases when there is no such subkey.
                }
            }
        }
    }

    public static bool ContextMenuAttachReadOnly {
        get {
            using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell\Attach read-only", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                return (rk != null);
            }
        }
        set {
            if (value == true) {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    using (var keyMain = rk.CreateSubKey("Attach read-only")) {
                        keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                        keyMain.SetValue("MultiSelectModel", "Player", RegistryValueKind.String);
                        using (var keyCommand = keyMain.CreateSubKey("command")) {
                            keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/readonly /attach"), RegistryValueKind.String);
                        }
                    }
                }
            } else {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    try {
                        rk.DeleteSubKeyTree("Attach read-only");
                    } catch (ArgumentException) { } //for cases when there is no such subkey.
                }
            }
        }
    }

    public static bool ContextMenuDetach {
        get {
            using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell\Detach", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                return (rk != null);
            }
        }
        set {
            if (value == true) {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    using (var keyMain = rk.CreateSubKey("Detach")) {
                        keyMain.SetValue("Icon", @"""" + pathToVhdAttach + @"""", RegistryValueKind.String);
                        keyMain.SetValue("MultiSelectModel", "Player", RegistryValueKind.String);
                        using (var keyCommand = keyMain.CreateSubKey("command")) {
                            keyCommand.SetValue(null, string.Format(CultureInfo.InvariantCulture, @"""{0}"" {1} ""%1""", pathToVhdAttach, "/detach"), RegistryValueKind.String);
                        }
                    }
                }
            } else {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"VhdAttachFile\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    try {
                        rk.DeleteSubKeyTree("Detach");
                    } catch (ArgumentException) { } //for cases when there is no such subkey.
                }
            }
        }
    }

    public static bool ContextMenuDetachDrive {
        get {
            using (var rk = Registry.ClassesRoot.OpenSubKey(@"Drive\shell\Detach drive", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey)) {
                return (rk != null);
            }
        }
        set {
            if (value == true) {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@"Drive\shell", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)) {
                    using (var keyMain = rk.CreateSubKey("Detach drive")) {
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
                        rk.DeleteSubKeyTree("Detach drive");
                    } catch (ArgumentException) { } //for cases when there is no such subkey.
                }
            }
        }
    }

    /// <summary>
    /// Returns true if VHD Attach is handling extension.
    /// </summary>
    public static bool ContextMenu {
        get {
            using (var rk = Registry.ClassesRoot.OpenSubKey(@".vhd", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey)) {
                if (rk != null) {
                    var text = rk.GetValue("", null) as string;
                    if ((text != null) && (text.Equals("VhdAttachFile"))) { return true; }
                }
                return false;
            }
        }
        set {
            if (value == true) {
                using (var rk = Registry.ClassesRoot.OpenSubKey(@".vhd", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue)) {
                    rk.SetValue("", "VhdAttachFile", RegistryValueKind.String);
                }
            }
        }
    }

}