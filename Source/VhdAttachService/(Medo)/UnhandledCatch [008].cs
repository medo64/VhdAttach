//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2007-12-30: New version.
//2008-01-02: Added support for inner exceptions.
//            Added thread-safe locking.
//2008-01-03: Added Resources.
//2008-01-06: System.Environment.Exit returns E_UNEXPECTED (0x8000ffff).
//2008-01-13: Changed default mode to ThrowException.
//            Uses FailFast to exit application.
//2009-03-31: Changed FailFast to optional in order to avoid WER messages.
//2010-11-07: Compatible with Mono (ignoring FailFast).
//2010-11-22: Changed default exception mode to CatchException.


using System;
using System.Threading;

namespace Medo.Application {

    /// <summary>
    /// Handling of unhandled errors.
    /// This class is thread-safe.
    /// </summary>
    public static class UnhandledCatch {

        /// <summary>
        /// Occurs when an exception is not caught.
        /// </summary>
        public static event EventHandler<ThreadExceptionEventArgs> ThreadException;

        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        public static void Attach() {
            lock (SyncRoot) {
                Attach(System.Windows.Forms.UnhandledExceptionMode.CatchException, true);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        public static void Attach(System.Windows.Forms.UnhandledExceptionMode mode) {
            lock (SyncRoot) {
                Attach(mode, true);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        /// <param name="threadScope">True to set the thread exception mode.</param>
        public static void Attach(System.Windows.Forms.UnhandledExceptionMode mode, bool threadScope) {
            lock (SyncRoot) {
                System.Windows.Forms.Application.SetUnhandledExceptionMode(mode, threadScope);
                System.Windows.Forms.Application.ThreadException += Application_ThreadException;
                System.AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        /// <param name="threadScope">True to set the thread exception mode.</param>
        /// <param name="useFailFast">When true, FailFast will be used to stop application. If false, standard Environment.Exit will be used instead.</param>
        [Obsolete("Use UseFailFast property instead.")]
        public static void Attach(System.Windows.Forms.UnhandledExceptionMode mode, bool threadScope, bool useFailFast) {
            lock (SyncRoot) {
                UnhandledCatch.UseFailFast = useFailFast;
                System.Windows.Forms.Application.SetUnhandledExceptionMode(mode, threadScope);
                System.Windows.Forms.Application.ThreadException += Application_ThreadException;
                System.AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            }
        }

        private static bool _useFailFast;
        /// <summary>
        /// Gets/sets whether to use FailFast terminating application.
        /// Under Mono this property always remains false.
        /// </summary>
        public static bool UseFailFast {
            get { lock (SyncRoot) { return _useFailFast; } }
            set { lock (SyncRoot) { _useFailFast = value && !IsRunningOnMono; } }
        }

        private static void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e) {
            lock (SyncRoot) {
                Process(e.ExceptionObject as System.Exception);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            lock (SyncRoot) {
                Process(e.Exception);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "General exceptions are catched on purpose because this is handler for unhandled exceptions.")]
        private static void Process(System.Exception exception) {
            lock (SyncRoot) {
                System.Environment.ExitCode = unchecked((int)0x8000ffff); //E_UNEXPECTED(0x8000ffff)

                if (exception != null) {
                    System.Diagnostics.Trace.TraceError(exception.ToString() + "  {Medo.Application.UnhandledCatch}");

                    System.Windows.Forms.Application.ThreadException -= Application_ThreadException;
                    System.AppDomain.CurrentDomain.UnhandledException -= AppDomain_UnhandledException;

                    if (ThreadException != null) { ThreadException(null, new ThreadExceptionEventArgs(exception)); }
                }

                System.Diagnostics.Trace.TraceError("Exit(E_UNEXPECTED): Unhandled exception has occurred.  {Medo.Application.UnhandledCatch}");

                if (UnhandledCatch.UseFailFast) {
                    System.Environment.FailFast(exception.Message);
                } else {
                    System.Environment.Exit(unchecked((int)0x8000ffff)); //E_UNEXPECTED(0x8000ffff)
                }
            }
        }

        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }

}
