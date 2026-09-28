@echo off
cd /d "%~dp0"
if exist "%~dp0publish\QLTK.exe" (
    start "" "%~dp0publish\QLTK.exe"
) else if exist "%~dp0bin\Debug\net8.0-windows\QLTK.exe" (
    start "" "%~dp0bin\Debug\net8.0-windows\QLTK.exe"
) else (
    start "" dotnet "%~dp0bin\Debug\net8.0-windows\QLTK.dll"
)
