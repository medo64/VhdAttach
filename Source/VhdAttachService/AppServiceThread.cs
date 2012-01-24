using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Medo.Net;

namespace VhdAttachService {

    internal static class AppServiceThread {

        private static Thread _thread;
        private static ManualResetEvent _cancelEvent;


        public static void Start() {
            if (_thread != null) { return; }

            _cancelEvent = new ManualResetEvent(false);
            _thread = new Thread(Run);
            _thread.Name = "Service";
            _thread.Start();
        }

        public static void Stop() {
            try {
                _cancelEvent.Set();
                for (int i = 0; i < 10; i++) {
                    if (_thread.IsAlive == false) { break; }
                    Thread.Sleep(10);
                }
                if (_thread.IsAlive) {
                    _thread.Abort();
                }
                _thread = null;
            } catch { }
        }

        private static void Run() {
            try {

                foreach (var file in ServiceSettings.AutoAttachVhdList) {
                    try {
                        using (var disk = new Medo.IO.VirtualDisk(file)) {
                            disk.Open();
                            disk.Attach(Medo.IO.VirtualDiskAttachOptions.PermanentLifetime);
                            disk.Close();
                        }
                        Thread.Sleep(1000);
                    } catch (Exception) { }
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
                            Debug.WriteLine(ex.Message);
                            PipeServer.Stop();
                            PipeServer.Start();
                            Thread.Sleep(50);
                        }
                    }
                } finally {
                    PipeServer.Stop();
                }

            } catch (ThreadAbortException) { }
        }

    }

}
