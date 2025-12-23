@echo off
echo ===============================================
echo   IOT Desktop Dashboard - Build and Run
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

echo [1/3] Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages!
    pause
    exit /b 1
)

echo.
echo [2/3] Building project...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo [3/3] Running application...
echo.
echo ===============================================
echo  Desktop app starting...
echo  Make sure your backend is running on:
echo  http://localhost:3000
echo ===============================================
echo.

dotnet run

pause

