Function un.onInit
  !insertmacro check64BitAndSetRegView

  MessageBox MB_OKCANCEL "$(areYouSureToUninstall)" IDOK +2
  Quit

  !insertmacro setInstallModePerUser
FunctionEnd

Section "Uninstall"
  !insertmacro setLinkVars

  ${ifNot} ${isKeepShortcuts}
    WinShell::UninstAppUserModelId "${APP_ID}"

    !ifndef DO_NOT_CREATE_DESKTOP_SHORTCUT
      WinShell::UninstShortcut "$oldDesktopLink"
      Delete "$oldDesktopLink"
    !endif

    !ifndef DO_NOT_CREATE_START_MENU_SHORTCUT
      WinShell::UninstShortcut "$oldStartMenuLink"

      Delete "$oldStartMenuLink"
      ReadRegStr $R1 SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" MenuDirectory
      ${ifNot} $R1 == ""
        RMDir "$SMPROGRAMS\$R1"
      ${endIf}
    !endif
  ${endIf}

  # refresh the desktop
  System::Call 'shell32::SHChangeNotify(i, i, i, i) v (0x08000000, 0, 0, 0)'

  # delete the installed files
  RMDir /r $INSTDIR

  Var /GLOBAL isDeleteAppData
  StrCpy $isDeleteAppData "0"

  ClearErrors
  ${GetParameters} $R0
  ${GetOptions} $R0 "--delete-app-data" $R1
  ${if} ${Errors}
    !ifdef DELETE_APP_DATA_ON_UNINSTALL
      ${ifNot} ${isUpdated}
        StrCpy $isDeleteAppData "1"
      ${endif}
    !endif
  ${else}
    StrCpy $isDeleteAppData "1"
  ${endIf}

  ${if} $isDeleteAppData == "1"
    SetShellVarContext current
    RMDir /r "$APPDATA\${APP_FILENAME}"
  ${endif}

  DeleteRegKey SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}"
  !ifdef UNINSTALL_REGISTRY_KEY_2
    DeleteRegKey SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY_2}"
  !endif
  DeleteRegKey SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}"
SectionEnd
