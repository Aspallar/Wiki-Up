@echo off
setlocal
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 
set tempfile=temp.txt
if exist %tempfile% goto tempExists
type src\WikiUpload\Properties\AssemblyInfo.cs | grep -E "^\[assembly: AssemblyVersion" | grep -Eo "[0-9]+\.[0-9]+\.[0-9]+" > %tempfile%
set /p version= < temp.txt
set zipname=WikiUp-%version%.zip
if exist %zipname% del %zipname%
7z a -tzip %zipname% ".\deploy\*"
del %tempfile%
goto :EOF

:tempExists
echo Temporary file temp.txt already exists. Please delete before using this.
goto :EOF

:wrongfolder
echo Please run from the root folder.
