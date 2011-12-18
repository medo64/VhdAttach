using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Medo.Extensions;
using System.Runtime.InteropServices;
using Medo.Localization.Croatia;

namespace VhdAttach {

    internal partial class MainForm : Form {

        private string _vhdFileName;
        private Medo.Configuration.RecentFiles _recent;
        private static readonly NumberDeclination CylinderSuffix = new NumberDeclination("cylinder", "cylinders", "cylinders");
        private static readonly NumberDeclination HeadSuffix = new NumberDeclination("head", "heads", "heads");
        private static readonly NumberDeclination SectorSuffix = new NumberDeclination("sector", "sectors", "sectors");

        public MainForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.TaskbarProgress.DefaultOwner = this;
            Medo.Windows.Forms.TaskbarProgress.DoNotThrowNotImplementedException = true;

            float dpiRatioX, dpiRatioY;
            using (var g = this.CreateGraphics()) {
                dpiRatioX = (float)Math.Round(g.DpiX / 96, 2);
                dpiRatioY = (float)Math.Round(g.DpiY / 96, 2);
            }
            mnu.ImageScalingSize = new Size((int)(16 * dpiRatioX), (int)(16 * dpiRatioY));
            mnu.Scale(new SizeF(dpiRatioX, dpiRatioY));

