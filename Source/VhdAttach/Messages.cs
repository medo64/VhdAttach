using System;
using System.Windows.Forms;

namespace VhdAttach {
    internal static class Messages {

        public static readonly string ServiceIOException = "Cannot contact VHD Attach service.";

        public static void ShowServiceIOException(IWin32Window owner, Exception ex) {
            Medo.MessageBox.ShowError(owner, string.Format(ServiceIOException + "\n\n{0}", ex.Message));
        }


        public static readonly string DiskTooSmall = "Disk is too small (8 MB minimum).";

    }
}
