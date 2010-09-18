using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;
using Medo.Extensions;
using System.Management;

namespace VhdAttach {

    internal partial class MainForm : Form {

        private string _vhdFileName;
        private Medo.Configuration.RecentFiles _recent;


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
            mnx.ImageScalingSize = new Size((int)(16 * dpiRatioX), (int)(16 * dpiRatioY));
            mnx.Scale(new SizeF(dpiRatioX, dpiRatioY));

            this._recent = new Medo.Configuration.RecentFiles();
        }

        private void MainForm_Deactivate(object sender, EventArgs e) {
            //mnu.Visible = Settings.ShowMenu;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            //mnu.Visible = Settings.ShowMenu;
            Medo.Windows.Forms.State.Load(this, list);
            OpenFromCommandLineArgs();
            UpdateRecent();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            //Debug.WriteLine("MainForm_KeyDown: " + e.KeyData.ToString());

            //if (!Settings.ShowMenu) {
            //    switch (e.KeyData) {

            //case (Keys.Alt | Keys.F):
            //    mnu.Visible = true;
            //    mnuFile.ShowDropDown();
            //    this._suppressMenuKey = true;
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;
            //    break;

            //case (Keys.Alt | Keys.E):
            //    mnu.Visible = true;
            //    mnuEdit.ShowDropDown();
            //    this._suppressMenuKey = true;
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;
            //    break;

            //case (Keys.Alt | Keys.A):
            //    mnu.Visible = true;
            //    mnuAction.ShowDropDown();
            //    this._suppressMenuKey = true;
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;
            //    break;

            //case (Keys.Alt | Keys.T):
            //    mnu.Visible = true;
            //    mnuTools.ShowDropDown();
            //    this._suppressMenuKey = true;
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;
            //    break;

            //case (Keys.Alt | Keys.H):
            //    mnu.Visible = true;
            //    mnuHelp.ShowDropDown();
            //    this._suppressMenuKey = true;
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;
            //    break;

            //case (Keys.Alt | Keys.Menu):
            //    break;

            //default:
            //    if (e.Alt) { this._suppressMenuKey = true; }
            //    break;

            //}
            //}//if(!ShowMenu)


            switch (e.KeyData) {
                case (Keys.Alt | Keys.O): {
                        mnuFileOpen_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case (Keys.Alt | Keys.A): {
                        mnuActionAttach_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case (Keys.Alt | Keys.D): {
                        mnuActionDetach_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;
            }

        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e) {
            //Debug.WriteLine("MainForm_KeyUp: " + e.KeyData.ToString());

            //if (!Settings.ShowMenu) {
            //    if (e.KeyData == Keys.Menu) {
            //        if (this._suppressMenuKey) {
            //            this._suppressMenuKey = false;
            //        } else {
            //            if (!mnu.Visible) {
            //                mnu.Visible = true;
            //                mnu.Select();
            //                mnuFile.Select();
            //            } else {
            //                mnu.Visible = false;
            //            }
            //            e.Handled = true;
            //            e.SuppressKeyPress = true;
            //        }
            //    }
            //}
        }

        private void mnu_Leave(object sender, EventArgs e) {
            //if (!Settings.ShowMenu) {
            //    if (mnu.Visible) { mnu.Visible = false; }
            //}
        }
        private void mnu_MenuDeactivate(object sender, EventArgs e) {
            //mnu_Leave(null, null);
        }


        private void mnu_VisibleChanged(object sender, EventArgs e) {
            //MainForm_Resize(null, null);
        }

        private void MainForm_Resize(object sender, EventArgs e) {
            list.Left = this.ClientRectangle.Left + 3;
            //if (mnu.Visible) {
            //    list.Top = this.ClientRectangle.Top + mnu.Height + mnx.Height + 3;
            //} else {
            list.Top = this.ClientRectangle.Top + mnx.Height + 3;
            //}
            list.Width = this.ClientRectangle.Width - 6;
            list.Height = this.ClientRectangle.Height - list.Top - 3;
        }



        private void UpdateData(string vhdFileName) {
            if (vhdFileName == null) {
                list.Items.Clear();
                mnuActionAttach.Enabled = false;
                mnuActionDetach.Enabled = false;
                mnxAttach.Enabled = mnuActionAttach.Enabled;
                mnxDetach.Enabled = mnuActionDetach.Enabled;
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
                        string deviceText = "Unknown";
                        if (deviceId == 1) { deviceText = "ISO"; }
                        if (deviceId == 2) { deviceText = "VHD"; }
                        string vendorText = "Unknown";
                        if (vendorId.Equals(new Guid("EC984AEC-A0F9-47e9-901F-71415A66345B"))) { vendorText = "Microsoft"; }
                        items.Add(new ListViewItem(new string[] { "Device ID", string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", deviceText, deviceId) }));
                        items.Add(new ListViewItem(new string[] { "Vendor ID", string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", vendorText, vendorId) }));
                    } catch { }

                    try {
                        items.Add(new ListViewItem(new string[] { "Provider subtype", string.Format(CultureInfo.CurrentUICulture, "{0} (0x{0:x8})", document.GetProviderSubtype()) }));
                    } catch { }

                    mnuActionAttach.Enabled = string.IsNullOrEmpty(attachedPath);
                    mnuActionDetach.Enabled = !mnuActionAttach.Enabled;
                    mnxAttach.Enabled = mnuActionAttach.Enabled;
                    mnxDetach.Enabled = mnuActionDetach.Enabled;


                    list.BeginUpdate();
                    list.Items.Clear();
                    foreach (var iItem in items) {
                        list.Items.Add(iItem);
                    }
                    list.EndUpdate();
                }

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
            mnuFileRecent.DropDownItems.Clear();
            mnxFileOpen.DropDownItems.Clear();
            foreach (var iRecentFile in this._recent.AsReadOnly()) {
                var item = new ToolStripMenuItem(iRecentFile.Title);
                item.Tag = iRecentFile;
                item.Click += new EventHandler(recentItem_Click);
                mnuFileRecent.DropDownItems.Add(item);

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
            //TODO: New
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

        private void mnuEdit_DropDownOpening(object sender, EventArgs e) {
            mnuEditCopy.Enabled = (list.SelectedItems.Count > 0);
        }


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

        private void mnuActionAttach_Click(object sender, EventArgs e) {
            if (this._vhdFileName == null) { return; }

            if (Settings.UseService) {

                using (var form = new AttachForm(new FileInfo[] { new FileInfo(this._vhdFileName) })) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this._vhdFileName);

            } else {

                mnu.Enabled = false;
                mnx.Enabled = false;

                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Attach """ + this._vhdFileName + @"""");

                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);

            }
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
                mnx.Enabled = false;

                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Detach """ + this._vhdFileName + @"""");

                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);

            }
        }

        #endregion

        #region Menu: Tools

        private void mnuToolsRefresh_Click(object sender, EventArgs e) {
            UpdateData(this._vhdFileName);
        }

        private void mnuToolsOptions_Click(object sender, EventArgs e) {
            using (var form = new SettingsForm()) {
                if (form.ShowDialog(this) == DialogResult.OK) {
                    //mnu.Visible = Settings.ShowMenu;
                }
            }
        }

        #endregion

        #region Menu: Help

        private void mnuHelpReportABug_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/ErrorReport/"));
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/?page=vhdattach"));
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
                var process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
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
            mnx.Enabled = true;
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

        private void mnxHelpAbout_Click(object sender, EventArgs e) {
            mnuHelpAbout_Click(null, null);
        }

        private void mnxHelpReportABug_Click(object sender, EventArgs e) {
            mnuHelpReportABug_Click(null, null);
        }

    }

}
