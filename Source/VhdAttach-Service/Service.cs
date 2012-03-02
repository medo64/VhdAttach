using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace VhdAttachService {

    internal static class Service {

        private static Thread _thread;
        private static ManualResetEvent _cancelEvent;


        public static void Start() {
            if (_thread != null) { return; }

            _cancelEvent = new ManualResetEvent(false);
            _thread = new Thread(Run);
            _thread.Name = "Service";
            _thread.Start();
            Debug.WriteLine("Service thread started.");
        }

        public static void Stop() {
            Debug.WriteLine("Service thread stopping...");
            try {
                _cancelEvent.Set();
                PipeServer.Pipe.Close();
                NativeMethods.DeleteFile(PipeServer.Pipe.FullPipeName); //I have no idea why exactly this unblocks ConnectNamedPipe...
                for (int i = 0; i < 10; i++) {
                    if (_thread.IsAlive) { Thread.Sleep(100); } else { break; }
                }
                if (_thread.IsAlive) {
                    Debug.WriteLine("Service thread aborting...");
                    _thread.Abort();
                }
                _thread = null;
            } catch { }
            Debug.WriteLine("Service thread stoped.");
        }

        private static void Run() {
            try {

                foreach (var file in ServiceSettings.AutoAttachVhdList) {
                    try {
                        string fileName;
                        var options = Medo.IO.VirtualDiskAttachOptions.PermanentLifetime;
                        if (file.StartsWith("/")) {
                            /*
                             * Each file can have additional settings area that starts with / and ends with next /.
                             * E.g. "/readonly,nodriveletter/D:\Test.vhd"
                             */
                            var iEndPipe = file.IndexOf("/", 1);
                            var additionalSettings = file.Substring(1, iEndPipe - 1);
                            fileName = file.Substring(iEndPipe + 1);
                            foreach (var setting in additionalSettings.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                                switch (additionalSettings.ToUpperInvariant()) {
                                    case "READONLY": options |= Medo.IO.VirtualDiskAttachOptions.ReadOnly; break;
                                    case "NODRIVELETTER": options |= Medo.IO.VirtualDiskAttachOptions.NoDriveLetter; break;
                                }
                            }
                        } else {
                            fileName = file;
                        }
                        using (var disk = new Medo.IO.VirtualDisk(fileName)) {
                            disk.Open();
                            disk.Attach(options);
                            disk.Close();
                        }
                        Thread.Sleep(1000);
                    } catch (Exception ex) {
                        Trace.TraceError("E: Cannot attach file \"" + file + "\". " + ex.Message);
                    }
                }


                try {
                    PipeServer.Start();
                    while (!_cancelEvent.WaitOne(0, false)) {
                        try {
                            var response = PipeServer.Receive();
                            if (response != null) {
                                PipeServer.Reply(response);
                            }
                        } catch (Exception ex) {
                            if (_cancelEvent.WaitOne(0, false)) { return; }
                            Debug.WriteLine(ex.Message);
                            PipeServer.Stop();
                            PipeServer.Start();
                            Thread.Sleep(50);
                        }
                    }
                } finally {
                    Debug.WriteLine("AppServiceThread.Run: Finally.");
                    PipeServer.Stop();
                }

            } catch (ThreadAbortException) {
                Debug.WriteLine("AppServiceThread.Run: Thread aborted.");
            }
        }



        private static class NativeMethods {

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean DeleteFile(String lpFileName);

        }

    }

}
