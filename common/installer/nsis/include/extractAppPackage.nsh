!macro extractEmbeddedAppPackage
  !ifdef COMPRESS
    SetCompress off
  !endif

  File /oname=$PLUGINSDIR\app-bundle.7z "${APP_BUNDLE}"

  !ifdef COMPRESS
    SetCompress "${COMPRESS}"
  !endif

  Nsis7z::ExtractWithDetails "$PLUGINSDIR\app-bundle.7z" "Unpacking bundle %s..."

  DetailPrint "Copying files..."
  nsExec::ExecToStack '"$SYSDIR\cmd.exe" /c xcopy.exe "$INSTDIR\RuminoidToolbox\*" "$INSTDIR" /Y /E /I'
  Pop $0
  Pop $1
  RMDir /r "$INSTDIR\RuminoidToolbox"

  WriteUninstaller "$INSTDIR\${UNINSTALLER_OUT_FILE}"
!macroend
