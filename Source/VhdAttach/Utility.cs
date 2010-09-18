using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.IO.Pipes;

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
