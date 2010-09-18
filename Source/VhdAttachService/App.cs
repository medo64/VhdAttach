using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Configuration.Install;
using System.Reflection;
using System.Diagnostics;

namespace VhdAttachService {

    static class App {

        [STAThread()]
        static void Main() {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (Medo.Application.Args.Current.ContainsKey("Interactive")) {

                AppServiceThread.Start();
                Medo.Windows.Forms.AboutBox.ShowDialog();
                AppServiceThread.Stop();
                System.Environment.Exit(0);

            } else if (Medo.Application.Args.Current.ContainsKey("Install")) {

                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                System.Environment.Exit(ExitCodes.OK);

            } else if (Medo.Application.Args.Current.ContainsKey("Uninstall")) {

                try {
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                    System.Environment.Exit(ExitCodes.OK);
                } catch (System.Configuration.Install.InstallException) { //no service with that name
                    System.Environment.Exit(ExitCodes.GenericError);
                }

            } else {
                ServiceBase.Run(new ServiceBase[] { AppService.Instance });
            }
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Medo.Diagnostics.ErrorReport.SaveToTemp(e.ExceptionObject as Exception);
            AppService.Instance.ExitCode = 1064; //ERROR_EXCEPTION_IN_SERVICE
            AppService.Instance.AutoLog = false;
            System.Threading.Thread.Sleep(1000); //just to sort it properly in event log.
            Environment.Exit(AppService.Instance.ExitCode);
        }

    }

}
