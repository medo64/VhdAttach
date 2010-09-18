using System;
using System.Threading;

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
                while (_thread.IsAlive) { Thread.Sleep(10); }
                _thread = null;
            } catch { }
        }

        private static void Run() {
            try {

                WcfPipeServer.Start();

                foreach (var file in ServiceSettings.AutoAttachVhdList) {
                    try {
                        using (var disk = new Medo.IO.VirtualDisk(file)) {
                            disk.Open();
                            disk.Attach(Medo.IO.VirtualDiskAttachOptions.PermanentLifetime);
                            disk.Close();
                        }
                        Thread.Sleep(1000);
                    } catch (Exception) {}
                }

                while (!_cancelEvent.WaitOne(0, false)) {
                    Thread.Sleep(100);
                }
                WcfPipeServer.Stop();

            } catch (ThreadAbortException) { }
        }

    }

}
