@echo off
setlocal
cd /d "%~dp0"
dotnet restore Azunt.Site.slnx || exit /b 1
dotnet build Azunt.Site.slnx || exit /b 1
endlocal
