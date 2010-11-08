//Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com

//2007-11-21: Initial version.
//2007-12-24: Changed SubKeyPath to include full path.
//2007-12-27: Added Load overloads for multiple controls.
//            Obsoleted Subscribe method.
//2008-01-02: Calls to MoveSplitterTo method are now checked.
//2008-01-05: Removed obsoleted methods.
//2008-01-10: Moved private methods to Helper class.
//2008-01-31: Fixed bug that caused only first control to be loaded/saved.
//2008-02-15: Fixed bug with positioning of centered forms.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (CompoundWordsShouldBeCasedCorrectly).
//2008-05-11: Windows with fixed borders are no longer resized.
//2008-07-10: Fixed resize on load when window is maximized.
//2008-12-27: Added LoadNowAndSaveOnClose method.
//2009-07-04: Compatibility with Mono 2.4.
//2010-10-17: Limited all loaded forms to screen's working area.
//            Changed LoadNowAndSaveOnClose to use SetupOnLoadAndClose.
//2010-10-31: Added option to skip registry writes (NoRegistryWrites).


using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System;
using System.Security;
using System.Globalization;
using System.Drawing;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Enables storing and loading of windows control's state.
    /// It is written in State key at HKEY_CURRENT_USER branch withing SubKeyPath of Settings class.
    /// This class is not thread-safe since it should be called only from one thread - one that has interface.
    /// </summary>
    public static class State {

        private static string _subkeyPath;
        /// <summary>
        /// Gets/sets subkey used for registry storage.
        /// </summary>
        public static string SubkeyPath {
            get {
                if (_subkeyPath == null) {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();

                    string company = null;
                    object[] companyAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), true);
                    if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
                        company = ((System.Reflection.AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company;
                    }

                    string product = null;
                    object[] productAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), true);
                    if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                        product = ((System.Reflection.AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
                    } else {
                        object[] titleAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true);
                        if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                            product = ((System.Reflection.AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                        } else {
                            product = assembly.GetName().Name;
                        }
                    }

                    string path = "Software";
                    if (!string.IsNullOrEmpty(company)) { path += "\\" + company; }
                    if (!string.IsNullOrEmpty(product)) { path += "\\" + product; }

                    _subkeyPath = path + "\\State";
                }
                return _subkeyPath;
            }
            set { _subkeyPath = value; }
        }

        /// <summary>
        /// Gets/sets whether settings should be written to registry.
        /// </summary>
        public static bool NoRegistryWrites { get; set; }


        #region Load Save - All

        /// <summary>
        /// Loads previous state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="form">Form on which's FormClosing handler this function will attach. State will not be altered for this parameter.</param>
        /// <param name="controls">Controls to load and to save.</param>
        /// <exception cref="System.ArgumentNullException">Form is null.</exception>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        /// <exception cref="System.ArgumentException">Form already used.</exception>
        [Obsolete("Use SetupOnLoadAndClose instead.")]
        public static void LoadNowAndSaveOnClose(Form form, params Control[] controls) {
            SetupOnLoadAndClose(form, controls);
        }

        /// <summary>
        /// Loads previous state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="form">Form on which's FormClosing handler this function will attach. State will not be altered for this parameter.</param>
        /// <param name="controls">Controls to load and to save.</param>
        /// <exception cref="System.ArgumentNullException">Form is null.</exception>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        /// <exception cref="System.ArgumentException">Form setup already done.</exception>
        public static void SetupOnLoadAndClose(Form form, params Control[] controls) {
            if (form == null) { throw new ArgumentNullException("form", "Form is null."); }

            if (formSetup.ContainsKey(form)) { throw new System.ArgumentException("Form setup already done.", "form"); }

            Load(form);
            if (controls != null) { Load(controls); }

            formSetup.Add(form, controls);
            form.Load += new EventHandler(form_Load);
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
        }

        private static Dictionary<Form, Control[]> formSetup = new Dictionary<Form, Control[]>();

        private static void form_Load(object sender, EventArgs e) {
            var form = sender as Form;
            if (formSetup.ContainsKey(form)) {
                Load(form);
                Load(formSetup[form]);
            }
        }

        private static void form_FormClosed(object sender, FormClosedEventArgs e) {
            var form = sender as Form;
            if (formSetup.ContainsKey(form)) {
                Save(form);
                Save(formSetup[form]);
                formSetup.Remove(form);
            }
        }

        /// <summary>
        /// Loads previous state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="controls">Controls to load.</param>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Load(params System.Windows.Forms.Control[] controls) {
            if (controls == null) { return; }
            for (int i = 0; i < controls.Length; ++i) {
                Form form = controls[i] as Form;
                if (form != null) {
                    Load(form);
                    continue;
                }

                PropertyGrid propertyGrid = controls[i] as PropertyGrid;
                if (propertyGrid != null) {
                    Load(propertyGrid);
                    continue;
                }

                ListView listView = controls[i] as ListView;
                if (listView != null) {
                    Load(listView);
                    continue;
                }

                SplitContainer splitContainer = controls[i] as SplitContainer;
                if (splitContainer != null) {
                    Load(splitContainer);
                    continue;
                }
            }
        }

        /// <summary>
        /// Saves control's state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="controls">Controls to save.</param>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Save(params System.Windows.Forms.Control[] controls) {
            if (controls == null) { return; }
            for (int i = 0; i < controls.Length; ++i) {
                Form form = controls[i] as Form;
                if (form != null) {
                    Save(form);
                    continue;
                }

                PropertyGrid propertyGrid = controls[i] as PropertyGrid;
                if (propertyGrid != null) {
                    Save(propertyGrid);
                    continue;
                }

                ListView listView = controls[i] as ListView;
                if (listView != null) {
                    Save(listView);
                    continue;
                }

                SplitContainer splitContainer = controls[i] as SplitContainer;
                if (splitContainer != null) {
                    Save(splitContainer);
                    continue;
                }
            }
        }

        #endregion


        #region Load Save - Form

        /// <summary>
        /// Saves Form state (Left,Top,Width,Height,WindowState).
        /// </summary>
        /// <param name="form">Form.</param>
        /// <exception cref="System.ArgumentNullException">Form is null.</exception>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Save(System.Windows.Forms.Form form) {
            if (form == null) { throw new ArgumentNullException("form", "Form is null."); }

            string baseValueName = Helper.GetControlPath(form);

            Helper.Write(baseValueName + ".WindowState", System.Convert.ToInt32(form.WindowState, System.Globalization.CultureInfo.InvariantCulture));
            if (form.WindowState == System.Windows.Forms.FormWindowState.Normal) {
                Helper.Write(baseValueName + ".Left", form.Bounds.Left);
                Helper.Write(baseValueName + ".Top", form.Bounds.Top);
                Helper.Write(baseValueName + ".Width", form.Bounds.Width);
                Helper.Write(baseValueName + ".Height", form.Bounds.Height);
            } else {
                Helper.Write(baseValueName + ".Left", form.RestoreBounds.Left);
                Helper.Write(baseValueName + ".Top", form.RestoreBounds.Top);
                Helper.Write(baseValueName + ".Width", form.RestoreBounds.Width);
                Helper.Write(baseValueName + ".Height", form.RestoreBounds.Height);
            }
        }

        /// <summary>
        /// Loads previous Form state (Left,Top,Width,Height,WindowState).
        /// If StartupPosition value is Manual, saved settings are used, for other types, it tryes to resemble original behaviour.
        /// </summary>
        /// <param name="form">Form.</param>
        /// <exception cref="System.ArgumentNullException">Form is null.</exception>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Load(System.Windows.Forms.Form form) {
            if (form == null) { throw new ArgumentNullException("form", "Form is null."); }

            string baseValueName = Helper.GetControlPath(form);

            int currWindowState = System.Convert.ToInt32(form.WindowState, System.Globalization.CultureInfo.InvariantCulture);
            int currLeft, currTop, currWidth, currHeight;
            if (form.WindowState == System.Windows.Forms.FormWindowState.Normal) {
                currLeft = form.Bounds.Left;
                currTop = form.Bounds.Top;
                currWidth = form.Bounds.Width;
                currHeight = form.Bounds.Height;
            } else {
                currLeft = form.RestoreBounds.Left;
                currTop = form.RestoreBounds.Top;
                currWidth = form.RestoreBounds.Width;
                currHeight = form.RestoreBounds.Height;
            }

            int newLeft = Helper.Read(baseValueName + ".Left", currLeft);
            int newTop = Helper.Read(baseValueName + ".Top", currTop);
            int newWidth = Helper.Read(baseValueName + ".Width", currWidth);
            int newHeight = Helper.Read(baseValueName + ".Height", currHeight);
            int newWindowState = Helper.Read(baseValueName + ".WindowState", currWindowState);

            if ((form.FormBorderStyle == FormBorderStyle.Fixed3D) || (form.FormBorderStyle == FormBorderStyle.FixedDialog) || (form.FormBorderStyle == FormBorderStyle.FixedSingle) || (form.FormBorderStyle == FormBorderStyle.FixedToolWindow)) {
                newWidth = currWidth;
                newHeight = currHeight;
            }

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle(new System.Drawing.Rectangle(newLeft, newTop, newWidth, newHeight));

            switch (form.StartPosition) {

                case System.Windows.Forms.FormStartPosition.CenterParent: {
                        if (form.Parent != null) {
                            newLeft = form.Parent.Left + (form.Parent.Width - newWidth) / 2;
                            newTop = form.Parent.Top + (form.Parent.Height - newHeight) / 2;
                        } else if (form.Owner != null) {
                            newLeft = form.Owner.Left + (form.Owner.Width - newWidth) / 2;
                            newTop = form.Owner.Top + (form.Owner.Height - newHeight) / 2;
                        } else {
                            newLeft = screen.WorkingArea.Left + (screen.WorkingArea.Width - newWidth) / 2;
                            newTop = screen.WorkingArea.Top + (screen.WorkingArea.Height - newHeight) / 2;
                        }
                    } break;

                case System.Windows.Forms.FormStartPosition.CenterScreen: {
                        newLeft = screen.WorkingArea.Left + (screen.WorkingArea.Width - newWidth) / 2;
                        newTop = screen.WorkingArea.Top + (screen.WorkingArea.Height - newHeight) / 2;
                    } break;

            }

            if (newWidth <= 0) { newWidth = currWidth; }
            if (newHeight <= 0) { newHeight = currHeight; }
            if (newWidth > screen.WorkingArea.Width) { newWidth = screen.WorkingArea.Width; }
            if (newHeight > screen.WorkingArea.Height) { newHeight = screen.WorkingArea.Height; }
            if (newLeft + newWidth > screen.WorkingArea.Right) { newLeft = screen.WorkingArea.Left + (screen.WorkingArea.Width - newWidth); }
            if (newTop + newHeight > screen.WorkingArea.Bottom) { newTop = screen.WorkingArea.Top + (screen.WorkingArea.Height - newHeight); }
            if (newLeft < screen.WorkingArea.Left) { newLeft = screen.WorkingArea.Left; }
            if (newTop < screen.WorkingArea.Top) { newTop = screen.WorkingArea.Top; }

            form.Location = new Point(newLeft, newTop);
            form.Size = new Size(newWidth, newHeight);

            if (newWindowState == System.Convert.ToInt32(System.Windows.Forms.FormWindowState.Maximized, System.Globalization.CultureInfo.InvariantCulture)) {
                form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            } //no need for any code - it is already either in normal state or minimized (will be restored to normal).
        }

        #endregion

        #region Load Save - PropertyGrid

        /// <summary>
        /// Loads previous PropertyGrid state (LabelWidth, PropertySort).
        /// </summary>
        /// <param name="control">PropertyGrid.</param>
        /// <exception cref="System.ArgumentNullException">Control is null.</exception>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "PropertyGrid is passed because of reflection upon its member.")]
        public static void Load(PropertyGrid control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            string baseValueName = Helper.GetControlPath(control);

            try {
                control.PropertySort = (System.Windows.Forms.PropertySort)(Helper.Read(baseValueName + ".PropertySort", System.Convert.ToInt32(control.PropertySort, System.Globalization.CultureInfo.InvariantCulture)));
            } catch (System.ComponentModel.InvalidEnumArgumentException) { }

            System.Reflection.FieldInfo fieldGridView = control.GetType().GetField("gridView", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object gridViewObject = fieldGridView.GetValue(control);
            if (gridViewObject != null) {
                int currentlabelWidth = 0;
                System.Reflection.PropertyInfo propertyInternalLabelWidth = gridViewObject.GetType().GetProperty("InternalLabelWidth", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (propertyInternalLabelWidth != null) {
                    object val = propertyInternalLabelWidth.GetValue(gridViewObject, null);
                    if (val is int) {
                        currentlabelWidth = (int)val;
                    }
                }
                int labelWidth = Helper.Read(baseValueName + ".LabelWidth", currentlabelWidth);
                if ((labelWidth > 0) && (labelWidth < control.Width)) {
                    System.Reflection.BindingFlags methodMoveSplitterToFlags = System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                    System.Reflection.MethodInfo methodMoveSplitterTo = gridViewObject.GetType().GetMethod("MoveSplitterTo", methodMoveSplitterToFlags);
                    if (methodMoveSplitterTo != null) {
                        methodMoveSplitterTo.Invoke(gridViewObject, methodMoveSplitterToFlags, null, new object[] { labelWidth }, System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
            }
        }

        /// <summary>
        /// Saves PropertyGrid state (LabelWidth).
        /// </summary>
        /// <param name="control">PropertyGrid.</param>
        /// <exception cref="System.ArgumentNullException">Control is null.</exception>
        /// <exception cref="System.NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "PropertyGrid is passed because of reflection upon its member.")]
        public static void Save(System.Windows.Forms.PropertyGrid control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            string baseValueName = Helper.GetControlPath(control);

            Helper.Write(baseValueName + ".PropertySort", System.Convert.ToInt32(control.PropertySort, System.Globalization.CultureInfo.InvariantCulture));

            System.Reflection.FieldInfo fieldGridView = control.GetType().GetField("gridView", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object gridViewObject = fieldGridView.GetValue(control);
            if (gridViewObject != null) {
                System.Reflection.PropertyInfo propertyInternalLabelWidth = gridViewObject.GetType().GetProperty("InternalLabelWidth", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (propertyInternalLabelWidth != null) {
                    object val = propertyInternalLabelWidth.GetValue(gridViewObject, null);
                    if (val is int) {
                        Helper.Write(baseValueName + ".LabelWidth", (int)val);
                    }
                }
            }
        }

        #endregion

        #region Load Save - ListView

        /// <summary>
        /// Loads previous ListView state (Column header width).
        /// </summary>
        /// <param name="control">ListView.</param>
        /// <exception cref="System.ArgumentNullException">Control is null.</exception>
        public static void Load(System.Windows.Forms.ListView control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            string baseValueName = Helper.GetControlPath(control);

            for (int i = 0; i < control.Columns.Count; i++) {
                int width = Helper.Read(baseValueName + ".ColumnHeaderWidth[" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]", control.Columns[i].Width);
                if (width > control.ClientRectangle.Width) { width = control.ClientRectangle.Width; }
                control.Columns[i].Width = width;
            }
        }

        /// <summary>
        /// Saves ListView state (Column header width).
        /// </summary>
        /// <param name="control">ListView.</param>
        /// <exception cref="System.ArgumentNullException">Control is null.</exception>
        public static void Save(System.Windows.Forms.ListView control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            string baseValueName = Helper.GetControlPath(control);

            for (int i = 0; i < control.Columns.Count; i++) {
                Helper.Write(baseValueName + ".ColumnHeaderWidth[" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]", control.Columns[i].Width);
            }
        }

        #endregion

        #region Load Save - SplitContainer

        /// <summary>
        /// Loads previous SplitContainer state.
        /// </summary>
        /// <param name="control">SplitContainer.</param>
        /// <exception cref="System.ArgumentNullException">Control is null.</exception>
        public static void Load(SplitContainer control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            string baseValueName = Helper.GetControlPath(control);

            try {
                control.Orientation = (System.Windows.Forms.Orientation)(Helper.Read(baseValueName + ".Orientation", System.Convert.ToInt32(control.Orientation, System.Globalization.CultureInfo.InvariantCulture)));
            } catch (System.ComponentModel.InvalidEnumArgumentException) { }
            try {
                int distance = Helper.Read(baseValueName + ".SplitterDistance", control.SplitterDistance);
                try {
                    control.SplitterDistance = distance;
                } catch (System.ArgumentOutOfRangeException) { }
            } catch (System.ComponentModel.InvalidEnumArgumentException) { }
        }

        /// <summary>
        /// Saves SplitContainer state.
        /// </summary>
        /// <param name="control">SplitContainer.</param>
        /// <exception cref="System.ArgumentNullException">Control is null.</exception>
        public static void Save(System.Windows.Forms.SplitContainer control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            string baseValueName = Helper.GetControlPath(control);

            Helper.Write(baseValueName + ".Orientation", System.Convert.ToInt32(control.Orientation, System.Globalization.CultureInfo.InvariantCulture));
            Helper.Write(baseValueName + ".SplitterDistance", control.SplitterDistance);
        }

        #endregion


        private static class Helper {

            internal static void Write(string valueName, int value) {
                if (State.NoRegistryWrites == false) {
                    try {
                        if (State.SubkeyPath.Length == 0) { return; }
                        using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(State.SubkeyPath)) {
                            if (rk != null) {
                                rk.SetValue(valueName, value, RegistryValueKind.DWord);
                            }
                        }
                    } catch (IOException) { //key is deleted. 
                    } catch (UnauthorizedAccessException) { } //key is write protected. 
                }
            }

            internal static int Read(string valueName, int defaultValue) {
                try {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(State.SubkeyPath, false)) {
                        if (rk != null) {
                            object value = rk.GetValue(valueName, null);
                            if (value == null) { return defaultValue; }
                            var valueKind = RegistryValueKind.DWord;
                            if (!State.Helper.IsRunningOnMono) { valueKind = rk.GetValueKind(valueName); }
                            if ((value != null) && (valueKind == RegistryValueKind.DWord)) {
                                return (int)value;
                            }
                        }
                    }
                } catch (SecurityException) { }
                return defaultValue;
            }


            internal static string GetControlPath(System.Windows.Forms.Control control) {
                System.Text.StringBuilder sbPath = new System.Text.StringBuilder();

                System.Windows.Forms.Control currControl = control;
                while (true) {
                    System.Windows.Forms.Control parentControl = currControl.Parent;

                    if (parentControl == null) {
                        if (sbPath.Length > 0) { sbPath.Insert(0, "."); }
                        sbPath.Insert(0, currControl.GetType().FullName);
                        break;
                    } else {
                        if (string.IsNullOrEmpty(currControl.Name)) {
                            //throw new System.NotSupportedException("This control's parents cannot be resolved using Name property.");
                        } else {
                            if (sbPath.Length > 0) { sbPath.Insert(0, "."); }
                            sbPath.Insert(0, currControl.Name);
                        }
                    }

                    currControl = parentControl;
                }

                return sbPath.ToString();
            }

            private static bool IsRunningOnMono {
                get {
                    return (Type.GetType("Mono.Runtime") != null);
                }
            }

        }

    }

}
