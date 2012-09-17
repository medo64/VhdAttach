//Copyright (c) 2008 Josip Medved <jmedved@jmedved.com>

//2008-01-07: First version.
//2008-01-15: Added overloads for SaveToTemp and ShowDialog.
//2008-01-17: SendToWeb returns false instead of throwing WebException.
//2008-01-27: Changed Version format.
//2008-03-01: Added Product and Time.
//2008-03-30: Added SaveToEventLog.
//            Fixed mixed English and Croatian messages.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (NormalizeStringsToUppercase, SpecifyStringComparison).
//2008-12-17: Changed SendToWeb to PostToWeb.
//2009-03-30: Refactoring.
//            Added user confirmation form in PostToWeb.
//2009-04-07: Success check is done with status code.
//2009-06-26: Added Version and removed EntryAssembly from arguments.
//            Obsoleted PostToWeb, ShowDialog took over functionality.
//            Deleted old obsoleted methods.
//2009-12-09: Changed source to use only preallocated buffers for writing to log file.
//            Log file name is now ErrorReport.{application}.log (does not contain version info).
//            NameValueCollection is no longer used for passing custom parameters. String array is used instead.
//            If sending of message does not succeed whole message is saved in temp as ErrorReport.{application}.{time}.log.
//2010-02-13: Send button is disabled if there is neither exception nor message.
//            Log file is now ErrorReport "[{application}].log" and "ErrorReport [{application}] {time}.log".
//2010-02-13: Added TopMost.
//2010-03-02: Line wrapping at 72nd character.
//2010-03-07: Changed Math.* to System.Math.*.
//2010-10-30: Fixed bug with sending error report.
//2010-11-06: Graphical update.
//2012-09-16: Added retry upon send failure.


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Medo.Diagnostics {

    /// <summary>
    /// Creating of error reports.
    /// This class is thread-safe.
    /// </summary>
    public static class ErrorReport {

        private static readonly object _syncRoot = new object();
        private static readonly StringBuilder _logBuffer = new StringBuilder(8000);
        private static readonly string _logSeparator = Environment.NewLine + new string('-', 72) + Environment.NewLine + Environment.NewLine;
        private static readonly string _logFileName;

        private static readonly string _infoProductTitle;
        private static readonly string _infoProductVersion;
        private static readonly string _infoAssemblyFullName;
        private static readonly string _infoOsVersion = System.Environment.OSVersion.ToString();
        private static readonly string _infoFrameworkVersion = ".NET Framework " + System.Environment.Version.ToString();
        private static readonly string[] _infoReferencedAssemblies;


        /// <summary>
        /// Setting up of initial variable values in order to avoid setting them once problems (e.g. OutOfMemoryException) occur.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "This cannot be done with in-field assignments.")]
        static ErrorReport() {
            var assembly = Assembly.GetEntryAssembly();

            object[] productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                ErrorReport._infoProductTitle = ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
            } else {
                object[] titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                    ErrorReport._infoProductTitle = ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                } else {
                    ErrorReport._infoProductTitle = assembly.GetName().Name;
                }
            }
            _infoProductVersion = assembly.GetName().Version.ToString();
            ErrorReport._logFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), string.Format(System.Globalization.CultureInfo.InvariantCulture, "ErrorReport [{0}].log", _infoProductTitle));

            _infoAssemblyFullName = assembly.FullName;

            var listReferencedAssemblies = new List<string>();
            foreach (var iRefAss in assembly.GetReferencedAssemblies()) {
                listReferencedAssemblies.Add(iRefAss.ToString());
            }
            _infoReferencedAssemblies = listReferencedAssemblies.ToArray();
        }


        /// <summary>
        /// Writes file with exception details in temp directory.
        /// Returns true if write succeded.
        /// </summary>
        /// <param name="exception">Exception which is processed.</param>
        public static bool SaveToTemp(Exception exception) {
            lock (_syncRoot) {
                return SaveToTemp(exception, null);
            }
        }

        /// <summary>
        /// Writes file with exception details in temp directory.
        /// Returns true if write succeded.
        /// </summary>
        /// <param name="exception">Exception which is processed.</param>
        /// <param name="additionalInformation">Additional information to be added in log.</param>
        public static bool SaveToTemp(Exception exception, params string[] additionalInformation) {
            lock (_syncRoot) {
                LogBufferFillFromException(exception, additionalInformation);
                LogBufferSaveToLogFile();

                return true;
            }
        }


        /// <summary>
        /// Shows dialog and sends error to support web if user chooses so.
        /// Returns DialogResult.OK if posting error report succeded.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        /// <param name="exception">Exception which is processed. If exception is null, this is considered feature request.</param>
        /// <param name="address">Address of form which will receive data. Form should expect POST request with fields "EntryAssembly", "Message" and "Details".</param>
        /// <param name="additionalInformation">Additional information to be added in log.</param>
        public static DialogResult ShowDialog(IWin32Window owner, Exception exception, Uri address, params string[] additionalInformation) {
            lock (_syncRoot) {
                LogBufferFillFromException(exception, additionalInformation);

                if ((exception != null) && !ErrorReport.DisableAutomaticSaveToTemp) {
                    LogBufferSaveToLogFile();
                }

                try {
                    if (address != null) { //send to web

                        if (exception != null) {
                            if (ShowDialogInform(owner, address) == DialogResult.OK) {
                                string message, email, displayName;
                                if (ShowDialogCollect(owner, exception, out message, out email, out displayName) == DialogResult.OK) {
                                    string fullMessage = LogBufferGetStringWithUserInformation(message, displayName, email);
                                    while (true) {
                                        var result = ShowDialogSend(owner, address, fullMessage, email, displayName);
                                        if (result != DialogResult.Retry) {
                                            return result;
                                        }
                                    }
                                }
                            }
                        } else {
                            string message, email, displayName;
                            if (ShowDialogCollect(owner, exception, out message, out email, out displayName) == DialogResult.OK) {
                                string fullMessage = LogBufferGetStringWithUserInformation(message, displayName, email);
                                while (true) {
                                    var result = ShowDialogSend(owner, address, fullMessage, email, displayName);
                                    if (result != DialogResult.Retry) {
                                        return result;
                                    }
                                }
                            }
                        }

                    } else { //don't send to web

                        if (exception != null) {
                            if (ShowDialogInform(owner, address) == DialogResult.OK) {
                                return DialogResult.OK;
                            }
                        } else {
                            return DialogResult.Cancel;
                        }

                    }

                } catch (WebException ex) {
                    System.Diagnostics.Debug.WriteLine("W: " + ex.Message + ".    {{Medo.Diagnostics.ErrorReport}}");
                }

                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Shows dialog and sends error to support web if user chooses so.
        /// Returns DialogResult.OK if posting error report succeded.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        /// <param name="exception">Exception which is processed.</param>
        /// <param name="address">Address of form which will receive data. Form should expect POST request with fields "EntryAssembly", "Message" and "Details".</param>
        public static DialogResult ShowDialog(IWin32Window owner, Exception exception, Uri address) {
            lock (_syncRoot) {
                return ShowDialog(owner, exception, address, null);
            }
        }

        /// <summary>
        /// Shows dialog with exception.
        /// Returns DialogResult.OK if posting error report succeded.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        /// <param name="exception">Exception which is processed.</param>
        public static DialogResult ShowDialog(IWin32Window owner, Exception exception) {
            return ShowDialog(owner, exception, null, null);
        }


        /// <summary>
        /// Writes file with exception details to EventLog.
        /// Returns true if write succeded.
        /// </summary>
        /// <param name="exception">Exception which is processed.</param>
        /// <param name="eventLog">EventLog to write to.</param>
        public static bool SaveToEventLog(Exception exception, EventLog eventLog) {
            lock (_syncRoot) {
                return SaveToEventLog(exception, eventLog, 0, null);
            }
        }

        /// <summary>
        /// Writes file with exception details to EventLog.
        /// Returns true if write succeded.
        /// </summary>
        /// <param name="exception">Exception which is processed.</param>
        /// <param name="eventLog">EventLog to write to.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        public static bool SaveToEventLog(Exception exception, EventLog eventLog, int eventId) {
            lock (_syncRoot) {
                return SaveToEventLog(exception, eventLog, eventId, null);
            }
        }

        /// <summary>
        /// Writes file with exception details to EventLog.
        /// Returns true if write succeded.
        /// </summary>
        /// <param name="exception">Exception which is processed.</param>
        /// <param name="eventLog">EventLog to write to.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="additionalInformation">Additional information to be added in log.</param>
        public static bool SaveToEventLog(Exception exception, EventLog eventLog, int eventId, params string[] additionalInformation) {
            lock (_syncRoot) {
                LogBufferFillFromException(exception, additionalInformation);

                if ((exception != null) && !ErrorReport.DisableAutomaticSaveToTemp) {
                    LogBufferSaveToLogFile();
                }

                if (eventLog == null) { return false; }

                eventLog.WriteEntry(LogBufferGetString(), EventLogEntryType.Error, eventId);
                return true;
            }
        }


        /// <summary>
        /// Gets/sets whether report is automatically saved to temporary folder for any reporting method.
        /// Default is to save all reports to temporary folder before any other action.
        /// </summary>
        public static bool DisableAutomaticSaveToTemp { get; set; }

        /// <summary>
        /// Gets/sets whether window will appear top-most.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TopMost", Justification = "Spelling is same as Form's TopMost property.")]
        public static bool TopMost { get; set; }


        private static DialogResult ShowDialogInform(IWin32Window owner, Uri address) {
            var ownerForm = owner as Form;

            using (var form = new Form())
            using (var label = new Label())
            using (var sendButton = new Button())
            using (var closeButton = new Button()) {
                form.AcceptButton = sendButton;
                form.CancelButton = closeButton;
                form.ControlBox = true;
                form.Font = SystemFonts.MessageBoxFont;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                if (ownerForm != null) {
                    form.Icon = ownerForm.Icon;
                    form.StartPosition = FormStartPosition.CenterParent;
                } else {
                    form.Icon = null;
                    form.StartPosition = FormStartPosition.CenterScreen;
                }
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ShowInTaskbar = false;
                form.Text = _infoProductTitle;
                form.TopMost = ErrorReport.TopMost;

                int dluW, dluH;
                using (var graphics = form.CreateGraphics()) {
                    var fewCharSize = graphics.MeasureString(Resources.MeasurementText, form.Font);
                    dluW = (int)System.Math.Ceiling(fewCharSize.Width / Resources.MeasurementText.Length);
                    dluH = (int)System.Math.Ceiling(fewCharSize.Height);
                }
                var dluBorder = dluH / 2;

                form.ClientSize = new Size(System.Math.Min(36 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), System.Math.Min(6 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

                //sendButton
                sendButton.AutoEllipsis = true;
                sendButton.ClientSize = new Size(dluW * 20, (int)(dluH * 1.5));
                sendButton.DialogResult = DialogResult.OK;
                sendButton.Location = new Point(dluBorder, form.ClientRectangle.Bottom - sendButton.Height - dluBorder);
                sendButton.Text = Resources.GetInCurrentLanguage("Report a bug", "Pošalji prijavu greške");

                //closeButton
                closeButton.AutoEllipsis = true;
                closeButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
                closeButton.DialogResult = DialogResult.Cancel;
                closeButton.Location = new Point(form.ClientRectangle.Right - closeButton.Width - dluBorder, form.ClientRectangle.Bottom - closeButton.Height - dluBorder);
                closeButton.Text = Resources.GetInCurrentLanguage("Close", "Zatvori");

                //label
                label.AutoEllipsis = true;
                label.AutoSize = false;
                label.BackColor = SystemColors.Window;
                label.ForeColor = SystemColors.WindowText;
                label.TextAlign = ContentAlignment.TopCenter;
                label.ClientSize = new Size(form.ClientSize.Width - 14, dluH * 2);
                label.Location = new Point(0, 0);
                label.Padding = new Padding(dluBorder);
                label.Size = new Size(form.ClientSize.Width, form.ClientSize.Height - dluBorder - closeButton.Height - dluBorder);
                label.Text = Resources.GetInCurrentLanguage("Unexpected error occurred.", "Dogodila se neočekivana greška.");

                form.Controls.Add(label);
                if (address != null) {
                    form.Controls.Add(sendButton);
                }
                form.Controls.Add(closeButton);


                return form.ShowDialog(owner);
            }
        }

        private static DialogResult ShowDialogCollect(IWin32Window owner, Exception exception, out string message, out string email, out string displayName) {
            var ownerForm = owner as Form;

            using (var form = new Form())
            using (var panelContent = new Panel())
            using (var labelHelp = new Label())
            using (var labelMessage = new Label())
            using (var textMessage = new TextBox())
            using (var labelName = new Label())
            using (var textName = new TextBox())
            using (var labelEmail = new Label())
            using (var textEmail = new TextBox())
            using (var labelReport = new Label())
            using (var textReport = new TextBox())
            using (var sendButton = new Button())
            using (var cancelButton = new Button()) {
                form.AcceptButton = sendButton;
                form.CancelButton = cancelButton;
                form.ControlBox = true;
                form.Font = SystemFonts.MessageBoxFont;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                if (ownerForm != null) {
                    form.Icon = ownerForm.Icon;
                    form.StartPosition = FormStartPosition.CenterParent;
                } else {
                    form.Icon = null;
                    form.StartPosition = FormStartPosition.CenterScreen;
                }
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ShowInTaskbar = false;
                form.Text = _infoProductTitle;
                form.TopMost = ErrorReport.TopMost;

                int dluW, dluH;
                using (var graphics = form.CreateGraphics()) {
                    var fewCharSize = graphics.MeasureString(Resources.MeasurementText, form.Font);
                    dluW = (int)System.Math.Ceiling(fewCharSize.Width / Resources.MeasurementText.Length);
                    dluH = (int)System.Math.Ceiling(fewCharSize.Height);
                }
                var dluBorder = dluH / 2;
                var dluSpace = dluH / 3;

                form.ClientSize = new Size(System.Math.Min(64 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), System.Math.Min(32 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

                //panel
                panelContent.BackColor = SystemColors.Window;
                panelContent.ForeColor = SystemColors.WindowText;
                panelContent.Location = new Point(0, 0);
                panelContent.Size = new Size(form.ClientRectangle.Width, form.ClientRectangle.Height - sendButton.Height - dluBorder * 3);

                //labelHelp
                labelHelp.AutoEllipsis = true;
                labelHelp.AutoSize = false;
                labelHelp.BackColor = SystemColors.Window;
                labelHelp.ForeColor = SystemColors.Highlight;
                labelHelp.Text = Resources.GetInCurrentLanguage("This report will not contain any personal data not provided by you.", "Ovaj izvještaj neće sadržavati osobne podatke osim onih koje Vi odlučite dati.");
                labelHelp.ClientSize = new Size(form.ClientSize.Width - dluBorder - dluBorder, dluH * 2);
                labelHelp.Location = new Point(dluBorder, dluBorder);

                //labelMessage
                labelMessage.AutoEllipsis = true;
                labelMessage.AutoSize = false;
                labelMessage.BackColor = SystemColors.Window;
                labelMessage.ForeColor = SystemColors.WindowText;
                if (exception != null) {
                    labelMessage.Text = Resources.GetInCurrentLanguage("What were you doing when error occurred?", "Što ste radili kada se dogodila greška?");
                } else {
                    labelMessage.Text = Resources.GetInCurrentLanguage("What do you wish to report?", "Što želite prijaviti?");
                }
                labelMessage.ClientSize = new Size(form.ClientSize.Width - dluBorder - dluBorder, dluH);
                labelMessage.Location = new Point(dluBorder, labelHelp.Bottom + dluSpace);

                //textMessage
                textMessage.BackColor = SystemColors.Window;
                textMessage.ForeColor = SystemColors.WindowText;
                textMessage.AcceptsReturn = true;
                textMessage.Multiline = true;
                textMessage.ScrollBars = ScrollBars.Vertical;
                textMessage.Size = new Size(form.ClientRectangle.Width - dluBorder * 2, 3 * dluH);
                textMessage.Location = new Point(dluBorder, labelMessage.Bottom + dluSpace);
                if (exception == null) {
                    textMessage.Tag = sendButton;
                    textMessage.TextChanged += new EventHandler(textMessage_TextChanged);
                }
                textMessage.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

                //textEmail
                textEmail.BackColor = SystemColors.Window;
                textEmail.ForeColor = SystemColors.WindowText;
                textEmail.Size = new Size(form.ClientRectangle.Width - dluW * 15 - dluBorder * 2, dluH);
                textEmail.Location = new Point(form.ClientRectangle.Width - textEmail.Width - dluBorder, textMessage.Bottom + dluSpace);
                textEmail.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

                //labelEmail
                labelEmail.BackColor = SystemColors.Window;
                labelEmail.ForeColor = SystemColors.WindowText;
                labelEmail.AutoEllipsis = true;
                labelEmail.AutoSize = false;
                labelEmail.ClientSize = new Size(dluW * 14, dluH);
                labelEmail.Location = new Point(dluBorder, textEmail.Top + (textEmail.Height - labelEmail.Height) / 2);
                labelEmail.Text = Resources.GetInCurrentLanguage("E-mail (optional):", "E-mail (neobvezno):");

                //textName
                textName.BackColor = SystemColors.Window;
                textName.ForeColor = SystemColors.WindowText;
                textName.Size = new Size(form.ClientRectangle.Width - dluW * 15 - dluBorder * 2, dluH);
                textName.Location = new Point(form.ClientRectangle.Width - textName.Width - dluBorder, textEmail.Bottom + dluSpace + dluSpace);
                textName.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

                //labelName
                labelName.AutoEllipsis = true;
                labelName.AutoSize = false;
                labelName.BackColor = SystemColors.Window;
                labelName.ForeColor = SystemColors.WindowText;
                labelName.ClientSize = new Size(dluW * 14, dluH);
                labelName.Location = new Point(dluBorder, textName.Top + (textName.Height - labelName.Height) / 2);
                labelName.Text = Resources.GetInCurrentLanguage("Name (optional):", "Ime (neobvezno):");

                //labelReport
                labelReport.AutoEllipsis = true;
                labelReport.AutoSize = false;
                labelReport.BackColor = SystemColors.Window;
                labelReport.ForeColor = SystemColors.WindowText;
                labelReport.Text = Resources.GetInCurrentLanguage("Additional data that will be sent:", "Dodatni podaci koji će biti poslani:");
                labelReport.ClientSize = new Size(form.ClientRectangle.Width - dluBorder * 2, dluH);
                labelReport.Location = new Point(dluBorder, textName.Bottom + dluBorder);

                //textReport
                textReport.BackColor = SystemColors.Control;
                textReport.ForeColor = SystemColors.ControlText;
                textReport.Font = new Font(FontFamily.GenericMonospace, form.Font.Size * 1F, FontStyle.Regular, form.Font.Unit);
                textReport.Multiline = true;
                textReport.ReadOnly = true;
                textReport.ScrollBars = ScrollBars.Vertical;
                textReport.Text = LogBufferGetString();
                textReport.Location = new Point(dluBorder, labelReport.Bottom + dluSpace);
                textReport.Size = new Size(form.ClientRectangle.Width - dluBorder * 2, panelContent.ClientRectangle.Height - textReport.Top - dluBorder);
                textReport.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

                //sendButton
                sendButton.AutoEllipsis = true;
                sendButton.ClientSize = new Size(dluW * 20, (int)(dluH * 1.5));
                sendButton.Enabled = (exception != null);
                sendButton.DialogResult = DialogResult.OK;
                sendButton.Location = new Point(dluBorder, form.ClientRectangle.Bottom - sendButton.Height - dluBorder);
                sendButton.Text = Resources.GetInCurrentLanguage("Report a bug", "Pošalji prijavu greške");

                //cancelButton
                cancelButton.AutoEllipsis = true;
                cancelButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(form.ClientRectangle.Right - cancelButton.Width - dluBorder, form.ClientRectangle.Bottom - cancelButton.Height - dluBorder);
                cancelButton.Text = Resources.GetInCurrentLanguage("Cancel", "Odustani");

                panelContent.Controls.Add(labelHelp);
                panelContent.Controls.Add(labelMessage);
                panelContent.Controls.Add(textMessage);
                panelContent.Controls.Add(labelEmail);
                panelContent.Controls.Add(textEmail);
                panelContent.Controls.Add(labelName);
                panelContent.Controls.Add(textName);
                panelContent.Controls.Add(labelReport);
                panelContent.Controls.Add(textReport);

                form.Controls.Add(panelContent);
                form.Controls.Add(sendButton);
                form.Controls.Add(cancelButton);


                if (form.ShowDialog(owner) == DialogResult.OK) {
                    message = textMessage.Text.Trim();
                    email = textEmail.Text.Trim();
                    displayName = textName.Text.Trim();
                    return DialogResult.OK;
                } else {
                    message = null;
                    email = null;
                    displayName = null;
                    return DialogResult.Cancel;
                }
            }
        }

        private static void textMessage_TextChanged(object sender, EventArgs e) {
            var senderTextBox = sender as TextBox;
            if (senderTextBox != null) {
                var button = senderTextBox.Tag as Button;
                if (button != null) {
                    button.Enabled = senderTextBox.Text.Length > 0;
                }
            }
        }

        private static void text_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            var senderTextBox = sender as TextBox;
            if (senderTextBox != null) {
                if (e.KeyData == (Keys.Control | Keys.A)) {
                    senderTextBox.SelectAll();
                    e.IsInputKey = false;
                }
            }
        }

        private static DialogResult ShowDialogSend(IWin32Window owner, Uri address, string message, string email, string displayName) {
            var ownerForm = owner as Form;

            using (var form = new Form())
            using (var label = new Label())
            using (var backgroundWorker = new BackgroundWorker())
            using (var progressBar = new ProgressBar()) {
                form.ControlBox = false;
                form.Font = SystemFonts.MessageBoxFont;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                if (ownerForm != null) {
                    form.Icon = ownerForm.Icon;
                    form.StartPosition = FormStartPosition.CenterParent;
                } else {
                    form.Icon = null;
                    form.StartPosition = FormStartPosition.CenterScreen;
                }
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ShowInTaskbar = false;
                form.Text = _infoProductTitle;
                form.TopMost = ErrorReport.TopMost;

                int dluW, dluH;
                using (var graphics = form.CreateGraphics()) {
                    var fewCharSize = graphics.MeasureString(Resources.MeasurementText, form.Font);
                    dluW = (int)System.Math.Ceiling(fewCharSize.Width / Resources.MeasurementText.Length);
                    dluH = (int)System.Math.Ceiling(fewCharSize.Height);
                }

                var dluBorder = dluH / 2;
                var dluSpace = dluH / 3;

                form.ClientSize = new Size(System.Math.Min(36 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), System.Math.Min(4 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

                //label
                label.AutoEllipsis = true;
                label.TextAlign = ContentAlignment.TopCenter;
                label.ClientSize = new Size(form.ClientSize.Width - dluBorder * 2, dluH * 2);
                label.Location = new Point(dluBorder, dluBorder);
                label.Text = Resources.GetInCurrentLanguage("Sending error report...", "Prijava greške u tijeku...");

                //progressBar
                progressBar.MarqueeAnimationSpeed = 50;
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.ClientSize = new Size(form.ClientRectangle.Width - dluBorder * 2, dluH);
                progressBar.Location = new Point(dluBorder, form.ClientRectangle.Bottom - dluBorder - progressBar.Height);

                //backgroundWorker
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);


                form.Controls.Add(label);
                form.Controls.Add(progressBar);

                var allFormParameters = new NameValueCollection();
                allFormParameters.Add("Product", _infoProductTitle);
                allFormParameters.Add("Version", _infoProductVersion);
                allFormParameters.Add("Message", message);
                if (!string.IsNullOrEmpty(email)) { allFormParameters.Add("Email", email); }
                if (!string.IsNullOrEmpty(displayName)) { allFormParameters.Add("DisplayName", displayName); }

                backgroundWorker.RunWorkerAsync(new object[] { form, address, allFormParameters });

                MessageBoxOptions mbOptions = 0;
                if (owner == null) { mbOptions |= MessageBoxOptions.ServiceNotification; }
                if ((ownerForm != null) && (ownerForm.RightToLeft == RightToLeft.Yes)) { mbOptions |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading; }
                if (form.ShowDialog(owner) == DialogResult.OK) {
                    System.Windows.Forms.MessageBox.Show(owner, Resources.GetInCurrentLanguage("Error report was successfully sent.", "Izvještaj o grešci je uspješno poslan."), _infoProductTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, mbOptions);
                    return DialogResult.OK;
                } else {
                    if (!ErrorReport.DisableAutomaticSaveToTemp) {
                        try {
                            string fullLogFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), string.Format(System.Globalization.CultureInfo.InvariantCulture, @"ErrorReport [{0}] {1:yyyyMMdd\THHmmss}.log", _infoProductTitle, DateTime.Now));
                            System.IO.File.WriteAllText(fullLogFileName, message);
                        } catch (System.Security.SecurityException) {
                        } catch (System.IO.IOException) { }
                    }
                    if (System.Windows.Forms.MessageBox.Show(owner, Resources.GetInCurrentLanguage("Error report cannot be sent.\nDo you wish to retry?", "Izvještaj o grešci ne može biti poslan.\nŽelite li ponovno pokušati?"), _infoProductTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, mbOptions) == DialogResult.Yes) {
                        return DialogResult.Retry;
                    } else {
                        return DialogResult.Cancel;
                    }
                }
            }
        }

        private static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            var transferBag = (object[])e.Argument;
            var form = (Form)transferBag[0];
            var address = (Uri)transferBag[1];
            var allFormParameters = (NameValueCollection)transferBag[2];


            try {

                WebRequest request = WebRequest.Create(address);
                request.Method = "POST";
                request.Proxy = HttpWebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

                StringBuilder sbPostData = new StringBuilder();
                for (int i = 0; i < allFormParameters.Count; ++i) {
                    if (sbPostData.Length > 0) { sbPostData.Append("&"); }
                    sbPostData.Append(UrlEncode(allFormParameters.GetKey(i)) + "=" + UrlEncode(allFormParameters[i]));
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                using (System.IO.Stream dataStream = request.GetRequestStream()) {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse()) {
                    if (response.StatusCode == HttpStatusCode.OK) {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream())) {
                            string responseFromServer = reader.ReadToEnd();
                            if (responseFromServer.Length == 0) { //no data is outputed in case of real 200 response (instead of 500 wrapped in generic 200 page)
                                e.Result = new object[] { form, DialogResult.OK };
                            } else {
                                e.Result = new object[] { form, DialogResult.Cancel };
                            }
                        }
                        e.Result = new object[] { form, DialogResult.OK };
                    } else {
                        e.Result = new object[] { form, DialogResult.Cancel };
                    }
                }

            } catch (WebException ex) {
                System.Diagnostics.Debug.WriteLine("W: " + ex.Message + ".    {{Medo.Diagnostics.ErrorReport}}");
                e.Result = new object[] { form, DialogResult.Cancel };
            }
        }

        private static void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var transferBag = (object[])e.Result;
            var form = (Form)transferBag[0];
            var result = (DialogResult)transferBag[1];

            form.DialogResult = result;
        }

        private static void LogBufferSaveToLogFile() {
            if (System.IO.File.Exists(_logFileName)) {
                System.IO.File.AppendAllText(_logFileName, _logSeparator);
            }

            System.IO.File.AppendAllText(_logFileName, LogBufferGetString());
        }

        private static void LogBufferFillFromException(Exception exception, params string[] additionalInformation) {
            if (_logBuffer.Length != 0) { _logBuffer.Length = 0; }
            var isOutOfMemoryException = (exception is OutOfMemoryException);

            AppendLine("Environment", _logBuffer);
            AppendLine("", _logBuffer);
            AppendLine(_infoAssemblyFullName, _logBuffer, 1, true);
            AppendLine(_infoOsVersion, _logBuffer, 1, true);
            AppendLine(_infoFrameworkVersion, _logBuffer, 1, true);
            if (isOutOfMemoryException == false) {
                AppendLine("Local time is " + DateTime.Now.ToString(@"yyyy\-MM\-dd\THH\:mm\:ssK", System.Globalization.CultureInfo.InvariantCulture), _logBuffer, 1, true); //it will fail in OutOfMemory situation
            }

            if (exception != null) {
                AppendLine("", _logBuffer);
                Exception ex = exception;
                int exLevel = 0;
                while (ex != null) {
                    AppendLine("", _logBuffer);

                    if (exLevel == 0) {
                        AppendLine("Exception", _logBuffer);
                    } else if (exLevel == 1) {
                        AppendLine("Inner exception (1)", _logBuffer);
                    } else if (exLevel == 2) {
                        AppendLine("Inner exception (2)", _logBuffer);
                    } else {
                        AppendLine("Inner exception (...)", _logBuffer);
                    }
                    AppendLine("", _logBuffer);
                    if (isOutOfMemoryException == false) {
                        AppendLine(ex.GetType().ToString(), _logBuffer, 1, true);
                    }
                    AppendLine(ex.Message, _logBuffer, 1, true);
                    if (!string.IsNullOrEmpty(ex.StackTrace)) {
                        AppendLine(ex.StackTrace, _logBuffer, 2, false);
                    }

                    ex = ex.InnerException;
                    exLevel += 1;
                }

                AppendLine("", _logBuffer);
                AppendLine("", _logBuffer);
                AppendLine("Referenced assemblies", _logBuffer);
                AppendLine("", _logBuffer);
                for (int i = 0; i < _infoReferencedAssemblies.Length; ++i) {
                    AppendLine(_infoReferencedAssemblies[i], _logBuffer, 1, true);
                }
            }

            if ((additionalInformation != null) && (additionalInformation.Length > 0)) {
                AppendLine("", _logBuffer);
                AppendLine("", _logBuffer);
                AppendLine("Additional information", _logBuffer);
                AppendLine("", _logBuffer);
                for (int i = 0; i < additionalInformation.Length; ++i) {
                    AppendLine(additionalInformation[i], _logBuffer, 1, true);
                }
            }
        }

        private static string LogBufferGetStringWithUserInformation(string message, string name, string email) {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(message)) {
                AppendLine(message, sb);
                AppendLine("", sb);
                AppendLine("", sb);
            }
            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(email)) {
                AppendLine("User information", sb);
                AppendLine("", sb);
                if (!string.IsNullOrEmpty(name)) {
                    AppendLine("Name: " + name, sb, 1, true);
                }
                if (!string.IsNullOrEmpty(email)) {
                    AppendLine("E-mail: " + email, sb, 1, true);
                }
                AppendLine("", sb);
                AppendLine("", sb);
            }
            sb.Append(_logBuffer);

            return sb.ToString();
        }

        private static string LogBufferGetString() {
            return _logBuffer.ToString();
        }

        private static string UrlEncode(string text) {
            byte[] source = System.Text.UTF8Encoding.UTF8.GetBytes(text);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < source.Length; ++i) {
                if (((source[i] >= 48) && (source[i] <= 57)) || ((source[i] >= 65) && (source[i] <= 90)) || ((source[i] >= 97) && (source[i] <= 122)) || (source[i] == 45) || (source[i] == 46) || (source[i] == 95) || (source[i] == 126)) { //A-Z a-z - . _ ~
                    sb.Append(System.Convert.ToChar(source[i]));
                } else {
                    sb.Append("%" + source[i].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            return sb.ToString();
        }


        private const int LineLength = 72;

        private static void AppendLine(string input, StringBuilder output) {
            AppendLine(input, output, 0, false);
        }

        private static void AppendLine(string input, StringBuilder output, int indentLevel, bool tickO) {
            if (input == null) { return; }
            if (input.Length == 0) {
                output.AppendLine();
                return;
            }

            if (tickO) {
                indentLevel += 1;
            }


            int maxWidth = LineLength - indentLevel * 3;
            int end = input.Length - 1;

            int firstChar = 0;

            int lastChar;
            int nextChar;
            do {
                if ((end - firstChar) < maxWidth) {
                    lastChar = end;
                    nextChar = end + 1;
                } else {
                    int nextCrBreak = input.IndexOf('\r', firstChar, maxWidth);
                    int nextLfBreak = input.IndexOf('\n', firstChar, maxWidth);
                    int nextCrLfBreak;
                    if (nextCrBreak == -1) {
                        nextCrLfBreak = nextLfBreak;
                    } else if (nextLfBreak == -1) {
                        nextCrLfBreak = nextCrBreak;
                    } else {
                        nextCrLfBreak = System.Math.Min(nextCrBreak, nextLfBreak);
                    }
                    if ((nextCrLfBreak != -1) && ((nextCrLfBreak - firstChar) <= maxWidth)) {
                        lastChar = nextCrLfBreak - 1;
                        nextChar = lastChar + 2;
                        if (nextChar <= end) {
                            if ((input[nextChar] == '\n') || (input[nextChar] == '\r')) {
                                nextChar += 1;
                            }
                        }
                    } else {
                        int nextSpaceBreak = input.LastIndexOf(' ', firstChar + maxWidth, maxWidth);
                        if ((nextSpaceBreak != -1) && ((nextSpaceBreak - firstChar) <= maxWidth)) {
                            lastChar = nextSpaceBreak;
                            nextChar = lastChar + 1;
                        } else {
                            int nextOtherBreak1 = input.LastIndexOf('-', firstChar + maxWidth, maxWidth);
                            int nextOtherBreak2 = input.LastIndexOf(':', firstChar + maxWidth, maxWidth);
                            int nextOtherBreak3 = input.LastIndexOf('(', firstChar + maxWidth, maxWidth);
                            int nextOtherBreak4 = input.LastIndexOf(',', firstChar + maxWidth, maxWidth);
                            int nextOtherBreak = System.Math.Max(nextOtherBreak1, System.Math.Max(nextOtherBreak2, System.Math.Max(nextOtherBreak3, nextOtherBreak4)));
                            if ((nextOtherBreak != -1) && ((nextOtherBreak - firstChar) <= maxWidth)) {
                                lastChar = nextOtherBreak;
                                nextChar = lastChar + 1;
                            } else {
                                lastChar = firstChar + maxWidth;
                                if (lastChar > end) { lastChar = end; }
                                nextChar = lastChar;
                            }
                        }
                    }
                }

                if (tickO) {
                    for (int i = 0; i < indentLevel - 1; ++i) { output.Append("   "); }
                    output.Append("o  ");
                    tickO = false;
                } else {
                    for (int i = 0; i < indentLevel; ++i) { output.Append("   "); }
                }
                for (int i = firstChar; i <= lastChar; ++i) {
                    output.Append(input[i]);
                }
                output.AppendLine();

                firstChar = nextChar;
            } while (nextChar <= end);
        }


        private static class Resources {

            internal static string MeasurementText { get { return "mMiI"; } }

            internal static string GetInCurrentLanguage(string en_US, string hr_HR) {
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

    }

}
