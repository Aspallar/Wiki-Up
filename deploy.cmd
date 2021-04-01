@echo off
setlocal
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 
md Deploy 1>nul 2>nul
md Deploy\de-DE 1>nul 2>nul
md Deploy\et-EE 1>nul 2>nul
del Deploy\*.* /q
del Deploy\de-DE\*.* /q
del Deploy\et-EE\*.* /q
set src=src\WikiUpload\bin\release
copy %src%\WikiUp.exe Deploy\.
copy %src%\WikiUp.exe.config Deploy\.
copy %src%\MahApps.Metro.IconPacks.Core.dll Deploy\.
copy %src%\MahApps.Metro.IconPacks.FontAwesome.dll Deploy\.
copy %src%\Ninject.dll Deploy\.
copy %src%\ToggleSwitch.dll Deploy\.
copy %src%\AngleSharp.dll Deploy\.
copy %src%\System.Runtime.CompilerServices.Unsafe.dll Deploy\.
copy %src%\System.Text.Encoding.CodePages.dll Deploy\.
copy %src%\Google.Apis.Auth.dll Deploy\.
copy %src%\Google.Apis.Auth.PlatformServices.dll Deploy\.
copy %src%\Google.Apis.Core.dll Deploy\.
copy %src%\Google.Apis.dll Deploy\.
copy %src%\Google.Apis.PlatformServices.dll Deploy\.
copy %src%\Google.Apis.YouTube.v3.dll Deploy\.
copy %src%\Newtonsoft.Json.dll Deploy\.
copy %src%\de-DE\WikiUp.resources.dll Deploy\de-DE\.
copy %src%\et-EE\WikiUp.resources.dll Deploy\et-EE\.

goto :eof

:wrongfolder
echo Please run from the root folder.
