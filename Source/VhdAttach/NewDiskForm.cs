using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Medo.Localization.Croatia;
using Medo.Extensions;

namespace VhdAttach {
    internal partial class NewDiskForm : Form {
        public NewDiskForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            erpError.SetIconAlignment(btnOK, ErrorIconAlignment.MiddleLeft);
            erpError.SetIconPadding(btnOK, SystemInformation.Border3DSize.Width);
        }


        private static readonly NumberDeclination BytesSuffix = new NumberDeclination("byte", "bytes", "bytes");
        public string FileName { get; private set; }


        private void Form_Load(object sender, EventArgs e) {
            try { nudSize.Value = Settings.LastSize; } catch (ArgumentOutOfRangeException) { nudSize.Value = 100; }
            try { dudSizeUnit.SelectedIndex = Settings.LastSizeUnitIndex; } catch (ArgumentOutOfRangeException) { dudSizeUnit.SelectedIndex = 1; }
            chbThousandSize.Checked = Settings.LastSizeThousandBased;
            radFixed.Checked = Settings.LastSizeFixed;
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Settings.LastSize = Convert.ToInt32(nudSize.Value);
            Settings.LastSizeUnitIndex = dudSizeUnit.SelectedIndex;
            Settings.LastSizeThousandBased = chbThousandSize.Checked;
            Settings.LastSizeFixed = radFixed.Checked;
        }


        private void btnOK_Click(object sender, EventArgs e) {
            try {
                this.Cursor = Cursors.WaitCursor;

                using (var frm = new SaveFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Virtual disk files (*.vhd)|*.vhd|All files (*.*)|*.*", FilterIndex = 0, OverwritePrompt = true, Title = "New disk", ValidateNames = true }) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        this.FileName = frm.FileName;
                    } else {
                        return;
                    }
                }

                DriveInfo drive = null;
                try {
                    drive = new DriveInfo(this.FileName);
                    if (drive.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase) == false) {
                        if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) < 6000002) { //Windows 8
                            if (Medo.MessageBox.ShowWarning(this, "Due to operating system limitations, virtual disk created on this drive will not be attachable.\nIn order to attach virtual disk it will need to be on NTFS-formatted drive.\n\nDo you wish to continue?", MessageBoxButtons.YesNo) == DialogResult.No) {
                                return;
                            }
                        }
                    }
                } catch (ArgumentException) { }

                try {
                    File.Delete(this.FileName);
                } catch (IOException ex) {
                    this.Cursor = Cursors.Default;
                    Medo.MessageBox.ShowError(this, "File cannot be deleted.\n\n" + ex.Message);
                    return;
                } catch (UnauthorizedAccessException ex) {
                    this.Cursor = Cursors.Default;
                    Medo.MessageBox.ShowError(this, "File cannot be deleted.\n\n" + ex.Message);
                    return;
                }

                var diskSize = GetSizeInBytes();
                if (drive != null) {
                    if (drive.AvailableFreeSpace < diskSize) {
                        if (Medo.MessageBox.ShowWarning(this, string.Format("There is not enough free space available!\nVirtual disk will require {0} while drive has only {1} free.\n\nDo you wish to continue?", BinaryPrefixExtensions.ToBinaryPrefixString(diskSize, "B", "0"), BinaryPrefixExtensions.ToBinaryPrefixString(drive.AvailableFreeSpace, "B", "0")), MessageBoxButtons.YesNo) == DialogResult.No) {
                            return;
                        }
                    }
                    if (drive.DriveFormat.Equals("FAT32", StringComparison.OrdinalIgnoreCase)) {
                        if (Medo.MessageBox.ShowWarning(this, "Due to operating system limitations it will not be possible to create virtual disk larger than 2 GB.\n\nDo you wish to continue?", MessageBoxButtons.YesNo) == DialogResult.No) {
                            return;
                        }
                    }
                }

                try {
                    if (radFixed.Checked) {
                        using (var frm = new CreateFixedDiskForm(this.FileName, diskSize)) {
                            if (frm.ShowDialog(this) == DialogResult.Cancel) {
                                return;
                            }
                        }
                    } else { //Dynamic
                        using (var vhd = new Medo.IO.VirtualDisk(this.FileName)) {
                            vhd.Create(diskSize, Medo.IO.VirtualDiskCreateOptions.None);
                        }
                    }
                } catch (IOException ex) {
                    this.Cursor = Cursors.Default;
                    Medo.MessageBox.ShowError(this, "Virtual disk cannot be created.\n\n" + ex.Message);
                    return;
                }

                using (var form = new AttachForm(new FileInfo(this.FileName), false, true)) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }

                this.DialogResult = DialogResult.OK;
            } finally {
                this.Cursor = Cursors.Default;
            }
        }

        private void control_Changed(object sender, EventArgs e) {
            var sizeInBytes = GetSizeInBytes();
            txtSizeInBytes.Text = string.Format(CultureInfo.CurrentCulture, "{0:#,##0}", sizeInBytes);

            var sizeInBytesOk = (sizeInBytes >= 8 * 1000 * 1000 / 4096 * 4096);
            if (sizeInBytesOk == false) {
                erpError.SetError(btnOK, "Disk is too small.");
            } else {
                erpError.SetError(btnOK, null);
            }

            btnOK.Enabled = sizeInBytesOk;
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
