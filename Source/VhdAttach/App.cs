using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace VhdAttach {
    internal static class App {

        [STAThread]
        static void Main() {
            bool createdNew;
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow));
            using (var setupMutex = new Mutex(false, @"Global\JosipMedved_VhdAttach", out createdNew, mutexSecurity)) {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                Medo.Application.UnhandledCatch.ThreadException += new EventHandler<ThreadExceptionEventArgs>(UnhandledCatch_ThreadException);
                Medo.Application.UnhandledCatch.Attach();

                if (!((Environment.OSVersion.Version.Build < 7000) || (App.IsRunningOnMono))) {
                    var appId = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                    if (appId.Length > 127) { appId = @"JosipMedved_VhdAttach\" + appId.Substring(appId.Length - 127 - 20); }
                    NativeMethods.SetCurrentProcessExplicitAppUserModelID(appId);
                } else {
                    Medo.MessageBox.ShowError(null, "This program requires Windows 7 or later.");
                    System.Environment.Exit(1);
                }

                Medo.Windows.Forms.TaskbarProgress.DoNotThrowNotImplementedException = true;


                bool doAttach = Medo.Application.Args.Current.ContainsKey("Attach");
                bool doDetach = Medo.Application.Args.Current.ContainsKey("Detach") && (!doAttach);
                bool doDetachDrive = Medo.Application.Args.Current.ContainsKey("DetachDrive") && (!doAttach) && (!doDetach);
                bool doChangeLetter = Medo.Application.Args.Current.ContainsKey("ChangeLetter") && (!doAttach) && (!doDetach) && (!doDetachDrive);

                bool doAnything = doAttach || doDetach || doDetachDrive || doChangeLetter;

                if (doAnything) {

                    string[] argfiles = Medo.Application.Args.Current.GetValues("");

                    if (doChangeLetter) {
                        CommandLineAddon cla = new CommandLineAddon();
                        int res = cla.ChangeDriveLetter(argfiles);
                        System.Environment.Exit(res);
                        return;
                    }
                   
                    var files = new List<FileInfo>();
                    foreach (var iFile in argfiles) {
                        files.Add(new FileInfo(iFile.TrimEnd(new char[] { '\"' })));
                    }

                    if (files.Count == 0) {
                        System.Environment.Exit(1);
                        return;
                    }

                    Form appForm = null;
                    if (doAttach) {
                        appForm = new AttachForm(files, Medo.Application.Args.Current.ContainsKey("readonly"), false);
                    } else if (doDetach) {
                        appForm = new DetachForm(files);
                    } else if (doDetachDrive) {
                        appForm = new DetachDriveForm(files);
                    }

                    if (appForm != null) {
                        Medo.Windows.Forms.TaskbarProgress.DefaultOwner = appForm;
                        Application.Run(appForm);
                        System.Environment.Exit(System.Environment.ExitCode);
                    } else {
                        System.Environment.Exit(1);
                    }

                } else { //open localy

                    Application.Run(new MainForm());

                }
            }
        }



        private static void UnhandledCatch_ThreadException(object sender, ThreadExceptionEventArgs e) {
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("http://jmedved.com/feedback/"));
#else
            throw e.Exception;
#endif
        }


        private static class NativeMethods {

            [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern UInt32 SetCurrentProcessExplicitAppUserModelID(String AppID);

        }

        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }
}
