using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VhdAttach {
    internal partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
            toolVhdOrder.Renderer = new ToolStripBorderlessSystemRenderer();
            this.Font = SystemFonts.MessageBoxFont;
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            checkAttach.Checked = ServiceSettings.ContextMenuAttach;
            checkAttachReadOnly.Checked = ServiceSettings.ContextMenuAttachReadOnly;
            checkDetach.Checked = ServiceSettings.ContextMenuDetach;
            checkDetachDrive.Checked = ServiceSettings.ContextMenuDetachDrive;
            foreach (var fileName in ServiceSettings.AutoAttachVhdList) {
                listAutoAttach.Items.Add(new ListViewVhdItem(fileName));
            }
            btnRegisterExtension.Visible = !ServiceSettings.ContextMenu;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            try {
                this.Cursor = Cursors.WaitCursor;

                var vhds = new List<string>();
                foreach (ListViewVhdItem item in listAutoAttach.Items) {
                    vhds.Add(item.FileName);
                }
                var res = PipeClient.WriteSettings(checkAttach.Checked, checkAttachReadOnly.Checked, checkDetach.Checked, checkDetachDrive.Checked, vhds.ToArray());
                if (res.ExitCode != ExitCodes.OK) {
                    Medo.MessageBox.ShowError(this, res.Message);
                }
            } finally {
                this.Cursor = Cursors.Default;
            }
        }


        private void SettingsForm_Resize(object sender, EventArgs e) {
            listAutoAttach.Columns[0].Width = listAutoAttach.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
        }

        private void buttonMoveVhdUp_Click(object sender, EventArgs e) {
            if (listAutoAttach.FocusedItem != null) {
                if (listAutoAttach.FocusedItem.Index > 0) {
                    listAutoAttach.BeginUpdate();
                    var index = listAutoAttach.FocusedItem.Index;
                    var item = listAutoAttach.FocusedItem;
                    listAutoAttach.Items.RemoveAt(index);
                    listAutoAttach.Items.Insert(index - 1, item);
                    item.Focused = true;
                    item.Selected = true;
                    listAutoAttach.EnsureVisible(item.Index);
                    listAutoAttach.EndUpdate();
                }
            }
        }

        private void buttonMoveVhdDown_Click(object sender, EventArgs e) {
            if (listAutoAttach.FocusedItem != null) {
                if (listAutoAttach.FocusedItem.Index < (listAutoAttach.Items.Count - 1)) {
                    listAutoAttach.BeginUpdate();
                    var index = listAutoAttach.FocusedItem.Index;
                    var item = listAutoAttach.FocusedItem;
                    listAutoAttach.Items.RemoveAt(index);
                    listAutoAttach.Items.Insert(index + 1, item);
                    item.Focused = true;
                    item.Selected = true;
                    listAutoAttach.EnsureVisible(item.Index);
                    listAutoAttach.EndUpdate();
                }
            }
        }

        private void listAutoAttach_SelectedIndexChanged(object sender, EventArgs e) {
            if (listAutoAttach.FocusedItem != null) {
                buttonVhdRemove.Enabled = true;
                buttonMoveVhdUp.Enabled = (listAutoAttach.FocusedItem.Index > 0);
                buttonMoveVhdDown.Enabled = (listAutoAttach.FocusedItem.Index < (listAutoAttach.Items.Count - 1));
            } else {
                buttonVhdRemove.Enabled = false;
                buttonMoveVhdUp.Enabled = false;
                buttonMoveVhdDown.Enabled = false;
            }
        }

        private void listAutoAttach_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyData) {
                case Keys.Alt | Keys.Up: {
                        buttonMoveVhdUp_Click(null, null);
                        listAutoAttach_SelectedIndexChanged(null, null);
                        e.IsInputKey = false;
                    } break;
                case Keys.Alt | Keys.Down: {
                        buttonMoveVhdDown_Click(null, null);
                        listAutoAttach_SelectedIndexChanged(null, null);
                        e.IsInputKey = false;
                    } break;
            }
        }

        private void buttonVhdAdd_Click(object sender, EventArgs e) {
            using (var dialog = new OpenFileDialog()) {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = true;
                dialog.ShowReadOnly = false;
                dialog.Filter = "Virtual Disk (*.vhd)|*.vhd|All files (*.*)|*.*";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    listAutoAttach.BeginUpdate();
                    ListViewItem item = null;
                    foreach (var file in dialog.FileNames) {
                        ListViewItem duplicate = null;
                        foreach (ListViewVhdItem oldItem in listAutoAttach.Items) {
                            if (string.Compare(file, oldItem.FileName, StringComparison.CurrentCultureIgnoreCase) == 0) {
                                duplicate = oldItem;
                                break;
                            }
                        }
                        if (duplicate == null) {
                            item = new ListViewVhdItem(file);
                            listAutoAttach.Items.Add(item);
                        } else {
                            item = duplicate;
                        }
                    }
                    if (item != null) {
                        item.Focused = true;
                        item.Selected = true;
                        listAutoAttach.EnsureVisible(item.Index);
                    }
                    listAutoAttach.EndUpdate();
                }
            }
        }

        private void buttonVhdRemove_Click(object sender, EventArgs e) {
            if (listAutoAttach.FocusedItem != null) {
                listAutoAttach.BeginUpdate();
                var index = listAutoAttach.FocusedItem.Index;
                listAutoAttach.Items.RemoveAt(index);
                if (listAutoAttach.Items.Count > 0) {
                    ListViewItem item;
                    if (index > (listAutoAttach.Items.Count - 1)) {
                        item = listAutoAttach.Items[listAutoAttach.Items.Count - 1];
                    } else {
                        item = listAutoAttach.Items[index];
                    }
                    item.Focused = true;
                    item.Selected = true;
                    listAutoAttach.EnsureVisible(item.Index);
                }
                listAutoAttach.EndUpdate();
                listAutoAttach_SelectedIndexChanged(null, null);
            }
        }

        private void btnRegisterExtension_Click(object sender, EventArgs e) {
            var res = PipeClient.RegisterExtension();
            if (res.ExitCode != ExitCodes.OK) {
                Medo.MessageBox.ShowError(this, res.Message);
            }
            this.DialogResult = DialogResult.OK;
        }

    }
}
