using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System;
using System.IO;
using System.Management;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
class PipeService : IPipeService {

    public byte[] Execute(string action, byte[] data) {
        Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Action=\"{0}\".", action));

        try {
            switch (action) {
                case "Attach": {
                        var file = JsonAttachDetachData.FromJson(data);
                        try {
                            using (var disk = new Medo.IO.VirtualDisk(file.Path)) {
                                disk.Open();
                                disk.Attach(Medo.IO.VirtualDiskAttachOptions.PermanentLifetime);
                                disk.Close();
                            }
                        } catch (Exception ex) {
                            throw new InvalidOperationException(string.Format("Virtual disk file \"{0}\" cannot be attached.", (new FileInfo(file.Path)).Name), ex);
                        }
                    } return (new JsonResponseData(ExitCodes.OK, null)).ToJson();

                case "Detach": {
                        var file = JsonAttachDetachData.FromJson(data);
                        try {
                            using (var disk = new Medo.IO.VirtualDisk(file.Path)) {
                                disk.Open();
                                disk.Detach();
                                disk.Close();
                            }
                        } catch (Exception ex) {
                            throw new InvalidOperationException(string.Format("Virtual disk file \"{0}\" cannot be detached.", (new FileInfo(file.Path)).Name), ex);
                        }
                    } return (new JsonResponseData(ExitCodes.OK, null)).ToJson();

                case "DetachDrive": {
                        var file = JsonAttachDetachData.FromJson(data);
                        try {
                            DetachDrive(file.Path);
                        } catch (Exception ex) {
                            //throw new InvalidOperationException(string.Format("Disk drive \"{0}\" cannot be detached.", file.Path), ex);
                            throw new InvalidOperationException(ex.Message);
                        }
                    } return (new JsonResponseData(ExitCodes.OK, null)).ToJson();

                case "WriteSettings": {
                        var settings = JsonSettingsData.FromJson(data);
                        try {
                            ServiceSettings.ContextMenuAttach = settings.ContextMenuAttach;
                            ServiceSettings.ContextMenuDetach = settings.ContextMenuDetach;
                            ServiceSettings.ContextMenuDetachDrive = settings.ContextMenuDetachDrive;
                            ServiceSettings.AutoAttachVhdList = settings.AutoAttachList;
                        } catch (Exception ex) {
                            throw new InvalidOperationException("Settings cannot be written.", ex);
                        }
                    } return (new JsonResponseData(ExitCodes.OK, null)).ToJson();

                default: return null;
            }
        } catch (InvalidOperationException ex) {
            return (new JsonResponseData(ExitCodes.GenericError, ex.Message)).ToJson();
        }
    }



    private static void DetachDrive(string path) {
        FileSystemInfo iDirectory = null;
        var wmiQuery = new ObjectQuery("SELECT Antecedent, Dependent FROM Win32_LogicalDiskToPartition");
        var wmiSearcher = new ManagementObjectSearcher(wmiQuery);

        try {
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
                        break;
                    } else {
                        throw new FormatException("Cannot retrieve physical disk number.");
                    }
                }
            }

            var diskPartScriptFile = Path.GetTempFileName();
            File.WriteAllText(diskPartScriptFile, "list vdisk");

            var startInfo = new ProcessStartInfo();
            startInfo.Arguments = "/s \"" + diskPartScriptFile + "\"";
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            startInfo.FileName = "diskpart";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            var diskPartProcess = new Process();
            diskPartProcess.StartInfo = startInfo;
            diskPartProcess.Start();

            var diskPartResult = diskPartProcess.StandardOutput.ReadToEnd();
            var diskPartLines = diskPartResult.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            string diskPartDashedLine = null;
            int minDashPosition = int.MaxValue;
            int dashedLineIndex = -1;
            for (int i = 0; i < diskPartLines.Length; ++i) {
                int dashPosition = diskPartLines[i].IndexOf("---");
                if ((dashPosition >= 0) && (dashPosition < minDashPosition)) {
                    minDashPosition = dashPosition;
                    diskPartDashedLine = diskPartLines[i];
                    dashedLineIndex = i;
                }
            }
            if (diskPartDashedLine == null) {
                throw new FormatException("Cannot find attached virtual disk drives.");
            }
            var dataLines = new List<string>();
            for (int i = dashedLineIndex + 1; i < diskPartLines.Length; ++i) {
                dataLines.Add(diskPartLines[i]);
            }


            var dashedLineStarts = new List<int>();
            int dashIndex = diskPartDashedLine.IndexOf('-');
            dashedLineStarts.Add(dashIndex);
            int lastDashIndex = dashIndex;
            while (true) {
                dashIndex = diskPartDashedLine.IndexOf('-', lastDashIndex + 1);
                if (dashIndex < 0) { break; }
                if (dashIndex > lastDashIndex + 1) { //there is space in-between last two dashes
                    dashedLineStarts.Add(dashIndex);
                }
                lastDashIndex = dashIndex;
            }
            if (dashedLineStarts.Count != 5) {
                throw new FormatException("Cannot determine attached virtual disk drives.");
            }

            FileInfo vhdFile = null;
            foreach (var iLine in dataLines) {
                var diskText = iLine.Substring(dashedLineStarts[1], dashedLineStarts[2] - dashedLineStarts[1] - 1).Trim();
                var diskNumberMatch = ExtractDiskNumberRegex.Match(diskText);
                if (diskNumberMatch.Success) {
                    int diskPartDiskNumber;
                    if (int.TryParse(diskNumberMatch.Value, NumberStyles.Integer, CultureInfo.CurrentCulture, out diskPartDiskNumber)) {
                        if (diskPartDiskNumber == wmiPhysicalDiskNumber) {
                            vhdFile = new FileInfo(iLine.Substring(dashedLineStarts[4], iLine.Length - dashedLineStarts[4]).TrimStart());
                        }
                    }
                }
            }

            if (vhdFile != null) {
                using (var disk = new Medo.IO.VirtualDisk(vhdFile.FullName)) {
                    disk.Open();
                    disk.Detach();
                    disk.Close();
                }
            } else {
                throw new FormatException("Drive cannot be linked to virtual disk file.");
            }
        } catch (Exception ex) {
            throw new InvalidOperationException(string.Format("Drive \"{0}\" cannot be detached.", path), ex);
        }
    }

    private static readonly Regex ExtractDiskNumberRegex = new Regex(@"\d+$", RegexOptions.Compiled);

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

}
