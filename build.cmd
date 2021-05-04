@echo off
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 
cd src
if "%1" == "clean" devenv WikiUpload.sln /Clean Release
msbuild /m /v:m /p:Configuration=Release
msbuild /m /v:m /p:Configuration=Install
cd ..
call deploy.cmd
call zip.cmd
goto :EOF

:wrongfolder
echo Please run from the root folder.
