using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VirtualHardDiskImage;

namespace VhdAttach {
    internal partial class CreateFixedDiskForm : Form {

        public CreateFixedDiskForm(string fileName, long sizeInBytes, bool isVhdX) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.FileName = fileName;
            this.SizeInBytes = sizeInBytes;
            this.IsVhdX = isVhdX;
        }


        private readonly string FileName;
        private readonly long SizeInBytes;
        private readonly bool IsVhdX;


        private void Form_Load(object sender, System.EventArgs e) {
            bgw.RunWorkerAsync();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                btnCancel.PerformClick();
                e.Cancel = true;
                Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Error);
            }
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.NoProgress);
        }


        private void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            if (this.IsVhdX) {
                bgw.ReportProgress(-1);
                if (CreateVhdX() == false) {
                    e.Cancel = true;
                    return;
                }
            } else {
                if (CreateVhd() == false) {
                    e.Cancel = true;
                    return;
                }
            }

            bgw.ReportProgress(100);
        }

        private void bgw_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            if (e.ProgressPercentage >= 0) {
                Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Normal);
                Medo.Windows.Forms.TaskbarProgress.SetPercentage(e.ProgressPercentage);
                prg.Style = ProgressBarStyle.Continuous;
                prg.Value = e.ProgressPercentage;
            } else {
                Medo.Windows.Forms.TaskbarProgress.SetState(Medo.Windows.Forms.TaskbarProgressState.Indeterminate);
                prg.Style = ProgressBarStyle.Marquee;
            }
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


        private bool CreateVhd() {
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
                        return false;
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
            return true;
        }

        private bool CreateVhdX() {
            using (var vhdx = new Medo.IO.VirtualDisk(this.FileName)) {
                vhdx.CreateAsync(this.SizeInBytes, Medo.IO.VirtualDiskCreateOptions.FullPhysicalAllocation, 0, 0, Medo.IO.VirtualDiskType.Vhdx);
                var progress = vhdx.GetCreateProgress();
                while (progress.IsDone == false) {
                    //bgw.ReportProgress(progress.ProgressPercentage);
                    bgw.ReportProgress(-1);
                    if (bgw.CancellationPending) {
                        //TODO
                        return false;
                    }
                    Thread.Sleep(500);
                    progress = vhdx.GetCreateProgress();
                }
            }
            return true;
        }

    }
}
