using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Medo.Net;

namespace VhdAttachService {
    internal static class PipeServer {

        public static Medo.IO.NamedPipe Pipe = new Medo.IO.NamedPipe("JosipMedved-VhdAttach-Commands");

        public static void Start() {
            Pipe.CreateWithFullAccess();
        }

        public static TinyPacket Receive() {
            Pipe.Connect();

            while (Pipe.HasBytesToRead == false) { Thread.Sleep(100); }
            var buffer = Pipe.ReadAvailable();

            var packet = TinyPacket.Parse(buffer);
            if (packet != null) {
                if (packet.Product != "VhdAttach") { return null; }
                try {
                    switch (packet.Operation) {
                        case "Attach": {
                                try {
                                    var path = packet["Path"];
                                    var isReadOnly = packet["MountReadOnly"].Equals("True", StringComparison.OrdinalIgnoreCase);
                                    var shouldInitialize = packet["InitializeDisk"].Equals("True", StringComparison.OrdinalIgnoreCase);
                                    string diskPath = null;
                                    using (var disk = new Medo.IO.VirtualDisk(path)) {
                                        var access = Medo.IO.VirtualDiskAccessMask.All;
                                        var options = Medo.IO.VirtualDiskAttachOptions.PermanentLifetime;
                                        if (isReadOnly) {
                                            if (shouldInitialize == false) {
                                                access = Medo.IO.VirtualDiskAccessMask.AttachReadOnly;
                                            }
                                            options |= Medo.IO.VirtualDiskAttachOptions.ReadOnly;
                                        }
                                        disk.Open(access);
                                        disk.Attach(options);
                                        if (shouldInitialize) { diskPath = disk.GetAttachedPath(); }
                                        disk.Close();
                                    }
                                    if (shouldInitialize) {
                                        DiskIO.InitializeDisk(diskPath);
                                    }
                                } catch (Exception ex) {
                                    throw new InvalidOperationException(string.Format("Virtual disk file \"{0}\" cannot be attached.", (new FileInfo(packet["Path"])).Name), ex);
                                }
                            } return GetResponse(packet);

                        case "Detach": {
                                try {
                                    var path = packet["Path"];
                                    using (var disk = new Medo.IO.VirtualDisk(path)) {
                                        disk.Open(Medo.IO.VirtualDiskAccessMask.Detach);
                                        disk.Detach();
                                        disk.Close();
                                    }
                                } catch (Exception ex) {
                                    throw new InvalidOperationException(string.Format("Virtual disk file \"{0}\" cannot be detached.", (new FileInfo(packet["Path"])).Name), ex);
                                }
                            } return GetResponse(packet);

                        case "DetachDrive": {
                                try {
                                    DetachDrive(packet["Path"]);
                                } catch (Exception ex) {
                                    throw new InvalidOperationException(string.Format("Drive \"{0}\" cannot be detached.", packet["Path"]), ex);
                                    throw new InvalidOperationException(ex.Message);
                                }
                            } return GetResponse(packet);

                        case "WriteSettings": {
                                try {
                                    ServiceSettings.ContextMenuAttach = bool.Parse(packet["ContextMenuAttach"]);
                                    ServiceSettings.ContextMenuAttachReadOnly = bool.Parse(packet["ContextMenuAttachReadOnly"]);
                                    ServiceSettings.ContextMenuDetach = bool.Parse(packet["ContextMenuDetach"]);
                                    ServiceSettings.ContextMenuDetachDrive = bool.Parse(packet["ContextMenuDetachDrive"]);
                                    ServiceSettings.AutoAttachVhdList = GetFwoArray(packet["AutoAttachList"]);
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

        private static FileWithOptions[] GetFwoArray(string lines) {
            var files = new List<FileWithOptions>();
            foreach (var line in lines.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)) {
                files.Add(new FileWithOptions(line));
            }
            return files.ToArray();
        }

        public static void Reply(TinyPacket response) {
            var buffer = response.GetBytes();
            Pipe.Write(buffer);
            Pipe.Flush();
            Pipe.Disconnect();
        }

        public static void Stop() {
            Pipe.Close();
        }


        public static TinyPacket GetResponse(TinyPacket packet) {
            var data = new Dictionary<string, string>();
            data.Add("IsError", false.ToString(CultureInfo.InvariantCulture));
            data.Add("Message", "");
            return new TinyPacket(packet.Product, packet.Operation, data);
        }

        public static TinyPacket GetResponse(TinyPacket packet, Exception ex) {
            var data = new Dictionary<string, string>();
            data.Add("IsError", true.ToString(CultureInfo.InvariantCulture));
            if (ex.InnerException != null) {
                data.Add("Message", ex.Message + "\r\n" + ex.InnerException.Message);
            } else {
                data.Add("Message", ex.Message);
            }
            return new TinyPacket(packet.Product, packet.Operation, data);
        }



        private static void DetachDrive(string path) {
            var device = DeviceFromPath.GetDevice(path);

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

                    try {
                        IVdsDisk disk;
                        provider.GetDiskFromVDisk(vDisk, out disk);

                        VDS_DISK_PROP diskProperties;
                        disk.GetProperties(out diskProperties);

                        if (diskProperties.pwszName.Equals(device, StringComparison.OrdinalIgnoreCase)) {
                            vhdFile = new FileInfo(vdiskProperties.pPath);
                            break;
                        } else {
                            Trace.TraceError(diskProperties.pwszName + " = " + vdiskProperties.pPath);
                        }
                        Console.WriteLine("-> Disk Name=" + diskProperties.pwszName);
                        Console.WriteLine("-> Disk Friendly=" + diskProperties.pwszFriendlyName);
                    } catch (COMException) { }
                }
                if (vhdFile != null) { break; }
            }

            #endregion

            if (vhdFile != null) {
                using (var disk = new Medo.IO.VirtualDisk(vhdFile.FullName)) {
                    disk.Open(Medo.IO.VirtualDiskAccessMask.Detach);
                    disk.Detach();
                    disk.Close();
                }
            } else {
                throw new FormatException(string.Format("Drive \"{0}\" is not a virtual hard disk.", path));
            }
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
