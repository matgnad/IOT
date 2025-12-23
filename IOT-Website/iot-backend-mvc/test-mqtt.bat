@echo off
echo ================================================
echo   MQTT System Health Check
echo ================================================
echo.

REM Check if mosquitto is installed
where mosquitto_sub >nul 2>&1
if %errorlevel% neq 0 (
    echo [X] mosquitto_sub not found!
    echo     Install Mosquitto from: https://mosquitto.org/download/
    echo.
    pause
    exit /b 1
)

echo [1/3] Testing MQTT broker connection...
echo.

REM Test connection (timeout after 5 seconds)
echo Attempting to connect to MQTT broker...
echo Topic: esp8266/sensors
echo Host: 172.20.10.2
echo User: ThanhHai
echo.
echo Press Ctrl+C to stop when you see messages
echo.

mosquitto_sub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -v

pause

