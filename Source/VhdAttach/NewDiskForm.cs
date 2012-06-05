using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Medo.Extensions;
using Medo.Localization.Croatia;

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

        private SizeStorage Sizes = new SizeStorage();


        private void Form_Load(object sender, EventArgs e) {
            try { cmbSizeUnit.SelectedIndex = Settings.LastSizeUnitIndex; } catch (ArgumentOutOfRangeException) { cmbSizeUnit.SelectedIndex = 1; }
            FillSizes();
            if (cmbSize.Items.Count > 0) {
                cmbSize.Text = cmbSize.Items[0].ToString();
            } else {
                cmbSize.Text = SizeStorage.GetSizeText(104857600, cmbSizeUnit.SelectedIndex);
            }
            chbThousandSize.Checked = Settings.LastSizeThousandBased;
            radFixed.Checked = Settings.LastSizeFixed;
        }

        private void FillSizes() {
            cmbSize.Items.Clear();
            foreach (var size in this.Sizes.GetSizes()) {
                cmbSize.Items.Add(SizeStorage.GetSizeText(size, cmbSizeUnit.SelectedIndex));
            }
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

        private void cmbSizeUnit_SelectedIndexChanged(object sender, EventArgs e) {
            FillSizes();
            control_Changed(null, null);
        }


        private long GetSizeInBytes() {
            long sizeInBytes = SizeStorage.GetSizeInBytes(cmbSize.Text, cmbSizeUnit.SelectedIndex, chbThousandSize.Checked);
            return (sizeInBytes / 4096) * 4096; //round to 4096 blocks
        }


        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            this.Sizes.AddSize(SizeStorage.GetSizeInBytes(cmbSize.Text, cmbSizeUnit.SelectedIndex));
            Settings.LastSizeUnitIndex = cmbSizeUnit.SelectedIndex;
            Settings.LastSizeThousandBased = chbThousandSize.Checked;
            Settings.LastSizeFixed = radFixed.Checked;
        }


        private class SizeStorage {

            public SizeStorage() {
                foreach (var text in Settings.LastSizes.Split('|')) {
                    long value;
                    if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
                        if (this.Sizes.Contains(value) == false) {
                            this.Sizes.Add(value);
                        }
                    }
                }
            }

            private IList<long> Sizes = new List<long>();


            public IEnumerable<long> GetSizes() {
                foreach (var size in this.Sizes)
                    yield return size;
            }

            public void AddSize(long value) {
                if (value == 0) { return; }
                if (this.Sizes.Contains(value)) {
                    this.Sizes.Remove(value);
                }
                this.Sizes.Insert(0, value);
                while (this.Sizes.Count > 10) { this.Sizes.RemoveAt(10); }

                var sb = new StringBuilder();
                foreach (var size in GetSizes()) {
                    if (sb.Length > 0) { sb.Append("|"); }
                    sb.Append(size.ToString(CultureInfo.InvariantCulture));
                }
                Settings.LastSizes = sb.ToString();
            }

            private static readonly NumberDeclination BytesSuffix = new NumberDeclination("byte", "bytes", "bytes");
            public static string GetSizeText(long size, int selectedUnitIndex) {
                switch (selectedUnitIndex) {
                    case 0: return (size / 1024.0 / 1024.0).ToString("0.#", CultureInfo.CurrentCulture);
                    case 1: return (size / 1024.0 / 1024.0 / 1024.0).ToString("0.##", CultureInfo.CurrentCulture);
                    default: return size.ToString(CultureInfo.CurrentUICulture);
                }
            }

            public static long GetSizeInBytes(string text, int selectedUnitIndex, bool use1000 = false) {
                double value;
                if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value) || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
                    if (value > int.MaxValue) { return 0; }
                    var divider = use1000 ? 1000 : 1024;
                    switch (selectedUnitIndex) {
                        case 0: return Convert.ToInt64(value * divider * divider);
                        case 1: return Convert.ToInt64(value * divider * divider * divider);
                        default: return Convert.ToInt64(value);
                    }
                } else {
                    return 0;
                }
            }

        }

    }
}
