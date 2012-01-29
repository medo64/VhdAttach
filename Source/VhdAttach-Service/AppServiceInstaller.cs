using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace VhdAttachService {
    
    [RunInstaller(true)]
    public class AppServiceInstaller : Installer {

        public AppServiceInstaller() {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;
            this.Installers.Add(serviceProcessInstaller);

            ServiceInstaller serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = AppService.Instance.ServiceName;
            serviceInstaller.DisplayName = Medo.Reflection.CallingAssembly.Product;
            serviceInstaller.Description = Medo.Reflection.CallingAssembly.Description;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            this.Installers.Add(serviceInstaller);
        }


        protected override void OnCommitted(IDictionary savedState) {
            Debug.WriteLine("OnCommitted : Begin.");
            base.OnCommitted(savedState);
            using (ServiceController sc = new ServiceController(AppService.Instance.ServiceName)) {
                Debug.WriteLine("OnCommitted : Service starting...");
                sc.Start();
                Debug.WriteLine("OnCommitted : Service started.");
            }
            Debug.WriteLine("OnCommitted : End.");
        }

        protected override void OnBeforeUninstall(IDictionary savedState) {
            Debug.WriteLine("OnBeforeUninstall : Begin.");
            using (ServiceController sc = new ServiceController(AppService.Instance.ServiceName)) {
                if (sc.Status != ServiceControllerStatus.Stopped) {
                    Debug.WriteLine("OnBeforeUninstall : Service stopping...");
                    sc.Stop();
                    Debug.WriteLine("OnBeforeUninstall : Service stopped...");
                }
            }
            base.OnBeforeUninstall(savedState);
            Debug.WriteLine("OnBeforeUninstall : End.");
        }

    }

}
