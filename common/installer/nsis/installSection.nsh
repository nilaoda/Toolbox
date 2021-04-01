!include installer.nsh

InitPluginsDir

StrCpy $appExe "$INSTDIR\${APP_EXECUTABLE_FILENAME}"

# must be called before uninstallOldVersion
!insertmacro setLinkVars

Var /GLOBAL keepShortcuts
StrCpy $keepShortcuts "false"
!insertMacro setIsTryToKeepShortcuts
${if} $isTryToKeepShortcuts == "true"
  ReadRegStr $R1 SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" KeepShortcuts

  ${if} $R1 == "true"
  ${andIf} ${FileExists} "$appExe"
    StrCpy $keepShortcuts "true"
  ${endIf}
${endif}

!insertmacro uninstallOldVersion SHELL_CONTEXT

SetOutPath $INSTDIR

!insertmacro installApplicationFiles
!insertmacro registryAddInstallInfo
!insertmacro addStartMenuLink $keepShortcuts
!insertmacro addDesktopLink $keepShortcuts