            this._recent = new Medo.Configuration.RecentFiles();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            Medo.Windows.Forms.State.Load(this, list);
            OpenFromCommandLineArgs();
            UpdateRecent();
            staStolenExtension.Visible = !ServiceSettings.ContextMenu;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {

                case (Keys.Alt | Keys.Menu):
                    mnu.Select();
                    mnu.Items[0].Select();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.O:
                    mnuFileOpen_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F5:
                    mnuToolsRefresh_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F6:
                    mnuAttach_ButtonClick(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.A:
                    mnuAttach_ButtonClick(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.C:
                    mnuEditCopy_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.D:
                    mnuActionDetach_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.O:
                    mnuFileOpen_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.N:
                    mnuFileNew_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

            }
        }

        private void MainForm_Resize(object sender, EventArgs e) {
            list.Left = this.ClientRectangle.Left + 3;
            list.Top = this.ClientRectangle.Top + mnu.Height + 3;
            list.Width = this.ClientRectangle.Width - 6;
            list.Height = this.ClientRectangle.Height - list.Top - 3;
        }



        private void UpdateData(string vhdFileName) {
            if (vhdFileName == null) {
                list.Items.Clear();
                mnxAttach.Enabled = false;
                mnxDetach.Enabled = false;
                mnxAutoMount.Text = "Auto-mount";
                mnxAutoMount.Enabled = false;
                return;
            }

            try {
                this.Cursor = Cursors.WaitCursor;

                using (var document = new Medo.IO.VirtualDisk(vhdFileName)) {
                    var items = new List<ListViewItem>();
                    var fileInfo = new FileInfo(document.FileName);
                    items.Add(new ListViewItem(new string[] { "File path", fileInfo.Directory.FullName }));
                    items.Add(new ListViewItem(new string[] { "File name", fileInfo.Name }));

                    try {
                        var fi = new FileInfo(document.FileName);
                        items.Add(new ListViewItem(new string[] { "File size", string.Format("{0} ({1} bytes)", fi.Length.ToBinaryPrefixString("B", "0"), fi.Length) }));
                    } catch { }

                    document.Open(Medo.IO.VirtualDiskAccessMask.GetInfo);
                    string attachedPath = null;
                    string attachedPathLetters = null;
                    try {
                        attachedPath = document.GetAttachedPath();

                        try {
                            int physicalDrive;
                            if (attachedPath.StartsWith(@"\\.\PHYSICALDRIVE", StringComparison.InvariantCulture)) {
                                if (int.TryParse(attachedPath.Substring(17), NumberStyles.Integer, CultureInfo.InvariantCulture, out physicalDrive)) {
                                    attachedPathLetters = GetPathLetters(physicalDrive);
                                    if (string.IsNullOrEmpty(attachedPathLetters)) {
                                        Thread.Sleep(1000);
                                        attachedPathLetters = GetPathLetters(physicalDrive);
                                    }
                                    if (string.IsNullOrEmpty(attachedPathLetters)) {
                                        Thread.Sleep(1000);
                                        attachedPathLetters = GetPathLetters(physicalDrive);
                                    }
                                    if (string.IsNullOrEmpty(attachedPathLetters)) {
                                        Thread.Sleep(1000);
                                        attachedPathLetters = GetPathLetters(physicalDrive);
                                    }
                                }
                            }
                        } catch { }
                    } catch { }
                    if (string.IsNullOrEmpty(attachedPath)) {
                        items.Add(new ListViewItem(new string[] { "Attached path", string.Format(CultureInfo.CurrentUICulture, "{0}", attachedPath) }));
                    } else {
                        items.Add(new ListViewItem(new string[] { "Attached path", string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", attachedPath, attachedPathLetters) }));
                    }

                    try {
                        long virtualSize;
                        long physicalSize;
                        int blockSize;
                        int sectorSize;
                        document.GetSize(out virtualSize, out physicalSize, out blockSize, out sectorSize);
                        items.Add(new ListViewItem(new string[] { "Virtual size", string.Format(CultureInfo.CurrentUICulture, "{0} ({1} bytes)", virtualSize.ToBinaryPrefixString("B", "0"), virtualSize) }));
                        items.Add(new ListViewItem(new string[] { "Physical size", string.Format(CultureInfo.CurrentUICulture, "{0} ({1} bytes)", physicalSize.ToBinaryPrefixString("B", "0"), physicalSize) }));
                        if (blockSize == 0) {
                            items.Add(new ListViewItem(new string[] { "Block size", "Default" }));
                        } else {
                            items.Add(new ListViewItem(new string[] { "Block size", string.Format(CultureInfo.CurrentUICulture, "{0} ({1} bytes)", ((long)blockSize).ToBinaryPrefixString("B", "0"), blockSize) }));
                        }
                        items.Add(new ListViewItem(new string[] { "Sector size", string.Format(CultureInfo.CurrentUICulture, "{0} ({1} bytes)", ((long)sectorSize).ToBinaryPrefixString("B", "0"), sectorSize) }));
                    } catch { }

                    try {
                        items.Add(new ListViewItem(new string[] { "Identifier", document.GetIdentifier().ToString() }));
                    } catch { }

                    try {
                        int deviceId;
                        Guid vendorId;
                        document.GetVirtualStorageType(out deviceId, out vendorId);
                        string deviceText = string.Format(CultureInfo.CurrentUICulture, "Unknown ({0})", deviceId);
                        if (deviceId == 1) { deviceText = "ISO"; }
                        if (deviceId == 2) { deviceText = "VHD"; }
                        string vendorText = string.Format(CultureInfo.CurrentUICulture, "Unknown ({0})", vendorId);
                        if (vendorId.Equals(new Guid("EC984AEC-A0F9-47e9-901F-71415A66345B"))) { vendorText = "Microsoft"; }
                        items.Add(new ListViewItem(new string[] { "Device ID", deviceText }));
                        items.Add(new ListViewItem(new string[] { "Vendor ID", vendorText }));
                    } catch { }

                    try {
                        items.Add(new ListViewItem(new string[] { "Provider subtype", string.Format(CultureInfo.CurrentUICulture, "{0} (0x{0:x8})", document.GetProviderSubtype()) }));
                    } catch { }


                    try {
                        var footer = new byte[512];
                        using (var vhdFile = new FileStream(vhdFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                            vhdFile.Position = vhdFile.Length - 512;
                            vhdFile.Read(footer, 0, 512);
                        }
                        if ((footer[0] == 0x63) && (footer[1] == 0x6f) && (footer[2] == 0x6e) && (footer[3] == 0x65) && (footer[4] == 0x63) && (footer[5] == 0x74) && (footer[6] == 0x69) && (footer[7] == 0x78)) {
                            var timeStamp = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToUInt32(new byte[] { footer[27], footer[26], footer[25], footer[24] }, 0));
                            items.Add(new ListViewItem(new string[] { "Creation time stamp", string.Format(CultureInfo.CurrentUICulture, "{0}", timeStamp.ToLocalTime()) }));

                            var creatorApplication = System.Text.ASCIIEncoding.ASCII.GetString(footer, 28, 4);
                            var creatorApplicationText = string.Format(CultureInfo.CurrentUICulture, "Unknown ({0})", creatorApplication.TrimEnd());
                            switch (creatorApplication) {
                                case "vbox": creatorApplicationText = "Oracle VirtualBox"; break;
                                case "vpc ": creatorApplicationText = "Microsoft Virtual PC"; break;
                                case "vs  ": creatorApplicationText = "Microsoft Virtual Server"; break;
                                case "win ": creatorApplicationText = "Microsoft Windows"; break;
                            }
                            var creatorVersionMajor = BitConverter.ToUInt16(new byte[] { footer[33], footer[32] }, 0);
                            var creatorVersionMinor = BitConverter.ToUInt16(new byte[] { footer[35], footer[34] }, 0);
                            items.Add(new ListViewItem(new string[] { "Creator application", string.Format(CultureInfo.CurrentUICulture, "{0} {1}.{2}", creatorApplicationText, creatorVersionMajor, creatorVersionMinor) }));

                            var creatorHostOs = BitConverter.ToUInt32(new byte[] { footer[39], footer[38], footer[37], footer[36] }, 0);
                            var creatorHostOsText = string.Format(CultureInfo.CurrentUICulture, "Unknown (0x{0:x4})", creatorHostOs);
                            switch (creatorHostOs) {
                                case 0x5769326B: creatorHostOsText = "Windows"; break;
                                case 0x4D616320: creatorHostOsText = "Macintosh"; break;
                            }
                            items.Add(new ListViewItem(new string[] { "Creator host OS", creatorHostOsText }));

                            var diskGeometryCylinder = BitConverter.ToUInt16(new byte[] { footer[57], footer[56] }, 0);
                            var diskGeometryHeads = footer[58];
                            var diskGeometrySectors = footer[59];
                            items.Add(new ListViewItem(new string[] { "Disk geometry", string.Format(CultureInfo.CurrentUICulture, "{0}, {1}, {2}", CylinderSuffix.GetText(diskGeometryCylinder), HeadSuffix.GetText(diskGeometryHeads), SectorSuffix.GetText(diskGeometrySectors)) }));

                            var diskType = BitConverter.ToUInt32(new byte[] { footer[63], footer[62], footer[61], footer[60] }, 0);
                            var diskTypeText = string.Format(CultureInfo.CurrentUICulture, "Unknown ({0}: 0x{0:x4})", diskType);
                            switch (diskType) {
                                case 0: diskTypeText = "None"; break;
                                case 1: diskTypeText = "Reserved (deprecated: 1)"; break;
                                case 2: diskTypeText = "Fixed hard disk"; break;
                                case 3: diskTypeText = "Dynamic hard disk"; break;
                                case 4: diskTypeText = "Differencing hard disk"; break;
                                case 5: diskTypeText = "Reserved (deprecated: 5)"; break;
                                case 6: diskTypeText = "Reserved (deprecated: 6)"; break;
                            }
                            items.Add(new ListViewItem(new string[] { "Disk type", diskTypeText }));
                        }

                    } catch { }


                    mnxAttach.Enabled = string.IsNullOrEmpty(attachedPath);
                    mnxDetach.Enabled = !mnxAttach.Enabled;
                    mnxAutoMount.Enabled = true;

                    bool isAutoMount = false;
                    foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                        if (string.Compare(document.FileName, fileName, StringComparison.OrdinalIgnoreCase) == 0) {
                            isAutoMount = true;
                            break;
                        }
                    }
                    mnxAutoMount.Checked = isAutoMount;

                    list.BeginUpdate();
                    list.Items.Clear();
                    foreach (var iItem in items) {
                        list.Items.Add(iItem);
                    }
                    list.EndUpdate();
                }

                mnxAutoMount.Text = mnxAutoMount.Checked ? "Auto-mounted" : "Not auto-mounted";
            } finally {
                this.Cursor = Cursors.Default;
            }


            this.Text = GetFileTitle(vhdFileName) + " - " + Medo.Reflection.EntryAssembly.Title;
        }

        private string GetPathLetters(int physicalDrive) {
            var attachedPathLetters = new StringBuilder();
            var wmiQuery = new ObjectQuery("SELECT Antecedent, Dependent FROM Win32_LogicalDiskToPartition");
            using (var wmiSearcher = new ManagementObjectSearcher(wmiQuery)) {
                foreach (var iReturn in wmiSearcher.Get()) {
                    var disk = GetSubsubstring((string)iReturn["Antecedent"], "Win32_DiskPartition.DeviceID", "Disk #", ",");
                    var partition = GetSubsubstring((string)iReturn["Dependent"], "Win32_LogicalDisk.DeviceID", "", "");
                    int diskNumber;
                    if (int.TryParse(disk, NumberStyles.Integer, CultureInfo.InvariantCulture, out diskNumber)) {
                        if (physicalDrive == diskNumber) {
                            if (attachedPathLetters.Length > 0) { attachedPathLetters.Append(", "); }
                            attachedPathLetters.Append(partition);
                        }
                    }
                }
            }
            return attachedPathLetters.ToString();
        }

        private void UpdateRecent() {
            mnxFileOpen.DropDownItems.Clear();
            foreach (var iRecentFile in this._recent.AsReadOnly()) {
                var item2 = new ToolStripMenuItem(iRecentFile.Title);
                item2.Tag = iRecentFile;
                item2.Click += new EventHandler(recentItem_Click);
                mnxFileOpen.DropDownItems.Add(item2);
            }
        }

        void recentItem_Click(object sender, EventArgs e) {
            var item = (ToolStripMenuItem)sender;
            var recentItem = (Medo.Configuration.RecentFile)item.Tag;
            try {
                var newDocument = new Medo.IO.VirtualDisk(recentItem.FileName);
                UpdateData(newDocument.FileName);
                this._vhdFileName = newDocument.FileName;
                _recent.Push(recentItem.FileName);
                UpdateRecent();
            } catch (Exception ex) {
                var exFile = new FileInfo(recentItem.FileName);
                if (Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}\n\nDo you wish to remove it from list?", exFile.Name, ex.Message), MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    _recent.Remove(recentItem.FileName);
                    UpdateRecent();
                }
            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            Medo.Windows.Forms.State.Save(this, list);
            this._recent.Save();
        }


        #region Menu: File

        private void mnuFileNew_Click(object sender, EventArgs e) {
            using (var frm = new SaveFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Virtual disk files (*.vhd)|*.vhd|All files (*.*)|*.*", FilterIndex = 0, OverwritePrompt = true, Title = "New disk", ValidateNames = true }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    using (var frm2 = new NewDiskForm(frm.FileName)) {
                        if (frm2.ShowDialog(this) == DialogResult.OK) {
                            AllowSetForegroundWindowToExplorer();
                            var fileName = frm.FileName;
                            try {
                                var newDocument = new Medo.IO.VirtualDisk(fileName);
                                UpdateData(newDocument.FileName);
                                this._vhdFileName = newDocument.FileName;
                                _recent.Push(fileName);
                                UpdateRecent();
                            } catch (Exception ex) {
                                var exFile = new FileInfo(fileName);
                                Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", exFile.Name, ex.Message));
                            }
                        }
                    }
                }
            }
        }

        private void mnuFileOpen_Click(object sender, EventArgs e) {
            using (var dialog = new OpenFileDialog()) {
                dialog.AddExtension = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.DefaultExt = "vhd";
                dialog.Filter = "Virtual disk files (*.vhd)|*.vhd|All files (*.*)|*.*";
                dialog.Multiselect = false;
                dialog.ShowReadOnly = false;
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    try {
                        var newDocument = new Medo.IO.VirtualDisk(dialog.FileName);
                        UpdateData(newDocument.FileName);
                        this._vhdFileName = newDocument.FileName;
                        _recent.Push(dialog.FileName);
                        UpdateRecent();
                    } catch (Exception ex) {
                        var exFile = new FileInfo(dialog.FileName);
                        Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", exFile.Name, ex.Message));
                    }
                }
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        #endregion


        #region Menu: Edit

        private void mnuEditCopy_Click(object sender, EventArgs e) {
            if (list.SelectedItems.Count > 0) {
                var sb = new StringBuilder();
                foreach (var iItem in list.SelectedItems) {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} = \"{1}\"", ((ListViewItem)iItem).Text, ((ListViewItem)iItem).SubItems[1].Text));
                }
                Clipboard.SetText(sb.ToString());
            }
        }

