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
            this.mnu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAction = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionAttach = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionDetach = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpReportABug = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnx = new System.Windows.Forms.ToolStrip();
            this.mnxFileNew = new System.Windows.Forms.ToolStripButton();
            this.mnxFileOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxAttach = new System.Windows.Forms.ToolStripButton();
            this.mnxDetach = new System.Windows.Forms.ToolStripButton();
            this.mnxHelpAbout = new System.Windows.Forms.ToolStripButton();
            this.mnxHelpReportABug = new System.Windows.Forms.ToolStripButton();
            this.mnxToolsOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxToolsRefresh = new System.Windows.Forms.ToolStripButton();
            this.list = new System.Windows.Forms.ListView();
            this.list_Property = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.list_Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnxList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnxListCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxListEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.bwExecutor = new System.ComponentModel.BackgroundWorker();
            this.mnu.SuspendLayout();
            this.mnx.SuspendLayout();
            this.mnxList.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnu
            // 
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuAction,
            this.mnuTools,
            this.mnuHelp});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(582, 28);
            this.mnu.TabIndex = 0;
            this.mnu.Visible = false;
            this.mnu.MenuDeactivate += new System.EventHandler(this.mnu_MenuDeactivate);
            this.mnu.VisibleChanged += new System.EventHandler(this.mnu_VisibleChanged);
            this.mnu.Leave += new System.EventHandler(this.mnu_Leave);
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileRecent,
            this.toolStripMenuItem1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(44, 24);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileNew.Image")));
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuFileNew.Size = new System.Drawing.Size(167, 24);
            this.mnuFileNew.Text = "&New";
            this.mnuFileNew.Visible = false;
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileOpen.Image")));
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuFileOpen.Size = new System.Drawing.Size(167, 24);
            this.mnuFileOpen.Text = "&Open";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileRecent
            // 
            this.mnuFileRecent.Name = "mnuFileRecent";
            this.mnuFileRecent.Size = new System.Drawing.Size(167, 24);
            this.mnuFileRecent.Text = "&Recent files";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(164, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(167, 24);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditCopy,
            this.toolStripMenuItem4,
            this.mnuEditSelectAll});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(47, 24);
            this.mnuEdit.Text = "&Edit";
            this.mnuEdit.DropDownOpening += new System.EventHandler(this.mnuEdit_DropDownOpening);
            // 
            // mnuEditCopy
            // 
            this.mnuEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCopy.Image")));
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuEditCopy.Size = new System.Drawing.Size(190, 24);
            this.mnuEditCopy.Text = "&Copy";
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuEditSelectAll
            // 
            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuEditSelectAll.Size = new System.Drawing.Size(190, 24);
            this.mnuEditSelectAll.Text = "&Select all";
            this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
            // 
            // mnuAction
            // 
            this.mnuAction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuActionAttach,
            this.mnuActionDetach});
            this.mnuAction.Name = "mnuAction";
            this.mnuAction.Size = new System.Drawing.Size(64, 24);
            this.mnuAction.Text = "&Action";
            // 
            // mnuActionAttach
            // 
            this.mnuActionAttach.Enabled = false;
            this.mnuActionAttach.Image = ((System.Drawing.Image)(resources.GetObject("mnuActionAttach.Image")));
            this.mnuActionAttach.Name = "mnuActionAttach";
            this.mnuActionAttach.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.mnuActionAttach.Size = new System.Drawing.Size(145, 24);
            this.mnuActionAttach.Text = "&Attach";
            this.mnuActionAttach.Click += new System.EventHandler(this.mnuActionAttach_Click);
            // 
            // mnuActionDetach
            // 
            this.mnuActionDetach.Enabled = false;
            this.mnuActionDetach.Image = ((System.Drawing.Image)(resources.GetObject("mnuActionDetach.Image")));
            this.mnuActionDetach.Name = "mnuActionDetach";
            this.mnuActionDetach.Size = new System.Drawing.Size(145, 24);
            this.mnuActionDetach.Text = "&Detach";
            this.mnuActionDetach.Click += new System.EventHandler(this.mnuActionDetach_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsRefresh,
            this.toolStripMenuItem12,
            this.mnuToolsOptions});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(57, 24);
            this.mnuTools.Text = "&Tools";
            // 
            // mnuToolsRefresh
            // 
            this.mnuToolsRefresh.Image = ((System.Drawing.Image)(resources.GetObject("mnuToolsRefresh.Image")));
            this.mnuToolsRefresh.Name = "mnuToolsRefresh";
            this.mnuToolsRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuToolsRefresh.Size = new System.Drawing.Size(151, 24);
            this.mnuToolsRefresh.Text = "&Refresh";
            this.mnuToolsRefresh.Click += new System.EventHandler(this.mnuToolsRefresh_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(148, 6);
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuToolsOptions.Image")));
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.Size = new System.Drawing.Size(151, 24);
            this.mnuToolsOptions.Text = "&Options";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpReportABug,
            this.toolStripMenuItem5,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(53, 24);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpReportABug
            // 
            this.mnuHelpReportABug.Image = ((System.Drawing.Image)(resources.GetObject("mnuHelpReportABug.Image")));
            this.mnuHelpReportABug.Name = "mnuHelpReportABug";
            this.mnuHelpReportABug.Size = new System.Drawing.Size(165, 24);
            this.mnuHelpReportABug.Text = "Report a &bug";
            this.mnuHelpReportABug.Click += new System.EventHandler(this.mnuHelpReportABug_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(162, 6);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("mnuHelpAbout.Image")));
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(165, 24);
            this.mnuHelpAbout.Text = "&About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // mnx
            // 
            this.mnx.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mnx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxFileNew,
            this.mnxFileOpen,
            this.toolStripSeparator2,
            this.mnxAttach,
            this.mnxDetach,
            this.mnxHelpAbout,
            this.mnxHelpReportABug,
            this.mnxToolsOptions,
            this.toolStripSeparator4,
            this.toolStripSeparator1,
            this.mnxToolsRefresh});
            this.mnx.Location = new System.Drawing.Point(0, 0);
            this.mnx.Name = "mnx";
            this.mnx.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnx.Size = new System.Drawing.Size(582, 27);
            this.mnx.Stretch = true;
            this.mnx.TabIndex = 1;
            // 
            // mnxFileNew
            // 
            this.mnxFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnxFileNew.Image")));
            this.mnxFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxFileNew.Name = "mnxFileNew";
            this.mnxFileNew.Size = new System.Drawing.Size(59, 24);
            this.mnxFileNew.Text = "New";
            this.mnxFileNew.ToolTipText = "New virtual disk (Ctrl+N)";
            this.mnxFileNew.Visible = false;
            this.mnxFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // mnxAttach
            // 
            this.mnxAttach.Enabled = false;
            this.mnxAttach.Image = ((System.Drawing.Image)(resources.GetObject("mnxAttach.Image")));
            this.mnxAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxAttach.Name = "mnxAttach";
            this.mnxAttach.Size = new System.Drawing.Size(72, 24);
            this.mnxAttach.Text = "&Attach";
            this.mnxAttach.ToolTipText = "Attach virtual disk (F6)";
            this.mnxAttach.Click += new System.EventHandler(this.mnuActionAttach_Click);
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
            // mnxHelpAbout
            // 
            this.mnxHelpAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnxHelpAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnxHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("mnxHelpAbout.Image")));
            this.mnxHelpAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxHelpAbout.Name = "mnxHelpAbout";
            this.mnxHelpAbout.Size = new System.Drawing.Size(23, 24);
            this.mnxHelpAbout.Text = "About";
            this.mnxHelpAbout.Click += new System.EventHandler(this.mnxHelpAbout_Click);
            // 
            // mnxHelpReportABug
            // 
            this.mnxHelpReportABug.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnxHelpReportABug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnxHelpReportABug.Image = ((System.Drawing.Image)(resources.GetObject("mnxHelpReportABug.Image")));
            this.mnxHelpReportABug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnxHelpReportABug.Name = "mnxHelpReportABug";
            this.mnxHelpReportABug.Size = new System.Drawing.Size(23, 24);
            this.mnxHelpReportABug.Text = "Report a bug";
            this.mnxHelpReportABug.Click += new System.EventHandler(this.mnxHelpReportABug_Click);
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
            // list
            // 
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 375);
            this.Controls.Add(this.list);
            this.Controls.Add(this.mnx);
            this.Controls.Add(this.mnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnu;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MainForm";
            this.Text = "VHD Attach";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.mnx.ResumeLayout(false);
            this.mnx.PerformLayout();
            this.mnxList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuAction;
        private System.Windows.Forms.ToolStripMenuItem mnuActionAttach;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStrip mnx;
        private System.Windows.Forms.ToolStripButton mnxFileNew;
        private System.Windows.Forms.ToolStripButton mnxAttach;
        private System.Windows.Forms.ListView list;
        private System.Windows.Forms.ColumnHeader list_Property;
        private System.Windows.Forms.ColumnHeader list_Value;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuFileRecent;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        private System.Windows.Forms.ToolStripSplitButton mnxFileOpen;
        private System.Windows.Forms.ContextMenuStrip mnxList;
        private System.Windows.Forms.ToolStripMenuItem mnxListCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem mnuEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnxListEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuActionDetach;
        private System.Windows.Forms.ToolStripButton mnxDetach;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpReportABug;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
        private System.Windows.Forms.ToolStripButton mnxToolsOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.ComponentModel.BackgroundWorker bwExecutor;
        private System.Windows.Forms.ToolStripButton mnxHelpAbout;
        private System.Windows.Forms.ToolStripButton mnxHelpReportABug;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton mnxToolsRefresh;
    }
}

