using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VhdAttach {
    internal partial class CreateFixedDiskForm : Form {

        public CreateFixedDiskForm(string fileName, long sizeInBytes) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.FileName = fileName;
            this.SizeInBytes = sizeInBytes;
        }


        private readonly string FileName;
        private readonly long SizeInBytes;
        private void CreateFixedDiskForm_Load(object sender, System.EventArgs e) {
            bgw.RunWorkerAsync();
        }


        private void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            using (var vhd = new Medo.IO.VirtualDisk(this.FileName)) {
                var options = Medo.IO.VirtualDiskCreateOptions.FullPhysicalAllocation;
                vhd.Create(this.SizeInBytes, options);
            }
        }

        private void bgw_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            prg.Value = e.ProgressPercentage;
        }

        private void bgw_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            this.DialogResult = DialogResult.OK;
        }


        private void btnCancel_Click(object sender, System.EventArgs e) {
            bgw.CancelAsync();
        }

    }
}
