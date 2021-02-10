@echo off
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 
md Deploy 1>nul 2>nul
md Deploy\de-DE 1>nul 2>nul
md Deploy\et-EE 1>nul 2>nul
del Deploy\*.* /q
del Deploy\de-DE\*.* /q
del Deploy\et-EE\*.* /q
copy src\WikiUpload\bin\release\WikiUp.exe Deploy\.
copy src\WikiUpload\bin\release\WikiUp.exe.config Deploy\.
copy src\WikiUpload\bin\release\Microsoft.Windows.Shell.dll Deploy\.
copy src\WikiUpload\bin\release\MahApps.Metro.IconPacks.Core.dll Deploy\.
copy src\WikiUpload\bin\release\MahApps.Metro.IconPacks.FontAwesome.dll Deploy\.
copy src\WikiUpload\bin\release\Ninject.dll Deploy\.
copy src\WikiUpload\bin\release\ToggleSwitch.dll Deploy\.
copy src\WikiUpload\bin\release\de-DE\WikiUp.resources.dll Deploy\de-DE\.
copy src\WikiUpload\bin\release\et-EE\WikiUp.resources.dll Deploy\et-EE\.

goto :eof

:wrongfolder
echo Please run from the root folder.
