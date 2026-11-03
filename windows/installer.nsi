; N5 Kanji Dojo — Windows installer (per-user, no admin required)
Unicode true
Name "N5 Kanji Dojo"
OutFile "N5-Kanji-Dojo-Setup-v1.6.exe"
InstallDir "$LOCALAPPDATA\Programs\N5KanjiDojo"
RequestExecutionLevel user
SetCompressor /SOLID lzma
Icon "dojo-win\app.ico"
UninstallIcon "dojo-win\app.ico"
LicenseData "dojo-win\www\licenses.txt"

VIProductVersion "1.6.0.0"
VIAddVersionKey "ProductName" "N5 Kanji Dojo"
VIAddVersionKey "CompanyName" "Ayush"
VIAddVersionKey "FileDescription" "N5 Kanji Dojo Setup — JLPT N5 kanji learning app (fully offline)"
VIAddVersionKey "FileVersion" "1.6.0.0"
VIAddVersionKey "LegalCopyright" "Built by Ayush"

!include "MUI2.nsh"
!define MUI_ICON "dojo-win\app.ico"
!define MUI_UNICON "dojo-win\app.ico"
!define MUI_WELCOMEPAGE_TEXT "This setup installs N5 Kanji Dojo on your computer.$\r$\n$\r$\nLearn 103 JLPT N5 kanji with readings, stroke-order practice, 514 vocabulary words, grammar, 15 quiz modes and working audio pronunciation on every word. The app works fully offline and asks for no account.$\r$\n$\r$\nBuilt by Ayush"
!define MUI_FINISHPAGE_RUN "$INSTDIR\N5KanjiDojo.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Start N5 Kanji Dojo"
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "dojo-win\www\licenses.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

Section "Install"
  SetOutPath "$INSTDIR"
  File "publish\N5KanjiDojo.exe"
  File "dojo-win\www\licenses.txt"
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  CreateDirectory "$SMPROGRAMS\N5 Kanji Dojo"
  CreateShortcut "$SMPROGRAMS\N5 Kanji Dojo\N5 Kanji Dojo.lnk" "$INSTDIR\N5KanjiDojo.exe" "" "$INSTDIR\N5KanjiDojo.exe" 0
  CreateShortcut "$SMPROGRAMS\N5 Kanji Dojo\Uninstall N5 Kanji Dojo.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
  CreateShortcut "$DESKTOP\N5 Kanji Dojo.lnk" "$INSTDIR\N5KanjiDojo.exe" "" "$INSTDIR\N5KanjiDojo.exe" 0

  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "DisplayName" "N5 Kanji Dojo"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "DisplayVersion" "1.6"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "Publisher" "Ayush"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "DisplayIcon" "$INSTDIR\N5KanjiDojo.exe"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "NoModify" 1
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo" "NoRepair" 1
SectionEnd

Section "Uninstall"
  Delete "$INSTDIR\N5KanjiDojo.exe"
  Delete "$INSTDIR\licenses.txt"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  Delete "$DESKTOP\N5 Kanji Dojo.lnk"
  Delete "$SMPROGRAMS\N5 Kanji Dojo\N5 Kanji Dojo.lnk"
  Delete "$SMPROGRAMS\N5 Kanji Dojo\Uninstall N5 Kanji Dojo.lnk"
  RMDir "$SMPROGRAMS\N5 Kanji Dojo"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\N5KanjiDojo"
  ; Note: study progress in %LOCALAPPDATA%\N5KanjiDojo is kept on purpose,
  ; so reinstalling never wipes learning data.
SectionEnd
