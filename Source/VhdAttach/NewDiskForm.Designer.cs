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
            this.lblSize = new System.Windows.Forms.Label();
            this.dudSizeUnit = new System.Windows.Forms.DomainUpDown();
            this.nudSize = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chbThousandSize = new System.Windows.Forms.CheckBox();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.radFixed = new System.Windows.Forms.RadioButton();
            this.radDynamic = new System.Windows.Forms.RadioButton();
            this.lblSizeInBytes = new System.Windows.Forms.Label();
            this.txtSizeInBytes = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).BeginInit();
            this.grpType.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(12, 14);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(39, 17);
            this.lblSize.TabIndex = 0;
            this.lblSize.Text = "Size:";
            // 
            // dudSizeUnit
            // 
            this.dudSizeUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dudSizeUnit.Items.Add("MB");
            this.dudSizeUnit.Items.Add("GB");
            this.dudSizeUnit.Location = new System.Drawing.Point(222, 12);
            this.dudSizeUnit.Name = "dudSizeUnit";
            this.dudSizeUnit.ReadOnly = true;
            this.dudSizeUnit.Size = new System.Drawing.Size(60, 22);
            this.dudSizeUnit.TabIndex = 2;
            this.dudSizeUnit.Text = "MB";
            this.dudSizeUnit.SelectedItemChanged += new System.EventHandler(this.control_Changed);
            // 
            // nudSize
            // 
            this.nudSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSize.Location = new System.Drawing.Point(112, 12);
            this.nudSize.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSize.Name = "nudSize";
            this.nudSize.Size = new System.Drawing.Size(104, 22);
            this.nudSize.TabIndex = 1;
            this.nudSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSize.ValueChanged += new System.EventHandler(this.control_Changed);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(76, 194);
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
            this.btnCancel.Location = new System.Drawing.Point(182, 194);
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
            this.chbThousandSize.Location = new System.Drawing.Point(112, 40);
            this.chbThousandSize.Name = "chbThousandSize";
            this.chbThousandSize.Size = new System.Drawing.Size(106, 21);
            this.chbThousandSize.TabIndex = 3;
            this.chbThousandSize.Text = "1000-based";
            this.chbThousandSize.UseVisualStyleBackColor = true;
            this.chbThousandSize.CheckedChanged += new System.EventHandler(this.control_Changed);
            // 
            // grpType
            // 
            this.grpType.Controls.Add(this.radFixed);
            this.grpType.Controls.Add(this.radDynamic);
            this.grpType.Location = new System.Drawing.Point(12, 95);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(270, 81);
            this.grpType.TabIndex = 6;
            this.grpType.TabStop = false;
            this.grpType.Text = "Format";
            // 
            // radFixed
            // 
            this.radFixed.AutoSize = true;
            this.radFixed.Enabled = false;
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
            this.lblSizeInBytes.Location = new System.Drawing.Point(12, 70);
            this.lblSizeInBytes.Name = "lblSizeInBytes";
            this.lblSizeInBytes.Size = new System.Drawing.Size(92, 17);
            this.lblSizeInBytes.TabIndex = 4;
            this.lblSizeInBytes.Text = "Size in bytes:";
            // 
            // txtSizeInBytes
            // 
            this.txtSizeInBytes.Location = new System.Drawing.Point(112, 67);
            this.txtSizeInBytes.Name = "txtSizeInBytes";
            this.txtSizeInBytes.ReadOnly = true;
            this.txtSizeInBytes.Size = new System.Drawing.Size(170, 22);
            this.txtSizeInBytes.TabIndex = 5;
            this.txtSizeInBytes.TabStop = false;
            this.txtSizeInBytes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NewDiskForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(294, 234);
            this.Controls.Add(this.txtSizeInBytes);
            this.Controls.Add(this.lblSizeInBytes);
            this.Controls.Add(this.grpType);
            this.Controls.Add(this.chbThousandSize);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.nudSize);
            this.Controls.Add(this.dudSizeUnit);
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
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).EndInit();
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.DomainUpDown dudSizeUnit;
        private System.Windows.Forms.NumericUpDown nudSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbThousandSize;
        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.RadioButton radFixed;
        private System.Windows.Forms.RadioButton radDynamic;
        private System.Windows.Forms.Label lblSizeInBytes;
        private System.Windows.Forms.TextBox txtSizeInBytes;
    }
}