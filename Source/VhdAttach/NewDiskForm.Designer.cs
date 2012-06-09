namespace VhdAttach {
    partial class NewDiskForm {
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
            this.lblSize = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chbThousandSize = new System.Windows.Forms.CheckBox();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.radFixed = new System.Windows.Forms.RadioButton();
            this.radDynamic = new System.Windows.Forms.RadioButton();
            this.lblSizeInBytes = new System.Windows.Forms.Label();
            this.txtSizeInBytes = new System.Windows.Forms.TextBox();
            this.erpError = new System.Windows.Forms.ErrorProvider(this.components);
            this.cmbSizeUnit = new System.Windows.Forms.ComboBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.grpType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.erpError)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(9, 15);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(39, 17);
            this.lblSize.TabIndex = 0;
            this.lblSize.Text = "Size:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(68, 196);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "Create";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(174, 196);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chbThousandSize
            // 
            this.chbThousandSize.AutoSize = true;
            this.chbThousandSize.Location = new System.Drawing.Point(113, 42);
            this.chbThousandSize.Name = "chbThousandSize";
            this.chbThousandSize.Size = new System.Drawing.Size(106, 21);
            this.chbThousandSize.TabIndex = 3;
            this.chbThousandSize.Text = "1000-based";
            this.chbThousandSize.UseVisualStyleBackColor = true;
            this.chbThousandSize.CheckedChanged += new System.EventHandler(this.control_Changed);
            // 
            // grpType
            // 
            this.grpType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpType.Controls.Add(this.radFixed);
            this.grpType.Controls.Add(this.radDynamic);
            this.grpType.Location = new System.Drawing.Point(12, 97);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(262, 81);
            this.grpType.TabIndex = 6;
            this.grpType.TabStop = false;
            this.grpType.Text = "Format";
            // 
            // radFixed
            // 
            this.radFixed.AutoSize = true;
            this.radFixed.Location = new System.Drawing.Point(9, 54);
            this.radFixed.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.radFixed.Name = "radFixed";
            this.radFixed.Size = new System.Drawing.Size(91, 21);
            this.radFixed.TabIndex = 1;
            this.radFixed.TabStop = true;
            this.radFixed.Text = "Fixed size";
            this.radFixed.UseVisualStyleBackColor = true;
            // 
            // radDynamic
            // 
            this.radDynamic.AutoSize = true;
            this.radDynamic.Checked = true;
            this.radDynamic.Location = new System.Drawing.Point(9, 27);
            this.radDynamic.Margin = new System.Windows.Forms.Padding(6, 9, 3, 3);
            this.radDynamic.Name = "radDynamic";
            this.radDynamic.Size = new System.Drawing.Size(173, 21);
            this.radDynamic.TabIndex = 0;
            this.radDynamic.TabStop = true;
            this.radDynamic.Text = "Dynamically expanding";
            this.radDynamic.UseVisualStyleBackColor = true;
            // 
            // lblSizeInBytes
            // 
            this.lblSizeInBytes.AutoSize = true;
            this.lblSizeInBytes.Location = new System.Drawing.Point(12, 72);
            this.lblSizeInBytes.Name = "lblSizeInBytes";
            this.lblSizeInBytes.Size = new System.Drawing.Size(92, 17);
            this.lblSizeInBytes.TabIndex = 4;
            this.lblSizeInBytes.Text = "Size in bytes:";
            // 
            // txtSizeInBytes
            // 
            this.txtSizeInBytes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSizeInBytes.Location = new System.Drawing.Point(113, 69);
            this.txtSizeInBytes.Name = "txtSizeInBytes";
            this.txtSizeInBytes.ReadOnly = true;
            this.txtSizeInBytes.Size = new System.Drawing.Size(161, 22);
            this.txtSizeInBytes.TabIndex = 5;
            this.txtSizeInBytes.TabStop = false;
            this.txtSizeInBytes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // erpError
            // 
            this.erpError.ContainerControl = this;
            // 
            // cmbSizeUnit
            // 
            this.cmbSizeUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSizeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSizeUnit.Items.AddRange(new object[] {
            "MB",
            "GB"});
            this.cmbSizeUnit.Location = new System.Drawing.Point(219, 12);
            this.cmbSizeUnit.Name = "cmbSizeUnit";
            this.cmbSizeUnit.Size = new System.Drawing.Size(55, 24);
            this.cmbSizeUnit.TabIndex = 2;
            this.cmbSizeUnit.SelectedIndexChanged += new System.EventHandler(this.cmbSizeUnit_SelectedIndexChanged);
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(113, 13);
            this.txtSize.MaxLength = 8;
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(100, 22);
            this.txtSize.TabIndex = 1;
            this.txtSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSize.TextChanged += new System.EventHandler(this.control_Changed);
            this.txtSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSize_KeyDown);
            this.txtSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSize_KeyPress);
            // 
            // NewDiskForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(286, 236);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.cmbSizeUnit);
            this.Controls.Add(this.txtSizeInBytes);
            this.Controls.Add(this.lblSizeInBytes);
            this.Controls.Add(this.grpType);
            this.Controls.Add(this.chbThousandSize);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewDiskForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New virtual disk";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
            this.Load += new System.EventHandler(this.Form_Load);
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.erpError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbThousandSize;
        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.RadioButton radFixed;
        private System.Windows.Forms.RadioButton radDynamic;
        private System.Windows.Forms.Label lblSizeInBytes;
        private System.Windows.Forms.TextBox txtSizeInBytes;
        private System.Windows.Forms.ErrorProvider erpError;
        private System.Windows.Forms.ComboBox cmbSizeUnit;
        private System.Windows.Forms.TextBox txtSize;
    }
}