        private void mnuEditSelectAll_Click(object sender, EventArgs e) {
            foreach (var iItem in list.Items) {
                ((ListViewItem)iItem).Selected = true;
            }
        }

        #endregion


        #region Menu: Action

        private void mnuAttach_ButtonClick(object sender, EventArgs e) {
            if (this._vhdFileName == null) { return; }

            if (Settings.UseService) {
                using (var form = new AttachForm(new FileInfo(this._vhdFileName), false, false)) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this._vhdFileName);
            } else {
                mnu.Enabled = false;
                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Attach """ + this._vhdFileName + @"""");
                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);
            }
            AllowSetForegroundWindowToExplorer();
        }

        private void mnuAttachReadOnly_Click(object sender, EventArgs e) {
            if (this._vhdFileName == null) { return; }

            if (Settings.UseService) {
                using (var form = new AttachForm(new FileInfo(this._vhdFileName), true, false)) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this._vhdFileName);
            } else {
                mnu.Enabled = false;
                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Attach """ + this._vhdFileName + @""" /ReadOnly");
                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);
            }
            AllowSetForegroundWindowToExplorer();
        }

        private void mnuActionDetach_Click(object sender, EventArgs e) {
            if (this._vhdFileName == null) { return; }

            if (Settings.UseService) {

                using (var form = new DetachForm(new FileInfo[] { new FileInfo(this._vhdFileName) })) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this._vhdFileName);

            } else {

                mnu.Enabled = false;

                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Detach """ + this._vhdFileName + @"""");

                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);

            }
        }

        #endregion

        #region Menu: Tools

        private void mnuToolsRefresh_Click(object sender, EventArgs e) {
            staStolenExtension.Visible = !ServiceSettings.ContextMenu;
            UpdateData(this._vhdFileName);
        }

        private void mnuToolsOptions_Click(object sender, EventArgs e) {
            using (var form = new SettingsForm()) {
                if (form.ShowDialog(this) == DialogResult.OK) {
                    mnuToolsRefresh_Click(null, null);
                }
            }
        }

        #endregion

        #region Menu: Help

        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            mnuHelpReportABug_Click(null, null);
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            using (var frm = new UpgradeForm()) {
                frm.ShowDialog(this);
            }
        }

        private void mnuAppDonate_Click(object sender, EventArgs e) {
            Process.Start("http://www.jmedved.com/donate/");
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            mnuHelpAbout_Click(null, null);
        }

        private void mnuHelpReportABug_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/ErrorReport/"));
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/vhdattach/"));
        }

        #endregion


        #region ContextMenu: List

        private void mnxList_Opening(object sender, CancelEventArgs e) {
            mnxListCopy.Enabled = (list.SelectedItems.Count > 0);
            mnxListEditSelectAll.Enabled = (list.Items.Count > 0);
        }


        private void mnxListCopy_Click(object sender, EventArgs e) {
            mnuEditCopy_Click(sender, e);
        }

        private void mnxListEditSelectAll_Click(object sender, EventArgs e) {
            mnuEditSelectAll_Click(sender, e);
        }

        #endregion


        private void OpenFromCommandLineArgs() { //goes through all files until it can open one and redirects all other files to new instances with /OpenOrExit argument. That argument ensures that each new instance will close in case of error.
            var filesToOpen = Medo.Application.Args.Current.GetValues(null);
            FileInfo iFile;
            for (int i = 0; i < filesToOpen.Length; ++i) {
                iFile = new FileInfo(filesToOpen[i]);
                try {
                    var newDocument = new Medo.IO.VirtualDisk(iFile.FullName);
                    UpdateData(newDocument.FileName);
                    this._vhdFileName = newDocument.FileName;
                    _recent.Push(iFile.FullName);

                    //send all other files to second instances
                    for (int j = i + 1; j < filesToOpen.Length; ++j) {
                        var jFile = new FileInfo(filesToOpen[j]);
                        Process.Start(Utility.GetProcessStartInfo(Assembly.GetExecutingAssembly().Location, @"/ OpenOrExit """ + jFile.FullName + @""""));
                        Thread.Sleep(100 / Environment.ProcessorCount);
                    }
                    break; //i

                } catch (Exception ex) {
                    Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", iFile.Name, ex.Message));
                    if (Medo.Application.Args.Current.ContainsKey("OpenOrExit")) {
                        this.Close();
                        System.Environment.Exit(2);
                    }
                }
            }
        }

        private string GetFileTitle(string vhdFileName) {
            string fileNamePart;
            if (vhdFileName == null) {
                fileNamePart = "Untitled";
            } else {
                var fi = new FileInfo(vhdFileName);
                fileNamePart = fi.Name;
            }
            return fileNamePart;
        }

        private void bwExecutor_DoWork(object sender, DoWorkEventArgs e) {
            try {
                var startInfo = (ProcessStartInfo)e.Argument;
                startInfo.Arguments += string.Format(CultureInfo.InvariantCulture, " /ParentWindow=\"{0},{1},{2},{3}\"", this.Left, this.Top, this.Width, this.Height);
                using (var process = new Process()) {
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                Thread.Sleep(250);
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        private void bwExecutor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                Medo.MessageBox.ShowError(this, string.Format("Error during execution of external process.\n\n{0}", e.Error.Message));
            }
            mnu.Enabled = true;
            UpdateData(this._vhdFileName);
            this.Cursor = Cursors.Default;
        }


        private string GetSubsubstring(string value, string type, string start, string end) {
            var xStart0 = value.IndexOf(":" + type + "=\"");
            if (xStart0 < 0) { return null; }
            var xStart1 = value.IndexOf("\"", xStart0 + 1);
            if (xStart1 < 0) { return null; }
            var xEnd1 = value.IndexOf("\"", xStart1 + 1);
            if (xEnd1 < 0) { return null; }
            var extract = value.Substring(xStart1 + 1, xEnd1 - xStart1 - 1);

            int xStart2 = 0;
            if (!string.IsNullOrEmpty(start)) { xStart2 = extract.IndexOf(start); }
            if (xStart2 < 0) { return null; }

            int xEnd2 = extract.Length;
            if (!string.IsNullOrEmpty(end)) { xEnd2 = extract.IndexOf(end); }
            if (xEnd2 < 0) { return null; }

            return extract.Substring(xStart2 + start.Length, xEnd2 - xStart2 - start.Length);
        }

        private void mnxAutoMount_Click(object sender, EventArgs e) {
            mnxAutoMount.Text = mnxAutoMount.Checked ? "Auto-mounted" : "Not auto-mounted";
            try {
                this.Cursor = Cursors.WaitCursor;
                var vhds = new List<string>();
                if (mnxAutoMount.Checked) { //add if possible
                    bool isIn = false;
                    foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                        vhds.Add(fileName);
                        if (string.Compare(this._vhdFileName, fileName, StringComparison.OrdinalIgnoreCase) == 0) {
                            isIn = true;
                        }
                    }
                    if (isIn == false) {
                        vhds.Add(this._vhdFileName);
                    }
                } else { //remove if exists
                    foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                        if (string.Compare(this._vhdFileName, fileName, StringComparison.OrdinalIgnoreCase) != 0) {
                            vhds.Add(fileName);
                        }
                    }
                }
                var data = new SettingsRequestData(ServiceSettings.ContextMenuAttach, ServiceSettings.ContextMenuAttachReadOnly, ServiceSettings.ContextMenuDetach, ServiceSettings.ContextMenuDetachDrive, vhds.ToArray());
                var resBytes = WcfPipeClient.Execute("WriteSettings", data.ToJson());
                var res = ResponseData.FromJson(resBytes);
                if (res.ExitCode != ExitCodes.OK) {
                    Medo.MessageBox.ShowError(this, res.Message);
                }
            } finally {
                this.Cursor = Cursors.Default;
            }
        }


        private static void AllowSetForegroundWindowToExplorer() {
            var pathToExplorer = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "explorer.exe");
            foreach (var process in Process.GetProcesses()) {
                try {
                    var mainModule = process.MainModule;
                    if (mainModule != null) {
                        var fileName = mainModule.FileName;
                        if ((fileName != null) && (fileName.Equals(pathToExplorer, StringComparison.OrdinalIgnoreCase))) {
                            NativeMethods.AllowSetForegroundWindow(process.Id);
                        }
                    }
                } catch (Exception) { }
            }
        }

        private static class NativeMethods {

            [DllImportAttribute("user32.dll", EntryPoint = "AllowSetForegroundWindow")]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool AllowSetForegroundWindow(int dwProcessId);

        }

        private void list_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effect = (files.Length == 1) ? DragDropEffects.Move : DragDropEffects.None;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void list_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1) {
                    try {
                        var newDocument = new Medo.IO.VirtualDisk(files[0]);
                        UpdateData(newDocument.FileName);
                        this._vhdFileName = newDocument.FileName;
                        _recent.Push(files[0]);
                        UpdateRecent();
                    } catch (Exception ex) {
                        var exFile = new FileInfo(files[0]);
                        Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", exFile.Name, ex.Message));
                    }
                }
            }
        }

        private void staStolenExtensionText_Click(object sender, EventArgs e) {
            mnuToolsOptions_Click(null, null);
        }

    }

}
