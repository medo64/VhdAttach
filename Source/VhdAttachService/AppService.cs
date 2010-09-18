using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace VhdAttachService {    
    internal class AppService : ServiceBase {

        private static AppService _instance = new AppService();
        public static AppService Instance { get { return _instance; } }


        private AppService() {
            this.AutoLog = true;
            this.CanStop = true;
            this.ServiceName = "VhdAttach";
        }

        protected override void OnStart(string[] args) {
            AppServiceThread.Start();
        }

        protected override void OnStop() {
            AppServiceThread.Stop();
        }

    }
}
