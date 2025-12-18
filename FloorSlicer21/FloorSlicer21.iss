; Script generated and customized for FloorSlicer21 Revit Add-in
; ------------------------------------------------------------
#define MyAppName "FloorSlicer21"
#define MyAppVersion "1.0.0"
#define MyCompanyName "First Option"

; 🔧 Define build folders for each Revit version
#define SourceFolder2020 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2020"
#define SourceFolder2021 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2021"
#define SourceFolder2022 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2022"
#define SourceFolder2023 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2023"
#define SourceFolder2024 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2024"
#define SourceFolder2025 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2025"
#define SourceFolder2026 "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\R2026"

#define AddinFile "C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\FloorSlicer21.addin"

[Setup]
AppId={{BE8401B4-6FFF-4027-825D-BF6E2A5A78FF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyCompanyName}
DefaultGroupName={#MyAppName}
CreateAppDir=no
OutputDir=C:\Users\DELL\source\repos\FirstOptionTools\FloorSlicer21\bin\Setup
OutputBaseFilename=FloorSlicer21Installer
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

; ----------------------------------------------------------------------
; FILES SECTION — one source folder per Revit version
; ----------------------------------------------------------------------

[Files]
; Revit 2020
Source: "{#SourceFolder2020}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2020\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2020"; Flags: ignoreversion

; Revit 2021
Source: "{#SourceFolder2021}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021"; Flags: ignoreversion

; Revit 2022
Source: "{#SourceFolder2022}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2022\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2022"; Flags: ignoreversion

; Revit 2023
Source: "{#SourceFolder2023}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2023\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2023"; Flags: ignoreversion

; Revit 2024
Source: "{#SourceFolder2024}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2024\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2024"; Flags: ignoreversion

; Revit 2025
Source: "{#SourceFolder2025}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2025\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2025"; Flags: ignoreversion

; Revit 2026
Source: "{#SourceFolder2026}\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2026\FloorSlicer21"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#AddinFile}"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2026"; Flags: ignoreversion

; ----------------------------------------------------------------------
; ICONS SECTION (optional)
; ----------------------------------------------------------------------

[Icons]
; Shortcut not really needed for Revit add-ins, but optional
Name: "{group}\{#MyAppName} Info"; Filename: "{userappdata}\Autodesk\Revit\Addins\2025\FloorSlicer21\Readme.txt"; Flags: closeonexit

; ----------------------------------------------------------------------
; RUN SECTION (no app to launch)
; ----------------------------------------------------------------------

[Run]
; No need to run anything — Revit loads add-in automatically
