using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VhdAttachCommon;

namespace VhdAttach {
    internal partial class ChangeDriveLetterForm : Form {

        public ChangeDriveLetterForm(Volume volume) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Volume = volume;

            string currDrive = volume.DriveLetter2;
            if (currDrive == null) {
                btnOK.Text = "Add";
            } else {
                cmbDriveLetter.Items.Add("");
            }

            var drives = new List<string>();
            for (char letter = 'A'; letter <= 'Z'; letter++) {
                drives.Add(letter.ToString() + ":");
            }

            foreach (var drive in DriveInfo.GetDrives()) {
                var driveName = drive.Name.Substring(0, 2);
                if (driveName.Equals(currDrive, StringComparison.OrdinalIgnoreCase) == false) {
                    drives.Remove(driveName);
                }
            }

            foreach (var drive in drives) {
                cmbDriveLetter.Items.Add(drive);
            }
            if (currDrive != null) { cmbDriveLetter.SelectedItem = currDrive; }
        }

        private readonly Volume Volume;


        private void cmbDriveLetter_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Volume.DriveLetter2 != null) {
                btnOK.Text = string.IsNullOrEmpty(cmbDriveLetter.Text) ? "Remove" : "Change";
            }
            btnOK.Enabled = !(cmbDriveLetter.Text.Equals(this.Volume.DriveLetter2, StringComparison.InvariantCultureIgnoreCase));
        }


        private void btnOK_Click(object sender, EventArgs e) {
            var driveLetter = string.IsNullOrEmpty(cmbDriveLetter.Text) ? "" : cmbDriveLetter.Text + "\\";
            using (var frm = new ServiceWaitForm("Changing drive letter",
                delegate() {
                    var res = PipeClient.ChangeDriveLetter(this.Volume.VolumeName, driveLetter);
                    if (res.IsError) {
                        throw new InvalidOperationException(res.Message);
                    }
                })) {

                frm.ShowDialog(this);
            }
        }

    }
}
