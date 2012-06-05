namespace VhdAttach {

    internal static class Settings {

        public static bool ShowMenu {
            get { return Medo.Configuration.Settings.Read("ShowMenu", false); }
            set { Medo.Configuration.Settings.Write("ShowMenu", value); }
        }

        public static bool UseService {
            get { return Medo.Configuration.Settings.Read("UseService", true); }
            set { Medo.Configuration.Settings.Write("UseService", value); }
        }


        public static string LastSizes {
            get { return Medo.Configuration.Settings.Read("LastSizes", "104857600"); }
            set { Medo.Configuration.Settings.Write("LastSizes", value); }
        }

        public static int LastSizeUnitIndex {
            get { return Medo.Configuration.Settings.Read("LastSizeUnitIndex", 0); }
            set { Medo.Configuration.Settings.Write("LastSizeUnitIndex", value); }
        }

        public static bool LastSizeThousandBased {
            get { return Medo.Configuration.Settings.Read("LastSizeThousandBased", false); }
            set { Medo.Configuration.Settings.Write("LastSizeThousandBased", value); }
        }

        public static bool LastSizeFixed {
            get { return Medo.Configuration.Settings.Read("LastSizeFixed", false); }
            set { Medo.Configuration.Settings.Write("LastSizeFixed", value); }
        }

        public static int WriteBufferSize {
            get { return 1024 * 1024; }
        }

    }

}
