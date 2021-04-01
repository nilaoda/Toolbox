Function un.onInit
  !insertmacro check64BitAndSetRegView

  MessageBox MB_OKCANCEL "$(areYouSureToUninstall)" IDOK +2
  Quit

  !insertmacro setInstallModePerUser
FunctionEnd

Section "Uninstall"
  !insertmacro setLinkVars

  WinShell::UninstAppUserModelId "${APP_ID}"

  WinShell::UninstShortcut "$oldDesktopLink"
  Delete "$oldDesktopLink"

  WinShell::UninstShortcut "$oldStartMenuLink"

  Delete "$oldStartMenuLink"
  ReadRegStr $R1 SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" MenuDirectory
  ${ifNot} $R1 == ""
    RMDir "$SMPROGRAMS\$R1"
  ${endIf}

  # refresh the desktop
  System::Call 'shell32::SHChangeNotify(i, i, i, i) v (0x08000000, 0, 0, 0)'

  # delete the installed files
  RMDir /r $INSTDIR

  DeleteRegKey SHELL_CONTEXT "${UNINSTALL_REGISTRY_KEY}"
  DeleteRegKey SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}"
SectionEnd
