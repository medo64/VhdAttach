using System;
using System.Drawing;
using System.Globalization;
using System.IO;
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

        private void Form_Load(object sender, EventArgs e) {
            cmbSizeUnit.SelectedItem = Settings.LastSizeUnit;
            txtSize.Text = GetSizeText(Settings.LastSize, cmbSizeUnit.SelectedIndex);
            chbThousandSize.Checked = Settings.LastSizeThousandBased;
            radFixed.Checked = Settings.LastSizeFixed;
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
            decimal dummyNumber;
            if (TryParseNumber(txtSize.Text, out dummyNumber) == false) {
                txtSize.Text = "";
            }

            var sizeInBytes = GetSizeInBytes();
            txtSizeInBytes.Text = string.Format(CultureInfo.CurrentCulture, "{0:#,##0}", sizeInBytes);

            var sizeInBytesOk = (sizeInBytes >= 8 * 1000 * 1000 / 4096 * 4096);
            if (sizeInBytesOk == false) {
                erpError.SetError(btnOK, Messages.DiskTooSmall);
            } else {
                erpError.SetError(btnOK, null);
            }

            btnOK.Enabled = sizeInBytesOk;
        }

        private void txtSize_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsControl(e.KeyChar)) { return; }
            if (char.IsDigit(e.KeyChar)) { return; }

            if (txtSize.Text.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) || txtSize.Text.Contains(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)) {
                e.Handled = true;
                return;
            }

            if (Array.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray(), e.KeyChar) >= 0) { return; }
            if ((CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator.Length == 1) && (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length == 1) && (e.KeyChar.ToString().Equals(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator))) {
                e.KeyChar = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                return;
            }
            e.Handled = true;
        }


        private void txtSize_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Up: {
                        e.Handled = true;
                        decimal number;
                        if (TryParseNumber(txtSize.Text, out number)) {
                            number = Math.Floor(number);
                            if (number < 99999999) {
                                number += 1;
                                var allSelected = (txtSize.SelectionLength == txtSize.TextLength);
                                txtSize.Text = number.ToString(CultureInfo.CurrentCulture);
                                if (allSelected) { txtSize.SelectAll(); } else { txtSize.SelectionStart = txtSize.TextLength; }
                            }
                        }
                    } break;

                case Keys.Down: {
                        e.Handled = true;
                        decimal number;
                        if (TryParseNumber(txtSize.Text, out number)) {
                            number = Math.Floor(number);
                            if (number > 0) {
                                number -= 1;
                                var allSelected = (txtSize.SelectionLength == txtSize.TextLength);
                                txtSize.Text = number.ToString(CultureInfo.CurrentCulture);
                                if (allSelected) { txtSize.SelectAll(); } else { txtSize.SelectionStart = txtSize.TextLength; }
                            }
                        }
                    } break;

                case Keys.Control | Keys.A:
                    e.Handled = true;
                    txtSize.SelectAll();
                    break;

            }
        }

        private void cmbSizeUnit_SelectedIndexChanged(object sender, EventArgs e) {
            control_Changed(null, null);
        }


        private long GetSizeInBytes() {
            long sizeInBytes = GetSizeInBytes(txtSize.Text, cmbSizeUnit.SelectedIndex, chbThousandSize.Checked);
            return (sizeInBytes / 4096) * 4096; //round to 4096 blocks
        }


        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Settings.LastSize = GetSizeInBytes(txtSize.Text, cmbSizeUnit.SelectedIndex);
            Settings.LastSizeUnit = cmbSizeUnit.Text;
            Settings.LastSizeThousandBased = chbThousandSize.Checked;
            Settings.LastSizeFixed = radFixed.Checked;
        }



        private static string GetSizeText(long size, int selectedUnitIndex) {
            switch (selectedUnitIndex) {
                case 0: return (size / 1024.0 / 1024.0).ToString("0.#", CultureInfo.CurrentCulture);
                case 1: return (size / 1024.0 / 1024.0 / 1024.0).ToString("0.##", CultureInfo.CurrentCulture);
                default: return size.ToString(CultureInfo.CurrentUICulture);
            }
        }

        private static long GetSizeInBytes(string text, int selectedUnitIndex, bool use1000 = false) {
            decimal value;
            if (TryParseNumber(text, out value)) {
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

        private static bool TryParseNumber(string text, out decimal value) {
            decimal number;
            if (decimal.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out number) || decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number)) {
                value = number;
                return true;
            } else {
                value = 0;
                return false;
            }
        }

    }
}
