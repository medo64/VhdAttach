//Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com  http://medo64.blogspot.com

//2009-07-01: New version.


using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Medo.Windows.Forms {

    /// <summary>
    /// 
    /// </summary>
    public static class TaskbarProgress {

        private static readonly object _syncRoot = new object();


        /// <summary>
        /// Gets/sets default window for progress indicator.
        /// </summary>
        public static IWin32Window DefaultOwner { get; set; }

        /// <summary>
        /// Gets/sets whether NotImplementedException will be thrown in case of Operating System earlier than Windows 7.
        /// </summary>
        public static bool DoNotThrowNotImplementedException { get; set; }


        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button.
        /// </summary>
        /// <param name="newState">New progress state.</param>
        /// <exception cref="System.InvalidOperationException">Default owner must be set before calling this method -or- Native error..</exception>
        /// <exception cref="System.NotImplementedException">Operation is only supported on Windows 7 and above.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Native error.</exception>
        public static void SetState(TaskbarProgressState newState) {
            if (DefaultOwner == null) { throw new InvalidOperationException("Default owner must be set before calling this method."); }
            SetState(DefaultOwner, newState);
        }

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button.
        /// </summary>
        /// <param name="owner">The window in which the progress of an operation is being shown. This window's associated taskbar button will display the progress bar. If owner is null, default owner is used.</param>
        /// <param name="newState">New progress state.</param>
        /// <exception cref="System.ArgumentNullException">Owner cannot be null.</exception>
        /// <exception cref="System.NotImplementedException">Operation is only supported on Windows 7 and above.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Native error.</exception>
        public static void SetState(IWin32Window owner, TaskbarProgressState newState) {
            if (owner == null) { throw new ArgumentNullException("owner", "Owner cannot be null."); }
            if (TaskbarProgress.IsRunningOnMono) { return; } //Mono has troubles with accessing COM.
            if (System.Environment.OSVersion.Version.Build < 7000) {
                if (DoNotThrowNotImplementedException) {
                    return;
                } else {
                    throw new NotImplementedException("Operation is only supported on Windows 7 and above.");
                }
            }
            Init();
            var res = _taskbarList.SetProgressState(owner.Handle, newState);
            if (res != NativeMethods.S_OK) { throw new Win32Exception(string.Format(CultureInfo.InvariantCulture, "Native error {0:x8}.", res)); }
        }

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="newProgressPercentage">Percentage to show.</param>
        /// <exception cref="System.InvalidOperationException">Default owner must be set before calling this method.</exception>
        /// <exception cref="System.NotImplementedException">Operation is only supported on Windows 7 and above.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Native error.</exception>
        public static void SetPercentage(int newProgressPercentage) {
            if (DefaultOwner == null) { throw new InvalidOperationException("Default owner must be set before calling this method."); }
            SetPercentage(DefaultOwner, newProgressPercentage);
        }

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="owner">The window in which the progress of an operation is being shown. This window's associated taskbar button will display the progress bar. If owner is null, default owner is used.</param>
        /// <param name="newProgressPercentage">Percentage to show.</param>
        /// <exception cref="System.ArgumentNullException">Owner cannot be null.</exception>
        /// <exception cref="System.NotImplementedException">Operation is only supported on Windows 7 and above.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Native error.</exception>
        public static void SetPercentage(IWin32Window owner, int newProgressPercentage) {
            if (owner == null) { throw new ArgumentNullException("owner", "Owner cannot be null."); }
            if (TaskbarProgress.IsRunningOnMono) { return; } //Mono has troubles with accessing COM.
            if (System.Environment.OSVersion.Version.Build < 7000) {
                if (DoNotThrowNotImplementedException) {
                    return;
                } else {
                    throw new NotImplementedException("Operation is only supported on Windows 7 and above.");
                }
            }
            Init();
            var res = _taskbarList.SetProgressValue(owner.Handle, (ulong)newProgressPercentage, 100);
            if (res != NativeMethods.S_OK) { throw new Win32Exception(string.Format(CultureInfo.InvariantCulture, "Native error {0:x8}.", res)); }
        }



        private static NativeMethods.ITaskbarList3 _taskbarList;
        private static void Init() {
            lock (_syncRoot) {
                if (_taskbarList == null) {
                    _taskbarList = (NativeMethods.ITaskbarList3)new NativeMethods.CTaskbarList();
                    var res = _taskbarList.HrInit();
                    if (res != NativeMethods.S_OK) { throw new Win32Exception(string.Format(CultureInfo.InvariantCulture, "Native error {0:x8}.", res)); }
                }
            }
        }


        private static class NativeMethods {

            internal const Int32 S_OK = 0x00000000;

            [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
            [ClassInterfaceAttribute(ClassInterfaceType.None)]
            [ComImportAttribute()]
            internal class CTaskbarList { }

            [ComImportAttribute()]
            [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
            [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
            internal interface ITaskbarList3 {
                Int32 HrInit();
                Int32 AddTab(IntPtr hwnd);
                Int32 DeleteTab(IntPtr hwnd);
                Int32 ActivateTab(IntPtr hwnd);
                Int32 SetActiveAlt(IntPtr hwnd);
                Int32 MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
                Int32 SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
                Int32 SetProgressState(IntPtr hwnd, TaskbarProgressState tbpFlags);
                Int32 RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
                Int32 UnregisterTab(IntPtr hwndTab);
                Int32 SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
                Int32 ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
                Int32 SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
                Int32 SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
            }

        }


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }



    /// <summary>
    /// Flags that control the current state of the progress button. Specify only one of the following flags; all states are mutually exclusive of all others.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "These values are not flags. Only one can be set at any given time.")]
    public enum TaskbarProgressState {
        /// <summary>
        /// Stops displaying progress and returns the button to its normal state. Call this method with this flag to dismiss the progress bar when the operation is complete or cancelled.
        /// </summary>
        NoProgress = 0,
        /// <summary>
        /// The progress indicator does not grow in size, but cycles repeatedly along the length of the taskbar button. This indicates activity without specifying what proportion of the progress is complete. Progress is taking place, but there is no prediction as to how long the operation will take.
        /// </summary>
        Indeterminate = 0x1,
        /// <summary>
        /// The progress indicator grows in size from left to right in proportion to the estimated amount of the operation completed. This is a determinate progress indicator; a prediction is being made as to the duration of the operation.
        /// </summary>
        Normal = 0x2,
        /// <summary>
        /// The progress indicator turns red to show that an error has occurred in one of the windows that is broadcasting progress. This is a determinate state. If the progress indicator is in the indeterminate state, it switches to a red determinate display of a generic percentage not indicative of actual progress.
        /// </summary>
        Error = 0x4,
        /// <summary>
        /// The progress indicator turns yellow to show that progress is currently stopped in one of the windows but can be resumed by the user. No error condition exists and nothing is preventing the progress from continuing. This is a determinate state. If the progress indicator is in the indeterminate state, it switches to a yellow determinate display of a generic percentage not indicative of actual progress.
        /// </summary>
        Paused = 0x8
    }

}