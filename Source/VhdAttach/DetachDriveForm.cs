using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace VhdAttach {
    partial class DetachDriveForm : Form {

        private IList<FileInfo> _files;
        private List<Exception> _exceptions;


        public DetachDriveForm(IList<FileInfo> file) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this._files = file;

            Medo.Windows.Forms.TaskbarProgress.DefaultOwner = this;
            Medo.Windows.Forms.TaskbarProgress.DoNotThrowNotImplementedException = true;
        }

        private void DetachForm_Load(object sender, EventArgs e) {
            bw.RunWorkerAsync();
        }

        private void DetachForm_Shown(object sender, EventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Indeterminate);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            this._exceptions = new List<Exception>();
            FileSystemInfo iDirectory = null;
            try {
                for (var i = 0; i < this._files.Count; ++i) {
                    iDirectory = new DirectoryInfo(this._files[i].FullName);
                    bw.ReportProgress(-1, iDirectory.Name);

                    var data = new JsonAttachDetachData(iDirectory.FullName);
                    var resBytes = WcfPipeClient.Execute("DetachDrive", data.ToJson());
                    var res = JsonResponseData.FromJson(resBytes);
                    if (res.ExitCode != ExitCodes.OK) {
                        this._exceptions.Add(new InvalidOperationException(iDirectory.Name, new Exception(res.Message)));
                    }
                }
            } catch (TimeoutException) {
                this._exceptions.Add(new InvalidOperationException(iDirectory.Name, new Exception("Cannot access VHD Attach service.")));
            } catch (Exception ex) {
                this._exceptions.Add(new InvalidOperationException(iDirectory.Name, ex));
            }
            if (this._exceptions.Count > 0) { throw new InvalidOperationException(); }
        }

        private static int IndexOfAny(string text, int startingIndex, params string[] fragment) {
            if ((fragment == null) || (fragment.Length == 0)) { return -1; }
            int minValue = text.IndexOf(fragment[0], startingIndex, StringComparison.InvariantCultureIgnoreCase);
            for (int i = 1; i < fragment.Length; ++i) {
                int iCurrMinValue = text.IndexOf(fragment[i], startingIndex);
                if ((iCurrMinValue >= 0) && (iCurrMinValue < minValue)) {
                    minValue = iCurrMinValue;
                }
            }
            return minValue;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.UserState != null) {
                this.StatusLabel.Text = "Detaching drive" + Environment.NewLine + e.UserState.ToString();
            }
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
                    Medo.MessageBox.ShowError(this, string.Format("Drive \"{0}\" cannot be detached.\n\n{1}", iException.Message, iException.InnerException.Message));
                }
            }
            this.Close();
        }

    }
}
