using System.Diagnostics;

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

    }
}
