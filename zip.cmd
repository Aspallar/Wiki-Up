@echo off
setlocal
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 

md Dist 1>nul 2>nul
del Dist\*.* /q

set tempfile=temp.txt
if exist %tempfile% goto tempExists
type src\WikiUpload\Properties\AssemblyInfo.cs | grep -E "^\[assembly: AssemblyVersion" | grep -Eo "[0-9]+\.[0-9]+\.[0-9]+" > %tempfile%
set /p version= < temp.txt
set zipname=Dist\WikiUp-%version%.zip
if exist %zipname% del %zipname%
7z a -tzip %zipname% ".\deploy\*"
del %tempfile%

set installsrc=src\WikiUpInstaller\bin\Release
copy %installsrc%\en-US\WikiUpInstaller.msi Dist\WikiUpInstaller-English-%version%.msi
copy %installsrc%\de-DE\WikiUpInstaller.msi Dist\WikiUpInstaller-Deutsch-%version%.msi
copy %installsrc%\et-EE\WikiUpInstaller.msi Dist\WikiUpInstaller-Eesti-%version%.msi
copy %installsrc%\fr-FR\WikiUpInstaller.msi Dist\WikiUpInstaller-Francais-%version%.msi

goto :EOF

:tempExists
echo Temporary file temp.txt already exists. Please delete before using this.
goto :EOF

:wrongfolder
echo Please run from the root folder.
