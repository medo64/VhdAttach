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

        internal static void UpdateToolstripImages(ToolStrip toolstrip) {
            var form = toolstrip.Parent as Form;

            using (var g = form.CreateGraphics()) {
                var scale = Math.Max(Math.Max(g.DpiX, g.DpiY), 96.0) / 96.0;

                int size;
                string set;
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

                toolstrip.ImageScalingSize = new Size(size, size);

                var resources = VhdAttach.Properties.Resources.ResourceManager;
                foreach (ToolStripItem item in toolstrip.Items) {
                    if (string.IsNullOrEmpty(item.Name)) { continue; }
                    var resourceName = item.Name + set;
                    var bitmap = resources.GetObject(resourceName) as Bitmap;
                    if (bitmap != null) {
                        item.Image = bitmap;
                    }
                }
            }
        }

        #endregion

    }
}
