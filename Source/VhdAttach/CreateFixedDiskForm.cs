using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VirtualHardDiskImage;

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

        private void Form_Load(object sender, System.EventArgs e) {
            bgw.RunWorkerAsync();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                btnCancel.PerformClick();
                e.Cancel = true;
            }
        }


        private void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            using (var stream = new FileStream(this.FileName, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.WriteThrough)) {
                var footer = new HardDiskFooter();
                footer.BeginUpdate();
                footer.CreatorApplication = VhdCreatorApplication.JosipMedvedVhdAttach;
                footer.CreatorVersion = Medo.Reflection.EntryAssembly.Version;
                footer.SetSize((UInt64)this.SizeInBytes);
                footer.OriginalSize = footer.CurrentSize;
                footer.DiskType = VhdDiskType.FixedHardDisk;

                footer.EndUpdate();

                var lastReport = DateTime.UtcNow;
                byte[] buffer = new byte[Settings.WriteBufferSize];
                ulong remaining = footer.CurrentSize;
                while (remaining > 0) {
                    if (bgw.CancellationPending) {
                        stream.Dispose();
                        File.Delete(this.FileName);
                        e.Cancel = true;
                        return;
                    }
                    ulong count = (ulong)buffer.Length;
                    if ((ulong)count > remaining) { count = remaining; }
                    stream.Write(buffer, 0, (int)count);
                    remaining -= count;
                    if (lastReport.AddSeconds(1) < DateTime.UtcNow) {
                        bgw.ReportProgress(100 - (int)(remaining * 100 / footer.CurrentSize));
                        lastReport = DateTime.UtcNow;
                    }
                }
                buffer = footer.Bytes;
                stream.Write(buffer, 0, buffer.Length);
            }

            bgw.ReportProgress(100);
        }

        private void bgw_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            prg.Value = e.ProgressPercentage;
        }

        private void bgw_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                this.DialogResult = DialogResult.Cancel;
            } else {
                this.DialogResult = DialogResult.OK;
            }
        }


        private void btnCancel_Click(object sender, System.EventArgs e) {
            bgw.CancelAsync();
            btnCancel.Enabled = false;
        }

    }
}
