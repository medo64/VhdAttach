[Setup]
AppName=VHD Attach
AppVerName=VHD Attach 2.00
DefaultDirName={pf}\Josip Medved\VHD Attach
OutputBaseFilename=vhdattach200b1
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_VhdAttach
AppMutex=Global\JosipMedved_VhdAttach
AppPublisher=Josip Medved
AppPublisherURL=http://www.jmedved.com/?page=vhdattach
UninstallDisplayIcon={app}\VhdAttach.exe
AlwaysShowComponentsList=no
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
MergeDuplicateFiles=yes
MinVersion=0,6.01.7200
PrivilegesRequired=admin
ShowLanguageDialog=no
SolidCompression=yes
ChangesAssociations=yes
DisableWelcomePage=yes

[Files]
Source: "VhdAttach.exe";         DestDir: "{app}"; Flags: ignoreversion;
Source: "VhdAttachService.exe";  DestDir: "{app}"; Flags: ignoreversion;
Source: "ReadMe.txt";            DestDir: "{app}"; Attribs: readonly; Flags: overwritereadonly uninsremovereadonly;

[Icons]
Name: "{userstartmenu}\VHD Attach"; Filename: "{app}\VhdAttach.exe"

[Registry]
Root: HKCU; Subkey: "Software\Josip Medved\VhdAttach"; ValueType: dword; ValueName: "Installed"; ValueData: "1"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Josip Medved"; Flags: uninsdeletekeyifempty

Root: HKCR; Subkey: ".vhd"; ValueType: string; ValueName: ""; ValueData: "VhdAttachFile"; Flags: uninsclearvalue;

Root: HKCR; Subkey: "VhdAttachFile"; ValueType: none; Flags: uninsdeletekey;
Root: HKCR; Subkey: "VhdAttachFile"; ValueType: string; ValueName: ""; ValueData: "Virtual Disk";
Root: HKCR; Subkey: "VhdAttachFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\VhdAttach.exe";

Root: HKCR; Subkey: "VhdAttachFile\shell\Open"; ValueType: string; ValueName: "MultiSelectModel"; ValueData: "Player";
Root: HKCR; Subkey: "VhdAttachFile\shell\Open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\VhdAttach.exe"" ""%1""";

Root: HKCR; Subkey: "VhdAttachFile\shell\Attach"; ValueType: string; ValueName: "MultiSelectModel"; ValueData: "Player";
Root: HKCR; Subkey: "VhdAttachFile\shell\Attach\command"; ValueType: string; ValueName: ""; ValueData: """{app}\VhdAttach.exe"" /attach ""%1""";

Root: HKCR; Subkey: "VhdAttachFile\shell\Detach"; ValueType: string; ValueName: "MultiSelectModel"; ValueData: "Player";
Root: HKCR; Subkey: "VhdAttachFile\shell\Detach\command"; ValueType: string; ValueName: ""; ValueData: """{app}\VhdAttach.exe"" /detach ""%1""";

Root: HKCR; Subkey: "Drive\shell\Detach"; ValueType: none; Flags: deletekey;
Root: HKCR; Subkey: "Drive\shell\Detach drive"; ValueType: none; Flags: uninsdeletekey;
Root: HKCR; Subkey: "Drive\shell\Detach drive"; ValueType: string; ValueName: "MultiSelectModel"; ValueData: "Single";
Root: HKCR; Subkey: "Drive\shell\Detach drive\command"; ValueType: string; ValueName: ""; ValueData: """{app}\VhdAttach.exe"" /detachdrive ""%1""";

[Run]
Filename: "{app}\VhdAttachService.exe"; Parameters: "/Install"; Flags: runascurrentuser waituntilterminated
Filename: "{app}\ReadMe.txt"; Description: "View ReadMe.txt"; Flags: postinstall runasoriginaluser shellexec nowait skipifsilent unchecked
Filename: "{app}\VhdAttach.exe"; Description: "Launch application now"; Flags: postinstall nowait skipifsilent runasoriginaluser unchecked

[UninstallRun]
Filename: "{app}\VhdAttachService.exe"; Parameters: "/Uninstall"; Flags: runascurrentuser waituntilterminated



[Code]

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
    ResultCode: Integer;
begin
    Exec(ExpandConstant('{app}\VhdAttachService.exe'), '/Uninstall', '', SW_SHOW, ewWaitUntilTerminated, ResultCode)
end;

