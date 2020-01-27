; The name of the installer
Name "Rogueskiv Installer"

; Build Unicode installer
Unicode True

# define name of installer
OutFile "..\rogueskiv-win-x64-installer.exe"
 
# define installation directory
InstallDir $PROGRAMFILES64\Rogueskiv
 
# For removing Start Menu shortcut in Windows 7
RequestExecutionLevel admin
 
; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Rogueskiv" "Install_Dir"

;--------------------------------

; Pages

Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
# start default section
Section "Rogueskiv"
 
    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR
    
    File /r *.*
 
    # create a shortcut in the start menu programs directory
    # point the new shortcut at the program uninstaller
    CreateShortCut "$DESKTOP\Uninstall Rogueskiv.lnk" "$INSTDIR\uninstall.exe"
    CreateShortCut "$DESKTOP\Rogueskiv.lnk" "$INSTDIR\Rogueskiv.Run.exe" "" "$INSTDIR\Rogueskiv.ico"

    ; Write the installation path into the registry
    WriteRegStr HKLM SOFTWARE\Rogueskiv "Install_Dir" "$INSTDIR"

    ; Write the uninstall keys for Windows
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Rogueskiv" "DisplayName" "Rogueskiv"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Rogueskiv" "UninstallString" '"$INSTDIR\uninstall.exe"'
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Rogueskiv" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Rogueskiv" "NoRepair" 1
    WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd
 
;--------------------------------

; Uninstaller

Section "Uninstall"
  
    ; Remove registry keys
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Rogueskiv"
    DeleteRegKey HKLM SOFTWARE\Rogueskiv

    ; Remove directories used
    RMDir /r "$INSTDIR"

    # second, remove the link from the start menu
    Delete "$DESKTOP\Rogueskiv.lnk"
    Delete "$DESKTOP\Uninstall Rogueskiv.lnk"
SectionEnd
