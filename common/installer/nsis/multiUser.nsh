!include FileFunc.nsh

!define FOLDERID_UserProgramFiles {5CD7AEE2-2219-4A67-B85D-6C9CE15660CB}
!define KF_FLAG_CREATE 0x00008000

# allow user to define own custom
!define /ifndef INSTALL_REGISTRY_KEY "Software\${APP_GUID}"
!define /ifndef UNINSTALL_REGISTRY_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_GUID}"

Var PerUserInstallationFolder

!macro setInstallModePerUser
  SetShellVarContext current
  # сhecks registry for previous installation path
  ReadRegStr $perUserInstallationFolder HKCU "${INSTALL_REGISTRY_KEY}" InstallLocation
  ${if} $perUserInstallationFolder != ""
    StrCpy $INSTDIR $perUserInstallationFolder
  ${else}
    StrCpy $0 "$LocalAppData\Programs"
    System::Store S
    # Win7 has a per-user programfiles known folder and this can be a non-default location
    System::Call 'SHELL32::SHGetKnownFolderPath(g "${FOLDERID_UserProgramFiles}", i ${KF_FLAG_CREATE}, p 0, *p .r2)i.r1'
    ${If} $1 == 0
      System::Call '*$2(&w${NSIS_MAX_STRLEN} .s)'
      StrCpy $0 $1
      System::Call 'OLE32::CoTaskMemFree(p r2)'
    ${endif}
    System::Store L
    StrCpy $INSTDIR "$0\${APP_FILENAME}"
  ${endif}
!macroend
