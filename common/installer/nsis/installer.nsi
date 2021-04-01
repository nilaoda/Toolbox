; Ruminoid Toolbox Install/Uninstall Script
;
; This script is from electron-builder.
;
; Defines:
; -DVERSION=0.1.0
;

; Global Defines

; App ID
!define APP_ID "world.ruminoid.toolbox"
; Install/Uninstall Key
!define APP_GUID "df89a553-97f8-4f9c-912d-553afc6f0772"
!define PRODUCT_NAME "Ruminoid Toolbox"
!define SHORTCUT_NAME "Toolbox"
; Install Directory
!define APP_FILENAME "RuminoidToolbox"
!define APP_DESCRIPTION "Ruminoid Toolbox"
!define MUI_ICON "..\..\Assets\rmbox.ico"
!define MUI_UNICON "..\..\Assets\rmbox.ico"
!define APP_BUNDLE "..\..\..\dist\rmbox.7z"
!define COMPANY_NAME "Il Harper"
!define UNINSTALL_DISPLAY_NAME "Ruminoid Toolbox"
!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\nsis3-metro.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\nsis3-metro.bmp"
!define COMPRESS "auto"
!define UNINSTALLER_OUT_FILE "rmbox-uninstall.exe"

; Custom Defines

!define MENU_FILENAME "Ruminoid"
!define APP_EXECUTABLE_FILENAME "rmbenv.exe"

; PreRun Scripts

OutFile "rmbox-install.exe"

VIProductVersion "${VERSION}.0"
VIAddVersionKey /LANG=1033 "ProductName" "Ruminoid Toolbox"
VIAddVersionKey /LANG=1033 "ProductVersion" "${VERSION}"
VIAddVersionKey /LANG=1033 "LegalCopyright" "Copyright © 2021 Il Harper"
VIAddVersionKey /LANG=1033 "FileDescription" "Ruminoid Toolbox"
VIAddVersionKey /LANG=1033 "FileVersion" "${VERSION}"
VIAddVersionKey /LANG=1033 "CompanyName" "Il Harper"
Unicode "true"
SetCompressor "zlib"

!include "include\StdUtils.nsh"
!addincludedir "include"

!macro addLangs
  !insertmacro MUI_LANGUAGE "English"
  !insertmacro MUI_LANGUAGE "German"
  !insertmacro MUI_LANGUAGE "French"
  !insertmacro MUI_LANGUAGE "SpanishInternational"
  !insertmacro MUI_LANGUAGE "SimpChinese"
  !insertmacro MUI_LANGUAGE "TradChinese"
  !insertmacro MUI_LANGUAGE "Japanese"
  !insertmacro MUI_LANGUAGE "Korean"
  !insertmacro MUI_LANGUAGE "Italian"
  !insertmacro MUI_LANGUAGE "Dutch"
  !insertmacro MUI_LANGUAGE "Danish"
  !insertmacro MUI_LANGUAGE "Swedish"
  !insertmacro MUI_LANGUAGE "Norwegian"
  !insertmacro MUI_LANGUAGE "Finnish"
  !insertmacro MUI_LANGUAGE "Russian"
  !insertmacro MUI_LANGUAGE "Portuguese"
  !insertmacro MUI_LANGUAGE "PortugueseBR"
  !insertmacro MUI_LANGUAGE "Polish"
  !insertmacro MUI_LANGUAGE "Ukrainian"
  !insertmacro MUI_LANGUAGE "Czech"
  !insertmacro MUI_LANGUAGE "Slovak"
  !insertmacro MUI_LANGUAGE "Hungarian"
  !insertmacro MUI_LANGUAGE "Arabic"
  !insertmacro MUI_LANGUAGE "Turkish"
  !insertmacro MUI_LANGUAGE "Thai"
  !insertmacro MUI_LANGUAGE "Vietnamese"
!macroend

!addplugindir /x86-unicode "..\..\..\.tmp\nsis\plugins\x86-unicode"

!include "messages.nsh"

Var newStartMenuLink
Var oldStartMenuLink
Var newDesktopLink
Var oldDesktopLink
Var oldShortcutName
Var oldMenuDirectory

!include "common.nsh"
!include "MUI2.nsh"
!include "multiUser.nsh"

RequestExecutionLevel user

Var appExe

!include "assistedInstaller.nsh"

!insertmacro addLangs

Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
  !insertmacro check64BitAndSetRegView
  !insertmacro setInstallModePerUser
FunctionEnd

!include "installUtil.nsh"

Section "install"
  !include "installSection.nsh"
SectionEnd

!include "uninstaller.nsh"
