# Quick Start Guide - IOT Desktop Dashboard

## 🚀 Get Started in 3 Minutes

### Step 1: Install .NET 8.0 SDK

**Check if already installed**:
```bash
dotnet --version
```

**If not installed**, download from: https://dotnet.microsoft.com/download/dotnet/8.0

### Step 2: Build the Application

Open PowerShell or Command Prompt:

```bash
cd C:\UP\iot\IOT-Desktop-App
dotnet restore
dotnet build
```

### Step 3: Start the Backend

Open a **new terminal**:

```bash
cd C:\UP\iot\IOT-Website\iot-backend-mvc
npm start
```

**Verify backend is running**: Open browser to `http://localhost:3000/api/sensors/latest`

### Step 4: Run the Desktop App

```bash
cd C:\UP\iot\IOT-Desktop-App
dotnet run
```

**That's it!** 🎉 The dashboard should open and start displaying sensor data.

---

## 🔧 Troubleshooting

### "dotnet command not found"
- Install .NET 8.0 SDK from Microsoft website
- Restart PowerShell after installation

### "Failed to connect to backend"
- Ensure backend is running on port 3000
- Check `App.config` has correct URL: `http://localhost:3000`

### "No sensor data available"
- ESP8266 must be publishing data to MQTT broker
- Check backend logs for incoming MQTT messages
- Verify database has data: `SELECT * FROM sensors ORDER BY id DESC LIMIT 10;`

---

## 📱 What You Should See

After launching, the dashboard displays:

1. **Top Cards**: Current temperature and humidity
2. **Statistics**: Min/Max/Avg for today
3. **Chart**: Line graph showing trends
4. **Table**: Last 50 sensor readings
5. **Status Bar**: Connection status and last update time

Data refreshes automatically every 10 seconds.

---

## 🎯 Next Steps

- Customize `App.config` for your network settings
- Modify refresh interval (default: 10 seconds)
- Review full documentation in `README.md`

---

**Need Help?** See the full [README.md](README.md) for detailed documentation.

