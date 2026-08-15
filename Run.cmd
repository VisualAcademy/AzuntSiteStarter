@echo off
setlocal
cd /d "%~dp0"
dotnet run --project src\Azunt.Web\Azunt.Web.csproj --launch-profile https
endlocal
