@echo off
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 
md Deploy 1>nul 2>nul
del Deploy\*.* /q
copy src\WikiUpload\bin\release\WikiUp.exe Deploy\.
copy src\WikiUpload\bin\release\WikiUp.exe.config Deploy\.
copy src\WikiUpload\bin\release\Microsoft.Windows.Shell.dll Deploy\.
copy src\WikiUpload\bin\release\MahApps.Metro.IconPacks.Core.dll Deploy\.
copy src\WikiUpload\bin\release\MahApps.Metro.IconPacks.FontAwesome.dll Deploy\.
copy src\WikiUpload\bin\release\ToggleSwitch.dll Deploy\.
goto :eof

:wrongfolder
echo Please run from the root folder.
