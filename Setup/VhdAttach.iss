[Setup]
AppName=VHD Attach
AppVerName=VHD Attach 3.00
DefaultDirName={pf}\Josip Medved\VHD Attach
OutputBaseFilename=vhdattach300
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_VhdAttach
AppMutex=Global\JosipMedved_VhdAttach
AppPublisher=Josip Medved
AppPublisherURL=http://www.jmedved.com/vhdattach/
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
Source: "VhdAttach.exe";         DestDir: "{app}";                      Flags: ignoreversion;
Source: "VhdAttachService.exe";  DestDir: "{app}";                      Flags: ignoreversion;
Source: "ReadMe.txt";            DestDir: "{app}";  Attribs: readonly;  Flags: overwritereadonly uninsremovereadonly;


[Tasks]
Name: context_attach;          GroupDescription: "Context menu items:";  Description: "Attach";
Name: context_attachreadonly;  GroupDescription: "Context menu items:";  Description: "Attach read-only";  Flags: unchecked;
Name: context_detach;          GroupDescription: "Context menu items:";  Description: "Detach";
Name: context_detachdrive;     GroupDescription: "Context menu items:";  Description: "Detach drive";      Flags: unchecked;


[Icons]
Name: "{userstartmenu}\VHD Attach";  Filename: "{app}\VhdAttach.exe"

[Registry]
Root: HKCU;  Subkey: "Software\Josip Medved";                                                                                                          Flags: uninsdeletekeyifempty
Root: HKCU;  Subkey: "Software\Josip Medved\VhdAttach";               ValueType: none;                                                                 Flags: deletekey uninsdeletekey;
Root: HKCU;  Subkey: "Software\Josip Medved\VHD Attach";              ValueType: dword;   ValueName: "Installed";         ValueData: "1";              Flags: uninsdeletekey

Root: HKCR;  Subkey: ".vhd";                                          ValueType: string;  ValueName: "";                  ValueData: "VhdAttachFile";  Flags: uninsclearvalue;

Root: HKCR;  Subkey: "VhdAttachFile";                                 ValueType: none;    Flags: uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile";                                 ValueType: string;  ValueName: "";                  ValueData: "Virtual Disk";

Root: HKCR;  Subkey: "VhdAttachFile\shell\Open";                      ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Open";                      ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";
Root: HKCR;  Subkey: "VhdAttachFile\shell\Open";                      ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Player";
Root: HKCR;  Subkey: "VhdAttachFile\shell\Open\command";              ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" ""%1""";

Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                    ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                    ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_attach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                    ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Player";                                            Tasks: context_attach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach\command";            ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /attach ""%1""";            Tasks: context_attach;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";          ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";          ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_attachreadonly;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";          ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Player";                                            Tasks: context_attachreadonly;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only\command";  ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /readonly /attach ""%1""";  Tasks: context_attachreadonly;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                    ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                    ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_detach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                    ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Player";                                            Tasks: context_detach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach\command";            ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detach ""%1""";            Tasks: context_detach;

Root: HKCR;  Subkey: "Drive\shell\Detach";                            ValueType: none;    Flags: deletekey;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                      ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                      ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                      ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Single";                                            Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\Detach drive\command";              ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detachdrive ""%1""";       Tasks: context_detachdrive;


[Run]
Filename: "{app}\VhdAttachService.exe";  Parameters: "/Install";  Flags: runascurrentuser waituntilterminated;
Filename: "{app}\ReadMe.txt";                                     Flags: postinstall runasoriginaluser shellexec nowait skipifsilent unchecked;  Description: "View ReadMe.txt";
Filename: "{app}\VhdAttach.exe";                                  Flags: postinstall nowait skipifsilent runasoriginaluser unchecked;            Description: "Launch application now";

[UninstallRun]
Filename: "{app}\VhdAttachService.exe";  Parameters: "/Uninstall";  Flags: runascurrentuser waituntilterminated



[Code]

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
    ResultCode: Integer;
begin
    Exec(ExpandConstant('{app}\VhdAttachService.exe'), '/Uninstall', '', SW_SHOW, ewWaitUntilTerminated, ResultCode)
end;
