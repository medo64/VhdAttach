using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VhdAttachCommon;

namespace VhdAttach {
    internal partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
            toolVhdOrder.Renderer = new ToolStripBorderlessSystemRenderer();
            this.Font = SystemFonts.MessageBoxFont;
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            var isWindows8 = ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) >= 6000002); //show if equal to or higher than Windows 8
            checkVhdDetachDrive.Visible = !(isWindows8);
            checkIsoOpen.Visible = isWindows8;
            checkIsoAttachReadOnly.Visible = isWindows8;
            checkIsoDetach.Visible = isWindows8;

            checkVhdOpen.Checked = ServiceSettings.ContextMenuVhdOpen;
            checkVhdAttach.Checked = ServiceSettings.ContextMenuVhdAttach;
            checkVhdAttachReadOnly.Checked = ServiceSettings.ContextMenuVhdAttachReadOnly;
            checkVhdDetach.Checked = ServiceSettings.ContextMenuVhdDetach;
            checkVhdDetachDrive.Checked = ServiceSettings.ContextMenuVhdDetachDrive;
            checkIsoOpen.Checked = ServiceSettings.ContextMenuIsoOpen;
            checkIsoAttachReadOnly.Checked = ServiceSettings.ContextMenuIsoAttachReadOnly;
            checkIsoDetach.Checked = ServiceSettings.ContextMenuIsoDetach;
            foreach (var fwo in ServiceSettings.AutoAttachVhdList) {
                listAutoAttach.Items.Add(new ListViewVhdItem(fwo));
            }
            btnRegisterExtensionVhd.Visible = !ServiceSettings.ContextMenuVhd;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            try {
                this.Cursor = Cursors.WaitCursor;

                var vhds = new List<string>();
                foreach (ListViewVhdItem item in listAutoAttach.Items) {
                    vhds.Add(item.GetSettingFileName());
                }
                var resVhd = PipeClient.WriteContextMenuVhdSettings(checkVhdOpen.Checked, checkVhdAttach.Checked, checkVhdAttachReadOnly.Checked, checkVhdDetach.Checked, checkVhdDetachDrive.Checked);
                if (resVhd.IsError) {
                    Medo.MessageBox.ShowError(this, resVhd.Message);
                }

                var isWindows8 = ((Environment.OSVersion.Version.Major * 1000000 + Environment.OSVersion.Version.Minor) >= 6000002); //show if equal to or higher than Windows 8
                if (isWindows8) {
                    var resIso = PipeClient.WriteContextMenuIsoSettings(checkIsoOpen.Checked, checkIsoAttachReadOnly.Checked, checkIsoDetach.Checked);
                    if (resIso.IsError) {
                        Medo.MessageBox.ShowError(this, resIso.Message);
                    }
                }
                var resAA = PipeClient.WriteAutoAttachSettings(vhds.ToArray());
                if (resAA.IsError) {
                    Medo.MessageBox.ShowError(this, resAA.Message);
                }
            } catch (IOException ex) {
                Messages.ShowServiceIOException(this, ex);
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

        private void buttonVhdReadOnly_Click(object sender, EventArgs e) {
            if (listAutoAttach.FocusedItem != null) {
                var item = (ListViewVhdItem)listAutoAttach.FocusedItem;
                item.IsReadOnly = !item.IsReadOnly;
            }
        }

        private void listAutoAttach_SelectedIndexChanged(object sender, EventArgs e) {
            if (listAutoAttach.FocusedItem != null) {
                buttonVhdRemove.Enabled = true;
                buttonMoveVhdUp.Enabled = (listAutoAttach.FocusedItem.Index > 0);
                buttonMoveVhdDown.Enabled = (listAutoAttach.FocusedItem.Index < (listAutoAttach.Items.Count - 1));
                buttonVhdReadOnly.Enabled = true;
                buttonVhdReadOnly.Checked = ((ListViewVhdItem)(listAutoAttach.FocusedItem)).IsReadOnly;
            } else {
                buttonVhdRemove.Enabled = false;
                buttonMoveVhdUp.Enabled = false;
                buttonMoveVhdDown.Enabled = false;
                buttonVhdReadOnly.Enabled = false;
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
                            item = new ListViewVhdItem(new FileWithOptions(file));
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

        private void btnRegisterExtensionVhd_Click(object sender, EventArgs e) {
            var res = PipeClient.RegisterExtensionVhd();
            if (res.IsError) {
                Medo.MessageBox.ShowError(this, res.Message);
            }
            this.DialogResult = DialogResult.OK;
        }

    }
}
