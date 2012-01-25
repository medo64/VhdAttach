using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using Medo.Net;

namespace VhdAttachService {
    internal static class PipeServer {

        public static Medo.IO.NamedPipe Pipe = new Medo.IO.NamedPipe("JosipMedved-VhdAttach-Commands");

        public static void Start() {
            Pipe.CreateWithFullAccess();
        }

        public static TinyPairPacket Receive() {
            Pipe.Connect();

            while (Pipe.HasBytesToRead == false) { Thread.Sleep(100); }
            var buffer = Pipe.ReadAvailable();

            var packet = TinyPairPacket.Parse(buffer);
            if (packet != null) {
                if (packet.Product != "VhdAttach") { return null; }
                try {
                    switch (packet.Operation) {
                        case "Attach": {
                                try {
                                    string diskPath;
                                    using (var disk = new Medo.IO.VirtualDisk(packet.Data["Path"])) {
                                        disk.Open();
                                        var options = Medo.IO.VirtualDiskAttachOptions.PermanentLifetime;
                                        if (packet.Data["MountReadOnly"] == "true") { options |= Medo.IO.VirtualDiskAttachOptions.ReadOnly; }
                                        disk.Attach(options);
                                        diskPath = disk.GetAttachedPath();
                                        disk.Close();
                                    }
                                    if (packet.Data["InitializeDisk"] == "true") {
                                        DiskIO.InitializeDisk(diskPath);
                                    }
                                } catch (Exception ex) {
                                    throw new InvalidOperationException(string.Format("Virtual disk file \"{0}\" cannot be attached.", (new FileInfo(packet.Data["Path"])).Name), ex);
                                }
                            } return GetResponse(packet);

                        case "Detach": {
                                try {
                                    using (var disk = new Medo.IO.VirtualDisk(packet.Data["Path"])) {
                                        disk.Open();
                                        disk.Detach();
                                        disk.Close();
                                    }
                                } catch (Exception ex) {
                                    throw new InvalidOperationException(string.Format("Virtual disk file \"{0}\" cannot be detached.", (new FileInfo(packet.Data["Path"])).Name), ex);
                                }
                            } return GetResponse(packet);

                        case "DetachDrive": {
                                try {
                                    DetachDrive(packet.Data["Path"]);
                                } catch (Exception ex) {
                                    throw new InvalidOperationException(string.Format("Disk drive \"{0}\" cannot be detached.", packet.Data["Path"]), ex);
                                    throw new InvalidOperationException(ex.Message);
                                }
                            } return GetResponse(packet);

                        case "WriteSettings": {
                                try {
                                    ServiceSettings.ContextMenuAttach = bool.Parse(packet.Data["ContextMenuAttach"]);
                                    ServiceSettings.ContextMenuAttachReadOnly = bool.Parse(packet.Data["ContextMenuAttachReadOnly"]);
                                    ServiceSettings.ContextMenuDetach = bool.Parse(packet.Data["ContextMenuDetach"]);
                                    ServiceSettings.ContextMenuDetachDrive = bool.Parse(packet.Data["ContextMenuDetachDrive"]);
                                    ServiceSettings.AutoAttachVhdList = packet.Data["AutoAttachList"].Split('|');
                                } catch (Exception ex) {
                                    Medo.Diagnostics.ErrorReport.SaveToTemp(ex);
                                    throw new InvalidOperationException("Settings cannot be written.", ex);
                                }
                            } return GetResponse(packet);

                        case "RegisterExtension": {
                                try {
                                    ServiceSettings.ContextMenu = true;
                                } catch (Exception ex) {
                                    Medo.Diagnostics.ErrorReport.SaveToTemp(ex);
                                    throw new InvalidOperationException("Settings cannot be written.", ex);
                                }
                            } return GetResponse(packet);

                        default: throw new InvalidOperationException("Unknown command.");
                    }
                } catch (Exception ex) {
                    return GetResponse(packet, ex);
                }
            } else {
                return null;
            }
        }

        public static void Reply(TinyPairPacket response) {
            var buffer = response.GetBytes();
            Pipe.Write(buffer);
            Pipe.Flush();
            Pipe.Disconnect();
        }

