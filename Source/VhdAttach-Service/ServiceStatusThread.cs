using System;
using System.Diagnostics;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;

namespace VhdAttachService {
    internal static class ServiceStatusThread {

        private static Thread Thread;
        private static ManualResetEvent CancelEvent;

        public static void Start() {
            if (ServiceStatusThread.Thread != null) { return; }

            ServiceStatusThread.CancelEvent = new ManualResetEvent(false);
            ServiceStatusThread.Thread = new Thread(Run);
            ServiceStatusThread.Thread.Name = "Service status";
            ServiceStatusThread.Thread.CurrentCulture = CultureInfo.InvariantCulture;
            ServiceStatusThread.Thread.Start();
        }

        public static void Stop() {
            try {
                ServiceStatusThread.CancelEvent.Set();
                while (ServiceStatusThread.Thread.IsAlive) { Thread.Sleep(10); }
                ServiceStatusThread.Thread = null;
            } catch { }
        }


        private static void Run() {
            try {
                var sw = new Stopwatch();
                using (var service = new ServiceController(AppService.Instance.ServiceName)) {
                    bool? lastIsRunning = null;
                    Tray.SetStatusToUnknown();
                    while (!ServiceStatusThread.CancelEvent.WaitOne(0, false)) {
                        if ((sw.IsRunning == false) || (sw.ElapsedMilliseconds > 1000)) {
                            bool? currIsRunning;
                            try {
                                service.Refresh();
                                currIsRunning = (service.Status == ServiceControllerStatus.Running);
                            } catch (InvalidOperationException) {
                                currIsRunning = null;
                            }
                            if (lastIsRunning != currIsRunning) {
                                if (currIsRunning == null) {
                                    Tray.SetStatusToUnknown();
                                } else if (currIsRunning == true) {
                                    Tray.SetStatusToRunning();
                                } else {
                                    Tray.SetStatusToStopped();
                                }
                            }
                            lastIsRunning = currIsRunning;
                            sw.Reset();
                            sw.Start();
                        }
                        Thread.Sleep(100);
                    }
                }
            } catch (ThreadAbortException) { }
        }

    }
}
