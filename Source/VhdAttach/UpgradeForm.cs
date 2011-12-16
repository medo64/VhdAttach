using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace VhdAttach {
    public partial class UpgradeForm : Form {
        public UpgradeForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }

        private void UpgradeForm_Load(object sender, EventArgs e) {
            bcwCheck.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            bcwCheck.CancelAsync();
            bcwDownload.CancelAsync();
        }

        private void bcwCheck_DoWork(object sender, DoWorkEventArgs e) {
            try {
                string url = "http://jmedved.com/upgrade/vhdattach/" + Assembly.GetEntryAssembly().GetName().Version.ToString() + "/";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.AllowAutoRedirect = false;
                request.Method = "HEAD";
                request.Proxy = HttpWebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    switch (response.StatusCode) {
                        case HttpStatusCode.Forbidden: e.Result = null; break; //no upgrade
                        case HttpStatusCode.SeeOther: e.Result = response.Headers["Location"]; break; //upgrade at Location
                        default: throw new InvalidOperationException("Unexpected answer from upgrade server (" + response.StatusCode.ToString() + ").");
                    }
                }
            } catch (InvalidOperationException ex) {
                var webEx = ex as WebException;
                if (webEx != null) {
                    var response = webEx.Response as HttpWebResponse;
                    if (response != null) {
                        if (response.StatusCode == HttpStatusCode.Forbidden) {
                            e.Result = null;
                            return;
                        }
                    }
                }
                throw ex;
            }
        }

        private void bcwCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            prb.Style = ProgressBarStyle.Continuous;
            if (e.Error != null) {
                Medo.MessageBox.ShowWarning(this, "Cannot check for upgrade at this time.\n\n" + e.Error.Message);
                this.Close();
            } else {
                var location = e.Result as string;
                if (location != null) {
                    prb.Value = 100;
                    lblStatus.Text = "Upgrade is available.";
                    btnDownload.Tag = location;
                    btnDownload.Visible = true;
                    btnDownload.Focus();
                } else {
                    lblStatus.Text = "No upgrade at this time.";
                    btnCancel.Text = "&Close";
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e) {
            prb.Value = 0;
            btnDownload.Enabled = false;
            bcwDownload.RunWorkerAsync(btnDownload.Tag);
            lblStatus.Text = "Download in progress.";
        }

        private void bcwDownload_DoWork(object sender, DoWorkEventArgs e) {
            try {
                string url = (string)e.Argument;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.Proxy = HttpWebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    var len = response.ContentLength;
                    var buffer = new byte[1024];
                    using (var stream = response.GetResponseStream()) {
                        stream.ReadTimeout = 5000;
                        using (var bytes = new MemoryStream()) {
                            while (bytes.Length < len) {
                                if (bcwDownload.CancellationPending) {
                                    e.Cancel = true;
                                    return;
                                }
                                var read = stream.Read(buffer, 0, buffer.Length);
                                if (read > 0) {
                                    bytes.Write(buffer, 0, read);
                                    bcwDownload.ReportProgress((int)(bytes.Length * 100 / len));
                                } else {
                                    break;
                                }
                            }
                            if (bytes.Length != len) {
                                throw new WebException("File content is corrupted.");
                            } else {
                                var folder = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                                var file = response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1];
                                var fileName = Path.Combine(folder, file);
                                File.WriteAllBytes(fileName, bytes.ToArray());
                                e.Result = fileName;
                            }
                        }
                    }
                }
            } catch (InvalidOperationException ex) {
                throw ex;
            }
        }

        private void bcwDownload_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            prb.Value = e.ProgressPercentage;
            var text = e.UserState as string;
            if (text != null) { lblStatus.Text = text; }
        }

        private void bcwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                prb.Value = 0;
                lblStatus.Text = "Cannot upgrade.";
                Medo.MessageBox.ShowWarning(this, "Cannot download upgrade at this time.\n\n" + e.Error.Message);
                this.Close();
            } else if (e.Cancelled) {
                this.Close();
            } else {
                lblStatus.Text = "Ready for upgrade.";
                var path = e.Result as string;
                if (path != null) {
                    if (Medo.MessageBox.ShowQuestion(this, "Do you wish to start installation at this time?\n\nIf you select No you will be asked where to save installation file.", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        Process.Start(path);
                        Application.Exit();
                    } else {
                        using (var frm = new SaveFileDialog() { AddExtension = false, CheckPathExists = true, FileName = new FileInfo(path).Name, InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) }) {
                            if (frm.ShowDialog(this) == DialogResult.OK) {
                                File.Move(path, frm.FileName);
                            } else {
                                File.Delete(path);
                            }
                            this.Close();
                        }
                    }
                }
            }
        }

    }
}