        public static void Stop() {
            Pipe.Close();
        }


        public static TinyPairPacket GetResponse(TinyPairPacket packet) {
            var data = new Dictionary<string, string>();
            data.Add("IsError", false.ToString(CultureInfo.InvariantCulture));
            data.Add("Message", "");
            return new TinyPairPacket(packet.Product, packet.Operation, data);
        }

        public static TinyPairPacket GetResponse(TinyPairPacket packet, Exception ex) {
            var data = new Dictionary<string, string>();
            data.Add("IsError", true.ToString(CultureInfo.InvariantCulture));
            if (ex.InnerException != null) {
                data.Add("Message", ex.Message + "\r\n" + ex.InnerException.Message);
            } else {
                data.Add("Message", ex.Message);
            }
            return new TinyPairPacket(packet.Product, packet.Operation, data);
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


                #region VDS COM

                FileInfo vhdFile = null;

                VdsServiceLoader loaderClass = new VdsServiceLoader();
                IVdsServiceLoader loader = (IVdsServiceLoader)loaderClass;

                IVdsService service;
                loader.LoadService(null, out service);

                service.WaitForServiceReady();

                IEnumVdsObject providerEnum;
                service.QueryProviders(VDS_QUERY_PROVIDER_FLAG.VDS_QUERY_VIRTUALDISK_PROVIDERS, out providerEnum);

                while (true) {
                    uint fetchedProvider;
                    object unknownProvider;
                    providerEnum.Next(1, out unknownProvider, out fetchedProvider);

                    if (fetchedProvider == 0) break;
                    IVdsVdProvider provider = (IVdsVdProvider)unknownProvider;
                    Console.WriteLine("Got VD Provider");

                    IEnumVdsObject diskEnum;
                    provider.QueryVDisks(out diskEnum);

                    while (true) {
                        uint fetchedDisk;
                        object unknownDisk;
                        diskEnum.Next(1, out unknownDisk, out fetchedDisk);
                        if (fetchedDisk == 0) break;
                        IVdsVDisk vDisk = (IVdsVDisk)unknownDisk;

                        VDS_VDISK_PROPERTIES vdiskProperties;
                        vDisk.GetProperties(out vdiskProperties);

                        IVdsDisk disk;
                        provider.GetDiskFromVDisk(vDisk, out disk);

                        VDS_DISK_PROP diskProperties;
                        disk.GetProperties(out diskProperties);

                        if (diskProperties.pwszName.StartsWith(@"\\?\PhysicalDrive")) {
                            int vdsDiskNumber;
                            if (int.TryParse(diskProperties.pwszName.Substring(17), NumberStyles.Integer, CultureInfo.CurrentCulture, out vdsDiskNumber)) {
                                if (vdsDiskNumber == wmiPhysicalDiskNumber) {
                                    vhdFile = new FileInfo(vdiskProperties.pPath);
                                    break;
                                }
                            }
                        } else {
                            Trace.TraceError(diskProperties.pwszName + " = " + vdiskProperties.pPath);
                        }
                        Console.WriteLine("-> Disk Name=" + diskProperties.pwszName);
                        Console.WriteLine("-> Disk Friendly=" + diskProperties.pwszFriendlyName);
                    }
                    if (vhdFile != null) { break; }
                }

                #endregion

                #region diskpart - not used

                //var diskPartScriptFile = Path.GetTempFileName();
                //File.WriteAllText(diskPartScriptFile, "list vdisk");

                //var startInfo = new ProcessStartInfo();
                //startInfo.Arguments = "/s \"" + diskPartScriptFile + "\"";
                //startInfo.CreateNoWindow = true;
                //startInfo.ErrorDialog = false;
                //startInfo.FileName = "diskpart";
                //startInfo.RedirectStandardOutput = true;
                //startInfo.UseShellExecute = false;

                //var diskPartProcess = new Process();
                //diskPartProcess.StartInfo = startInfo;
                //diskPartProcess.Start();

                //var diskPartResult = diskPartProcess.StandardOutput.ReadToEnd();
                //var diskPartLines = diskPartResult.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //string diskPartDashedLine = null;
                //int minDashPosition = int.MaxValue;
                //int dashedLineIndex = -1;
                //for (int i = 0; i < diskPartLines.Length; ++i) {
                //    int dashPosition = diskPartLines[i].IndexOf("---");
                //    if ((dashPosition >= 0) && (dashPosition < minDashPosition)) {
                //        minDashPosition = dashPosition;
                //        diskPartDashedLine = diskPartLines[i];
                //        dashedLineIndex = i;
                //    }
                //}
                //if (diskPartDashedLine == null) {
                //    throw new FormatException("Cannot find attached virtual disk drives.");
                //}
                //var dataLines = new List<string>();
                //for (int i = dashedLineIndex + 1; i < diskPartLines.Length; ++i) {
                //    dataLines.Add(diskPartLines[i]);
                //}


                //var dashedLineStarts = new List<int>();
                //int dashIndex = diskPartDashedLine.IndexOf('-');
                //dashedLineStarts.Add(dashIndex);
                //int lastDashIndex = dashIndex;
                //while (true) {
                //    dashIndex = diskPartDashedLine.IndexOf('-', lastDashIndex + 1);
                //    if (dashIndex < 0) { break; }
                //    if (dashIndex > lastDashIndex + 1) { //there is space in-between last two dashes
                //        dashedLineStarts.Add(dashIndex);
                //    }
                //    lastDashIndex = dashIndex;
                //}
                //if (dashedLineStarts.Count != 5) {
                //    throw new FormatException("Cannot determine attached virtual disk drives.");
                //}

                //FileInfo vhdFile = null;
                //foreach (var iLine in dataLines) {
                //    var diskText = iLine.Substring(dashedLineStarts[1], dashedLineStarts[2] - dashedLineStarts[1] - 1).Trim();
                //    var diskNumberMatch = ExtractDiskNumberRegex.Match(diskText);
                //    if (diskNumberMatch.Success) {
                //        int diskPartDiskNumber;
                //        if (int.TryParse(diskNumberMatch.Value, NumberStyles.Integer, CultureInfo.CurrentCulture, out diskPartDiskNumber)) {
                //            if (diskPartDiskNumber == wmiPhysicalDiskNumber) {
                //                vhdFile = new FileInfo(iLine.Substring(dashedLineStarts[4], iLine.Length - dashedLineStarts[4]).TrimStart());
                //            }
                //        }
                //    }
                //}

                #endregion


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


        private static class NativeMethods {

            public const uint FILE_ATTRIBUTE_NORMAL = 0;
            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;
            public const int INVALID_HANDLE_VALUE = -1;
            public const uint NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;
            public const uint OPEN_EXISTING = 3;
            public const uint PIPE_ACCESS_DUPLEX = 0x00000003;
            public const uint PIPE_READMODE_BYTE = 0x00000000;
            public const uint PIPE_TYPE_BYTE = 0x00000000;
            public const uint PIPE_UNLIMITED_INSTANCES = 255;
            public const uint PIPE_WAIT = 0x00000000;


            public class FileSafeHandle : SafeHandle {
                private static IntPtr minusOne = new IntPtr(-1);


                public FileSafeHandle()
                    : base(minusOne, true) { }


                public override bool IsInvalid {
                    get { return (this.IsClosed) || (base.handle == minusOne); }
                }

                protected override bool ReleaseHandle() {
                    return CloseHandle(this.handle);
                }

                public override string ToString() {
                    return this.handle.ToString();
                }

            }


            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(System.IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ConnectNamedPipe(System.IntPtr hNamedPipe, System.IntPtr lpOverlapped);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern FileSafeHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, System.IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, System.IntPtr hTemplateFile);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern System.IntPtr CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, System.IntPtr lpSecurityAttributes);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PeekNamedPipe(System.IntPtr hNamedPipe, byte[] lpBuffer, uint nBufferSize, ref uint lpBytesRead, ref uint lpTotalBytesAvail, ref uint lpBytesLeftThisMessage);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReadFile(System.IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteFile(FileSafeHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WaitNamedPipe(string lpNamedPipeName, uint nTimeOut);

        }


    }
}
