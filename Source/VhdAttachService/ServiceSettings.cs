using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Globalization;
using System.IO;
using System.Reflection;

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
                        //keyMain.SetValue("HasLUAShield", "", RegistryValueKind.String);
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
                        //keyMain.SetValue("HasLUAShield", "", RegistryValueKind.String);
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
                        //keyMain.SetValue("HasLUAShield", "", RegistryValueKind.String);
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

}