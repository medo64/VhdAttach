using System;
using System.IO;
using System.Windows.Forms;

namespace VhdAttach {
    internal class ListViewVhdItem : ListViewItem {

        public ListViewVhdItem(string fileName) {
            this.FileName = fileName;

            try {
                var file = new FileInfo(fileName);
                base.Text = file.Name;

                try {
                    if (file.Exists) {
                        if (file.IsReadOnly) {
                            this.ToolTipText = "File is read-only." + Environment.NewLine + fileName;
                            this.ImageIndex = 1;
                        } else {
                            this.ToolTipText = fileName;
                            this.ImageIndex = 0;
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

    }
}
