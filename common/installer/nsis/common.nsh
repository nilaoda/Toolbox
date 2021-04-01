!include x64.nsh
!include WinVer.nsh

BrandingText "${PRODUCT_NAME} ${VERSION}"
; ShowInstDetails nevershow
SpaceTexts none
FileBufSize 64
Name "${PRODUCT_NAME}"

!macro check64BitAndSetRegView
  ${If} ${IsWin2000}
  ${OrIf} ${IsWinME}
  ${OrIf} ${IsWinXP}
  ${OrIf} ${IsWinVista}
    MessageBox MB_OK "$(win7Required)"
    Quit
  ${EndIf}

  ${If} ${RunningX64}
    SetRegView 64
  ${Else}
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(x64WinRequired)"
    Quit
  ${EndIf}
!macroend

!macro setLinkVars
  # old desktop shortcut (could exist or not since the user might has selected to delete it)
  ReadRegStr $oldShortcutName SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" ShortcutName
  ${if} $oldShortcutName == ""
    StrCpy $oldShortcutName "${SHORTCUT_NAME}"
  ${endIf}
  StrCpy $oldDesktopLink "$DESKTOP\$oldShortcutName.lnk"

  # new desktop shortcut (will be created/renamed in case of a fresh installation or if the user haven't deleted the initial one)
  StrCpy $newDesktopLink "$DESKTOP\${SHORTCUT_NAME}.lnk"

  ReadRegStr $oldMenuDirectory SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" MenuDirectory
  ${if} $oldMenuDirectory == ""
    StrCpy $oldStartMenuLink "$SMPROGRAMS\$oldShortcutName.lnk"
  ${else}
    StrCpy $oldStartMenuLink "$SMPROGRAMS\$oldMenuDirectory\$oldShortcutName.lnk"
  ${endIf}

  # new menu shortcut (will be created/renamed in case of a fresh installation or if the user haven't deleted the initial one)
  !ifdef MENU_FILENAME
    StrCpy $newStartMenuLink "$SMPROGRAMS\${MENU_FILENAME}\${SHORTCUT_NAME}.lnk"
  !else
    StrCpy $newStartMenuLink "$SMPROGRAMS\${SHORTCUT_NAME}.lnk"
  !endif
!macroend
