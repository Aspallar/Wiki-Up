@echo off
setlocal
if not exist .git\ goto :wrongfolder 
if not exist src\ goto :wrongfolder 

set srcfolder=SecretsTemplates

set secretpath=src\WikiUpload\Services\Passwords
set secretfile=Entropy.cs
set msg=You should edit this file to provide your own encryption data.
call :copysecret

set secretpath=src\WikiUpload\Services\Youtube
set secretfile=YoutubeSecrets.cs
set msg=You should edit this file to provide your own google API key.
call :copysecret
goto :EOF

:copysecret
if exist %secretpath%\%secretfile% exit /B
copy %srcfolder%\%secretfile% %secretpath%\%secretfile%
echo %secretpath%\%secretfile% was created.
echo %msg%
echo.
exit /B

:wrongfolder
echo Please run from the root folder.
