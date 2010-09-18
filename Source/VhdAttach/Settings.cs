using System;
using System.Net;

namespace VhdAttach {

    static class Settings {

        public static bool ShowMenu {
            get { return Medo.Configuration.Settings.Read("ShowMenu", false); }
            set { Medo.Configuration.Settings.Write("ShowMenu", value); }
        }

        public static bool UseService {
            get { return Medo.Configuration.Settings.Read("UseService", true); }
            set { Medo.Configuration.Settings.Write("UseService", value); }
        }

    }

}
