using System;
using System.Drawing;
using System.Windows.Forms;

namespace VhdAttach {
    internal static class Helper {

        #region ToolStripRenderer

        internal class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
                e.Graphics.ResetClip();
                e.Graphics.DrawLine(SystemPens.ControlDark, e.ToolStrip.ClientRectangle.Left, e.ToolStrip.ClientRectangle.Bottom - 1, e.ToolStrip.ClientRectangle.Right, e.ToolStrip.ClientRectangle.Bottom - 1);
            }

        }


        internal class ToolStripBorderlessSystemRenderer : ToolStripSystemRenderer {

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
                //base.OnRenderToolStripBorder(e);
            }

        }

        #endregion


        #region Toolstrip images

        private static void GetToolstripSizeAndSet(ToolStrip toolstrip, out int size, out string set) {
            Form form = null;
            Control findParent = toolstrip;
            while ((findParent != null) && (form == null)) {
                form = findParent.Parent as Form;
                findParent = findParent.Parent;
            }
            if (form == null) { form = new Form(); } //workaround if parent is gone

            using (var g = form.CreateGraphics()) {
                var scale = Math.Max(Math.Max(g.DpiX, g.DpiY), 96.0) / 96.0;

                if (scale < 1.5) {
                    size = 16;
                    set = "_16";
                } else if (scale < 2) {
                    size = 24;
                    set = "_24";
                } else if (scale < 3) {
                    size = 32;
                    set = "_32";
                } else {
                    var base32 = 16 * scale / 32;
                    var base48 = 16 * scale / 48;
                    if ((base48 - (int)base48) < (base32 - (int)base32)) {
                        size = 48 * (int)base48;
                        set = "_48";
                    } else {
                        size = 32 * (int)base32;
                        set = "_32";
                    }
                }
            }
        }

        internal static void UpdateToolstripImages(ImageList imageList, ToolStrip toolstrip) {
            int size;
            string set;
            GetToolstripSizeAndSet(toolstrip, out size, out set);
            toolstrip.ImageScalingSize = new Size(size, size);

            var resources = VhdAttach.Properties.Resources.ResourceManager;
            foreach (ToolStripItem item in toolstrip.Items) {
                if (string.IsNullOrEmpty(item.Name)) { continue; }
                UpdateToolstripImage(item, item.Name, size, set);
            }

            if (imageList != null) {
                imageList.Images.Clear();
                imageList.ImageSize = new Size(size, size);
                imageList.Images.Add(resources.GetObject("StatusInformation" + set) as Bitmap);
                imageList.Images.Add(resources.GetObject("StatusWarning" + set) as Bitmap);
                imageList.Images.Add(resources.GetObject("StatusError" + set) as Bitmap);
                imageList.Images.Add(resources.GetObject("StatusCritical" + set) as Bitmap);
                imageList.Images.Add(resources.GetObject("StatusLocked" + set) as Bitmap);
            }
        }

        private static void UpdateToolstripImage(ToolStripItem item, string name, int size, string set) {
            var resources = VhdAttach.Properties.Resources.ResourceManager;
            var resourceName = name + set;
            var bitmap = resources.GetObject(resourceName) as Bitmap;
            if (bitmap != null) {
                item.ImageScaling = ToolStripItemImageScaling.None;
                item.Image = new Bitmap(bitmap, new Size(size, size));
            }
        }

        internal static void UpdateToolstripImage(ToolStripItem item, string name) {
            int size;
            string set;
            GetToolstripSizeAndSet(item.GetCurrentParent(), out size, out set);
            UpdateToolstripImage(item, name, size, set);
        }

        #endregion

    }
}
