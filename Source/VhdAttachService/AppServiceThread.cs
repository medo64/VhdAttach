using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Globalization;

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


                WcfPipeServer.Start();
                WaitForOpenPipe();
                while (!_cancelEvent.WaitOne(0, false)) {
                    if (WcfPipeServer.State != CommunicationState.Opened) {
                        Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "W: Unexpected pipe status ({0})." , WcfPipeServer.State));
                        WcfPipeServer.Stop();
                        WcfPipeServer.Start();
                        WaitForOpenPipe();
                        if (WcfPipeServer.State == CommunicationState.Opened) { Trace.WriteLine("I: Pipe re-opened."); }
                    }
                    Thread.Sleep(100);
                }
                WcfPipeServer.Stop();

            } catch (ThreadAbortException) { }
        }

        private static void WaitForOpenPipe() {
            var sw = new Stopwatch();
            sw.Start();
            while (!_cancelEvent.WaitOne(0, false)) {
                if (WcfPipeServer.State == CommunicationState.Opened) { break; }
                if (sw.ElapsedMilliseconds > 10000) { break; } //assume that waiting is futile
                Thread.Sleep(10);
            }
            sw.Stop();
        }

    }

}
