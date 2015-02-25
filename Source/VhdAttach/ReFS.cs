using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace VhdAttach {
    internal static class ReFS {

        public static void RemoveIntegrityStream(string fileName) {
            using (var handle = NativeMethods.CreateFile(fileName, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, FileShare.None, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero)) {
                RemoveIntegrityStream(handle);
            }
        }

        public static void RemoveIntegrityStream(SafeFileHandle handle) {
            var oldInfo = new NativeMethods.FSCTL_GET_INTEGRITY_INFORMATION_BUFFER();
            var oldInfoSizeReturn = 0;
            if (!NativeMethods.DeviceIoControl(handle, NativeMethods.FSCTL_GET_INTEGRITY_INFORMATION, IntPtr.Zero, 0, ref oldInfo, Marshal.SizeOf(oldInfo), out oldInfoSizeReturn, IntPtr.Zero)) {
                try {
                    throw new Win32Exception();
                } catch (Win32Exception ex) {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }

            if (oldInfo.ChecksumAlgorithm == NativeMethods.CHECKSUM_TYPE_NONE) { return; } //already done

            var newInfo = new NativeMethods.FSCTL_SET_INTEGRITY_INFORMATION_BUFFER() { ChecksumAlgorithm = NativeMethods.CHECKSUM_TYPE_NONE, Flags = oldInfo.Flags };
            var newInfoSizeReturn = 0;
            if (!NativeMethods.DeviceIoControl(handle, NativeMethods.FSCTL_SET_INTEGRITY_INFORMATION, ref newInfo, Marshal.SizeOf(newInfo), IntPtr.Zero, 0, out newInfoSizeReturn, IntPtr.Zero)) {
                try {
                    throw new Win32Exception();
                } catch (Win32Exception ex) {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }


        private static class NativeMethods {

            internal const Int32 CHECKSUM_TYPE_NONE = 0;
            internal const Int32 CHECKSUM_TYPE_CRC64 = 2;

            internal const Int32 GENERIC_READ = unchecked((Int32)0x80000000);
            internal const Int32 GENERIC_WRITE = 0x40000000;

            internal const Int32 FSCTL_GET_INTEGRITY_INFORMATION = 0x9027c;
            internal const Int32 FSCTL_SET_INTEGRITY_INFORMATION = 0x9c280;


            [StructLayout(LayoutKind.Sequential)]
            internal struct FSCTL_GET_INTEGRITY_INFORMATION_BUFFER {
                internal Int16 ChecksumAlgorithm;
                private Int16 Reserved;
                internal Int32 Flags;
                internal Int32 ChecksumChunkSizeInBytes;
                internal Int32 ClusterSizeInBytes;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct FSCTL_SET_INTEGRITY_INFORMATION_BUFFER {
                internal Int16 ChecksumAlgorithm;
                private Int16 Reserved;
                internal Int32 Flags;
            }


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(
                String lpFileName,
                Int32 dwDesiredAccess,
                FileShare dwShareMode,
                IntPtr lpSecurityAttributes,
                FileMode dwCreationDisposition,
                Int32 dwFlagsAndAttributes,
                IntPtr hTemplateFile
            );

            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean DeviceIoControl(
                SafeFileHandle hDevice,
                Int32 dwIoControlCode,
                IntPtr lpInBuffer,
                Int32 nInBufferSize,
                ref FSCTL_GET_INTEGRITY_INFORMATION_BUFFER lpOutBuffer,
                Int32 nOutBufferSize,
                out Int32 lpBytesReturned,
                IntPtr lpOverlapped
            );

            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean DeviceIoControl(
                SafeFileHandle hDevice,
                Int32 dwIoControlCode,
                ref FSCTL_SET_INTEGRITY_INFORMATION_BUFFER lpInBuffer,
                Int32 nInBufferSize,
                IntPtr lpOutBuffer,
                Int32 nOutBufferSize,
                out Int32 lpBytesReturned,
                IntPtr lpOverlapped
            );
        }

    }
}
