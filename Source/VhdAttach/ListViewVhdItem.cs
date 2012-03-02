using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VhdAttach {
    internal class ListViewVhdItem : ListViewItem {

        public ListViewVhdItem(string fileName) {
            if (fileName.StartsWith("/")) {
                /*
                 * Each file can have additional settings area that starts with / and ends with next /.
                 * E.g. "/readonly,nodriveletter/D:\Test.vhd"
                 */
                var iEndPipe = fileName.IndexOf("/", 1);
                var additionalSettings = fileName.Substring(1, iEndPipe - 1);
                this.FileName = fileName.Substring(iEndPipe + 1);
                foreach (var setting in additionalSettings.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    switch (additionalSettings.ToUpperInvariant()) {
                        case "READONLY": this.IsReadOnly = true; break;
                        case "NODRIVELETTER": this.HasNoDriveLetter = true; break;
                    }
                }
            } else {
                this.FileName = fileName;
            }

            try {
                var file = new FileInfo(this.FileName);
                base.Text = file.Name;

                try {
                    if (file.Exists) {
                        if (file.IsReadOnly) {
                            this.ToolTipText = "File is read-only." + Environment.NewLine + fileName;
                            this.ImageIndex = 1;
                        } else {
                            this.ToolTipText = fileName;
                            this.ImageIndex = (this.IsReadOnly) ? 4 : 0;
                        }
                    } else {
                        this.ToolTipText = "File not found." + Environment.NewLine + fileName;
                        this.ImageIndex = 2;
                    }
                } catch (Exception ex) {
                    this.ToolTipText = ex.Message + Environment.NewLine + fileName;
                    this.ImageIndex = 3;
                }
            } catch (ArgumentException) {
                var segments = fileName.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0) {
                    base.Text = segments[segments.Length - 1];
                } else {
                    base.Text = fileName;
                }
                this.ToolTipText = "Cannot parse file name \"" + fileName + "\"";
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
            var additionalSettings = new List<string>();
            if (this.IsReadOnly) { additionalSettings.Add("readonly"); }
            if (this.HasNoDriveLetter) { additionalSettings.Add("nodriveletter"); }

            var sb = new StringBuilder();
            if (additionalSettings.Count > 0) {
                sb.Append("/");
                sb.Append(string.Join(",", additionalSettings.ToArray()));
                sb.Append("/");
            }
            sb.Append(this.FileName);
            return sb.ToString();
        }


        public override string ToString() {
            return this.GetSettingFileName();
        }
    }
}
