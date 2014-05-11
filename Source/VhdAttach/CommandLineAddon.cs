/*
 * Command line Addon - Exposes additional functionality to the command line
 * Author: Ralf Ostertag, (C) 2014 Ralf Ostertag
 *  
 * ChangeLetter
 * Usage: VhdAttach -ChangeLetter VHDFilename DriveLetter 
 * Drive letter can be: L, L:, L:\, l, l:, l:\
 * 
 * 2014-04-27 First version (adds ChangeDriveLetter)
 * 
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using VhdAttachCommon;

namespace VhdAttach
{
    /// <summary>
    /// Exposes additional functionality to the command line
    /// </summary>
    class CommandLineAddon
    {
        /// <summary>
        /// Change the drive letter of an attached VHD.
        /// <summary>
        public int ChangeDriveLetter(string[] args)
        {

            // Init and parameter checks

            if (args.Length != 2) {
                string err = "Changing drive letter failed. Wrong number of arguments.\n";
                err += "Usage: VhdAttach -ChangeLetter VHDFilename Driveletter:";
                showError(err);
                return 1;
            }

            string fileName = args[0];

            if (!File.Exists(fileName)) {
                string err = "Changing drive letter failed. File not found: " + fileName + "\n";
                err += "Usage: VhdAttach -ChangeLetter VHDFilename Driveletter:";
                showError(err);
                return 1;
            }

            string driveLetterRaw = args[1];

            if (driveLetterRaw.Length > 3 || (driveLetterRaw.Length == 3 && driveLetterRaw.Substring(1,2) != ":\\") || (driveLetterRaw.Length == 2 && driveLetterRaw.Substring(1,1) != ":")) {
                string err = "Changing drive letter failed. Drive Letter parameter seems to be invalid.\n";
                err += "Usage: VhdAttach -ChangeLetter VHDFilename Driveletter:";
                showError(err);
                return 1;
            }

            string allowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string driveLetter1 = args[1].Substring(0, 1);
            string driveLetter2 = driveLetter1 + ":";

            if (allowedLetters.IndexOf(driveLetter1) == -1) {
                string err = "Changing drive letter failed. Drive Letter parameter seems to be invalid.\n";
                err += "Usage: VhdAttach -ChangeLetter VHDFilename Driveletter:";
                showError(err);
                return 1;
            }

            // Test-Code, please ignore
            // PipeClient.Attach(fileName, false, false);
            // PipeClient.Detach(fileName);

            // The actual drive letter operation begins here

            string attachedDevice = null;
            
            try {
                using (var document = new Medo.IO.VirtualDisk(fileName)) {
                    document.Open(Medo.IO.VirtualDiskAccessMask.GetInfo);
                    attachedDevice = document.GetAttachedPath();
                }
            } catch {
                string err = "Changing Drive letter failed. Could not open device.\n";
                err += "Possible reasons include:\n";
                err += "The specified file might not be attached or maybe isn't an VHD file.";
                showError(err);
                return 1;
            }

            if (attachedDevice != null) {
                var volumes = new List<Volume>(Volume.GetVolumesOnPhysicalDrive(attachedDevice));
                var availableVolumes = new List<Volume>();
                if (volumes != null && volumes.Count > 0) {
                    string oldLetter2 = volumes[0].DriveLetter2;
                    PipeResponse res = PipeClient.ChangeDriveLetter(volumes[0].VolumeName, driveLetter2);
                    if (res.IsError == true) {
                        PipeClient.ChangeDriveLetter(volumes[0].VolumeName, oldLetter2);
                        string err = "Changing drive letter failed. Drive letter possibly in use.";
                        showError(err);
                        return 1;
                    }
                }
            }

            return 0;
        }


        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.Dll")]
        static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        /// <summary>
        /// Error MessageBox with timeout
        /// </summary>
        private void showError(string errorText)
        {
            string caption = "VhdAttach Error";
            var timer = new System.Timers.Timer(5000) { AutoReset = false };
            timer.Elapsed += delegate
            {
                IntPtr hWnd = FindWindowByCaption(IntPtr.Zero, caption);
                if (hWnd.ToInt32() != 0) PostMessage(hWnd, WM_CLOSE, 0, 0);
            };
            timer.Enabled = true;
            MessageBox.Show(errorText, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

    }
}
