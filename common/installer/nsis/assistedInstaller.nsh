!include StrContains.nsh
!insertmacro MUI_PAGE_DIRECTORY
# pageDirectory leave doesn't work (it seems because $INSTDIR is set after custom leave function)
# so, we use instfiles pre
!define MUI_PAGE_CUSTOMFUNCTION_PRE instFilesPre
# sanitize the MUI_PAGE_DIRECTORY result to make sure it has a application name sub-folder
Function instFilesPre
  ${If} ${FileExists} "$INSTDIR\*"
    ${StrContains} $0 "${APP_FILENAME}" $INSTDIR
    ${If} $0 == ""
      StrCpy $INSTDIR "$INSTDIR\${APP_FILENAME}"
    ${endIf}
  ${endIf}
FunctionEnd
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
