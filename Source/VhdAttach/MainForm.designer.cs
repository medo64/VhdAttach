namespace VhdAttach {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.list = new System.Windows.Forms.ListView();
            this.list_Property = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.list_Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnxList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnxListCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxListSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.bwExecutor = new System.ComponentModel.BackgroundWorker();
            this.mnuNew = new System.Windows.Forms.ToolStripButton();
            this.mnuAttach = new System.Windows.Forms.ToolStripSplitButton();
            this.mnuAttachReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDetach = new System.Windows.Forms.ToolStripButton();
            this.mnuApp = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuAppOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuApp0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAppFeedback = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAppUpgrade = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuApp1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAppAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRefresh = new System.Windows.Forms.ToolStripButton();
            this.mnu = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAutomount = new System.Windows.Forms.ToolStripSplitButton();
            this.mnuAutomountNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAutomountReadonly = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAutomountDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDrive = new System.Windows.Forms.ToolStripSplitButton();
            this.staErrorStolenExtension = new System.Windows.Forms.StatusStrip();
            this.staErrorStolenExtensionText = new System.Windows.Forms.ToolStripStatusLabel();
            this.staErrorServiceMissing = new System.Windows.Forms.StatusStrip();
            this.staErrorServiceMissingText = new System.Windows.Forms.ToolStripStatusLabel();
            this.staErrorServiceNotRunning = new System.Windows.Forms.StatusStrip();
            this.staErrorServiceNotRunningText = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrUpdateMenu = new System.Windows.Forms.Timer(this.components);
            this.bwCheckForUpgrade = new System.ComponentModel.BackgroundWorker();
            this.mnxList.SuspendLayout();
            this.mnu.SuspendLayout();
            this.staErrorStolenExtension.SuspendLayout();
            this.staErrorServiceMissing.SuspendLayout();
            this.staErrorServiceNotRunning.SuspendLayout();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.AllowDrop = true;
            this.list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.list_Property,
            this.list_Value});
            this.list.ContextMenuStrip = this.mnxList;
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.FullRowSelect = true;
            this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.list.HideSelection = false;
            this.list.Location = new System.Drawing.Point(0, 32);
            this.list.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(677, 462);
            this.list.TabIndex = 2;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.DragDrop += new System.Windows.Forms.DragEventHandler(this.list_DragDrop);
            this.list.DragEnter += new System.Windows.Forms.DragEventHandler(this.list_DragEnter);
            // 
            // list_Property
            // 
            this.list_Property.Text = "Property";
            this.list_Property.Width = 150;
            // 
            // list_Value
            // 
            this.list_Value.Text = "Value";
            this.list_Value.Width = 408;
            // 
            // mnxList
            // 
            this.mnxList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnxList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxListCopy,
            this.toolStripMenuItem6,
            this.mnxListSelectAll});
            this.mnxList.Name = "mnxListAddress";
            this.mnxList.Size = new System.Drawing.Size(229, 70);
            this.mnxList.Opening += new System.ComponentModel.CancelEventHandler(this.mnxList_Opening);
            // 
            // mnxListCopy
            // 
            this.mnxListCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnxListCopy.Image")));
            this.mnxListCopy.Name = "mnxListCopy";
            this.mnxListCopy.ShortcutKeyDisplayString = "Ctrl+C";
            this.mnxListCopy.Size = new System.Drawing.Size(228, 30);
            this.mnxListCopy.Text = "&Copy";
            this.mnxListCopy.Click += new System.EventHandler(this.mnxListCopy_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(225, 6);
            // 
            // mnxListSelectAll
            // 
            this.mnxListSelectAll.Name = "mnxListSelectAll";
            this.mnxListSelectAll.ShortcutKeyDisplayString = "Ctrl+A";
            this.mnxListSelectAll.Size = new System.Drawing.Size(228, 30);
            this.mnxListSelectAll.Text = "&Select all";
            this.mnxListSelectAll.Click += new System.EventHandler(this.mnxListSelectAll_Click);
            // 
            // bwExecutor
            // 
            this.bwExecutor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwExecutor_DoWork);
            this.bwExecutor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwExecutor_RunWorkerCompleted);
            // 
            // mnuNew
            // 
            this.mnuNew.Image = global::VhdAttach.Properties.Resources.mnuNew_16;
            this.mnuNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.Size = new System.Drawing.Size(71, 29);
            this.mnuNew.Text = "New";
            this.mnuNew.ToolTipText = "New virtual disk (Ctrl+N)";
            this.mnuNew.Click += new System.EventHandler(this.mnuFile_Click);
            // 
            // mnuAttach
            // 
            this.mnuAttach.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAttachReadOnly});
            this.mnuAttach.Enabled = false;
            this.mnuAttach.Image = global::VhdAttach.Properties.Resources.mnuAttach_16;
            this.mnuAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuAttach.Name = "mnuAttach";
            this.mnuAttach.Size = new System.Drawing.Size(104, 29);
            this.mnuAttach.Text = "Attach";
            this.mnuAttach.ToolTipText = "Attach virtual disk (F6)";
            this.mnuAttach.ButtonClick += new System.EventHandler(this.mnuAttach_ButtonClick);
            // 
            // mnuAttachReadOnly
            // 
            this.mnuAttachReadOnly.Name = "mnuAttachReadOnly";
            this.mnuAttachReadOnly.Size = new System.Drawing.Size(229, 30);
            this.mnuAttachReadOnly.Text = "Attach read-only";
            this.mnuAttachReadOnly.Click += new System.EventHandler(this.mnuAttachReadOnly_Click);
            // 
            // mnuOpen
            // 
            this.mnuOpen.Image = global::VhdAttach.Properties.Resources.mnuOpen_16;
            this.mnuOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.Size = new System.Drawing.Size(97, 29);
            this.mnuOpen.Text = "Open";
            this.mnuOpen.ToolTipText = "Open virtual disk (Ctrl+O)";
            this.mnuOpen.ButtonClick += new System.EventHandler(this.mnuOpen_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 32);
            // 
            // mnuDetach
            // 
            this.mnuDetach.Enabled = false;
            this.mnuDetach.Image = global::VhdAttach.Properties.Resources.mnuDetach_16;
            this.mnuDetach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuDetach.Name = "mnuDetach";
            this.mnuDetach.Size = new System.Drawing.Size(91, 29);
            this.mnuDetach.Text = "Detach";
            this.mnuDetach.ToolTipText = "Detach virtual disk";
            this.mnuDetach.Click += new System.EventHandler(this.mnuDetach_Click);
            // 
            // mnuApp
            // 
            this.mnuApp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuApp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAppOptions,
            this.mnuApp0,
            this.mnuAppFeedback,
            this.mnuAppUpgrade,
            this.mnuApp1,
            this.mnuAppAbout});
            this.mnuApp.Image = global::VhdAttach.Properties.Resources.mnuApp_16;
            this.mnuApp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuApp.Name = "mnuApp";
            this.mnuApp.Size = new System.Drawing.Size(38, 29);
            this.mnuApp.Text = "Application";
            // 
            // mnuAppOptions
            // 
            this.mnuAppOptions.Name = "mnuAppOptions";
            this.mnuAppOptions.Size = new System.Drawing.Size(244, 30);
            this.mnuAppOptions.Text = "&Options";
            this.mnuAppOptions.Click += new System.EventHandler(this.mnuAppOptions_Click);
            // 
            // mnuApp0
            // 
            this.mnuApp0.Name = "mnuApp0";
            this.mnuApp0.Size = new System.Drawing.Size(241, 6);
            // 
            // mnuAppFeedback
            // 
            this.mnuAppFeedback.Name = "mnuAppFeedback";
            this.mnuAppFeedback.Size = new System.Drawing.Size(244, 30);
            this.mnuAppFeedback.Text = "Send &feedback";
            this.mnuAppFeedback.Click += new System.EventHandler(this.mnuAppFeedback_Click);
            // 
            // mnuAppUpgrade
            // 
            this.mnuAppUpgrade.Name = "mnuAppUpgrade";
            this.mnuAppUpgrade.Size = new System.Drawing.Size(244, 30);
            this.mnuAppUpgrade.Text = "Check for &upgrade";
            this.mnuAppUpgrade.Click += new System.EventHandler(this.mnuAppUpgrade_Click);
            // 
            // mnuApp1
            // 
            this.mnuApp1.Name = "mnuApp1";
            this.mnuApp1.Size = new System.Drawing.Size(241, 6);
            // 
            // mnuAppAbout
            // 
            this.mnuAppAbout.Name = "mnuAppAbout";
            this.mnuAppAbout.Size = new System.Drawing.Size(244, 30);
            this.mnuAppAbout.Text = "&About";
            this.mnuAppAbout.Click += new System.EventHandler(this.mnuAppAbout_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 32);
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuRefresh.Enabled = false;
            this.mnuRefresh.Image = global::VhdAttach.Properties.Resources.mnuRefresh_16;
            this.mnuRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.Size = new System.Drawing.Size(24, 24);
            this.mnuRefresh.Text = "Refresh";
            this.mnuRefresh.ToolTipText = "Refresh (F5)";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            // 
            // mnu
            // 
            this.mnu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNew,
            this.mnuOpen,
            this.toolStripSeparator1,
            this.mnuAttach,
            this.mnuDetach,
            this.mnuApp,
            this.toolStripSeparator4,
            this.toolStripSeparator5,
            this.mnuAutomount,
            this.toolStripSeparator2,
            this.mnuDrive,
            this.toolStripSeparator3,
            this.mnuRefresh});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.mnu.Size = new System.Drawing.Size(677, 32);
            this.mnu.Stretch = true;
            this.mnu.TabIndex = 1;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 32);
            // 
            // mnuAutomount
            // 
            this.mnuAutomount.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAutomountNormal,
            this.mnuAutomountReadonly,
            this.toolStripMenuItem2,
            this.mnuAutomountDisable});
            this.mnuAutomount.Enabled = false;
            this.mnuAutomount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuAutomount.Name = "mnuAutomount";
            this.mnuAutomount.Size = new System.Drawing.Size(132, 29);
            this.mnuAutomount.Text = "Auto-mount";
            this.mnuAutomount.ButtonClick += new System.EventHandler(this.mnuAutomount_ButtonClick);
            this.mnuAutomount.DropDownOpening += new System.EventHandler(this.mnuAutomount_DropDownOpening);
            // 
            // mnuAutomountNormal
            // 
            this.mnuAutomountNormal.Name = "mnuAutomountNormal";
            this.mnuAutomountNormal.Size = new System.Drawing.Size(277, 30);
            this.mnuAutomountNormal.Text = "Auto-mount";
            this.mnuAutomountNormal.Click += new System.EventHandler(this.mnuAutomountNormal_Click);
            // 
            // mnuAutomountReadonly
            // 
            this.mnuAutomountReadonly.Image = ((System.Drawing.Image)(resources.GetObject("mnuAutomountReadonly.Image")));
            this.mnuAutomountReadonly.Name = "mnuAutomountReadonly";
            this.mnuAutomountReadonly.Size = new System.Drawing.Size(277, 30);
            this.mnuAutomountReadonly.Text = "Auto-mount read-only";
            this.mnuAutomountReadonly.Click += new System.EventHandler(this.mnuAutomountReadonly_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(274, 6);
            // 
            // mnuAutomountDisable
            // 
            this.mnuAutomountDisable.Name = "mnuAutomountDisable";
            this.mnuAutomountDisable.Size = new System.Drawing.Size(277, 30);
            this.mnuAutomountDisable.Text = "Do not auto-mount";
            this.mnuAutomountDisable.Click += new System.EventHandler(this.mnuAutomountDisable_Click);
            // 
            // mnuDrive
            // 
            this.mnuDrive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuDrive.Enabled = false;
            this.mnuDrive.Image = ((System.Drawing.Image)(resources.GetObject("mnuDrive.Image")));
            this.mnuDrive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuDrive.Name = "mnuDrive";
            this.mnuDrive.Size = new System.Drawing.Size(74, 29);
            this.mnuDrive.Text = "Drive";
            this.mnuDrive.ButtonClick += new System.EventHandler(this.mnuDrive_ButtonClick);
            this.mnuDrive.DropDownOpening += new System.EventHandler(this.mnuDrive_DropDownOpening);
            // 
            // staErrorStolenExtension
            // 
            this.staErrorStolenExtension.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.staErrorStolenExtension.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staErrorStolenExtensionText});
            this.staErrorStolenExtension.Location = new System.Drawing.Point(0, 462);
            this.staErrorStolenExtension.Name = "staErrorStolenExtension";
            this.staErrorStolenExtension.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.staErrorStolenExtension.Size = new System.Drawing.Size(655, 31);
            this.staErrorStolenExtension.TabIndex = 3;
            this.staErrorStolenExtension.Visible = false;
            // 
            // staErrorStolenExtensionText
            // 
            this.staErrorStolenExtensionText.Image = ((System.Drawing.Image)(resources.GetObject("staErrorStolenExtensionText.Image")));
            this.staErrorStolenExtensionText.IsLink = true;
            this.staErrorStolenExtensionText.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.staErrorStolenExtensionText.Margin = new System.Windows.Forms.Padding(1, 3, 0, 2);
            this.staErrorStolenExtensionText.Name = "staErrorStolenExtensionText";
            this.staErrorStolenExtensionText.Size = new System.Drawing.Size(443, 26);
            this.staErrorStolenExtensionText.Text = "Another application is registered for VHD extension.";
            this.staErrorStolenExtensionText.Click += new System.EventHandler(this.staStolenExtensionText_Click);
            // 
            // staErrorServiceMissing
            // 
            this.staErrorServiceMissing.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.staErrorServiceMissing.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staErrorServiceMissingText});
            this.staErrorServiceMissing.Location = new System.Drawing.Point(0, 462);
            this.staErrorServiceMissing.Name = "staErrorServiceMissing";
            this.staErrorServiceMissing.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.staErrorServiceMissing.Size = new System.Drawing.Size(655, 31);
            this.staErrorServiceMissing.TabIndex = 4;
            this.staErrorServiceMissing.Visible = false;
            // 
            // staErrorServiceMissingText
            // 
            this.staErrorServiceMissingText.Image = ((System.Drawing.Image)(resources.GetObject("staErrorServiceMissingText.Image")));
            this.staErrorServiceMissingText.IsLink = true;
            this.staErrorServiceMissingText.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.staErrorServiceMissingText.Margin = new System.Windows.Forms.Padding(1, 3, 0, 2);
            this.staErrorServiceMissingText.Name = "staErrorServiceMissingText";
            this.staErrorServiceMissingText.Size = new System.Drawing.Size(327, 26);
            this.staErrorServiceMissingText.Text = "Service is not installed. Click to install.";
            this.staErrorServiceMissingText.Click += new System.EventHandler(this.staErrorServiceMissingText_Click);
            // 
            // staErrorServiceNotRunning
            // 
            this.staErrorServiceNotRunning.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.staErrorServiceNotRunning.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staErrorServiceNotRunningText});
            this.staErrorServiceNotRunning.Location = new System.Drawing.Point(0, 462);
            this.staErrorServiceNotRunning.Name = "staErrorServiceNotRunning";
            this.staErrorServiceNotRunning.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.staErrorServiceNotRunning.Size = new System.Drawing.Size(655, 31);
            this.staErrorServiceNotRunning.TabIndex = 5;
            this.staErrorServiceNotRunning.Visible = false;
            // 
            // staErrorServiceNotRunningText
            // 
            this.staErrorServiceNotRunningText.Image = ((System.Drawing.Image)(resources.GetObject("staErrorServiceNotRunningText.Image")));
            this.staErrorServiceNotRunningText.IsLink = true;
            this.staErrorServiceNotRunningText.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.staErrorServiceNotRunningText.Margin = new System.Windows.Forms.Padding(1, 3, 0, 2);
            this.staErrorServiceNotRunningText.Name = "staErrorServiceNotRunningText";
            this.staErrorServiceNotRunningText.Size = new System.Drawing.Size(313, 26);
            this.staErrorServiceNotRunningText.Text = "Service is not running. Click to start.";
            this.staErrorServiceNotRunningText.Click += new System.EventHandler(this.staErrorServiceNotRunningText_Click);
            // 
            // tmrUpdateMenu
            // 
            this.tmrUpdateMenu.Enabled = true;
            this.tmrUpdateMenu.Interval = 1000;
            this.tmrUpdateMenu.Tick += new System.EventHandler(this.tmrUpdateMenu_Tick);
            // 
            // bwCheckForUpgrade
            // 
            this.bwCheckForUpgrade.WorkerSupportsCancellation = true;
            this.bwCheckForUpgrade.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwCheckForUpgrade_DoWork);
            this.bwCheckForUpgrade.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwCheckForUpgrade_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 494);
            this.Controls.Add(this.list);
            this.Controls.Add(this.staErrorStolenExtension);
            this.Controls.Add(this.staErrorServiceMissing);
            this.Controls.Add(this.staErrorServiceNotRunning);
            this.Controls.Add(this.mnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(402, 236);
            this.Name = "MainForm";
            this.Text = "VHD Attach";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.mnxList.ResumeLayout(false);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.staErrorStolenExtension.ResumeLayout(false);
            this.staErrorStolenExtension.PerformLayout();
            this.staErrorServiceMissing.ResumeLayout(false);
            this.staErrorServiceMissing.PerformLayout();
            this.staErrorServiceNotRunning.ResumeLayout(false);
            this.staErrorServiceNotRunning.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView list;
        private System.Windows.Forms.ColumnHeader list_Property;
        private System.Windows.Forms.ColumnHeader list_Value;
        private System.Windows.Forms.ContextMenuStrip mnxList;
        private System.Windows.Forms.ToolStripMenuItem mnxListCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem mnxListSelectAll;
        private System.ComponentModel.BackgroundWorker bwExecutor;
        private System.Windows.Forms.ToolStripButton mnuNew;
        private System.Windows.Forms.ToolStripSplitButton mnuAttach;
        private System.Windows.Forms.ToolStripMenuItem mnuAttachReadOnly;
        private System.Windows.Forms.ToolStripSplitButton mnuOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton mnuDetach;
        private System.Windows.Forms.ToolStripDropDownButton mnuApp;
        private System.Windows.Forms.ToolStripMenuItem mnuAppFeedback;
        private System.Windows.Forms.ToolStripMenuItem mnuAppAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton mnuRefresh;
        private System.Windows.Forms.ToolStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuAppUpgrade;
        private System.Windows.Forms.ToolStripSeparator mnuApp1;
        private System.Windows.Forms.StatusStrip staErrorStolenExtension;
        private System.Windows.Forms.ToolStripStatusLabel staErrorStolenExtensionText;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip staErrorServiceMissing;
        private System.Windows.Forms.ToolStripStatusLabel staErrorServiceMissingText;
        private System.Windows.Forms.StatusStrip staErrorServiceNotRunning;
        private System.Windows.Forms.ToolStripStatusLabel staErrorServiceNotRunningText;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSplitButton mnuDrive;
        private System.Windows.Forms.ToolStripSplitButton mnuAutomount;
        private System.Windows.Forms.ToolStripMenuItem mnuAutomountNormal;
        private System.Windows.Forms.ToolStripMenuItem mnuAutomountReadonly;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuAutomountDisable;
        private System.Windows.Forms.Timer tmrUpdateMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuAppOptions;
        private System.Windows.Forms.ToolStripSeparator mnuApp0;
        private System.ComponentModel.BackgroundWorker bwCheckForUpgrade;
    }
}

