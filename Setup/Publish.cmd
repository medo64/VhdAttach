@ECHO OFF

SET            FILE_SETUP=".\VhdAttach.iss"
SET         FILE_SOLUTION="..\Source\VhdAttach.sln"
SET      FILES_EXECUTABLE="..\Binaries\VhdAttach.exe" "..\Binaries\VhdAttachService.exe"
SET           FILES_OTHER="..\Binaries\ReadMe.txt"
SET   CERTIFICATE_SUBJECT="Josip Medved"
SET CERTIFICATE_TIMESTAMP="http://www.startssl.com/timestamp/"


ECHO --- BUILD SOLUTION
ECHO.

RMDIR /Q /S "..\Binaries" 2> NUL
"%PROGRAMFILES(X86)%\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe" /Build "Release" %FILE_SOLUTION%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- SIGN SOLUTION
ECHO.

IF [%CERTIFICATE_TIMESTAMP%]==[] (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /v %FILES_EXECUTABLE%
) ELSE (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /tr %CERTIFICATE_TIMESTAMP% /v %FILES_EXECUTABLE%
)
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- BUILD SETUP
ECHO.

RMDIR /Q /S ".\Temp" 2> NUL
CALL "%PROGRAMFILES(x86)%\Inno Setup 5\iscc.exe" /O".\Temp" %FILE_SETUP%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

FOR /F %%I IN ('DIR ".\Temp\*.exe" /B') DO SET _SETUPEXE=%%I
ECHO Setup is in file %_SETUPEXE%

ECHO.


ECHO --- SIGN SETUP
ECHO.

IF [%CERTIFICATE_TIMESTAMP%]==[] (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /v ".\Temp\%_SETUPEXE%"
) ELSE (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /tr %CERTIFICATE_TIMESTAMP% /v ".\Temp\%_SETUPEXE%"
)
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- RELEASE
ECHO.

MOVE ".\Temp\*.*" "..\Releases\."
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%
RMDIR /Q /S ".\Temp"

ECHO.


ECHO.
ECHO Done.
ECHO.

PAUSE
