@echo off
setlocal
cd /d "%~dp0"
if exist artifacts\publish rmdir /s /q artifacts\publish
dotnet publish src\Azunt.Web\Azunt.Web.csproj -c Release -o artifacts\publish || exit /b 1
echo.
echo Published to: %CD%\artifacts\publish
endlocal
