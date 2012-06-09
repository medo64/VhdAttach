using System.Globalization;
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


        public static long LastSize {
            get {
                long size;
                if (long.TryParse(Medo.Configuration.Settings.Read("LastSize", "104857600"), NumberStyles.Integer, CultureInfo.InvariantCulture, out size)) {
                    return size;
                } else {
                    return 0;
                }
            }
            set { Medo.Configuration.Settings.Write("LastSize", value.ToString(CultureInfo.InvariantCulture)); }
        }

        public static string LastSizeUnit {
            get { return (Medo.Configuration.Settings.Read("LastSizeUnit", "GB").Equals("MB", System.StringComparison.OrdinalIgnoreCase)) ? "MB" : "GB"; }
            set { Medo.Configuration.Settings.Write("LastSizeUnit", value); }
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
