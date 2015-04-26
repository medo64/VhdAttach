### VHD Attach ###

This is small tool that adds Attach and Detach option to contextual (aka
right-click) menu of the Virtual disk (vhd) files. That enables those
operations to be done without trip to Disk Management console and allows for
automatic disk attachment during sysstem startup.


#### Shortcut Keys ####

  * `F5`                      Refresh.
  * `F6`                      Attach.
  * `Ctrl+A`                  Select all.
  * `Ctrl+C`                  Copy.
  * `Ctrl+N`                  New file.
  * `Ctrl+O`                  Open file.
  * `Alt+A`                   Show attach menu.
  * `Alt+D`                   Detach.
  * `Alt+M`                   Auto-mount.
  * `Alt+O`                   Show open menu (recent files).


#### Command Line Parameters ####

    [/attach|/detach] "disk.vhd"

Attaches (or detaches) virtual disk using file disk.vhd.

    [/detachdrive] "X:"

Detaches virtual disk attached to drive letter.
