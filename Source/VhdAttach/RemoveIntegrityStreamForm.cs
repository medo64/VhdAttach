using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VhdAttach {
    internal partial class RemoveIntegrityStreamForm : Form {
        public RemoveIntegrityStreamForm(FileInfo file) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.File = file;
        }

        private readonly FileInfo File;


        private void Form_Load(object sender, EventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Indeterminate);
            bwAction.RunWorkerAsync();
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.NoProgress);
        }


        private void bwAction_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            ReFS.RemoveIntegrityStream(this.File.FullName);
        }

        private void bwAction_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                Medo.MessageBox.ShowError(this, "Cannot remove integrity stream.\n\n" + e.Error.Message);
            }
            this.Close();
        }

    }
}
