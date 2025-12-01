[Setup]
AppId={{0F9B6E4C-7C07-4D24-A0E4-9E8E5A57D9A2}
AppName=PuntoSwitcherUkr
AppVersion=1.0.0
AppPublisher=You
DefaultDirName={autopf}\PuntoSwitcherUkr
DefaultGroupName=PuntoSwitcherUkr
OutputDir=.
OutputBaseFilename=PuntoSwitcherUkr_Setup
Compression=lzma
SolidCompression=yes
DisableDirPage=no
DisableProgramGroupPage=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\PuntoSwitcherUkr"; Filename: "{app}\PuntoSwitcherUkr.exe"
Name: "{autostartup}\PuntoSwitcherUkr"; Filename: "{app}\PuntoSwitcherUkr.exe"; Tasks: autostart

[Tasks]
Name: "autostart"; Description: "Запускати PuntoSwitcherUkr при вході в систему"; Flags: unchecked

[Run]
Filename: "{app}\PuntoSwitcherUkr.exe"; Description: "Запустити PuntoSwitcherUkr"; Flags: nowait postinstall skipifsilent

[Messages]
BeveledLabel=Встановлювач PuntoSwitcherUkr
