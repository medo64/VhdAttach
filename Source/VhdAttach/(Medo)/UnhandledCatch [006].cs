//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2007-12-30: New version.
//2008-01-02: Added support for inner exceptions.
//            Added thread-safe locking.
//2008-01-03: Added Resources.
//2008-01-06: System.Environment.Exit returns E_UNEXPECTED (0x8000ffff).
//2008-01-13: Changed default mode to ThrowException.
//            Uses FailFast to exit application.
//2009-03-31: Changed FailFast to optional in order to avoid WER messages.


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
        public static event System.EventHandler<ThreadExceptionEventArgs> ThreadException;

        private static readonly object _syncRoot = new object();
        private static bool _useFailFast;

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        public static void Attach() {
            lock (_syncRoot) {
                Attach(System.Windows.Forms.UnhandledExceptionMode.ThrowException, true);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        public static void Attach(System.Windows.Forms.UnhandledExceptionMode mode) {
            lock (_syncRoot) {
                Attach(mode, true);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        /// <param name="threadScope">True to set the thread exception mode.</param>
        public static void Attach(System.Windows.Forms.UnhandledExceptionMode mode, bool threadScope) {
            lock (_syncRoot) {
                Attach(mode, threadScope, false);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        /// <param name="threadScope">True to set the thread exception mode.</param>
        /// <param name="useFailFast">When true, FailFast will be used to stop application. If false, standard Environment.Exit will be used instead.</param>
        public static void Attach(System.Windows.Forms.UnhandledExceptionMode mode, bool threadScope, bool useFailFast) {
            lock (_syncRoot) {
                _useFailFast = useFailFast;
                System.Windows.Forms.Application.SetUnhandledExceptionMode(mode, threadScope);
                System.Windows.Forms.Application.ThreadException += Application_ThreadException;
                System.AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            }
        }


        private static void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e) {
            lock (_syncRoot) {
                Process(e.ExceptionObject as System.Exception);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            lock (_syncRoot) {
                Process(e.Exception);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "General exceptions are catched on purpose because this is handler for unhandled exceptions.")]
        private static void Process(System.Exception exception) {
            lock (_syncRoot) {
                System.Environment.ExitCode = unchecked((int)0x8000ffff); //E_UNEXPECTED(0x8000ffff)

                if (exception != null) {
                    System.Diagnostics.Trace.TraceError(exception.ToString() + "  {Medo.Application.UnhandledCatch}");

                    System.Windows.Forms.Application.ThreadException -= Application_ThreadException;
                    System.AppDomain.CurrentDomain.UnhandledException -= AppDomain_UnhandledException;

                    if (ThreadException != null) { ThreadException(null, new ThreadExceptionEventArgs(exception)); }
                }

                System.Diagnostics.Trace.TraceError("Exit(E_UNEXPECTED): Unhandled exception has occurred.  {Medo.Application.UnhandledCatch}");

                if (_useFailFast) {
                    System.Environment.FailFast(exception.Message);
                } else {
                    System.Environment.Exit(unchecked((int)0x8000ffff)); //E_UNEXPECTED(0x8000ffff)
                }
            }
        }

    }

}
