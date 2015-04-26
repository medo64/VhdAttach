### Building Project ###

First step would be to get current version of code from BitBucket.

After that you can download and install required tools.


#### Required Tools ####

* [Visual Studio Express 2013 for Windows Desktop](http://www.visualstudio.com/downloads/download-visual-studio-vs#d-express-windows-desktop)
* [Inno setup](http://www.jrsoftware.org/isinfo.php)
* [Sign tool](http://msdn.microsoft.com/en-us/library/windows/desktop/aa387764.aspx)


#### Script Modifications ####

In `Publish.cmd` check location of Visual Studio (`COMPILE_TOOL` variable) and
location of Sign Tool (`SIGN_TOOL` variable).

In case you have certificate that you want to use for signing, update it also
(`SIGN_HASH` variable). In case you don't have a certificate, it is ok, script
will just avoid signing executables.

After this you can start script and it will compile project and make its setup.
Executables will be in `Binaries` directory and setup will be in `Releases`.
