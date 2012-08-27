using System.Drawing;
using System.Windows.Forms;

namespace VhdAttach {
    internal class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            e.Graphics.ResetClip();
            e.Graphics.DrawLine(SystemPens.ControlDark, e.ToolStrip.ClientRectangle.Left, e.ToolStrip.ClientRectangle.Bottom - 1, e.ToolStrip.ClientRectangle.Right, e.ToolStrip.ClientRectangle.Bottom - 1);
        }

    }

}
