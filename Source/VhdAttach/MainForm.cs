using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Medo.Extensions;
using Medo.Localization.Croatia;
using VirtualHardDiskImage;

namespace VhdAttach {

    internal partial class MainForm : Form {

        private string VhdFileName;
        private Medo.Configuration.RecentFiles Recent;
        private static readonly NumberDeclination CylinderSuffix = new NumberDeclination("cylinder", "cylinders", "cylinders");
        private static readonly NumberDeclination HeadSuffix = new NumberDeclination("head", "heads", "heads");
        private static readonly NumberDeclination SectorSuffix = new NumberDeclination("sector", "sectors", "sectors");
        private static readonly ListViewGroup GroupFileSystem = new ListViewGroup("File system");
        private static readonly ListViewGroup GroupDetails = new ListViewGroup("Details");
        private static readonly ListViewGroup GroupInternals = new ListViewGroup("Internals");
        private static readonly ListViewGroup GroupInternalsDynamic = new ListViewGroup("Internals (dynamic disk)");

        public MainForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.TaskbarProgress.DefaultOwner = this;
            Medo.Windows.Forms.TaskbarProgress.DoNotThrowNotImplementedException = true;

            mnu.Renderer = new ToolStripBorderlessProfessionalRenderer();

            this.Recent = new Medo.Configuration.RecentFiles();
        }


        private bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { this.SuppressMenuKey = true; }

