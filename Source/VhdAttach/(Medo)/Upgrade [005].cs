/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2019-11-16: Allowing for TLS 1.2 and 1.3 where available.
//2015-12-31: Allowing for 301 redirect.
//2013-12-28: Message box adjustments.
//2012-03-13: UI adjustments.
//2012-03-05: Initial version.


using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Medo.Services
{

    /// <summary>
    /// Handles upgrade procedures.
    /// </summary>
    public static class Upgrade
    {

        /// <summary>
        /// Returns upgrade file if there is one or null if there is no upgrade.
        /// </summary>
        /// <param name="serviceUri">Service URI (e.g. https://medo64.com/upgrade/).</param>
        /// <exception cref="System.ArgumentNullException">Argument cannot be null (serviceUri).</exception>
        /// <exception cref="System.InvalidOperationException">Unexpected answer from upgrade server. -or- Cannot contact upgrade server.</exception>
        public static UpgradeFile GetUpgradeFile(Uri serviceUri)
        {
            return GetUpgradeFile(serviceUri, Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Returns upgrade file if there is one or null if there is no upgrade.
        /// </summary>
        /// <param name="serviceUri">Service URI (e.g. https://medo64.com/upgrade/).</param>
        /// <param name="assembly">Assembly.</param>
        /// <exception cref="System.ArgumentNullException">Argument cannot be null (serviceUri).</exception>
        /// <exception cref="System.InvalidOperationException">Unexpected answer from upgrade server. -or- Cannot contact upgrade server.</exception>
        public static UpgradeFile GetUpgradeFile(Uri serviceUri, Assembly assembly)
        {
            if (serviceUri == null) { throw new ArgumentNullException("serviceUri", "Argument cannot be null."); }
            if (assembly == null) { assembly = Assembly.GetEntryAssembly(); }
            var url = new StringBuilder();
            url.Append(serviceUri.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? serviceUri.AbsoluteUri : serviceUri.AbsoluteUri + "/");
            foreach (var ch in GetProduct(assembly))
            {
                if (char.IsLetterOrDigit(ch))
                {
                    url.Append(char.ToLowerInvariant(ch));
                }
            }
            url.Append("/");
            url.Append(assembly.GetName().Version.ToString());
            url.Append("/");
            return GetUpgradeFileFromURL(url.ToString());
        }


        /// <summary>
        /// Shows Upgrade dialog.
        /// </summary>
        /// <param name="owner">Shows the form as a modal dialog box with the specified owner.</param>
        /// <param name="serviceUri">Service URI (e.g. https://medo64.com/upgrade/).</param>
        /// <exception cref="System.ArgumentNullException">Argument cannot be null (serviceUri).</exception>
        public static DialogResult ShowDialog(IWin32Window owner, Uri serviceUri)
        {
            return ShowDialog(owner, serviceUri, Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Shows Upgrade dialog.
        /// </summary>
        /// <param name="owner">Shows the form as a modal dialog box with the specified owner.</param>
        /// <param name="serviceUri">Service URI (e.g. https://medo64.com/upgrade/).</param>
        /// <param name="assembly">Assembly.</param>
        /// <exception cref="System.ArgumentNullException">Argument cannot be null (serviceUri).</exception>
        public static DialogResult ShowDialog(IWin32Window owner, Uri serviceUri, Assembly assembly)
        {
            if (serviceUri == null) { throw new ArgumentNullException("serviceUri", "Argument cannot be null."); }
            if (assembly == null) { assembly = Assembly.GetEntryAssembly(); }
            TryProtocolUpgrade();
            using (var frm = new UpgradeForm(serviceUri, assembly))
            {
                if (owner != null)
                {
                    frm.ShowInTaskbar = false;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    return frm.ShowDialog(owner);
                }
                else
                {
                    frm.ShowInTaskbar = true;
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    return frm.ShowDialog();
                }
            }
        }


        private static UpgradeFile GetUpgradeFileFromURL(string url)
        {
            try
            {
                TryProtocolUpgrade();
                var request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.AllowAutoRedirect = false;
                request.Method = "HEAD";
                request.Proxy = HttpWebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Gone: return null; //no upgrade
                        case HttpStatusCode.MovedPermanently: return GetUpgradeFileFromURL(response.Headers["Location"]); //follow 301 redirect
                        case HttpStatusCode.SeeOther: return new UpgradeFile(new Uri(response.Headers["Location"])); //upgrade at Location
                        default: throw new InvalidOperationException("Unexpected answer from upgrade server (" + response.StatusCode.ToString() + " " + response.StatusDescription + ").");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex is WebException webEx)
                {
                    if (webEx.Response is HttpWebResponse response)
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.Gone: return null; //no upgrade
                            case HttpStatusCode.Forbidden: return null; //no upgrade (old code)
                            case HttpStatusCode.SeeOther: return new UpgradeFile(new Uri(response.Headers["Location"])); //upgrade at Location
                            default: throw new InvalidOperationException("Unexpected answer from upgrade server (" + response.StatusCode.ToString() + " " + response.StatusDescription + ").", ex);
                        }
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cannot contact upgrade server.", ex);
            }
        }

        private static string GetProduct(Assembly assembly)
        {
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1))
            {
                return ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
            }
            else
            {
                var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1))
                {
                    return ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                }
                else
                {
                    return assembly.GetName().Name;
                }
            }
        }

        private static void TryProtocolUpgrade()
        {
            try
            { //try TLS 1.3
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288 | (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;
            }
            catch (NotSupportedException)
            {
                try
                { //try TLS 1.2
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                }
                catch (NotSupportedException)
                {
                    try
                    { //try TLS 1.1
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                    }
                    catch (NotSupportedException)
                    { //TLS 1.0
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    }
                }
            }
        }


        #region Form

        private class UpgradeForm : Form
        {

            private readonly Uri ServiceUri;
            private readonly Assembly Assembly;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "All controls are disposed in Form's Dispose method.")]
            internal UpgradeForm(Uri serviceUri, Assembly assembly)
            {
                ServiceUri = serviceUri;
                Assembly = assembly;

                CancelButton = btnCancel;
                AutoSize = true;
                AutoScaleMode = AutoScaleMode.Font;
                Font = SystemFonts.MessageBoxFont;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MinimizeBox = false;
                MaximizeBox = false;
                ShowIcon = false;
                Text = string.Format(CultureInfo.CurrentUICulture, Resources.Caption, GetProduct(assembly));

                FormClosing += new FormClosingEventHandler(Form_FormClosing);

                Controls.Add(prgProgress);
                Controls.Add(lblStatus);
                Controls.Add(btnUpgrade);
                Controls.Add(btnDownload);
                Controls.Add(btnCancel);

                var width = 7 + btnUpgrade.Width + 7 + btnDownload.Width + 5 * 7 + btnCancel.Width + 7;
                var height = 7 + prgProgress.Height + 7 + lblStatus.Height + 21 + btnCancel.Height + 7;
                ClientSize = new Size(width, height);

                prgProgress.Location = new Point(7, 7);
                prgProgress.Width = width - 14;
                lblStatus.Location = new Point(7, prgProgress.Bottom + 7);

                btnUpgrade.Location = new Point(7, ClientSize.Height - btnUpgrade.Height - 7);
                btnDownload.Location = new Point(btnUpgrade.Right + 7, ClientSize.Height - btnDownload.Height - 7);
                btnCancel.Location = new Point(ClientSize.Width - btnCancel.Width - 7, ClientSize.Height - btnCancel.Height - 7);


                bwCheck.DoWork += new DoWorkEventHandler(bwCheck_DoWork);
                bwCheck.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheck_RunWorkerCompleted);
                bwCheck.RunWorkerAsync();

                btnUpgrade.Click += new EventHandler(btnUpgrade_Click);
                btnDownload.Click += new EventHandler(btnDownload_Click);

                bwDownload.DoWork += new DoWorkEventHandler(bwDownload_DoWork);
                bwDownload.ProgressChanged += new ProgressChangedEventHandler(bwDownload_ProgressChanged);
                bwDownload.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwDownload_RunWorkerCompleted);
            }


            private void Form_FormClosing(object sender, FormClosingEventArgs e)
            {
                prgProgress.Style = ProgressBarStyle.Continuous;
                prgProgress.Value = 0;
                lblStatus.Text = Resources.StatusCancelling;
                btnCancel.Enabled = false;
                if (bwCheck.IsBusy) { bwCheck.CancelAsync(); }
                if (bwDownload.IsBusy) { bwDownload.CancelAsync(); }
                e.Cancel = (bwCheck.IsBusy) || (bwDownload.IsBusy);
            }


            private readonly ProgressBar prgProgress = new ProgressBar() { Height = (int)(SystemInformation.HorizontalScrollBarHeight * 1.5), Style = ProgressBarStyle.Marquee };
            private readonly Label lblStatus = new Label() { AutoSize = true, Text = Resources.StatusChecking };
            private readonly Button btnCancel = new Button() { AutoSize = true, DialogResult = DialogResult.Cancel, Padding = new Padding(3, 1, 3, 1), Text = Resources.Cancel };
            private readonly Button btnUpgrade = new Button() { AutoSize = true, Padding = new Padding(3, 1, 3, 1), Text = Resources.Upgrade, Visible = false };
            private readonly Button btnDownload = new Button() { AutoSize = true, Padding = new Padding(3, 1, 3, 1), Text = Resources.Download, Visible = false };

            private readonly BackgroundWorker bwCheck = new BackgroundWorker() { WorkerSupportsCancellation = true };
            private readonly BackgroundWorker bwDownload = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };

            private UpgradeFile UpgradeFile = null;


            private void bwCheck_DoWork(object sender, DoWorkEventArgs e)
            {
                e.Result = Medo.Services.Upgrade.GetUpgradeFile(ServiceUri, Assembly);
                e.Cancel = bwCheck.CancellationPending;
            }

            private void bwCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Cancelled == false)
                {
                    if (e.Error == null)
                    {
                        UpgradeFile = e.Result as UpgradeFile;
                        prgProgress.Style = ProgressBarStyle.Continuous;
                        if (UpgradeFile != null)
                        {
                            lblStatus.Text = Resources.StatusUpgradeIsAvailable;
                            btnUpgrade.Visible = true;
                            btnDownload.Visible = true;
                            btnUpgrade.Focus();
                        }
                        else
                        {
                            lblStatus.Text = Resources.StatusUpgradeIsNotAvailable;
                            btnCancel.DialogResult = DialogResult.OK;
                            btnCancel.Text = Resources.Close;
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(this, Resources.ErrorCannotCheck + "\n\n" + e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                        DialogResult = DialogResult.Cancel;
                    }
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }

            private void btnUpgrade_Click(object sender, EventArgs e)
            {
                lblStatus.Text = Resources.StatusDownloading;
                btnUpgrade.Enabled = false;
                btnDownload.Visible = false;
                bwDownload.RunWorkerAsync();
            }

            private void btnDownload_Click(object sender, EventArgs e)
            {
                lblStatus.Text = Resources.StatusDownloading;
                btnUpgrade.Visible = false;
                btnDownload.Enabled = false;
                bwDownload.RunWorkerAsync();
            }


            private void bwDownload_DoWork(object sender, DoWorkEventArgs e)
            {
                var buffer = new byte[1024];
                using (var stream = UpgradeFile.GetStream())
                {
                    stream.ReadTimeout = 5000;
                    using (var bytes = new MemoryStream(1024 * 1024))
                    {
                        while (bwDownload.CancellationPending == false)
                        {
                            bwDownload.ReportProgress((int)(bytes.Length * 100 / UpgradeFile.StreamLength));
                            var read = stream.Read(buffer, 0, buffer.Length);
                            if (read > 0)
                            {
                                bytes.Write(buffer, 0, read);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (bwDownload.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (bytes.Length != UpgradeFile.StreamLength) { throw new InvalidOperationException("Content length mismatch."); }
                            e.Result = bytes.ToArray();
                        }
                    }
                }
            }

            private void bwDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                prgProgress.Value = e.ProgressPercentage;
                if (e.UserState is string text) { lblStatus.Text = text; }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "SaveFileDialog is disposed in using.")]
            private void bwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Cancelled == false)
                {
                    if (e.Error == null)
                    {
                        btnCancel.Enabled = false;
                        var bytes = e.Result as byte[];
                        var isUpgrade = btnUpgrade.Visible;
                        if (isUpgrade)
                        {
                            try
                            {
                                var fileName = Path.Combine(Path.GetTempPath(), UpgradeFile.FileName);
                                File.WriteAllBytes(fileName, bytes);
                                Process.Start(fileName);
                                System.Windows.Forms.Application.Exit();
                                DialogResult = DialogResult.OK;
                            }
                            catch (Win32Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(this, Resources.ErrorCannotUpgrade + "\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                                DialogResult = DialogResult.Cancel;
                            }
                        }
                        else
                        {
                            var filter = string.Format(CultureInfo.CurrentUICulture, Resources.Filter, new FileInfo(UpgradeFile.FileName).Extension);
                            using (var frm = new SaveFileDialog() { AddExtension = false, CheckPathExists = true, FileName = UpgradeFile.FileName, InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Filter = filter })
                            {
                                if (frm.ShowDialog(this) == DialogResult.OK)
                                {
                                    File.WriteAllBytes(frm.FileName, bytes);
                                    DialogResult = DialogResult.OK;
                                }
                                else
                                {
                                    DialogResult = DialogResult.Cancel;
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(this, Resources.ErrorCannotDownload + "\n\n" + e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                        DialogResult = DialogResult.Cancel;
                    }
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }


            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    foreach (Control iControl in Controls)
                    {
                        iControl.Dispose();
                    }
                    bwCheck.Dispose();
                    bwDownload.Dispose();
                    Controls.Clear();
                }
                base.Dispose(disposing);
            }

        }

        #endregion



        private static class Resources
        {

            internal static string Caption { get { return GetInCurrentLanguage("Upgrade {0}", "{0} nadogradnja"); } }
            internal static string Cancel { get { return GetInCurrentLanguage("Cancel", "Odustani"); } }
            internal static string Close { get { return GetInCurrentLanguage("Close", "Zatvori"); } }
            internal static string Upgrade { get { return GetInCurrentLanguage("Upgrade", "Nadogradi"); } }
            internal static string Download { get { return GetInCurrentLanguage("Download", "Preuzmi"); } }
            internal static string ErrorCannotCheck { get { return GetInCurrentLanguage("Cannot check for upgrade.", "Nije moguće provjeriti nadogradnju."); } }
            internal static string ErrorCannotUpgrade { get { return GetInCurrentLanguage("Cannot upgrade.", "Nadogradnja nije moguća."); } }
            internal static string ErrorCannotDownload { get { return GetInCurrentLanguage("Cannot download upgrade.", "Nije moguće preuzeti nadogradnju."); } }
            internal static string StatusChecking { get { return GetInCurrentLanguage("Checking for upgrade...", "Provjera nadogradnje u tijeku..."); } }
            internal static string StatusCancelling { get { return GetInCurrentLanguage("Cancelling...", "Odustajanje u tijeku..."); } }
            internal static string StatusDownloading { get { return GetInCurrentLanguage("Download in progress...", "Preuzimanje u tijeku..."); } }
            internal static string StatusUpgradeIsAvailable { get { return GetInCurrentLanguage("Upgrade is available.", "Nadogradnja je dostupna."); } }
            internal static string StatusUpgradeIsNotAvailable { get { return GetInCurrentLanguage("Upgrade is not available.", "Nadogradnja nije dostupna."); } }
            internal static string Filter { get { return GetInCurrentLanguage("Download|*{0}|All files|*.*", "Preuzimanje|*{0}|Sve datoteke|*.*"); } }


            private static string GetInCurrentLanguage(string en_US, string hr_HR)
            {
                switch (Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant())
                {
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

    }




    /// <summary>
    /// Handles upgrade file operations.
    /// </summary>
    public sealed class UpgradeFile
    {

        internal UpgradeFile(Uri uri)
        {
            Uri = uri;
        }


        /// <summary>
        /// Gets upgrade URI.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets file name.
        /// </summary>
        public string FileName { get { return Uri.Segments[Uri.Segments.Length - 1]; } }

        /// <summary>
        /// Returns content stream.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is appropriate since it can produce side effects and each call produces different result.")]
        public Stream GetStream()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(Uri);
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            request.Proxy = HttpWebRequest.DefaultWebProxy;
            request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            var response = (HttpWebResponse)request.GetResponse();
            StreamLength = response.ContentLength;
            return response.GetResponseStream();
        }

        /// <summary>
        /// Gets length of stream.
        /// Valid only once stream has been read.
        /// </summary>
        internal long? StreamLength { get; private set; }

        /// <summary>
        /// Retrieves whole file.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Content length mismatch.</exception>
        public byte[] GetBytes()
        {
            var buffer = new byte[1024];
            using (var stream = GetStream())
            {
                stream.ReadTimeout = 5000;
                using (var bytes = new MemoryStream(1024 * 1024))
                {
                    while (true)
                    {
                        var read = stream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            bytes.Write(buffer, 0, read);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (bytes.Length != StreamLength) { throw new InvalidOperationException("Content length mismatch."); }
                    return bytes.ToArray();
                }
            }
        }

    }
}
