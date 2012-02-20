using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VhdAttachService {
    internal static class Tray {

        private static NotifyIcon Notify;

        internal static void Show(bool interactive = false) {
            Tray.Notify = new NotifyIcon();
            Tray.Notify.ContextMenu = new ContextMenu();
            Tray.Notify.ContextMenu.MenuItems.Add(new MenuItem("Exit", Tray_Exit_OnClick));
            Tray.Notify.Icon = GetApplicationIcon();
            Tray.Notify.Text = Medo.Reflection.CallingAssembly.Title;
            Tray.Notify.Visible = true;
        }

        internal static void SetStatusToRunningInteractive() {
            Tray.Notify.Icon = GetAnnotatedIcon(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Medo.Reflection.CallingAssembly.Name + ".Resources.Service_RunningInteractive_12.png")));
            Tray.Notify.Text = Medo.Reflection.CallingAssembly.Title + " (PID=" + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture) + ")";
        }

        internal static void SetStatusToUnknown() {
            Tray.Notify.Icon = GetAnnotatedIcon(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Medo.Reflection.CallingAssembly.Name + ".Resources.Service_Unknown_12.png")));
            Tray.Notify.Text = Medo.Reflection.CallingAssembly.Title + " - Unknown state.";
        }

        internal static void SetStatusToRunning() {
            Tray.Notify.Icon = GetAnnotatedIcon(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Medo.Reflection.CallingAssembly.Name + ".Resources.Service_Running_12.png")));
            Tray.Notify.Text = Medo.Reflection.CallingAssembly.Title + " - Running.";
        }

        internal static void SetStatusToStopped() {
            Tray.Notify.Icon = GetAnnotatedIcon(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Medo.Reflection.CallingAssembly.Name + ".Resources.Service_Stopped_12.png")));
            Tray.Notify.Text = Medo.Reflection.CallingAssembly.Title + " - Stopped.";
        }

        internal static void Hide() {
            Tray.Notify.Visible = false;
        }


        private static void Tray_Exit_OnClick(object sender, EventArgs e) {
            Application.Exit();
        }



        #region Helpers

        private static Icon GetAnnotatedIcon(Image annotation) {
            var icon = GetApplicationIcon();

            if (icon != null) {
                var image = icon.ToBitmap();
                if (icon != null) {
                    using (var g = Graphics.FromImage(image)) {
                        g.DrawImage(annotation, (int)g.VisibleClipBounds.Width - annotation.Width - 2, (int)g.VisibleClipBounds.Height - annotation.Height - 2);
                        g.Flush();
                    }
                }
                return Icon.FromHandle(image.GetHicon());
            }
            return null;
        }

        private static Icon GetApplicationIcon() {
            IntPtr hLibrary = NativeMethods.LoadLibrary(Assembly.GetEntryAssembly().Location);
            if (!hLibrary.Equals(IntPtr.Zero)) {
                IntPtr hIcon = NativeMethods.LoadImage(hLibrary, "#32512", NativeMethods.IMAGE_ICON, 20, 20, 0);
                if (!hIcon.Equals(System.IntPtr.Zero)) {
                    Icon icon = Icon.FromHandle(hIcon);
                    if (icon != null) { return icon; }
                }
            }
            return null;
        }

        private static class NativeMethods {

            public const UInt32 IMAGE_ICON = 1;


            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadImage(IntPtr hInstance, String lpIconName, UInt32 uType, Int32 cxDesired, Int32 cyDesired, UInt32 fuLoad);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadLibrary(string lpFileName);

        }

        #endregion

    }
}