            switch (keyData) {

                case Keys.F10:
                    ToggleMenu();
                    return true;

                case Keys.Control | Keys.N:
                case Keys.Alt | Keys.N:
                    mnuNew.PerformClick();
                    return true;

                case Keys.Control | Keys.O:
                    mnuOpen.PerformButtonClick();
                    return true;

                case Keys.Alt | Keys.O:
                    mnuOpen.ShowDropDown();
                    return true;

                case Keys.F6:
                    mnuAttach.PerformButtonClick();
                    return true;

                case Keys.Alt | Keys.A:
                    if (mnuAttach.Enabled) {
                        mnuAttach.ShowDropDown();
                    }
                    return true;

                case Keys.Alt | Keys.D:
                    mnuDetach.PerformClick();
                    return true;

                case Keys.Alt | Keys.M:
                    mnuAutoMount.PerformClick();
                    return true;

                case Keys.F1:
                    mnuApp.ShowDropDown();
                    mnuAppAbout.Select();
                    return true;


                case Keys.F5:
                    mnuRefresh.PerformClick();
                    return true;


                case Keys.Control | Keys.C:
                    mnxListCopy.PerformClick();
                    return true;

                case Keys.Control | Keys.A:
                    mnxListSelectAll.PerformClick();
                    return true;

            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.KeyData == Keys.Menu) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            if (e.KeyData == Keys.Menu) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
            }
        }


        private void Form_Load(object sender, EventArgs e) {
            Medo.Windows.Forms.State.Load(this, list);
            OpenFromCommandLineArgs();
            UpdateRecent();

            CheckErrors();

            Form_Resize(null, null);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            Medo.Windows.Forms.State.Save(this, list);
            this.Recent.Save();
        }

        private void Form_Resize(object sender, EventArgs e) {
            using (var listGraphics = list.CreateGraphics()) {
                var x = listGraphics.MeasureString("XxWwAaZz »ËQqXxWw", list.Font).ToSize();
                list.Columns[0].Width = x.Width;
            }
            list.Columns[1].Width = list.ClientSize.Width - list.Columns[0].Width - SystemInformation.VerticalScrollBarWidth;
        }


        private void UpdateData(string vhdFileName) {
            if (vhdFileName == null) {
                list.Items.Clear();
                list.Groups.Clear();
                mnuAttach.Enabled = false;
                mnuDetach.Enabled = false;
                mnuAutoMount.Text = "Auto-mount";
                mnuAutoMount.Enabled = false;
                return;
            }

            try {
                this.Cursor = Cursors.WaitCursor;

                using (var document = new Medo.IO.VirtualDisk(vhdFileName)) {
                    var items = new List<ListViewItem>();
                    var fileInfo = new FileInfo(document.FileName);
                    items.Add(new ListViewItem(new string[] { "File path", fileInfo.Directory.FullName }) { Group = GroupFileSystem });
                    items.Add(new ListViewItem(new string[] { "File name", fileInfo.Name }) { Group = GroupFileSystem });

                    try {
                        var fi = new FileInfo(document.FileName);
                        items.Add(new ListViewItem(new string[] { "File size", string.Format(CultureInfo.CurrentCulture, "{0} ({1:#,##0} bytes)", BinaryPrefixExtensions.ToBinaryPrefixString(fi.Length, "B", "0"), fi.Length) }) { Group = GroupFileSystem });
                    } catch { }

                    document.Open(Medo.IO.VirtualDiskAccessMask.GetInfo);
                    string attachedDevice = null;
                    string[] attachedPaths = null;
                    try {
                        attachedDevice = document.GetAttachedPath();
                        attachedPaths = PathFromDevice.GetPath(attachedDevice);
                    } catch { }
                    if (attachedDevice != null) {
                        items.Add(new ListViewItem(new string[] { "Attached device", attachedDevice }) { Group = GroupFileSystem });
                    }
                    if (attachedPaths != null) {
                        for (int i = 0; i < attachedPaths.Length; i++) {
                            items.Add(new ListViewItem(new string[] { ((i == 0) ? "Attached path" : ""), attachedPaths[i] }) { Group = GroupFileSystem });
                        }
                    }

                    try {
                        long virtualSize;
                        long physicalSize;
                        int blockSize;
                        int sectorSize;
                        document.GetSize(out virtualSize, out physicalSize, out blockSize, out sectorSize);
                        items.Add(new ListViewItem(new string[] { "Virtual size", string.Format(CultureInfo.CurrentCulture, "{0} ({1:#,##0} bytes)", BinaryPrefixExtensions.ToBinaryPrefixString(virtualSize, "B", "0"), virtualSize) }) { Group = GroupDetails });
                        if (fileInfo.Length != physicalSize) {
                            items.Add(new ListViewItem(new string[] { "Physical size", string.Format(CultureInfo.CurrentCulture, "{0} ({1:#,##0} bytes)", BinaryPrefixExtensions.ToBinaryPrefixString(physicalSize, "B", "0"), physicalSize) }) { Group = GroupDetails });
                        }
                        if (blockSize != 0) {
                            items.Add(new ListViewItem(new string[] { "Block size", string.Format(CultureInfo.CurrentCulture, "{0} ({1:#,##0} bytes)", BinaryPrefixExtensions.ToBinaryPrefixString(((long)blockSize), "B", "0"), blockSize) }) { Group = GroupDetails });
                        }
                        items.Add(new ListViewItem(new string[] { "Sector size", string.Format(CultureInfo.CurrentCulture, "{0} ({1} bytes)", BinaryPrefixExtensions.ToBinaryPrefixString(((long)sectorSize), "B", "0"), sectorSize) }) { Group = GroupDetails });
                    } catch { }

                    try {
                        items.Add(new ListViewItem(new string[] { "Identifier", document.GetIdentifier().ToString() }) { Group = GroupDetails });
                    } catch { }

                    try {
                        int deviceId;
                        Guid vendorId;
                        document.GetVirtualStorageType(out deviceId, out vendorId);
                        string deviceText = string.Format(CultureInfo.InvariantCulture, "Unknown ({0})", deviceId);
                        if (deviceId == 1) { deviceText = "ISO"; }
                        if (deviceId == 2) { deviceText = "VHD"; }
                        string vendorText = string.Format(CultureInfo.InvariantCulture, "Unknown ({0})", vendorId);
                        if (vendorId.Equals(new Guid("EC984AEC-A0F9-47e9-901F-71415A66345B"))) { vendorText = "Microsoft"; }
                        items.Add(new ListViewItem(new string[] { "Device ID", deviceText }) { Group = GroupDetails });
                        items.Add(new ListViewItem(new string[] { "Vendor ID", vendorText }) { Group = GroupDetails });
                    } catch { }

                    try {
                        items.Add(new ListViewItem(new string[] { "Provider subtype", string.Format(CultureInfo.CurrentCulture, "{0} (0x{0:x8})", document.GetProviderSubtype()) }) { Group = GroupDetails });
                    } catch { }


                    if (document.Format == Medo.IO.VirtualDiskFormat.Vhd) {
                        try {
                            var footerCopyBytes = new byte[512];
                            var headerBytes = new byte[1024];
                            var footerBytes = new byte[512];
                            using (var vhdFile = new FileStream(vhdFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                                vhdFile.Read(footerCopyBytes, 0, 512);
                                vhdFile.Read(headerBytes, 0, 1024);
                                vhdFile.Position = vhdFile.Length - 512;
                                vhdFile.Read(footerBytes, 0, 512);
                            }

                            var footer = new HardDiskFooter(footerBytes);

                            if (footer.Cookie != "conectix") {
                                items.Add(new ListViewItem(new string[] { "Cookie", footer.Cookie }) { Group = GroupInternals });
                            }

                            items.Add(new ListViewItem(new string[] { "Creation time stamp", string.Format(CultureInfo.CurrentCulture, "{0}", footer.TimeStamp.ToLocalTime()) }) { Group = GroupInternals });

                            var creatorApplicationText = string.Format(CultureInfo.InvariantCulture, "Unknown (0x{0:x4})", (int)footer.CreatorApplication);
                            switch (footer.CreatorApplication) {
                                case VhdCreatorApplication.JosipMedvedVhdAttach: creatorApplicationText = "Josip Medved's VHD Attach"; break;
                                case VhdCreatorApplication.MicrosoftSysinternalsDisk2Vhd: creatorApplicationText = "Microsoft Sysinternals Disk2vhd"; break;
                                case VhdCreatorApplication.MicrosoftVirtualPC: creatorApplicationText = "Microsoft Virtual PC"; break;
                                case VhdCreatorApplication.MicrosoftVirtualServer: creatorApplicationText = "Microsoft Virtual Server"; break;
                                case VhdCreatorApplication.MicrosoftWindows: creatorApplicationText = "Microsoft Windows"; break;
                                case VhdCreatorApplication.OracleVirtualBox: creatorApplicationText = "Oracle VirtualBox"; break;
                            }
                            items.Add(new ListViewItem(new string[] { "Creator application", string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}", creatorApplicationText, footer.CreatorVersion.Major, footer.CreatorVersion.Minor) }) { Group = GroupInternals });

                            var creatorHostOsText = string.Format(CultureInfo.InvariantCulture, "Unknown (0x{0:x4})", (int)footer.CreatorHostOs);
                            switch (footer.CreatorHostOs) {
                                case VhdCreatorHostOs.Windows: creatorHostOsText = "Windows"; break;
                                case VhdCreatorHostOs.Macintosh: creatorHostOsText = "Macintosh"; break;
                            }
                            items.Add(new ListViewItem(new string[] { "Creator host OS", creatorHostOsText }) { Group = GroupInternals });

                            items.Add(new ListViewItem(new string[] { "Disk geometry", string.Format(CultureInfo.CurrentCulture, "{0}, {1}, {2}", CylinderSuffix.GetText(footer.DiskGeometryCylinders), HeadSuffix.GetText(footer.DiskGeometryHeads), SectorSuffix.GetText(footer.DiskGeometrySectors)) }) { Group = GroupInternals });

                            var diskTypeText = string.Format(CultureInfo.CurrentCulture, "Unknown ({0}: 0x{0:x4})", (int)footer.DiskType);
                            switch ((int)footer.DiskType) {
                                case 0: diskTypeText = "None"; break;
                                case 1: diskTypeText = "Reserved (deprecated: 0x0001)"; break;
                                case 2: diskTypeText = "Fixed hard disk"; break;
                                case 3: diskTypeText = "Dynamic hard disk"; break;
                                case 4: diskTypeText = "Differencing hard disk"; break;
                                case 5: diskTypeText = "Reserved (deprecated: 0x0005)"; break;
                                case 6: diskTypeText = "Reserved (deprecated: 0x0006)"; break;
                            }
                            items.Add(new ListViewItem(new string[] { "Disk type", diskTypeText }) { Group = GroupInternals });

                            if ((footer.DiskType == VhdDiskType.DynamicHardDisk) || (footer.DiskType == VhdDiskType.DifferencingHardDisk)) {

                                var header = new DynamicDiskHeader(headerBytes);

                                if (header.Cookie != "cxsparse") {
                                    items.Add(new ListViewItem(new string[] { "Cookie", header.Cookie }) { Group = GroupInternalsDynamic });
                                }

                                if (header.DataOffset != ulong.MaxValue) {
                                    items.Add(new ListViewItem(new string[] { "Data offset", header.DataOffset.ToString("#,##0") }) { Group = GroupInternalsDynamic });
                                }

                                items.Add(new ListViewItem(new string[] { "Max table entries", header.MaxTableEntries.ToString("#,##0") }) { Group = GroupInternalsDynamic });

                                items.Add(new ListViewItem(new string[] { "Block size", string.Format(CultureInfo.CurrentCulture, "{0} ({1:#,##0} bytes)", BinaryPrefixExtensions.ToBinaryPrefixString(header.BlockSize, "B", "0"), header.BlockSize) }) { Group = GroupInternalsDynamic });

                            }

                        } catch { }
                    }


                    mnuAttach.Enabled = string.IsNullOrEmpty(attachedDevice);
                    mnuDetach.Enabled = !mnuAttach.Enabled;
                    mnuAutoMount.Enabled = true;

                    bool isAutoMount = false;
                    foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                        if (string.Compare(document.FileName, fileName, StringComparison.OrdinalIgnoreCase) == 0) {
                            isAutoMount = true;
                            break;
                        }
                    }
                    mnuAutoMount.Checked = isAutoMount;

                    list.BeginUpdate();
                    list.Items.Clear();
                    list.Groups.Clear();
                    list.Groups.Add(GroupFileSystem);
                    list.Groups.Add(GroupDetails);
                    list.Groups.Add(GroupInternals);
                    list.Groups.Add(GroupInternalsDynamic);
                    foreach (var iItem in items) {
                        list.Items.Add(iItem);
                    }
                    list.EndUpdate();
                }

                mnuAutoMount.Text = mnuAutoMount.Checked ? "Auto-mounted" : "Not auto-mounted";
            } finally {
                this.Cursor = Cursors.Default;
            }


            this.Text = GetFileTitle(vhdFileName) + " - " + Medo.Reflection.EntryAssembly.Title;
        }

        private void UpdateRecent() {
            mnuOpen.DropDownItems.Clear();
            var paths = new List<string>();
            foreach (var iRecentFile in this.Recent.AsReadOnly()) {
                try {
                    var item2 = new ToolStripMenuItem(iRecentFile.Title);
                    item2.Tag = iRecentFile;
                    item2.Click += new EventHandler(recentItem_Click);
                    mnuOpen.DropDownItems.Add(item2);
                    paths.Add(iRecentFile.FileName);
                } catch (ArgumentException) { }
            }
            foreach (var file in ServiceSettings.AutoAttachVhdList) {
                if (paths.Contains(file) == false) {
                    Medo.Configuration.RecentFile iRecentFile;
                    if (file.StartsWith("/")) {
                        /*
                         * Each file can have additional settings area that starts with / and ends with next /.
                         * E.g. "/readonly,nodriveletter/D:\Test.vhd"
                         */
                        var iEndPipe = file.IndexOf("/", 1);
                        iRecentFile = new Medo.Configuration.RecentFile(file.Substring(iEndPipe + 1));
                    } else {
                        iRecentFile = new Medo.Configuration.RecentFile(file);
                    }
                    try {
                        var item2 = new ToolStripMenuItem(iRecentFile.Title);
                        item2.Tag = iRecentFile;
                        item2.Click += new EventHandler(recentItem_Click);
                        mnuOpen.DropDownItems.Add(item2);
                    } catch (ArgumentException) { }
                }
            }
        }

        void recentItem_Click(object sender, EventArgs e) {
            var item = (ToolStripMenuItem)sender;
            var recentItem = (Medo.Configuration.RecentFile)item.Tag;
            try {
                var newDocument = new Medo.IO.VirtualDisk(recentItem.FileName);
                UpdateData(newDocument.FileName);
                this.VhdFileName = newDocument.FileName;
                Recent.Push(recentItem.FileName);
                UpdateRecent();
            } catch (Exception ex) {
                var exFile = new FileInfo(recentItem.FileName);
                if (Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}\n\nDo you wish to remove it from list?", exFile.Name, ex.Message), MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Recent.Remove(recentItem.FileName);
                    UpdateRecent();
                }
            }
        }

        #region Menu

        private void mnuFile_Click(object sender, EventArgs e) {
            using (var frm = new NewDiskForm()) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    AllowSetForegroundWindowToExplorer();
                    var fileName = frm.FileName;
                    try {
                        var newDocument = new Medo.IO.VirtualDisk(fileName);
                        UpdateData(newDocument.FileName);
                        this.VhdFileName = newDocument.FileName;
                        Recent.Push(fileName);
                        UpdateRecent();
                    } catch (Exception ex) {
                        var exFile = new FileInfo(fileName);
                        Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", exFile.Name, ex.Message));
                    }
                }
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e) {
            using (var dialog = new OpenFileDialog()) {
                dialog.AddExtension = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.DefaultExt = "vhd";
                if ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) >= 6000002) {
                    dialog.Filter = "Supported files (*.vhd;*.iso)|*.vhd;*.iso|Virtual disk files (*.vhd)|*.vhd|ISO image files (*.iso)|*.iso|All files (*.*)|*.*";
                } else {
                    dialog.Filter = "Virtual disk files (*.vhd)|*.vhd|All files (*.*)|*.*";
                }
                dialog.Multiselect = false;
                dialog.ShowReadOnly = false;
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    try {
                        var newDocument = new Medo.IO.VirtualDisk(dialog.FileName);
                        UpdateData(newDocument.FileName);
                        this.VhdFileName = newDocument.FileName;
                        Recent.Push(dialog.FileName);
                        UpdateRecent();
                    } catch (Exception ex) {
                        var exFile = new FileInfo(dialog.FileName);
                        Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", exFile.Name, ex.Message));
                    }
                }
            }
        }

        private void mnuRefresh_Click(object sender, EventArgs e) {
            UpdateData(this.VhdFileName);
            CheckErrors();
        }

        private void mnuAttach_ButtonClick(object sender, EventArgs e) {
            if (this.VhdFileName == null) { return; }

            if (Settings.UseService) {
                using (var form = new AttachForm(new FileInfo(this.VhdFileName), false, false)) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this.VhdFileName);
            } else {
                mnu.Enabled = false;
                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Attach """ + this.VhdFileName + @"""");
                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);
            }
            AllowSetForegroundWindowToExplorer();
        }

        private void mnuAttachReadOnly_Click(object sender, EventArgs e) {
            if (this.VhdFileName == null) { return; }

            if (Settings.UseService) {
                using (var form = new AttachForm(new FileInfo(this.VhdFileName), true, false)) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this.VhdFileName);
            } else {
                mnu.Enabled = false;
                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Attach """ + this.VhdFileName + @""" /ReadOnly");
                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);
            }
            AllowSetForegroundWindowToExplorer();
        }

        private void mnuDetach_Click(object sender, EventArgs e) {
            if (this.VhdFileName == null) { return; }

            if (Settings.UseService) {

                using (var form = new DetachForm(new FileInfo[] { new FileInfo(this.VhdFileName) })) {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                }
                UpdateData(this.VhdFileName);

            } else {

                mnu.Enabled = false;

                var exe = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "VhdAttachExecutor.exe");
                var startInfo = Utility.GetProcessStartInfo(exe, @"/Detach """ + this.VhdFileName + @"""");

                this.Cursor = Cursors.WaitCursor;
                bwExecutor.RunWorkerAsync(startInfo);

            }
        }


        private void mnuAutoMount_Click(object sender, EventArgs e) {
            mnuAutoMount.Text = mnuAutoMount.Checked ? "Auto-mounted" : "Not auto-mounted";
            try {
                this.Cursor = Cursors.WaitCursor;
                var vhds = new List<string>();
                if (mnuAutoMount.Checked) { //add if possible
                    bool isIn = false;
                    foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                        vhds.Add(fileName);
                        if (string.Compare(this.VhdFileName, fileName, StringComparison.OrdinalIgnoreCase) == 0) {
                            isIn = true;
                        }
                    }
                    if (isIn == false) {
                        vhds.Add(this.VhdFileName);
                    }
                } else { //remove if exists
                    foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                        if (string.Compare(this.VhdFileName, fileName, StringComparison.OrdinalIgnoreCase) != 0) {
                            vhds.Add(fileName);
                        }
                    }
                }
                var res = PipeClient.WriteSettings(ServiceSettings.ContextMenuAttach, ServiceSettings.ContextMenuAttachReadOnly, ServiceSettings.ContextMenuDetach, ServiceSettings.ContextMenuDetachDrive, vhds.ToArray());
                if (res.IsError) {
                    Medo.MessageBox.ShowError(this, res.Message);
                }
            } finally {
                this.Cursor = Cursors.Default;
            }
        }


        private void mnuOptions_Click(object sender, EventArgs e) {
            using (var form = new SettingsForm()) {
                if (form.ShowDialog(this) == DialogResult.OK) {
                    staErrorStolenExtension.Visible = !ServiceSettings.ContextMenu;
                    UpdateData(this.VhdFileName);
                }
            }
        }


        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            mnuHelpReportABug_Click(null, null);
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            Medo.Services.Upgrade.ShowDialog(this, new Uri("http://jmedved.com/upgrade/"));
        }

        private void mnuAppDonate_Click(object sender, EventArgs e) {
            Process.Start("http://www.jmedved.com/donate/");
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            mnuHelpAbout_Click(null, null);
        }

        private void mnuHelpReportABug_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/feedback/"));
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/vhdattach/"));
        }

        #endregion


        #region ContextMenu: List

        private void mnxList_Opening(object sender, CancelEventArgs e) {
            mnxListCopy.Enabled = (list.SelectedItems.Count > 0);
            mnxListSelectAll.Enabled = (list.Items.Count > 0);
        }


        private void mnxListCopy_Click(object sender, EventArgs e) {
            if (list.SelectedItems.Count > 0) {
                var maxLength = 0;
                foreach (ListViewItem item in list.Items) {
                    if (maxLength < item.Text.Length) { maxLength = item.Text.Length; }
                }

                var sb = new StringBuilder();
                foreach (ListViewItem item in list.SelectedItems) {
                    var key = item.Text;
                    if (key.Length < maxLength) {
                        if (key.Length > 0) {
                            key += " ";
                            if (key.Length < maxLength) {
                                key += new string('.', maxLength - key.Length);
                            }
                        }
                    }
                    var value = item.SubItems[1].Text;
                    if (key.Length > 0) {
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", key, value));
                    } else {
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}  {1}", new string(' ', maxLength), value));
                    }
                }
                Clipboard.SetText(sb.ToString());
            }
        }

        private void mnxListSelectAll_Click(object sender, EventArgs e) {
            foreach (var iItem in list.Items) {
                ((ListViewItem)iItem).Selected = true;
            }
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
                    this.VhdFileName = newDocument.FileName;
                    Recent.Push(iFile.FullName);

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
            UpdateData(this.VhdFileName);
            this.Cursor = Cursors.Default;
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
                        this.VhdFileName = newDocument.FileName;
                        Recent.Push(files[0]);
                        UpdateRecent();
                    } catch (Exception ex) {
                        var exFile = new FileInfo(files[0]);
                        Medo.MessageBox.ShowError(this, string.Format("Cannot open \"{0}\".\n\n{1}", exFile.Name, ex.Message));
                    }
                }
            }
        }

        private void staStolenExtensionText_Click(object sender, EventArgs e) {
            mnuOptions_Click(null, null);
        }

        private void staErrorServiceMissingText_Click(object sender, EventArgs e) {
            try {
                var directory = (new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName;
                Process.Start(Path.Combine(directory, "VhdAttachService.exe"), "/Install").WaitForExit();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format("Cannot install service.\n\n{0}", ex.Message));
            }
            CheckErrors();
        }

        private void staErrorServiceNotRunningText_Click(object sender, EventArgs e) {
            try {
                var directory = (new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName;
                Process.Start(Path.Combine(directory, "VhdAttachService.exe"), "/Start").WaitForExit();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format("Cannot install service.\n\n{0}", ex.Message));
            }
            CheckErrors();
        }


        private void CheckErrors() {
            staErrorStolenExtension.Visible = false;
            staErrorServiceMissing.Visible = false;
            staErrorServiceNotRunning.Visible = false;

            using (var service = new ServiceController("VhdAttach")) {
                try {
                    if (service.Status != ServiceControllerStatus.Running) {
                        staErrorServiceNotRunning.Visible = true;
                    }
                } catch (InvalidOperationException) {
                    staErrorServiceMissing.Visible = true;
                }
            }

            if (ServiceSettings.ContextMenu == false) {
                staErrorStolenExtension.Visible = true;
            }
        }

        private void ToggleMenu() {
            if (mnu.ContainsFocus) {
                list.Select();
            } else {
                mnu.Select();
                mnu.Items[0].Select();
            }
        }

    }

}
