//Copyright (c) 2007 Josip Medved <jmedved@jmedved.com>

//2007-12-31: New version.
//2008-01-03: Added Resources.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (SpecifyMarshalingForPInvokeStringArguments, NormalizeStringsToUppercase).
//2008-11-14: Reworked code to use SafeHandle.
//            Fixed ToInt32 call on x64 bit windows.
//2008-12-01: Deleted methods without owner parameter.
//2009-07-04: Compatibility with Mono 2.4.
//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical).


using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Medo {

    /// <summary>
    /// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
    /// </summary>
    public static class MessageBox {

        private readonly static object _syncRoot = new object();


        #region With owner

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxIcon icon) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, buttons, icon, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, buttons, icon, defaultButton);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
            }
        }

        #endregion


        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
            if (!MessageBox.IsRunningOnMono) {
                lock (_syncRoot) {
                    if (owner != null) {
                        using (CbtHook ch = new CbtHook(owner)) {
                            return (DialogResult)NativeMethods.MessageBox(owner.Handle, text, caption, (uint)buttons | (uint)icon | (uint)defaultButton);
                        }
                    } else {
                        using (CbtHook ch = new CbtHook(null)) {
                            return (DialogResult)NativeMethods.MessageBox(System.IntPtr.Zero, text, caption, (uint)buttons | (uint)icon | (uint)defaultButton);
                        }
                    }
                } //lock
            } else { //MONO
                return System.Windows.Forms.MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, 0);
            }
        }


        #region ShowInformation

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton);
            }
        }

        #endregion


        #region ShowWarning

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Warning, defaultButton);
            }
        }

        #endregion


        #region ShowError

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowError(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowError(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowError(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowError(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowError(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Error, defaultButton);
            }
        }

        #endregion


        #region ShowQuestion

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Question, defaultButton);
            }
        }

        #endregion


        private static class Resources {

            internal static string DefaultCaption {
                get {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();

                    string caption;
                    object[] productAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), true);
                    if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                        caption = ((System.Reflection.AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
                    } else {
                        object[] titleAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true);
                        if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                            caption = ((System.Reflection.AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                        } else {
                            caption = assembly.GetName().Name;
                        }
                    }

                    return caption;
                }
            }


            internal static string OK {
                get { return GetInCurrentLanguage("OK", "U redu"); }
            }

            internal static string Cancel {
                get { return GetInCurrentLanguage("Cancel", "Odustani"); }
            }

            internal static string Abort {
                get { return GetInCurrentLanguage("&Abort", "P&rekini"); }
            }

            internal static string Retry {
                get { return GetInCurrentLanguage("&Retry", "&Ponovi"); }
            }

            internal static string Ignore {
                get { return GetInCurrentLanguage("&Ignore", "&Zanemari"); }
            }

            internal static string Yes {
                get { return GetInCurrentLanguage("&Yes", "&Da"); }
            }

            internal static string No {
                get { return GetInCurrentLanguage("&No", "&Ne"); }
            }


            internal static string ExceptionCbtHookCannotBeRemoved { get { return "CBT Hook cannot be removed."; } }


            internal static bool IsTranslatable {
                get {
                    switch (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant()) {
                        case "EN":
                        case "EN-US":
                        case "EN-GB":
                        case "HR":
                        case "HR-HR":
                        case "HR-BA":
                            return true;

                        default:
                            return false;
                    }
                }
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


        #region Native

        private class CbtHook : System.IDisposable {

            private IWin32Window _owner;

            private NativeMethods.WindowsHookSafeHandle _hook;
            private NativeMethods.CbtHookProcDelegate _cbtHookProc;


            public CbtHook(IWin32Window owner) {
                this._owner = owner;
                this._cbtHookProc = new NativeMethods.CbtHookProcDelegate(CbtHookProc);
                this._hook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_CBT, this._cbtHookProc, System.IntPtr.Zero, NativeMethods.GetCurrentThreadId());
                System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "I: Created CBT hook (ID={0}).    {{Medo.MessageBox}}", this._hook.ToString()));
            }

            ~CbtHook() {
                Dispose();
            }


            public System.IntPtr CbtHookProc(int nCode, System.IntPtr wParam, System.IntPtr lParam) {
                switch (nCode) {
                    case NativeMethods.HCBT_ACTIVATE:
                        System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "I: Dialog HCBT_ACTIVATE (hWnd={0}).    {{Medo.MessageBox}}", wParam.ToString()));

                        if (this._owner != null) {
                            NativeMethods.RECT rectMessage = new NativeMethods.RECT();
                            NativeMethods.RECT rectOwner = new NativeMethods.RECT();
                            if ((NativeMethods.GetWindowRect(wParam, ref rectMessage)) && (NativeMethods.GetWindowRect(this._owner.Handle, ref rectOwner))) {
                                int widthMessage = rectMessage.right - rectMessage.left;
                                int heightMessage = rectMessage.bottom - rectMessage.top;
                                int widthOwner = rectOwner.right - rectOwner.left;
                                int heightOwner = rectOwner.bottom - rectOwner.top;

                                int newLeft = rectOwner.left + (widthOwner - widthMessage) / 2;
                                int newTop = rectOwner.top + (heightOwner - heightMessage) / 2;

                                NativeMethods.SetWindowPos(wParam, System.IntPtr.Zero, newLeft, newTop, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                            }
                        }

                        if (Resources.IsTranslatable) {
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_OK, Resources.OK);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_CANCEL, Resources.Cancel);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_ABORT, Resources.Abort);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_RETRY, Resources.Retry);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_IGNORE, Resources.Ignore);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_YES, Resources.Yes);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_NO, Resources.No);
                        }

                        try {
                            return NativeMethods.CallNextHookEx(this._hook, nCode, wParam, lParam);
                        } finally {
                            this.Dispose();
                        }
                }
                return NativeMethods.CallNextHookEx(this._hook, nCode, wParam, lParam);
            }


            #region IDisposable Members

            public void Dispose() {
                //System.GC.KeepAlive(this._cbtHookProc);
                if (!this._hook.IsClosed) {
                    this._hook.Close();
                    if (this._hook.IsClosed) {
                        System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "I: CBT Hook destroyed (ID={0}).    {{Medo.MessageBox}}", this._hook.ToString()));
                    } else {
                        throw new System.InvalidOperationException(Resources.ExceptionCbtHookCannotBeRemoved);
                    }
                }
                this._hook.Dispose();
                //if (!this._hook.Equals(System.IntPtr.Zero)) {
                //    if (NativeMethods.UnhookWindowsHookEx(this._hook)) {
                //        System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "I: Medo.Windows.Forms.MessageBox: CBT Hook destroyed (ID={0}).", this._hook.ToInt32()));
                //        this._hook = System.IntPtr.Zero;
                //    } else {
                //        throw new System.InvalidOperationException(Resources.ExceptionCbtHookCannotBeRemoved);
                //    }
                //}
                System.GC.SuppressFinalize(this);
            }

            #endregion

        }


        private static class NativeMethods {

            public const int WH_CBT = 0x5;

            public const int DLG_ID_OK = 0x01;
            public const int DLG_ID_CANCEL = 0x02;
            public const int DLG_ID_ABORT = 0x03;
            public const int DLG_ID_RETRY = 0x04;
            public const int DLG_ID_IGNORE = 0x05;
            public const int DLG_ID_YES = 0x06;
            public const int DLG_ID_NO = 0x07;

            public const int HCBT_ACTIVATE = 0x5;

            public const int SWP_NOSIZE = 0x01;
            public const int SWP_NOZORDER = 0x04;
            public const int SWP_NOACTIVATE = 0x10;


            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public struct RECT {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }


            public class WindowsHookSafeHandle : SafeHandle {
                public WindowsHookSafeHandle()
                    : base(IntPtr.Zero, true) {
                }


                public override bool IsInvalid {
                    get { return (this.IsClosed) || (base.handle == IntPtr.Zero); }
                }

                protected override bool ReleaseHandle() {
                    return UnhookWindowsHookEx(this.handle);
                }

                public override string ToString() {
                    return this.handle.ToString();
                }

            }


            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern System.IntPtr CallNextHookEx(WindowsHookSafeHandle idHook, int nCode, System.IntPtr wParam, System.IntPtr lParam);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern int GetCurrentThreadId();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool GetWindowRect(System.IntPtr hWnd, ref RECT lpRect);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Managed equivalent does not support all needed features.")]
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int MessageBox(System.IntPtr hWnd, string lpText, string lpCaption, uint uType);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool SetDlgItemText(System.IntPtr hWnd, int nIDDlgItem, string lpString);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern WindowsHookSafeHandle SetWindowsHookEx(int idHook, CbtHookProcDelegate lpfn, System.IntPtr hInstance, int threadId);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [ReliabilityContract(Consistency.MayCorruptProcess, Cer.Success)]
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr idHook);


            public delegate System.IntPtr CbtHookProcDelegate(int nCode, System.IntPtr wParam, System.IntPtr lParam);

        }

        #endregion


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }

}
