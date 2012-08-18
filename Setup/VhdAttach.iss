#define AppName        GetStringFileInfo('..\Binaries\VhdAttach.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\Binaries\VhdAttach.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\Binaries\VhdAttach.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\Binaries\VhdAttach.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\Binaries\VhdAttach.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppSetupFile   AppBase + StringChange(AppVersion, '.', '')

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=http://www.jmedved.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersion}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={pf}\{#AppCompany}\{#AppName}
OutputBaseFilename={#AppSetupFile}
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_VhdAttach
AppMutex=Global\JosipMedved_VhdAttach
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
Source: "VhdAttach.exe";                DestDir: "{app}";                      Flags: ignoreversion;
Source: "VhdAttachService.exe";         DestDir: "{app}";                      Flags: ignoreversion;
Source: "ReadMe.txt";                   DestDir: "{app}";  Attribs: readonly;  Flags: overwritereadonly uninsremovereadonly;


[Tasks]
Name: context_attach;              GroupDescription: "Context menu items:";      Description: "Attach";                               OnlyBelowVersion: 6.2
Name: context_attachreadonly;      GroupDescription: "Context menu items:";      Description: "Attach read-only";  Flags: unchecked;  OnlyBelowVersion: 6.2
Name: context_detach;              GroupDescription: "Context menu items:";      Description: "Detach";                               OnlyBelowVersion: 6.2
Name: context_detachdrive;         GroupDescription: "Context menu items:";      Description: "Detach drive";      Flags: unchecked;  OnlyBelowVersion: 6.2

Name: context_vhd_attach;          GroupDescription: "VHD context menu items:";  Description: "Attach";                               MinVersion: 6.2
Name: context_vhd_attachreadonly;  GroupDescription: "VHD context menu items:";  Description: "Attach read-only";  Flags: unchecked;  MinVersion: 6.2
Name: context_vhd_detach;          GroupDescription: "VHD context menu items:";  Description: "Detach";                               MinVersion: 6.2
Name: context_iso_attachreadonly;  GroupDescription: "ISO context menu items:";  Description: "Attach";                               MinVersion: 6.2
Name: context_iso_detach;          GroupDescription: "ISO context menu items:";  Description: "Detach";                               MinVersion: 6.2


[Icons]
Name: "{userstartmenu}\VHD Attach";  Filename: "{app}\VhdAttach.exe"


[Registry]
Root: HKCU;  Subkey: "Software\Josip Medved";                                                                                                          Flags: uninsdeletekeyifempty
Root: HKCU;  Subkey: "Software\Josip Medved\VhdAttach";               ValueType: none;                                                                 Flags: deletekey uninsdeletekey;
Root: HKCU;  Subkey: "Software\Josip Medved\VHD Attach";              ValueType: none;    ValueName: "Installed";                                      Flags: deletevalue uninsdeletevalue
Root: HKLM;  Subkey: "Software\Josip Medved\VHD Attach";              ValueType: dword;   ValueName: "Installed";         ValueData: "1";              Flags: uninsdeletekey

Root: HKCR;  Subkey: ".vhd";                                          ValueType: string;  ValueName: "";                  ValueData: "VhdAttachFile";  Flags: uninsclearvalue;            Tasks: context_attach context_attachreadonly context_detach context_detachdrive context_vhd_attach context_vhd_attachreadonly context_vhd_detach;

Root: HKCR;  Subkey: "VhdAttachFile";                                 ValueType: none;    Flags: uninsdeletekey;                                                                          Tasks: context_attach context_attachreadonly context_detach context_detachdrive context_vhd_attach context_vhd_attachreadonly context_vhd_detach;
Root: HKCR;  Subkey: "VhdAttachFile";                                 ValueType: string;  ValueName: "";                  ValueData: "Virtual Disk";                                      Tasks: context_attach context_attachreadonly context_detach context_detachdrive context_vhd_attach context_vhd_attachreadonly context_vhd_detach;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Open";                      ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Open";                      ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";
Root: HKCR;  Subkey: "VhdAttachFile\shell\Open";                      ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Player";
Root: HKCR;  Subkey: "VhdAttachFile\shell\Open\command";              ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" ""%1""";

Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                    ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                    ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_attach context_vhd_attach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                    ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_attach context_vhd_attach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach\command";            ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /attach ""%1""";            Tasks: context_attach context_vhd_attach;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";          ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";          ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_attachreadonly context_vhd_attachreadonly;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";          ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_attachreadonly context_vhd_attachreadonly;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only\command";  ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /readonly /attach ""%1""";  Tasks: context_attachreadonly context_vhd_attachreadonly;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                    ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                    ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_detach context_vhd_detach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                    ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_detach context_vhd_detach;
Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach\command";            ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detach ""%1""";            Tasks: context_detach context_vhd_detach;

Root: HKCR;  Subkey: "Drive\shell\Detach";                            ValueType: none;    Flags: deletekey;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                      ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                      ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                      ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Single";                                            Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\Detach drive\command";              ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detachdrive ""%1""";       Tasks: context_detachdrive;


Root: HKCR;  Subkey: ".iso\OpenWithProgids";                          ValueType: string;  ValueName: "Windows.IsoFile";   ValueData: "";                                                  Tasks: context_iso_attachreadonly context_iso_detach;

Root: HKCR;  Subkey: "Windows.IsoFile\shell\Attach";                  ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_iso_attachreadonly;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\Attach";                  ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_iso_attachreadonly;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\Attach\command";          ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /readonly /attach ""%1""";  Tasks: context_iso_attachreadonly;

Root: HKCR;  Subkey: "Windows.IsoFile\shell\Detach";                  ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_iso_detach;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\Detach";                  ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_iso_detach;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\Detach\command";          ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detach ""%1""";            Tasks: context_iso_detach;


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
    Result := Result;
end;
