using System;
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;


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
            base.OnCommitted(savedState);
            using (ServiceController sc = new ServiceController(AppService.Instance.ServiceName)) {
                sc.Start();
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState) {
            using (ServiceController sc = new ServiceController(AppService.Instance.ServiceName)) {
                if (sc.Status != ServiceControllerStatus.Stopped) {
                    sc.Stop();
                }
            }
            base.OnBeforeUninstall(savedState);
        }

    }

}
