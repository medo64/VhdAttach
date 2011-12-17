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
            this.mnxListEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.bwExecutor = new System.ComponentModel.BackgroundWorker();
            this.mnxFileNew = new System.Windows.Forms.ToolStripButton();
            this.mnxAttach = new System.Windows.Forms.ToolStripSplitButton();
            this.mnuAttachReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxFileOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxDetach = new System.Windows.Forms.ToolStripButton();
            this.mnuApp = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuAppFeedback = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAppUpgrade = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAppDonate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAppAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxToolsOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxAutoMount = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxToolsRefresh = new System.Windows.Forms.ToolStripButton();
            this.mnu = new System.Windows.Forms.ToolStrip();
            this.staStolenExtension = new System.Windows.Forms.StatusStrip();
            this.staStolenExtensionText = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnxList.SuspendLayout();
            this.mnu.SuspendLayout();
            this.staStolenExtension.SuspendLayout();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.AllowDrop = true;
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.list_Property,
            this.list_Value});
            this.list.ContextMenuStrip = this.mnxList;
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.FullRowSelect = true;
            this.list.GridLines = true;
            this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.list.HideSelection = false;
            this.list.Location = new System.Drawing.Point(0, 27);
            this.list.Name = "list";
            this.list.ShowGroups = false;
            this.list.Size = new System.Drawing.Size(582, 348);
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
            this.mnxList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxListCopy,
            this.toolStripMenuItem6,
            this.mnxListEditSelectAll});
            this.mnxList.Name = "mnxListAddress";
            this.mnxList.Size = new System.Drawing.Size(191, 58);
            this.mnxList.Opening += new System.ComponentModel.CancelEventHandler(this.mnxList_Opening);
            // 
            // mnxListCopy
            // 
            this.mnxListCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnxListCopy.Image")));
            this.mnxListCopy.Name = "mnxListCopy";
            this.mnxListCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnxListCopy.Size = new System.Drawing.Size(190, 24);
            this.mnxListCopy.Text = "&Copy";
            this.mnxListCopy.Click += new System.EventHandler(this.mnxListCopy_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(187, 6);
            // 
            // mnxListEditSelectAll
            // 
            this.mnxListEditSelectAll.Name = "mnxListEditSelectAll";
            this.mnxListEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnxListEditSelectAll.Size = new System.Drawing.Size(190, 24);
            this.mnxListEditSelectAll.Text = "&Select all";
            this.mnxListEditSelectAll.Click += new System.EventHandler(this.mnxListEditSelectAll_Click);
            // 
            // bwExecutor
            // 
            this.bwExecutor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwExecutor_DoWork);
            this.bwExecutor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwExecutor_RunWorkerCompleted);
            // 
            // mnxFileNew
            // 
            this.mnxFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnxFileNew.Image")));
            this.mnxFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxFileNew.Name = "mnxFileNew";
            this.mnxFileNew.Size = new System.Drawing.Size(59, 24);
            this.mnxFileNew.Text = "New";
            this.mnxFileNew.ToolTipText = "New virtual disk (Ctrl+N)";
            this.mnxFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnxAttach
            // 
            this.mnxAttach.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAttachReadOnly});
            this.mnxAttach.Enabled = false;
            this.mnxAttach.Image = ((System.Drawing.Image)(resources.GetObject("mnxAttach.Image")));
            this.mnxAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxAttach.Name = "mnxAttach";
            this.mnxAttach.Size = new System.Drawing.Size(84, 24);
            this.mnxAttach.Text = "&Attach";
            this.mnxAttach.ToolTipText = "Attach virtual disk (F6)";
            this.mnxAttach.ButtonClick += new System.EventHandler(this.mnuAttach_ButtonClick);
            // 
            // mnuAttachReadOnly
            // 
            this.mnuAttachReadOnly.Name = "mnuAttachReadOnly";
            this.mnuAttachReadOnly.Size = new System.Drawing.Size(189, 24);
            this.mnuAttachReadOnly.Text = "Attach read-only";
            this.mnuAttachReadOnly.Click += new System.EventHandler(this.mnuAttachReadOnly_Click);
            // 
            // mnxFileOpen
            // 
            this.mnxFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnxFileOpen.Image")));
            this.mnxFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxFileOpen.Name = "mnxFileOpen";
            this.mnxFileOpen.Size = new System.Drawing.Size(77, 24);
            this.mnxFileOpen.Text = "&Open";
            this.mnxFileOpen.ToolTipText = "Open virtual disk (Ctrl+O)";
            this.mnxFileOpen.ButtonClick += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // mnxDetach
            // 
            this.mnxDetach.Enabled = false;
            this.mnxDetach.Image = ((System.Drawing.Image)(resources.GetObject("mnxDetach.Image")));
            this.mnxDetach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxDetach.Name = "mnxDetach";
            this.mnxDetach.Size = new System.Drawing.Size(76, 24);
            this.mnxDetach.Text = "&Detach";
            this.mnxDetach.ToolTipText = "Detach virtual disk";
            this.mnxDetach.Click += new System.EventHandler(this.mnuActionDetach_Click);
            // 
            // mnuApp
            // 
            this.mnuApp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuApp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAppFeedback,
            this.mnuAppUpgrade,
            this.mnuAppDonate,
            this.toolStripMenuItem1,
            this.mnuAppAbout});
            this.mnuApp.Image = ((System.Drawing.Image)(resources.GetObject("mnuApp.Image")));
            this.mnuApp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuApp.Name = "mnuApp";
            this.mnuApp.Size = new System.Drawing.Size(29, 24);
            this.mnuApp.Text = "VHD Attach";
            // 
            // mnuAppFeedback
            // 
            this.mnuAppFeedback.Name = "mnuAppFeedback";
            this.mnuAppFeedback.Size = new System.Drawing.Size(200, 24);
            this.mnuAppFeedback.Text = "Send &feedback";
            this.mnuAppFeedback.Click += new System.EventHandler(this.mnuAppFeedback_Click);
            // 
            // mnuAppUpgrade
            // 
            this.mnuAppUpgrade.Name = "mnuAppUpgrade";
            this.mnuAppUpgrade.Size = new System.Drawing.Size(200, 24);
            this.mnuAppUpgrade.Text = "Check for &upgrade";
            this.mnuAppUpgrade.Click += new System.EventHandler(this.mnuAppUpgrade_Click);
            // 
            // mnuAppDonate
            // 
            this.mnuAppDonate.Name = "mnuAppDonate";
            this.mnuAppDonate.Size = new System.Drawing.Size(200, 24);
            this.mnuAppDonate.Text = "&Donate";
            this.mnuAppDonate.Click += new System.EventHandler(this.mnuAppDonate_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 6);
            // 
            // mnuAppAbout
            // 
            this.mnuAppAbout.Name = "mnuAppAbout";
            this.mnuAppAbout.Size = new System.Drawing.Size(200, 24);
            this.mnuAppAbout.Text = "&About";
            this.mnuAppAbout.Click += new System.EventHandler(this.mnuAppAbout_Click);
            // 
            // mnxToolsOptions
            // 
            this.mnxToolsOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnxToolsOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnxToolsOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnxToolsOptions.Image")));
            this.mnxToolsOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxToolsOptions.Name = "mnxToolsOptions";
            this.mnxToolsOptions.Size = new System.Drawing.Size(23, 24);
            this.mnxToolsOptions.Text = "Options";
            this.mnxToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // mnxAutoMount
            // 
            this.mnxAutoMount.CheckOnClick = true;
            this.mnxAutoMount.Enabled = false;
            this.mnxAutoMount.Image = ((System.Drawing.Image)(resources.GetObject("mnxAutoMount.Image")));
            this.mnxAutoMount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxAutoMount.Name = "mnxAutoMount";
            this.mnxAutoMount.Size = new System.Drawing.Size(110, 24);
            this.mnxAutoMount.Text = "Auto-mount";
            this.mnxAutoMount.ToolTipText = "Selects whether VHD is mounted upon next system restart";
            this.mnxAutoMount.Click += new System.EventHandler(this.mnxAutoMount_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparator1.Visible = false;
            // 
            // mnxToolsRefresh
            // 
            this.mnxToolsRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnxToolsRefresh.Image = ((System.Drawing.Image)(resources.GetObject("mnxToolsRefresh.Image")));
            this.mnxToolsRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxToolsRefresh.Name = "mnxToolsRefresh";
            this.mnxToolsRefresh.Size = new System.Drawing.Size(23, 24);
            this.mnxToolsRefresh.Text = "Refresh";
            this.mnxToolsRefresh.ToolTipText = "Refresh (F5)";
            this.mnxToolsRefresh.Visible = false;
            // 
            // mnu
            // 
            this.mnu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxFileNew,
            this.mnxFileOpen,
            this.toolStripSeparator3,
            this.mnxAttach,
            this.mnxDetach,
            this.mnuApp,
            this.mnxToolsOptions,
            this.toolStripSeparator4,
            this.toolStripSeparator2,
            this.mnxAutoMount,
            this.toolStripSeparator1,
            this.mnxToolsRefresh});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(582, 27);
            this.mnu.Stretch = true;
            this.mnu.TabIndex = 1;
            // 
            // staStolenExtension
            // 
            this.staStolenExtension.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staStolenExtensionText});
            this.staStolenExtension.Location = new System.Drawing.Point(0, 350);
            this.staStolenExtension.Name = "staStolenExtension";
            this.staStolenExtension.Size = new System.Drawing.Size(582, 25);
            this.staStolenExtension.TabIndex = 3;
            this.staStolenExtension.Text = "statusStrip1";
            this.staStolenExtension.Visible = false;
            // 
            // staStolenExtensionText
            // 
            this.staStolenExtensionText.Image = ((System.Drawing.Image)(resources.GetObject("staStolenExtensionText.Image")));
            this.staStolenExtensionText.IsLink = true;
            this.staStolenExtensionText.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.staStolenExtensionText.Margin = new System.Windows.Forms.Padding(1, 3, 0, 2);
            this.staStolenExtensionText.Name = "staStolenExtensionText";
            this.staStolenExtensionText.Size = new System.Drawing.Size(370, 20);
            this.staStolenExtensionText.Text = "Another application is registered for VHD extension.";
            this.staStolenExtensionText.Click += new System.EventHandler(this.staStolenExtensionText_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 375);
            this.Controls.Add(this.staStolenExtension);
            this.Controls.Add(this.list);
            this.Controls.Add(this.mnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MainForm";
            this.Text = "VHD Attach";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mnxList.ResumeLayout(false);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.staStolenExtension.ResumeLayout(false);
            this.staStolenExtension.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem mnxListEditSelectAll;
        private System.ComponentModel.BackgroundWorker bwExecutor;
        private System.Windows.Forms.ToolStripButton mnxFileNew;
        private System.Windows.Forms.ToolStripSplitButton mnxAttach;
        private System.Windows.Forms.ToolStripMenuItem mnuAttachReadOnly;
        private System.Windows.Forms.ToolStripSplitButton mnxFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton mnxDetach;
        private System.Windows.Forms.ToolStripDropDownButton mnuApp;
        private System.Windows.Forms.ToolStripMenuItem mnuAppFeedback;
        private System.Windows.Forms.ToolStripMenuItem mnuAppAbout;
        private System.Windows.Forms.ToolStripButton mnxToolsOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton mnxAutoMount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton mnxToolsRefresh;
        private System.Windows.Forms.ToolStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuAppUpgrade;
        private System.Windows.Forms.ToolStripMenuItem mnuAppDonate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.StatusStrip staStolenExtension;
        private System.Windows.Forms.ToolStripStatusLabel staStolenExtensionText;
    }
}

