using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace VhdAttach {
    internal partial class ServiceWaitForm : Form {

        public ServiceWaitForm(string title, CrossAppDomainDelegate action) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            this.ControlBox = false;

            this.Text = title;
            bw.RunWorkerAsync(action);
        }


        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            var exceptions = new List<Exception>();
            var action = (Delegate)e.Argument;
            try {
                action.DynamicInvoke();
            } catch (TargetInvocationException ex) {
                if (ex.InnerException != null) {
                    throw ex.InnerException;
                } else {
                    throw;
                }
            }
        }

        private void bw_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (e.Error == null) {
                this.DialogResult = DialogResult.OK;
            } else {
                Messages.ShowServiceIOException(this, e.Error);
                this.DialogResult = DialogResult.Cancel;
            }
        }

    }
}
