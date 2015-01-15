#define AppName        GetStringFileInfo('..\Binaries\VhdAttach.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\Binaries\VhdAttach.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\Binaries\VhdAttach.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\Binaries\VhdAttach.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\Binaries\VhdAttach.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppSetupFile   AppBase + StringChange(AppVersion, '.', '')

#define AppVersionEx   StringChange(AppVersion, '0.00', '')
#if "" != HgNode
#  define AppVersionEx AppVersionEx + " (" + HgNode + ")"
#endif


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=http://jmedved.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersionEx}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={pf}\{#AppCompany}\{#AppName}
OutputBaseFilename={#AppSetupFile}
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_VhdAttach
CloseApplications="yes"
RestartApplications="no"
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
LicenseFile=..\Setup\License.txt


[Messages]
SetupAppTitle=Setup {#AppName} {#AppVersionEx}
SetupWindowTitle=Setup {#AppName} {#AppVersionEx}
BeveledLabel=jmedved.com


[Files]
Source: "VhdAttach.exe";                DestDir: "{app}";                      Flags: ignoreversion;
Source: "VhdAttachService.exe";         DestDir: "{app}";                      Flags: ignoreversion;
Source: "ReadMe.txt";                   DestDir: "{app}";  Attribs: readonly;  Flags: overwritereadonly uninsremovereadonly;


[Tasks]
Name: context_open;                GroupDescription: "Context menu items:";      Description: "Open";                                 OnlyBelowVersion: 6.2;
Name: context_attach;              GroupDescription: "Context menu items:";      Description: "Attach";                               OnlyBelowVersion: 6.2;
Name: context_attachreadonly;      GroupDescription: "Context menu items:";      Description: "Attach read-only";  Flags: unchecked;  OnlyBelowVersion: 6.2;
Name: context_detach;              GroupDescription: "Context menu items:";      Description: "Detach";                               OnlyBelowVersion: 6.2;
Name: context_detachdrive;         GroupDescription: "Context menu items:";      Description: "Detach drive";      Flags: unchecked;  OnlyBelowVersion: 6.2;

Name: context_vhd_open;            GroupDescription: "VHD context menu items:";  Description: "Open";                                 MinVersion: 6.2;
Name: context_vhd_attach;          GroupDescription: "VHD context menu items:";  Description: "Attach";                               MinVersion: 6.2;
Name: context_vhd_attachreadonly;  GroupDescription: "VHD context menu items:";  Description: "Attach read-only";  Flags: unchecked;  MinVersion: 6.2;
Name: context_vhd_detach;          GroupDescription: "VHD context menu items:";  Description: "Detach";                               MinVersion: 6.2;
Name: context_iso_open;            GroupDescription: "ISO context menu items:";  Description: "Open";                                 MinVersion: 6.2;
Name: context_iso_attachreadonly;  GroupDescription: "ISO context menu items:";  Description: "Attach";                               MinVersion: 6.2;
Name: context_iso_detach;          GroupDescription: "ISO context menu items:";  Description: "Detach";                               MinVersion: 6.2;


[Icons]
Name: "{userstartmenu}\VHD Attach";  Filename: "{app}\VhdAttach.exe"


[Registry]
Root: HKCU;  Subkey: "Software\Josip Medved";                                   ValueType: none;    Flags: uninsdeletekeyifempty;
Root: HKCU;  Subkey: "Software\Josip Medved\VhdAttach";                         ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCU;  Subkey: "Software\Josip Medved\VHD Attach";                        ValueType: none;    ValueName: "Installed";                                      Flags: deletevalue uninsdeletevalue;
Root: HKLM;  Subkey: "Software\Josip Medved\VHD Attach";                        ValueType: dword;   ValueName: "Installed";         ValueData: "1";              Flags: uninsdeletekey;

Root: HKCR;  Subkey: ".vhd";                                                    ValueType: none;    ValueName: "";                  Flags: deletevalue;                                                      Tasks: context_open context_attach context_attachreadonly context_detach context_detachdrive context_vhd_open context_vhd_attach context_vhd_attachreadonly context_vhd_detach;
Root: HKCR;  Subkey: ".vhd\OpenWithProgids";                                    ValueType: string;  ValueName: "Windows.VhdFile";   ValueData: "";                                                           Tasks: context_open context_attach context_attachreadonly context_detach context_detachdrive context_vhd_open context_vhd_attach context_vhd_attachreadonly context_vhd_detach;
Root: HKCR;  Subkey: "Windows.VhdFile\DefaultIcon";                             ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe""";  Flags: createvalueifdoesntexist;  Tasks: context_open context_attach context_attachreadonly context_detach context_detachdrive context_vhd_open context_vhd_attach context_vhd_attachreadonly context_vhd_detach;

Root: HKCR;  Subkey: "VhdAttachFile";                                           ValueType: none;    Flags: deletekey;

Root: HKCR;  Subkey: "Windows.VhdFile\shell";                                   ValueType: string;  ValueName: "";                  ValueData: "VhdAttach-Open";                                    Tasks: context_open context_vhd_open;

Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Open";                    ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Open";                    ValueType: string;  ValueName: "";                  ValueData: "Open with VHD Attach";                              Tasks: context_open context_vhd_open;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Open";                    ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_open context_vhd_open;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Open";                    ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_open context_vhd_open;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Open\command";            ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" ""%1""";                    Tasks: context_open context_vhd_open;
                                                                     
Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach";                              ValueType: none;    Flags: deletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Attach";                  ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Attach";                  ValueType: string;  ValueName: "";                  ValueData: "Attach";                                            Tasks: context_attach context_vhd_attach;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Attach";                  ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_attach context_vhd_attach;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Attach";                  ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_attach context_vhd_attach;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Attach\command";          ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /attach ""%1""";            Tasks: context_attach context_vhd_attach;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Attach read-only";                    ValueType: none;    Flags: deletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-AttachReadOnly";          ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-AttachReadOnly";          ValueType: string;  ValueName: "";                  ValueData: "Attach (read-only)";                                Tasks: context_attachreadonly context_vhd_attachreadonly;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-AttachReadOnly";          ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_attachreadonly context_vhd_attachreadonly;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-AttachReadOnly";          ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_attachreadonly context_vhd_attachreadonly;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-AttachReadOnly\command";  ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /readonly /attach ""%1""";  Tasks: context_attachreadonly context_vhd_attachreadonly;

Root: HKCR;  Subkey: "VhdAttachFile\shell\Detach";                              ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Detach";                  ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Detach";                  ValueType: string;  ValueName: "";                  ValueData: "Detach";                                            Tasks: context_detach context_vhd_detach;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Detach";                  ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_detach context_vhd_detach;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Detach";                  ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_detach context_vhd_detach;
Root: HKCR;  Subkey: "Windows.VhdFile\shell\VhdAttach-Detach\command";          ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detach ""%1""";            Tasks: context_detach context_vhd_detach;

Root: HKCR;  Subkey: "Drive\shell\Detach";                                      ValueType: none;    Flags: deletekey;
Root: HKCR;  Subkey: "Drive\shell\Detach drive";                                ValueType: none;    Flags: deletekey;
Root: HKCR;  Subkey: "Drive\shell\VhdAttach-DetachDrive";                       ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Drive\shell\VhdAttach-DetachDrive";                       ValueType: string;  ValueName: "";                  ValueData: "Detach drive";                                      Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\VhdAttach-DetachDrive";                       ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\VhdAttach-DetachDrive";                       ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Single";                                            Tasks: context_detachdrive;
Root: HKCR;  Subkey: "Drive\shell\VhdAttach-DetachDrive\command";               ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detachdrive ""%1""";       Tasks: context_detachdrive;


Root: HKCR;  Subkey: ".iso";                                                    ValueType: none;    ValueName: "";                  Flags: deletevalue;                                                      Tasks: context_iso_open context_iso_attachreadonly context_iso_detach;
Root: HKCR;  Subkey: ".iso\OpenWithProgids";                                    ValueType: string;  ValueName: "Windows.IsoFile";   ValueData: "";                                                           Tasks: context_iso_open context_iso_attachreadonly context_iso_detach;
Root: HKCR;  Subkey: "Windows.VhdFile\DefaultIcon";                             ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe""";  Flags: createvalueifdoesntexist;  Tasks: context_iso_open context_iso_attachreadonly context_iso_detach;

Root: HKCR;  Subkey: "Windows.IsoFile\shell";                                   ValueType: string;  ValueName: "";                  ValueData: "VhdAttach-Open";                                    Tasks: context_iso_open;

Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Open";                    ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Open";                    ValueType: string;  ValueName: "";                  ValueData: "Open with VHD Attach";                              Tasks: context_iso_open;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Open";                    ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_iso_open;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Open";                    ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_iso_open;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Open\command";            ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" ""%1""";                    Tasks: context_iso_open;

Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-AttachReadOnly";          ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-AttachReadOnly";          ValueType: string;  ValueName: "";                  ValueData: "Attach";                                            Tasks: context_iso_attachreadonly;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-AttachReadOnly";          ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_iso_attachreadonly;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-AttachReadOnly";          ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_iso_attachreadonly;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-AttachReadOnly\command";  ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /readonly /attach ""%1""";  Tasks: context_iso_attachreadonly;

Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Detach";                  ValueType: none;    Flags: deletekey uninsdeletekey;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Detach";                  ValueType: string;  ValueName: "";                  ValueData: "Detach";                                            Tasks: context_iso_detach;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Detach";                  ValueType: string;  ValueName: "Icon";              ValueData: """{app}\VhdAttach.exe""";                           Tasks: context_iso_detach;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Detach";                  ValueType: string;  ValueName: "MultiSelectModel";  ValueData: "Document";                                          Tasks: context_iso_detach;
Root: HKCR;  Subkey: "Windows.IsoFile\shell\VhdAttach-Detach\command";          ValueType: string;  ValueName: "";                  ValueData: """{app}\VhdAttach.exe"" /detach ""%1""";            Tasks: context_iso_detach;


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
