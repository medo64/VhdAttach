using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace VhdAttach {
    internal static class PipeClient {

        public static PipeResponse Attach(string path, bool mountReadOnly, bool initializeDisk) {
            var data = new Dictionary<string, string>();
            data.Add("Path", path);
            data.Add("MountReadOnly", mountReadOnly.ToString(CultureInfo.InvariantCulture));
            data.Add("InitializeDisk", initializeDisk.ToString(CultureInfo.InvariantCulture));
            return Send("Attach", data);
        }

        public static PipeResponse Detach(string path) {
            var data = new Dictionary<string, string>();
            data.Add("Path", path);
            return Send("Detach", data);
        }

        public static PipeResponse DetachDrive(string path) {
            var data = new Dictionary<string, string>();
            data.Add("Path", path);
            return Send("DetachDrive", data);
        }

        public static PipeResponse WriteSettings(bool contextMenuVhdAttach, bool contextMenuVhdAttachReadOnly, bool contextMenuVhdDetach, bool contextMenuVhdDetachDrive, bool contextMenuIsoAttachReadOnly, bool contextMenuIsoDetach, string[] autoAttachList) {
            var data = new Dictionary<string, string>();
            data.Add("ContextMenuVhdAttach", contextMenuVhdAttach.ToString(CultureInfo.InvariantCulture));
            data.Add("ContextMenuVhdAttachReadOnly", contextMenuVhdAttachReadOnly.ToString(CultureInfo.InvariantCulture));
            data.Add("ContextMenuVhdDetach", contextMenuVhdDetach.ToString(CultureInfo.InvariantCulture));
            data.Add("ContextMenuVhdDetachDrive", contextMenuVhdDetachDrive.ToString(CultureInfo.InvariantCulture));
            data.Add("ContextMenuIsoAttachReadOnly", contextMenuIsoAttachReadOnly.ToString(CultureInfo.InvariantCulture));
            data.Add("ContextMenuIsoDetach", contextMenuIsoDetach.ToString(CultureInfo.InvariantCulture));
            data.Add("AutoAttachList", string.Join("|", autoAttachList));
            return Send("WriteSettings", data);
        }

        public static PipeResponse ChangeDriveLetter(string volumeName, string newDriveLetter) {
            var data = new Dictionary<string, string>();
            data.Add("VolumeName", volumeName);
            data.Add("NewDriveLetter", newDriveLetter);
            return Send("ChangeDriveLetter", data);
        }

        public static PipeResponse RegisterExtension() {
            var data = new Dictionary<string, string>();
            return Send("RegisterExtension", data);
        }

        private static Medo.IO.NamedPipe Pipe = new Medo.IO.NamedPipe("JosipMedved-VhdAttach-Commands");

        private static PipeResponse Send(string operation, Dictionary<string, string> data, int timeout = 5000) {
            var packetOut = new Medo.Net.TinyPacket("VhdAttach", operation, data);
            try {
                Pipe.Open();
                Pipe.Write(packetOut.GetBytes());
                var timer = new Stopwatch();
                timer.Start();
                while (timer.ElapsedMilliseconds < timeout) {
                    if (Pipe.HasBytesToRead) { break; }
                    Thread.Sleep(100);
                }
                timer.Stop();
                if (Pipe.HasBytesToRead) {
                    var buffer = Pipe.ReadAvailable();
                    var packetIn = Medo.Net.TinyPacket.Parse(buffer);
                    return new PipeResponse(bool.Parse(packetIn["IsError"]), packetIn["Message"]);
                } else {
                    return new PipeResponse(true, "Cannot contact service.");
                }
            } finally {
                Pipe.Close();
            }
        }

    }
}
