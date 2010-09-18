//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2008-01-02: New version.
//2008-01-05: Top line now contains product name.
//2008-01-22: Changed caption to "About" instead of "About...".
//2008-01-25: Added product title parameter.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (NormalizeStringsToUppercase, SpecifyMarshalingForPInvokeStringArguments).
//2008-11-05: Refactoring (Microsoft.Maintainability : 'AboutBox.ShowDialog(IWin32Window, Uri, string)' has a cyclomatic complexity of 27, Microsoft.Maintainability : 'AboutBox.ShowDialog(IWin32Window, Uri, string)' is coupled with 38 different types from 10 different namespaces.).
//2008-12-20: Adjusted for high DPI mode.
//2009-10-25: Adjusted disposing of buttons.


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Simple about form.
    /// </summary>
    public static class AboutBox {

        private static readonly object _syncRoot = new object();
        private static int _titleHeight;
        private static PaintItem _paintImage;
        private static PaintItem _paintProduct;
        private static List<PaintItem> _infoLines;


        /// <summary>
        /// Shows modal dialog.
        /// </summary>
        public static DialogResult ShowDialog() {
            lock (_syncRoot) {
                return ShowDialog(null, null, null);
            }
        }

        /// <summary>
        /// Shows modal dialog.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        public static DialogResult ShowDialog(IWin32Window owner) {
            lock (_syncRoot) {
                return ShowDialog(owner, null, null);
            }
        }

        /// <summary>
        /// Shows modal dialog.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        /// <param name="webpage">URI of program's web page.</param>
        public static DialogResult ShowDialog(IWin32Window owner, Uri webpage) {
            return ShowDialog(owner, webpage, null);
        }

        /// <summary>
        /// Shows modal dialog.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        /// <param name="webpage">URI of program's web page.</param>
        /// <param name="productText">Title to use. If null, title will be provided from assembly info.</param>
        public static DialogResult ShowDialog(IWin32Window owner, Uri webpage, string productText) {
            lock (_syncRoot) {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                AssemblyName assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName();

                string title = GetAppTitleText(assembly);
                if (productText == null) { productText = GetAppProductText(assembly, title); }
                string versionText = assemblyName.Version.ToString();
                string copyrightText = GetAppCopyright(assembly);
                string applicationPath = Assembly.GetEntryAssembly().Location;


                ShowForm(owner, webpage, productText, title, versionText, copyrightText, applicationPath);

                return DialogResult.OK;
            }
        }

        private static void ShowForm(IWin32Window owner, Uri webpage, string productText, string title, string versionText, string copyrightText, string applicationPath) {
            using (Form form = new Form()) {
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ShowIcon = false;
                form.ShowInTaskbar = false;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AutoSize = false;
                form.AutoScaleMode = AutoScaleMode.None;
                form.Text = Resources.Caption;


                int imageHeight = 32;
                int maxRight = 0;
                int maxBottom = 0;
                using (Graphics graphics = form.CreateGraphics()) {
                    //icon
                    Bitmap bitmap = GetAppIcon(applicationPath);
                    if (bitmap != null) {
                        _paintImage = new PaintItem(bitmap, new Point(7, 7));
                    }

                    //title
                    int imageRight = 7;
                    if (_paintImage != null) {
                        imageRight = _paintImage.Rectangle.Right + 7;
                    }
                    if (_paintImage != null) {
                        imageHeight = _paintImage.Rectangle.Height;
                    }
                    _paintProduct = new PaintItem(productText, new Font(SystemFonts.MessageBoxFont.Name, imageHeight, System.Drawing.SystemFonts.MessageBoxFont.Style, System.Drawing.GraphicsUnit.Pixel, System.Drawing.SystemFonts.MessageBoxFont.GdiCharSet), imageRight, 7, imageHeight, VerticalAlignment.Center, graphics);

                    _titleHeight = 7 + imageHeight + 7;
                    maxRight = _paintProduct.Rectangle.Right;
                    maxBottom = _titleHeight;


                    //other stuff
                    _infoLines = new List<PaintItem>();

                    PaintItem fullName = new PaintItem(title + " " + versionText, new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.Size * 1.5f, SystemFonts.MessageBoxFont.Style, SystemFonts.MessageBoxFont.Unit), 7, _titleHeight + 2 + 7, 0, VerticalAlignment.Top, graphics);
                    maxRight = System.Math.Max(maxRight, fullName.Rectangle.Right);
                    maxBottom = System.Math.Max(maxBottom, fullName.Rectangle.Bottom);
                    _infoLines.Add(fullName);

                    PaintItem dotNetFramework = new PaintItem(".NET framework " + Environment.Version.ToString(), SystemFonts.MessageBoxFont, 7, fullName.Rectangle.Bottom + 7, 0, VerticalAlignment.Top, graphics);
                    maxRight = System.Math.Max(maxRight, dotNetFramework.Rectangle.Right);
                    maxBottom = System.Math.Max(maxBottom, dotNetFramework.Rectangle.Bottom);
                    _infoLines.Add(dotNetFramework);

                    PaintItem osVersion = new PaintItem(System.Environment.OSVersion.VersionString, SystemFonts.MessageBoxFont, 7, dotNetFramework.Rectangle.Bottom, 0, VerticalAlignment.Top, graphics);
                    maxRight = System.Math.Max(maxRight, osVersion.Rectangle.Right);
                    maxBottom = System.Math.Max(maxBottom, osVersion.Rectangle.Bottom);
                    _infoLines.Add(osVersion);

                    if (copyrightText != null) {
                        PaintItem copyright = new PaintItem(copyrightText, SystemFonts.MessageBoxFont, 7, osVersion.Rectangle.Bottom + 7, 0, VerticalAlignment.Top, graphics);
                        maxRight = System.Math.Max(maxRight, copyright.Rectangle.Right);
                        maxBottom = System.Math.Max(maxBottom, copyright.Rectangle.Bottom);
                        _infoLines.Add(copyright);
                    }
                }

                int buttonMinRight = 7;

                //Close button
                Button buttonClose = new Button();
                buttonClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
                buttonClose.AutoSize = true;
                buttonClose.DialogResult = DialogResult.OK;
                buttonClose.Text = Resources.Close;
                form.Controls.Add(buttonClose);
                buttonMinRight += buttonClose.Width + 11;

                //Readme button
                Button buttonReadme = null;
                string readMePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "readme.txt");
                if (System.IO.File.Exists(readMePath)) {
                    buttonReadme = new Button();
                    buttonReadme.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                    buttonReadme.AutoSize = true;
                    buttonReadme.Text = Resources.ReadMe;
                    buttonReadme.Tag = readMePath;
                    buttonReadme.Click += new System.EventHandler(buttonReadme_Click);
                    form.Controls.Add(buttonReadme);
                    buttonMinRight += buttonReadme.Width + 7;
                }

                //WebPage button
                Button buttonWebPage = null;
                if (webpage != null) {
                    buttonWebPage = new Button();
                    buttonWebPage.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                    buttonWebPage.AutoSize = true;
                    buttonWebPage.Text = Resources.WebPage;
                    buttonWebPage.Tag = webpage.ToString();
                    buttonWebPage.Click += new System.EventHandler(buttonWebPage_Click);
                    form.Controls.Add(buttonWebPage);
                    buttonMinRight += buttonWebPage.Width + 7;
                }

                maxRight = System.Math.Max(maxRight, buttonMinRight);


                int borderX = (form.Width - form.ClientRectangle.Width);
                int borderY = (form.Height - form.ClientRectangle.Height);
                form.Width = borderX + maxRight + 7;
                form.Height = borderY + maxBottom + 11 + buttonClose.Size.Height + 7;
                if (owner == null) {
                    form.StartPosition = FormStartPosition.CenterScreen;
                } else {
                    form.StartPosition = FormStartPosition.CenterParent;
                }


                int buttonLeft = form.ClientRectangle.Left + 7;

                if (buttonReadme != null) {
                    buttonReadme.Location = new Point(buttonLeft, form.ClientRectangle.Bottom - buttonClose.Height - 7);
                    buttonLeft += buttonReadme.Width + 7;
                }

                if (buttonWebPage != null) {
                    buttonWebPage.Location = new Point(buttonLeft, form.ClientRectangle.Bottom - buttonClose.Height - 7);
                    buttonLeft += buttonWebPage.Width + 7;
                }

                buttonClose.Location = new Point(form.ClientRectangle.Right - buttonClose.Width - 7, form.ClientRectangle.Bottom - buttonClose.Height - 7);



                form.AcceptButton = buttonClose;
                form.CancelButton = buttonClose;

                form.Paint += Form_Paint;

                if (owner != null) {
                    Form formOwner = owner as Form;
                    if ((formOwner != null) && (formOwner.TopMost == true)) {
                        form.TopMost = false;
                        form.TopMost = true;
                    }
                    form.ShowDialog(owner);
                } else {
                    form.ShowDialog();
                }

                buttonClose.Dispose();
                if (buttonReadme != null) { buttonReadme.Dispose(); }
                if (buttonWebPage != null) { buttonWebPage.Dispose(); }
            }

            if (_paintImage != null) { _paintImage.Dispose(); }
            if (_paintProduct != null) { _paintProduct.Dispose(); }
            if (_infoLines != null) {
                for (int i = 0; i < _infoLines.Count; ++i) {
                    _infoLines[i].Dispose();
                }
                _infoLines = null;
            }
        }

        private static string GetAppCopyright(Assembly assembly) {
            object[] copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if ((copyrightAttributes != null) && (copyrightAttributes.Length >= 1)) {
                return ((AssemblyCopyrightAttribute)copyrightAttributes[copyrightAttributes.Length - 1]).Copyright;
            }
            return null;
        }

        private static string GetAppProductText(Assembly assembly, string title) {
            object[] productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                return ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
            } else {
                return title;
            }
        }

        private static string GetAppTitleText(Assembly assembly) {
            object[] titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                return ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
            } else {
                return assembly.GetName().Name;
            }
        }


        static void buttonWebPage_Click(object sender, System.EventArgs e) {
            try {
                string url = (string)((Control)sender).Tag;
                System.Diagnostics.Process.Start(url);
            } catch (System.ComponentModel.Win32Exception) { }
        }

        static void buttonReadme_Click(object sender, System.EventArgs e) {
            try {
                string path = (string)((Control)sender).Tag;
                System.Diagnostics.Process.Start(path);
            } catch (System.ComponentModel.Win32Exception) { }
        }


        private static void Form_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            lock (_syncRoot) {
                e.Graphics.FillRectangle(SystemBrushes.ControlLight, e.ClipRectangle.Left, 0, e.ClipRectangle.Right, _titleHeight);
                e.Graphics.DrawLine(SystemPens.ControlDark, e.ClipRectangle.Left, _titleHeight, e.ClipRectangle.Right, _titleHeight);
                e.Graphics.DrawLine(SystemPens.ControlLightLight, e.ClipRectangle.Left, _titleHeight + 1, e.ClipRectangle.Right, _titleHeight + 1);
                if (_paintImage != null) { _paintImage.Paint(e.Graphics); }
                if (_paintProduct != null) { _paintProduct.Paint(e.Graphics); }
                if (_infoLines != null) {
                    for (int i = 0; i < _infoLines.Count; ++i) {
                        _infoLines[i].Paint(e.Graphics);
                    }
                }
            }

        }

        private static Bitmap GetAppIcon(string fileName) {
            if (!AboutBox.IsRunningOnMono) {
                System.IntPtr hLibrary = NativeMethods.LoadLibrary(fileName);
                if (!hLibrary.Equals(System.IntPtr.Zero)) {
                    System.IntPtr hIcon = NativeMethods.LoadIcon(hLibrary, "#32512");
                    if (!hIcon.Equals(System.IntPtr.Zero)) {
                        Bitmap bitmap = System.Drawing.Icon.FromHandle(hIcon).ToBitmap();
                        if (bitmap != null) { return bitmap; }
                    }
                }
            }
            return null;
        }


        private class PaintItem : System.IDisposable {

            public PaintItem(Image image, Point location) {
                this._image = image;
                this._location = location;
                this._rectangle = new Rectangle(location, image.Size);
            }

            public PaintItem(string text, Font font, int x, int y, int height, System.Windows.Forms.VisualStyles.VerticalAlignment align, Graphics measurementGraphics) {
                this._text = text;
                this._font = font;
                Size size = measurementGraphics.MeasureString(text, font, 600).ToSize();
                switch (align) {
                    case VerticalAlignment.Top:
                        this._location = new Point(x, y);
                        break;
                    case VerticalAlignment.Center:
                        this._location = new Point(x, y + (height - size.Height) / 2);
                        break;
                    case VerticalAlignment.Bottom:
                        this._location = new Point(x, y + height - size.Height);
                        break;
                }
                this._rectangle = new Rectangle(this.Location, size);
            }


            private Image _image;
            public Image Image {
                get { return this._image; }
            }

            private string _text;
            public string Text {
                get { return this._text; }
            }

            private Font _font;
            public Font Font {
                get { return this._font; }
            }

            private Point _location;
            public Point Location {
                get { return this._location; }
            }

            private Rectangle _rectangle;
            public Rectangle Rectangle {
                get { return this._rectangle; }
            }


            public void Paint(Graphics graphics) {
                if (this.Image != null) {
                    graphics.DrawImage(this.Image, this.Rectangle);
                } else if (this.Text != null) {
                    graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, this.Location);
                }
            }


            #region IDisposable Members

            /// <summary>
            /// Releases used resources.
            /// </summary>
            /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    if (this.Image != null) {
                        this.Image.Dispose();
                        this._image = null;
                    }
                    if (this.Font != null) {
                        if (!this.Font.IsSystemFont) {
                            this.Font.Dispose();
                        }
                        this._font = null;
                    }
                }
            }

            /// <summary>
            /// Releases used resources.
            /// </summary>
            public void Dispose() {
                Dispose(true);
                System.GC.SuppressFinalize(this);
            }

            #endregion

        }


        private static class Resources {

            internal static string Close {
                get { return GetInCurrentLanguage("Close", "Zatvori"); }
            }

            internal static string ReadMe {
                get { return GetInCurrentLanguage("Read me", "Pročitaj me"); }
            }

            internal static string WebPage {
                get { return GetInCurrentLanguage("Web page", "Web stranica"); }
            }

            internal static string Caption {
                get { return GetInCurrentLanguage("About", "O programu"); }
            }


            private static string GetInCurrentLanguage(string en_US, string hr_HR) {
                switch (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant()) {
                    case "EN":
                    case "EN-US":
                    case "EN-GB":
                        return en_US;

                    case "HR":
                    case "HR-HR":
                    case "HR-BA":
                        return hr_HR;

                    default:
                        return en_US;
                }
            }

        }


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }


        private static class NativeMethods {

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadLibrary(string lpFileName);

        }

    }

}
