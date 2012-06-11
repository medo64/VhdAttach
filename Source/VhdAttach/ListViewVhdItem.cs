using System;
using System.IO;
using System.Windows.Forms;
using VhdAttachCommon;

namespace VhdAttach {
    internal class ListViewVhdItem : ListViewItem {

        public ListViewVhdItem(FileWithOptions fwo) {
            this.FileName = fwo.FileName;
            this.IsReadOnly = fwo.ReadOnly;
            this.HasNoDriveLetter = fwo.NoDriveLetter;

            try {
                var file = new FileInfo(this.FileName);
                base.Text = file.Name;

                try {
                    if (file.Exists) {
                        if (file.IsReadOnly) {
                            this.ToolTipText = "File is read-only." + Environment.NewLine + this.FileName;
                            this.ImageIndex = 1;
                        } else {
                            this.ToolTipText = this.FileName;
                            this.ImageIndex = (this.IsReadOnly) ? 4 : 0;
                        }
                    } else {
                        this.ToolTipText = "File not found." + Environment.NewLine + this.FileName;
                        this.ImageIndex = 2;
                    }
                } catch (Exception ex) {
                    this.ToolTipText = ex.Message + Environment.NewLine + this.FileName;
                    this.ImageIndex = 3;
                }
            } catch (ArgumentException) {
                var segments = this.FileName.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0) {
                    base.Text = segments[segments.Length - 1];
                } else {
                    base.Text = this.FileName;
                }
                this.ToolTipText = "Cannot parse file name \"" + this.FileName + "\"";
                this.ImageIndex = 3;
            }

        }


        public string FileName { get; private set; }

        private bool _isReadOnly;
        public bool IsReadOnly {
            get { return this._isReadOnly; }
            set {
                this._isReadOnly = value;
                if ((this.ImageIndex == 0) || (this.ImageIndex == 4)) {
                    this.ImageIndex = (value) ? 4 : 0;
                }
            }
        }

        public bool HasNoDriveLetter { get; private set; }


        public string GetSettingFileName() {
            var fwo = new FileWithOptions(this.FileName);
            fwo.ReadOnly = this.IsReadOnly;
            fwo.NoDriveLetter = this.HasNoDriveLetter;
            return fwo.ToString();
        }


        public override string ToString() {
            return this.GetSettingFileName();
        }
    }
}
