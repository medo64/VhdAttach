using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace VhdAttach {
    internal static class Utility {

        public static ProcessStartInfo GetProcessStartInfo(string exe, string arguments) {
            var startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = Process.GetCurrentProcess().StartInfo.CreateNoWindow;
            startInfo.FileName = exe;
            startInfo.Arguments = arguments;
            startInfo.WorkingDirectory = System.Environment.CurrentDirectory;
            return startInfo;
        }


        public static void FixServiceErrorsIfNeeded() {
            using (var service = new ServiceController("VhdAttach")) {
                try {
                    if (service.Status != ServiceControllerStatus.Running) {
                        try {
                            Utility.ForceStartService();
                        } catch (InvalidOperationException) { }
                    }
                } catch (InvalidOperationException) {
                    try {
                        Utility.ForceInstallService();
                    } catch (InvalidOperationException) { }
                }
            }
        }

        public static void ForceStartService() {
            try {
                var directory = (new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName;
                Process.Start(Path.Combine(directory, "VhdAttachService.exe"), "/Start").WaitForExit();
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex); //cannot throw InvalidOperationException because of service existance detection
            }
        }

        public static void ForceInstallService() {
            try {
                var directory = (new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName;
                Process.Start(Path.Combine(directory, "VhdAttachService.exe"), "/Install").WaitForExit();
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

    }
}
