using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace VhdAttach {
    partial class NewDiskForm : Form {
        public NewDiskForm(string fileName) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.FileName = fileName;
        }

        private readonly string FileName;


        private void Form_Load(object sender, EventArgs e) {
            try { nudSize.Value = Settings.LastSize; } catch (ArgumentOutOfRangeException) { nudSize.Value = 100; }
            try { dudSizeUnit.SelectedIndex = Settings.LastSizeUnitIndex; } catch (ArgumentOutOfRangeException) { dudSizeUnit.SelectedIndex = 1; }
            chbThousandSize.Checked = Settings.LastSizeThousandBased;
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Settings.LastSize = Convert.ToInt32(nudSize.Value);
            Settings.LastSizeUnitIndex = dudSizeUnit.SelectedIndex;
            Settings.LastSizeThousandBased = chbThousandSize.Checked;
        }


        private void btnOK_Click(object sender, EventArgs e) {
            try {
                this.Cursor = Cursors.WaitCursor;

                try {
                    File.Delete(this.FileName);
                } catch (IOException ex) {
                    this.Cursor = Cursors.Default;
                    Medo.MessageBox.ShowError(this, "File cannot be deleted.\n\n" + ex.Message);
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                try {
                    var sizeInBytes = GetSizeInBytes();
                    using (var vhd = new Medo.IO.VirtualDisk(this.FileName)) {
                        vhd.Create(sizeInBytes);
                    }
                } catch (IOException ex) {
                    this.Cursor = Cursors.Default;
                    Medo.MessageBox.ShowError(this, "Virtual disk cannot be created.\n\n" + ex.Message);
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                using (var form = new AttachForm(new FileInfo(this.FileName), false)) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }

                this.DialogResult = DialogResult.OK;
            } finally {
                this.Cursor = Cursors.Default;
            }
        }

        private void control_Changed(object sender, EventArgs e) {
            btnOK.Enabled = (GetSizeInBytes() >= 8 * 1000 * 1000 / 4096 * 4096);
        }


        private long GetSizeInBytes() {
            long size = Convert.ToInt64(nudSize.Value);
            long divider = (chbThousandSize.Checked) ? 1000 : 1024;
            long multiplier;
            switch (dudSizeUnit.SelectedIndex) {
                case 0: multiplier = divider * divider; break;
                case 1: multiplier = divider * divider * divider; break;
                default: multiplier = divider * divider; break;
            }
            var sizeInBytes = size * multiplier;
            sizeInBytes = (sizeInBytes / 4096) * 4096; //round to 4096 blocks
            return sizeInBytes;
        }

    }
}
