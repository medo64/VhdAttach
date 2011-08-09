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


        public static int LastSize {
            get { return Medo.Configuration.Settings.Read("LastSize", 100); }
            set { Medo.Configuration.Settings.Write("LastSize", value); }
        }

        public static int LastSizeUnitIndex {
            get { return Medo.Configuration.Settings.Read("LastSizeUnitIndex", 0); }
            set { Medo.Configuration.Settings.Write("LastSizeUnitIndex", value); }
        }

        public static bool LastSizeThousandBased {
            get { return Medo.Configuration.Settings.Read("LastSizeThousandBased", false); }
            set { Medo.Configuration.Settings.Write("LastSizeThousandBased", value); }
        }

    }

}
