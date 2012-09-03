                                                        Josip Medved
                                                        http://www.jmedved.com/

                                   VHD Attach


SHORTCUT KEYS

    Alt+O                   Show open menu (recent files).
    Alt+A                   Show attach menu.
    Alt+D                   Detach.
    Alt+M                   Auto-mount.

    Ctrl+N                  New file.
    Ctrl+O                  Open file.
    F6                      Attach.

    F5                      Refresh.

    Ctrl+C                  Copy.
    Ctrl+A                  Select all.


COMMAND LINE PARAMETERS

    [/attach|/detach] "disk.vhd"

        Attaches (or detaches) virtual disk using file disk.vhd.

    [/detachdrive] "X:"

        Detaches virtual disk attached to drive letter.


VERSION HISTORY

    3.61 (2012-09-03)

        o Bug-fixing.

    3.60 (2012-08-20)

        o Improved Windows 8 compatibility.
        o Bug-fixing.

    3.50 (2012-07-20)

        o Added drive-letter change from within application.

    3.41 (2012-06-19)

        o Bug-fixing.

    3.40 (2012-06-16)

        o New icon.
        o Improving new disk dialog.
        o Bug-fixing.

    3.31 (2012-05-31)

        o Bug-fixing.

    3.30 (2012-05-30)

        o Added header parsing for dynamic virtual disks.
        o Auto-mounted VHD files are always added to Recent list.
        o Adding taskbar progress for disk creation.
        o Bug-fixing.

    3.20 (2012-03-15)

        o Added read-only mounting on startup.
        o Added experimental Windows 8 support (with ISO mounting).

    3.11 (2012-02-27)

        o Bug-fixing.

    3.10 (2012-02-18)

        o Possible to create fixed virtual disks.
        o Requires only .NET 2.0.
        o Refreshing GUI.

    3.02 (2012-01-24)

        o Removing application config files to simplify installation.

    3.01 (2012-01-16)

        o Works on Windows Thin PC (after .NET Framework 4.0 installation).

    3.00 (2012-01-14)

        o Added dynamic disk creation.
        o Some interface tweaks.
        o Added upgrade procedure.
        o Setup improvements.

    2.10 (2011-09-01)

        o Added readonly mounting.
        o Bug-fixing.

    2.01 (2011-04-04)

        o Fixed bug that caused inability to save settings.

    2.00 (2011-03-21)

        o Virtual disk service is used instead of parsing DISKPART.
        o Auto-mount is now directly on toolbar.
        o General bug-fixing.

    1.70 (2010-11-14)

        o Bugfix: Service returns proper code on crash.
        o Bugfix: Pipe name changed to VhdAttach.

    1.60 (2010-04-18)

        o Added auto-mounting.
        o Explorer context menu items can be enabled/disabled.
        o Removed menu.
        o Bugfix: Works on paths longer than 200 characters.

    1.50 (2010-02-14)

        o VHD Attach now works as service (no more anoying UAC prompts).
        o Improved behaviour on non-English locales.

    1.00 (2009-09-06)

        o First public release.


LICENCE (MIT)

    Copyright (c) 2009 Josip Medved <jmedved@jmedved.com>

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to
    deal in the Software without restriction, including without limitation the
    rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
    sell copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
        o The above copyright notice and this permission notice shall be
          included in all copies or substantial portions of the Software.
        
        o THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
          EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
          MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
          IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
          CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
          TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
          SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
