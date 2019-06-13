@echo off
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 
cd src
msbuild /m /v:m /p:Configuration=Release
cd ..
call deploy.cmd
call zip.cmd
goto :EOF

:wrongfolder
echo Please run from the root folder.
