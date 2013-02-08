@ECHO OFF

SET        FILE_SETUP=".\VhdAttach.iss"
SET     FILE_SOLUTION="..\Source\VhdAttach.sln"
SET  FILES_EXECUTABLE="..\Binaries\VhdAttach.exe" "..\Binaries\VhdAttachService.exe"
SET       FILES_OTHER="..\Binaries\ReadMe.txt"

SET      COMPILE_TOOL="%PROGRAMFILES(X86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe"
SET        SETUP_TOOL="%PROGRAMFILES(x86)%\Inno Setup 5\iscc.exe"

SET         SIGN_TOOL="%PROGRAMFILES(X86)%\Windows Kits\8.0\bin\x86\signtool.exe"
SET         SIGN_HASH="EB41D6069805B20D87219E0757E07836FB763958"
SET SIGN_TIMESTAMPURL="http://www.startssl.com/timestamp/"


ECHO --- BUILD SOLUTION
ECHO.

RMDIR /Q /S "..\Binaries" 2> NUL
%COMPILE_TOOL% /Build "Release" %FILE_SOLUTION%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


CERTUTIL -silent -verifystore -user My %SIGN_HASH% > NUL
IF %ERRORLEVEL%==0 (
    ECHO --- SIGN SOLUTION
    ECHO.
    
    IF [%SIGN_TIMESTAMPURL%]==[] (
        %SIGN_TOOL% sign /s "My" /sha1 %SIGN_HASH% /v %FILES_EXECUTABLE%
    ) ELSE (
        %SIGN_TOOL% sign /s "My" /sha1 %SIGN_HASH% /tr %SIGN_TIMESTAMPURL% /v %FILES_EXECUTABLE%
    )
    IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%
) ELSE (
    ECHO --- DID NOT SIGN SOLUTION
    IF NOT [%SIGN_HASH%]==[] (
        ECHO.
        ECHO No certificate with hash %SIGN_HASH%.
    ) 
)
ECHO.
ECHO.


ECHO --- BUILD SETUP
ECHO.

RMDIR /Q /S ".\Temp" 2> NUL
CALL %SETUP_TOOL% /O".\Temp" %FILE_SETUP%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

FOR /F %%I IN ('DIR ".\Temp\*.exe" /B') DO SET _SETUPEXE=%%I
ECHO Setup is in file %_SETUPEXE%

ECHO.
ECHO.


CERTUTIL -silent -verifystore -user My %SIGN_HASH% > NUL
IF %ERRORLEVEL%==0 (
    ECHO --- SIGN SETUP
    ECHO.
    
    IF [%SIGN_TIMESTAMPURL%]==[] (
        %SIGN_TOOL% sign /s "My" /sha1 %SIGN_HASH% /v ".\Temp\%_SETUPEXE%"
    ) ELSE (
        %SIGN_TOOL% sign /s "My" /sha1 %SIGN_HASH% /tr %SIGN_TIMESTAMPURL% /v ".\Temp\%_SETUPEXE%"
    )
    IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%
) ELSE (
    ECHO --- DID NOT SIGN SETUP
)
ECHO.
ECHO.


ECHO --- RELEASE
ECHO.

MKDIR "..\Releases" 2> NUL
MOVE ".\Temp\*.*" "..\Releases\." > NUL
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%
RMDIR /Q /S ".\Temp"

ECHO.


ECHO --- DONE
ECHO.

PAUSE
