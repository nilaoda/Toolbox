# functions (nsis macro) for installer

!include "extractAppPackage.nsh"

!macro installApplicationFiles
  !insertmacro extractEmbeddedAppPackage
  SetShellVarContext current
!macroend

!macro registryAddInstallInfo
  WriteRegStr SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" InstallLocation "$INSTDIR"
  WriteRegStr SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" KeepShortcuts "true"
  WriteRegStr SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" ShortcutName "${SHORTCUT_NAME}"
  !ifdef MENU_FILENAME
    WriteRegStr SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" MenuDirectory "${MENU_FILENAME}"
  !endif

  StrCpy $0 "/currentuser"
  StrCpy $1 ""

  WriteRegStr SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" DisplayName "${UNINSTALL_DISPLAY_NAME}$1"
  # https://github.com/electron-userland/electron-builder/issues/750
  StrCpy $2 "$INSTDIR\${UNINSTALLER_OUT_FILE}"
  WriteRegStr SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" UninstallString '"$2" $0'
  WriteRegStr SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" QuietUninstallString '"$2" $0 /S'

  WriteRegStr SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" "DisplayVersion" "${VERSION}"
  WriteRegStr SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" "DisplayIcon" "$appExe,0"

  !ifdef COMPANY_NAME
    WriteRegStr SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" "Publisher" "${COMPANY_NAME}"
  !endif

  WriteRegDWORD SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" NoModify 1
  WriteRegDWORD SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" NoRepair 1

  # allow user to define ESTIMATED_SIZE to avoid GetSize call
  !ifdef ESTIMATED_SIZE
    IntFmt $0 "0x%08X" ${ESTIMATED_SIZE}
  !else
    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
  !endif

  WriteRegDWORD SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}" "EstimatedSize" "$0"
!macroend

!macro cleanupOldMenuDirectory
  ${if} $oldMenuDirectory != ""
    !ifdef MENU_FILENAME
      ${if} $oldMenuDirectory != "${MENU_FILENAME}"
        RMDir "$SMPROGRAMS\$oldMenuDirectory"
      ${endIf}
    !else
      RMDir "$SMPROGRAMS\$oldMenuDirectory"
    !endif
  ${endIf}
!macroend

!macro createMenuDirectory
  !ifdef MENU_FILENAME
    CreateDirectory "$SMPROGRAMS\${MENU_FILENAME}"
    ClearErrors
  !endif
!macroend

!macro addStartMenuLink keepShortcuts
  # The keepShortcuts mechanism is NOT enabled.
  # Menu shortcut will be recreated.
  ${if} $keepShortcuts  == "false"
    !insertmacro cleanupOldMenuDirectory
    !insertmacro createMenuDirectory

    CreateShortCut "$newStartMenuLink" "$appExe" "" "$appExe" 0 "" "" "${APP_DESCRIPTION}"
    # clear error (if shortcut already exists)
    ClearErrors
    WinShell::SetLnkAUMI "$newStartMenuLink" "${APP_ID}"
  # The keepShortcuts mechanism IS enabled.
  # The menu shortcut could either not exist (it shouldn't be recreated) or exist in an obsolete location.
  ${elseif} $oldStartMenuLink != $newStartMenuLink
  ${andIf} ${FileExists} "$oldStartMenuLink"
    !insertmacro createMenuDirectory

    Rename $oldStartMenuLink $newStartMenuLink
    WinShell::UninstShortcut "$oldStartMenuLink"
    WinShell::SetLnkAUMI "$newStartMenuLink" "${APP_ID}"

    !insertmacro cleanupOldMenuDirectory
  ${endIf}
!macroend

!macro addDesktopLink keepShortcuts
  # The keepShortcuts mechanism is NOT enabled.
  # Shortcuts will be recreated.
  ${if} $keepShortcuts == "false"
    CreateShortCut "$newDesktopLink" "$appExe" "" "$appExe" 0 "" "" "${APP_DESCRIPTION}"
    ClearErrors
    WinShell::SetLnkAUMI "$newDesktopLink" "${APP_ID}"
  # The keepShortcuts mechanism IS enabled.
  # The desktop shortcut could exist in an obsolete location (due to name change).
  ${elseif} $oldDesktopLink != $newDesktopLink
  ${orIf} ${FileExists} "$oldDesktopLink"
    Rename $oldDesktopLink $newDesktopLink
    WinShell::UninstShortcut "$oldDesktopLink"
    WinShell::SetLnkAUMI "$newDesktopLink" "${APP_ID}"
  ${endIf}
  System::Call 'Shell32::SHChangeNotify(i 0x8000000, i 0, i 0, i 0)'
!macroend
