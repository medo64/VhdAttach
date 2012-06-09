using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VhdAttach {
    internal partial class AttachForm : Form {

        private readonly IList<FileInfo> Files;
        private readonly bool MountReadOnly;
        private readonly bool InitializeDisk;
        private List<Exception> _exceptions;

        private AttachForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }

        public AttachForm(IList<FileInfo> files, bool mountReadOnly, bool initializeDisk)
            : this() {
            this.Files = files;
            this.MountReadOnly = mountReadOnly;
            this.InitializeDisk = initializeDisk;
        }

        public AttachForm(FileInfo file, bool mountReadOnly, bool initializeDisk)
            : this(new FileInfo[] { file }, mountReadOnly, initializeDisk) {
        }

        private void Form_Load(object sender, EventArgs e) {
            bw.RunWorkerAsync();
        }

        private void Form_Shown(object sender, EventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Indeterminate);
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.NoProgress);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            this._exceptions = new List<Exception>();
            FileInfo iFile = null;
            try {
                for (var i = 0; i < this.Files.Count; ++i) {
                    iFile = this.Files[i];
                    bw.ReportProgress(-1, iFile.Name);

                    var res = PipeClient.Attach(iFile.FullName, this.MountReadOnly, this.InitializeDisk);
                    if (res.IsError) {
                        this._exceptions.Add(new InvalidOperationException(iFile.Name, new Exception(res.Message)));
                    }
                }
            } catch (IOException) {
                this._exceptions.Add(new InvalidOperationException(iFile.Name, new Exception(Messages.ServiceIOException)));
            } catch (Exception ex) {
                this._exceptions.Add(new InvalidOperationException(iFile.Name, ex));
            }
            if (this._exceptions.Count > 0) { throw new InvalidOperationException(); }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            this.StatusLabel.Text = "Attaching" + Environment.NewLine + e.UserState.ToString();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (this.IsDisposed) { return; }

            this.progress.Value = 100;
            Medo.Windows.Forms.TaskbarProgress.SetPercentage(100);
            if (e.Error == null) {
                Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Normal);
            } else {
                Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Error);
                System.Environment.ExitCode = 1;
                foreach (var iException in this._exceptions) {
                    Medo.MessageBox.ShowError(this, string.Format("Virtual disk file \"{0}\" cannot be attached.\n\n{1}", iException.Message, iException.InnerException.Message));
                }
            }
            this.Close();
        }

    }
}
