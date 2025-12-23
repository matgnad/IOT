@echo off
echo ===============================================
echo   IOT Desktop Dashboard - Publish Executable
echo ===============================================
echo.

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 8.0 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo This will create a standalone executable that can run
echo on any Windows 10/11 PC without installing .NET.
echo.
echo File size will be approximately 70 MB.
echo.

pause

echo.
echo [1/2] Publishing self-contained executable...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
if %errorlevel% neq 0 (
    echo ERROR: Publish failed!
    pause
    exit /b 1
)

echo.
echo ===============================================
echo  Publish completed successfully!
echo  Executable location:
echo  bin\Release\net8.0-windows\win-x64\publish\IOT-Dashboard.exe
echo.
echo  You can now copy this .exe file to any Windows PC
echo  and run it without installing .NET.
echo ===============================================
echo.

pause

