namespace VhdAttach {
    partial class SettingsForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.erp = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupContextMenu = new System.Windows.Forms.GroupBox();
            this.checkAttachReadOnly = new System.Windows.Forms.CheckBox();
            this.checkDetachDrive = new System.Windows.Forms.CheckBox();
            this.checkDetach = new System.Windows.Forms.CheckBox();
            this.checkAttach = new System.Windows.Forms.CheckBox();
            this.groupAutoAttach = new System.Windows.Forms.GroupBox();
            this.toolVhdOrder = new System.Windows.Forms.ToolStrip();
            this.buttonMoveVhdUp = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveVhdDown = new System.Windows.Forms.ToolStripButton();
            this.buttonVhdReadOnly = new System.Windows.Forms.ToolStripButton();
            this.buttonVhdRemove = new System.Windows.Forms.Button();
            this.buttonVhdAdd = new System.Windows.Forms.Button();
            this.listAutoAttach = new System.Windows.Forms.ListView();
            this.columnFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imagesAutoAttach = new System.Windows.Forms.ImageList(this.components);
            this.btnRegisterExtension = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.erp)).BeginInit();
            this.groupContextMenu.SuspendLayout();
            this.groupAutoAttach.SuspendLayout();
            this.toolVhdOrder.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(212, 288);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 12, 2, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(132, 288);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(2, 12, 2, 2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // erp
            // 
            this.erp.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.erp.ContainerControl = this;
            this.erp.Icon = ((System.Drawing.Icon)(resources.GetObject("erp.Icon")));
            // 
            // groupContextMenu
            // 
            this.groupContextMenu.Controls.Add(this.checkAttachReadOnly);
            this.groupContextMenu.Controls.Add(this.checkDetachDrive);
            this.groupContextMenu.Controls.Add(this.checkDetach);
            this.groupContextMenu.Controls.Add(this.checkAttach);
            this.groupContextMenu.Location = new System.Drawing.Point(9, 10);
            this.groupContextMenu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupContextMenu.Name = "groupContextMenu";
            this.groupContextMenu.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupContextMenu.Size = new System.Drawing.Size(278, 63);
            this.groupContextMenu.TabIndex = 0;
            this.groupContextMenu.TabStop = false;
            this.groupContextMenu.Text = "Explorer context menu";
            // 
            // checkAttachReadOnly
            // 
            this.checkAttachReadOnly.AutoSize = true;
            this.checkAttachReadOnly.Location = new System.Drawing.Point(139, 22);
            this.checkAttachReadOnly.Margin = new System.Windows.Forms.Padding(2, 7, 2, 2);
            this.checkAttachReadOnly.Name = "checkAttachReadOnly";
            this.checkAttachReadOnly.Size = new System.Drawing.Size(103, 17);
            this.checkAttachReadOnly.TabIndex = 1;
            this.checkAttachReadOnly.Text = "Attach read-only";
            this.checkAttachReadOnly.UseVisualStyleBackColor = true;
            // 
            // checkDetachDrive
            // 
            this.checkDetachDrive.AutoSize = true;
            this.checkDetachDrive.Location = new System.Drawing.Point(139, 44);
            this.checkDetachDrive.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkDetachDrive.Name = "checkDetachDrive";
            this.checkDetachDrive.Size = new System.Drawing.Size(87, 17);
            this.checkDetachDrive.TabIndex = 3;
            this.checkDetachDrive.Text = "Detach drive";
            this.checkDetachDrive.UseVisualStyleBackColor = true;
            // 
            // checkDetach
            // 
            this.checkDetach.AutoSize = true;
            this.checkDetach.Location = new System.Drawing.Point(4, 44);
            this.checkDetach.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkDetach.Name = "checkDetach";
            this.checkDetach.Size = new System.Drawing.Size(61, 17);
            this.checkDetach.TabIndex = 2;
            this.checkDetach.Text = "Detach";
            this.checkDetach.UseVisualStyleBackColor = true;
            // 
            // checkAttach
            // 
            this.checkAttach.AutoSize = true;
            this.checkAttach.Location = new System.Drawing.Point(4, 22);
            this.checkAttach.Margin = new System.Windows.Forms.Padding(2, 7, 2, 2);
            this.checkAttach.Name = "checkAttach";
            this.checkAttach.Size = new System.Drawing.Size(57, 17);
            this.checkAttach.TabIndex = 0;
            this.checkAttach.Text = "Attach";
            this.checkAttach.UseVisualStyleBackColor = true;
            // 
            // groupAutoAttach
            // 
            this.groupAutoAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupAutoAttach.Controls.Add(this.toolVhdOrder);
            this.groupAutoAttach.Controls.Add(this.buttonVhdRemove);
            this.groupAutoAttach.Controls.Add(this.buttonVhdAdd);
            this.groupAutoAttach.Controls.Add(this.listAutoAttach);
            this.groupAutoAttach.Location = new System.Drawing.Point(9, 83);
            this.groupAutoAttach.Margin = new System.Windows.Forms.Padding(2, 7, 2, 2);
            this.groupAutoAttach.Name = "groupAutoAttach";
            this.groupAutoAttach.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupAutoAttach.Size = new System.Drawing.Size(278, 191);
            this.groupAutoAttach.TabIndex = 1;
            this.groupAutoAttach.TabStop = false;
            this.groupAutoAttach.Text = "Auto-attach VHDs";
            // 
            // toolVhdOrder
            // 
            this.toolVhdOrder.AllowMerge = false;
            this.toolVhdOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolVhdOrder.AutoSize = false;
            this.toolVhdOrder.CanOverflow = false;
            this.toolVhdOrder.Dock = System.Windows.Forms.DockStyle.None;
            this.toolVhdOrder.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolVhdOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonMoveVhdUp,
            this.buttonMoveVhdDown,
            this.buttonVhdReadOnly});
            this.toolVhdOrder.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolVhdOrder.Location = new System.Drawing.Point(253, 22);
            this.toolVhdOrder.Name = "toolVhdOrder";
            this.toolVhdOrder.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolVhdOrder.Size = new System.Drawing.Size(23, 137);
            this.toolVhdOrder.TabIndex = 1;
            // 
            // buttonMoveVhdUp
            // 
            this.buttonMoveVhdUp.AutoSize = false;
            this.buttonMoveVhdUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveVhdUp.Enabled = false;
            this.buttonMoveVhdUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveVhdUp.Image")));
            this.buttonMoveVhdUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveVhdUp.Name = "buttonMoveVhdUp";
            this.buttonMoveVhdUp.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveVhdUp.Text = "Move Up";
            this.buttonMoveVhdUp.ToolTipText = "Move up (Alt+Up)";
            this.buttonMoveVhdUp.Click += new System.EventHandler(this.buttonMoveVhdUp_Click);
            // 
            // buttonMoveVhdDown
            // 
            this.buttonMoveVhdDown.AutoSize = false;
            this.buttonMoveVhdDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveVhdDown.Enabled = false;
            this.buttonMoveVhdDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveVhdDown.Image")));
            this.buttonMoveVhdDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveVhdDown.Name = "buttonMoveVhdDown";
            this.buttonMoveVhdDown.RightToLeftAutoMirrorImage = true;
            this.buttonMoveVhdDown.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveVhdDown.Text = "Move down";
            this.buttonMoveVhdDown.ToolTipText = "Move down (Alt+Down)";
            this.buttonMoveVhdDown.Click += new System.EventHandler(this.buttonMoveVhdDown_Click);
            // 
            // buttonVhdReadOnly
            // 
            this.buttonVhdReadOnly.AutoSize = false;
            this.buttonVhdReadOnly.CheckOnClick = true;
            this.buttonVhdReadOnly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonVhdReadOnly.Image = ((System.Drawing.Image)(resources.GetObject("buttonVhdReadOnly.Image")));
            this.buttonVhdReadOnly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonVhdReadOnly.Margin = new System.Windows.Forms.Padding(0, 8, 0, 2);
            this.buttonVhdReadOnly.Name = "buttonVhdReadOnly";
            this.buttonVhdReadOnly.Size = new System.Drawing.Size(23, 23);
            this.buttonVhdReadOnly.Text = "Read-only";
            this.buttonVhdReadOnly.Click += new System.EventHandler(this.buttonVhdReadOnly_Click);
            // 
            // buttonVhdRemove
            // 
            this.buttonVhdRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonVhdRemove.Enabled = false;
            this.buttonVhdRemove.Location = new System.Drawing.Point(84, 163);
            this.buttonVhdRemove.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonVhdRemove.Name = "buttonVhdRemove";
            this.buttonVhdRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonVhdRemove.TabIndex = 3;
            this.buttonVhdRemove.Text = "&Remove";
            this.buttonVhdRemove.UseVisualStyleBackColor = true;
            this.buttonVhdRemove.Click += new System.EventHandler(this.buttonVhdRemove_Click);
            // 
            // buttonVhdAdd
            // 
            this.buttonVhdAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonVhdAdd.Location = new System.Drawing.Point(4, 163);
            this.buttonVhdAdd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonVhdAdd.Name = "buttonVhdAdd";
            this.buttonVhdAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonVhdAdd.TabIndex = 2;
            this.buttonVhdAdd.Text = "&Add";
            this.buttonVhdAdd.UseVisualStyleBackColor = true;
            this.buttonVhdAdd.Click += new System.EventHandler(this.buttonVhdAdd_Click);
            // 
            // listAutoAttach
            // 
            this.listAutoAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listAutoAttach.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFileName});
            this.listAutoAttach.FullRowSelect = true;
            this.listAutoAttach.GridLines = true;
            this.listAutoAttach.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listAutoAttach.HideSelection = false;
            this.listAutoAttach.Location = new System.Drawing.Point(4, 22);
            this.listAutoAttach.Margin = new System.Windows.Forms.Padding(2, 7, 2, 2);
            this.listAutoAttach.MultiSelect = false;
            this.listAutoAttach.Name = "listAutoAttach";
            this.listAutoAttach.ShowItemToolTips = true;
            this.listAutoAttach.Size = new System.Drawing.Size(247, 137);
            this.listAutoAttach.SmallImageList = this.imagesAutoAttach;
            this.listAutoAttach.TabIndex = 0;
            this.listAutoAttach.UseCompatibleStateImageBehavior = false;
            this.listAutoAttach.View = System.Windows.Forms.View.Details;
            this.listAutoAttach.SelectedIndexChanged += new System.EventHandler(this.listAutoAttach_SelectedIndexChanged);
            this.listAutoAttach.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.listAutoAttach_PreviewKeyDown);
            // 
            // columnFileName
            // 
            this.columnFileName.Text = "File name";
            this.columnFileName.Width = 200;
            // 
            // imagesAutoAttach
            // 
            this.imagesAutoAttach.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagesAutoAttach.ImageStream")));
            this.imagesAutoAttach.TransparentColor = System.Drawing.Color.Transparent;
            this.imagesAutoAttach.Images.SetKeyName(0, "StatusInformation [16x16].png");
            this.imagesAutoAttach.Images.SetKeyName(1, "StatusWarning [16x16].png");
            this.imagesAutoAttach.Images.SetKeyName(2, "Status_SeriousWarning_16.png");
            this.imagesAutoAttach.Images.SetKeyName(3, "StatusError [16x16].png");
            this.imagesAutoAttach.Images.SetKeyName(4, "Lock (16x16).png");
            // 
            // btnRegisterExtension
            // 
            this.btnRegisterExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRegisterExtension.Location = new System.Drawing.Point(9, 288);
            this.btnRegisterExtension.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRegisterExtension.Name = "btnRegisterExtension";
            this.btnRegisterExtension.Size = new System.Drawing.Size(112, 23);
            this.btnRegisterExtension.TabIndex = 5;
            this.btnRegisterExtension.Text = "&Register extension";
            this.btnRegisterExtension.UseVisualStyleBackColor = true;
            this.btnRegisterExtension.Visible = false;
            this.btnRegisterExtension.Click += new System.EventHandler(this.btnRegisterExtension_Click);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(296, 321);
            this.Controls.Add(this.btnRegisterExtension);
            this.Controls.Add(this.groupAutoAttach);
            this.Controls.Add(this.groupContextMenu);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.Resize += new System.EventHandler(this.SettingsForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.erp)).EndInit();
            this.groupContextMenu.ResumeLayout(false);
            this.groupContextMenu.PerformLayout();
            this.groupAutoAttach.ResumeLayout(false);
            this.toolVhdOrder.ResumeLayout(false);
            this.toolVhdOrder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ErrorProvider erp;
        private System.Windows.Forms.GroupBox groupContextMenu;
        private System.Windows.Forms.CheckBox checkDetachDrive;
        private System.Windows.Forms.CheckBox checkDetach;
        private System.Windows.Forms.CheckBox checkAttach;
        private System.Windows.Forms.GroupBox groupAutoAttach;
        private System.Windows.Forms.Button buttonVhdRemove;
        private System.Windows.Forms.Button buttonVhdAdd;
        private System.Windows.Forms.ListView listAutoAttach;
        private System.Windows.Forms.ColumnHeader columnFileName;
        private System.Windows.Forms.ToolStrip toolVhdOrder;
        private System.Windows.Forms.ToolStripButton buttonMoveVhdUp;
        private System.Windows.Forms.ToolStripButton buttonMoveVhdDown;
        private System.Windows.Forms.ImageList imagesAutoAttach;
        private System.Windows.Forms.CheckBox checkAttachReadOnly;
        private System.Windows.Forms.Button btnRegisterExtension;
        private System.Windows.Forms.ToolStripButton buttonVhdReadOnly;
    }
}