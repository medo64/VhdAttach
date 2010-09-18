using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;

namespace VhdAttach {
    partial class AttachForm : Form {

        private IList<FileInfo> _files;
        private List<Exception> _exceptions;

        public AttachForm(IList<FileInfo> files) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this._files = files;

            Medo.Windows.Forms.TaskbarProgress.DefaultOwner = this;
            Medo.Windows.Forms.TaskbarProgress.DoNotThrowNotImplementedException = true;
        }

        private void AttachForm_Load(object sender, EventArgs e) {
            bw.RunWorkerAsync();
        }

        private void AttachForm_Shown(object sender, EventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Indeterminate);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            this._exceptions = new List<Exception>();
            FileInfo iFile = null;
            try {
                for (var i = 0; i < this._files.Count; ++i) {
                    iFile = this._files[i];
                    bw.ReportProgress(-1, iFile.Name);

                    var data = new JsonAttachDetachData(iFile.FullName);
                    var resBytes = WcfPipeClient.Execute("Attach", data.ToJson());
                    var res = JsonResponseData.FromJson(resBytes);
                    if (res.ExitCode != ExitCodes.OK) {
                        this._exceptions.Add(new InvalidOperationException(iFile.Name, new Exception(res.Message)));
                    }
                }
            } catch (TimeoutException) {
                this._exceptions.Add(new InvalidOperationException(iFile.Name, new Exception("Cannot access VHD Attach service.")));
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
                System.Environment.ExitCode = ExitCodes.CannotExecute;
                foreach (var iException in this._exceptions) {
                    Medo.MessageBox.ShowError(this, string.Format("Virtual disk file \"{0}\" cannot be attached.\n\n{1}", iException.Message, iException.InnerException.Message));
                }
            }
            this.Close();
        }

    }
}
