using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Medo.Text;

namespace VhdAttach {
    internal static class App {

        private static readonly Mutex SetupMutex = new Mutex(false, @"Global\JosipMedved_VhdAttach");


        [STAThread]
        static void Main() {
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


            bool doAttach = Medo.Application.Args.Current.ContainsKey("Attach");
            bool doDetach = Medo.Application.Args.Current.ContainsKey("Detach") && (!doAttach);
            bool doDetachDrive = Medo.Application.Args.Current.ContainsKey("DetachDrive") && (!doAttach) && (!doDetach);
            bool doAnything = doAttach || doDetach || doDetachDrive;

            if (doAnything) {

                if (Settings.UseService) { //send to Service

                    string[] argfiles = Medo.Application.Args.Current.GetValues("");
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
                        Application.Run(appForm);
                        System.Environment.Exit(System.Environment.ExitCode);
                    } else {
                        System.Environment.Exit(1);
                    }



                } else { //send to Executor

                    var args = new List<string>(Environment.GetCommandLineArgs());
                    args.RemoveAt(0);
                    var sbArgs = new StringAdder(" ");
                    foreach (var iArg in args) {
                        sbArgs.Append(iArg);
                    }

                    var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                    using (var process = new Process()) {
                        process.StartInfo = Utility.GetProcessStartInfo(exe, sbArgs.ToString());
                        process.Start();
                        process.WaitForExit();
                        System.Environment.Exit(process.ExitCode);
                    }
                }

            } else { //open localy

                //string[] argfiles = Medo.Application.Args.Current.GetValues("");
                Application.Run(new MainForm());

            }


            SetupMutex.Close();
        }



        private static void UnhandledCatch_ThreadException(object sender, ThreadExceptionEventArgs e) {
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("http://jmedved.com/ErrorReport/"));
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
