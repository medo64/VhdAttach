using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using VhdAttachCommon;

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
                ThreadPool.QueueUserWorkItem(new WaitCallback(RunAttachAutomatics));

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

        private static void RunAttachAutomatics(Object stateInfo) {
            var todoList = new List<FileWithOptions>(ServiceSettings.AutoAttachVhdList);
            var failedList = new List<FileWithOptions>();

            AttachAutomatics(todoList, failedList);
            if (failedList.Count > 0) {
                Thread.Sleep(1000); //give it a bit more time
                AttachAutomatics(failedList, null);
            }
        }

        private static void AttachAutomatics(List<FileWithOptions> todoList, List<FileWithOptions> failedList) {
            foreach (var fwo in todoList) {
                try {
                    Thread.Sleep(1000); //a bit of breather
                    var access = Medo.IO.VirtualDiskAccessMask.All;
                    var options = Medo.IO.VirtualDiskAttachOptions.PermanentLifetime;
                    if (fwo.ReadOnly) { options |= Medo.IO.VirtualDiskAttachOptions.ReadOnly; }
                    if (fwo.NoDriveLetter) { options |= Medo.IO.VirtualDiskAttachOptions.NoDriveLetter; }
                    var fileName = fwo.FileName;
                    using (var disk = new Medo.IO.VirtualDisk(fileName)) {
                        disk.Open(access);
                        disk.Attach(options);
                    }
                } catch (Exception ex) {
                    if (failedList != null) { failedList.Add(fwo); }
                    Trace.TraceError("E: Cannot attach file \"" + fwo.FileName + "\". " + ex.Message);
                    Medo.Diagnostics.ErrorReport.SaveToTemp(ex, fwo.FileName);
                }
            }
        }



        private static class NativeMethods {

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean DeleteFile(String lpFileName);

        }

    }

}
