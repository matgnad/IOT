# 🚀 START HERE - IOT Desktop Dashboard

Welcome to the **IOT Desktop Dashboard** project!

This is a **Windows Forms desktop application** that replaces your web frontend for monitoring temperature and humidity from ESP8266 sensors.

---

## 📖 Documentation Guide

This project has comprehensive documentation. Start with what you need:

### 🏃 Quick Start (I want to run it NOW!)

**Read**: [`QUICK_START.md`](QUICK_START.md)

Get started in 3 minutes:
1. Install .NET 6.0 SDK
2. Run `dotnet restore` and `dotnet build`
3. Run `dotnet run`

---

### 📘 Full Documentation (I want to understand everything)

**Read**: [`README.md`](README.md)

Comprehensive guide covering:
- Features overview
- Installation & build instructions
- Configuration options
- API endpoints
- Troubleshooting
- How it works

---

### 🏗️ Architecture & Design (I want to know WHY decisions were made)

**Read**: [`ARCHITECTURE.md`](ARCHITECTURE.md)

Deep dive into:
- Why C# was chosen over C
- Why REST API was chosen over MQTT
- System architecture diagram
- Design patterns used
- Data flow
- Performance considerations

---

### 📋 Project Summary (I want a quick overview)

**Read**: [`PROJECT_SUMMARY.md`](PROJECT_SUMMARY.md)

High-level overview:
- What was built
- Key decisions
- Requirements checklist
- Comparison with web frontend
- Future enhancements

---

### 🎨 UI Design (I want to see what it looks like)

**Read**: [`UI_MOCKUP.txt`](UI_MOCKUP.txt)

Visual ASCII mockup showing:
- Layout structure
- Color scheme
- UI controls
- Interaction flow

---

## 🛠️ Build Options

Choose your preferred method:

### Option 1: Command Line

```bash
cd C:\UP\iot\IOT-Desktop-App
dotnet restore
dotnet build
dotnet run
```

### Option 2: Batch Scripts (Easiest!)

Double-click one of these:
- **`build-and-run.bat`** - Build and run immediately
- **`build-only.bat`** - Build without running
- **`publish-exe.bat`** - Create standalone .exe

### Option 3: Visual Studio

1. Double-click **`IOT-Dashboard.sln`**
2. Press **F5** to run

---

## 📁 Project Structure

```
IOT-Desktop-App/
│
├── 📄 START_HERE.md              ← You are here!
├── 📄 QUICK_START.md             ← 3-minute setup guide
├── 📄 README.md                  ← Full documentation
├── 📄 ARCHITECTURE.md            ← Design decisions
├── 📄 PROJECT_SUMMARY.md         ← Project overview
├── 📄 UI_MOCKUP.txt              ← Visual layout
│
├── 🔧 IOT-Dashboard.sln          ← Visual Studio solution
├── 🔧 IOT-Dashboard.csproj       ← Project file
├── 🔧 App.config                 ← Configuration
├── 🔧 Program.cs                 ← Entry point
│
├── 🚀 build-and-run.bat          ← Build & run script
├── 🚀 build-only.bat             ← Build script
├── 🚀 publish-exe.bat            ← Create .exe
│
├── 📂 Forms/
│   ├── MainForm.cs               ← UI logic
│   └── MainForm.Designer.cs      ← UI controls
│
├── 📂 Services/
│   └── SensorApiService.cs       ← REST API client
│
└── 📂 Models/
    ├── SensorData.cs             ← Data models
    ├── ApiResponse.cs            ← API DTOs
    └── Statistics.cs             ← Statistics model
```

---

## ⚡ Quick Commands

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Create Executable
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### Test Backend Connection
Open browser: `http://localhost:3000/api/sensors/latest`

---

## 🎯 What This Application Does

✅ Displays **current temperature** and **humidity** in large cards  
✅ Shows **statistics** (min/max/avg) for today  
✅ Displays **historical data** in a sortable table  
✅ Renders **line chart** showing trends over time  
✅ **Auto-refreshes** every 10 seconds  
✅ Connects to your **existing Node.js backend** via REST API  
✅ **No backend modifications** required  

---

## 🔧 Prerequisites

1. **Windows 10/11** (64-bit)
2. **.NET 8.0 SDK** - Download: https://dotnet.microsoft.com/download/dotnet/8.0
3. **Backend running** on `http://localhost:3000`

---

## 🐛 Troubleshooting

**Problem**: "dotnet command not found"  
**Solution**: Install .NET 8.0 SDK and restart PowerShell

**Problem**: "Failed to connect to backend"  
**Solution**: Ensure backend is running (`npm start` in backend folder)

**Problem**: "No sensor data available"  
**Solution**: ESP8266 must be publishing data to MQTT broker

For more, see [`README.md`](README.md) → Troubleshooting section.

---

## 🤔 Language Decision: C# vs C

**We chose C#** because:
- ✅ Native Windows Forms support
- ✅ Built-in HTTP client and JSON parsing
- ✅ Rich UI controls (Chart, DataGridView)
- ✅ Async/await for non-blocking operations
- ✅ Fast development (days vs weeks)

**C would require**:
- ❌ Manual Win32 API coding
- ❌ Low-level socket programming
- ❌ External libraries for JSON
- ❌ Building UI controls from scratch

For details, see [`ARCHITECTURE.md`](ARCHITECTURE.md).

---

## 🔌 Communication: REST API vs MQTT

**We chose REST API** because:
- ✅ Uses existing backend APIs
- ✅ No backend modifications needed
- ✅ Simpler to implement and debug
- ✅ Centralized business logic

**MQTT would require**:
- ❌ Bypassing backend entirely
- ❌ Direct database access
- ❌ Duplicating statistics calculations
- ❌ More complex setup

For details, see [`ARCHITECTURE.md`](ARCHITECTURE.md).

---

## 📊 Key Features

### Dashboard Cards
- 🌡️ **Temperature**: Red card with large °C display
- 💧 **Humidity**: Blue card with large % display

### Statistics Panel
- Min/Max/Average for temperature
- Min/Max/Average for humidity
- Total readings count

### Line Chart
- Dual-series: Temperature (red) + Humidity (blue)
- Time-based X-axis
- Auto-scaling Y-axis

### Historical Table
- Last 50 sensor readings
- Sortable columns
- Alternating row colors

### Auto-Refresh
- Updates every 10 seconds
- Manual refresh button
- Non-blocking async operations

---

## 🎓 Perfect For

- ✅ IoT student projects
- ✅ Learning C# Windows Forms
- ✅ Understanding REST APIs
- ✅ Desktop application development
- ✅ Real-world IoT monitoring

---

## 📞 Need Help?

1. **Quick setup**: [`QUICK_START.md`](QUICK_START.md)
2. **Full guide**: [`README.md`](README.md)
3. **Troubleshooting**: [`README.md`](README.md) → Troubleshooting section
4. **Architecture**: [`ARCHITECTURE.md`](ARCHITECTURE.md)

---

## 🏁 Next Steps

### For First-Time Users:
1. Read [`QUICK_START.md`](QUICK_START.md)
2. Install .NET 8.0 SDK if not already installed
3. Run `build-and-run.bat`
4. Enjoy your desktop dashboard!

### For Developers:
1. Install .NET 8.0 SDK (required)
2. Read [`ARCHITECTURE.md`](ARCHITECTURE.md)
3. Read [`README.md`](README.md)
4. Open `IOT-Dashboard.sln` in Visual Studio
5. Explore the code!

### For Students:
1. Read [`PROJECT_SUMMARY.md`](PROJECT_SUMMARY.md)
2. Understand the architecture
3. Try implementing the "Future Enhancements"

---

## ✅ Requirements Met

All project requirements have been successfully implemented:

✅ Desktop application (no web browser)  
✅ Display latest sensor readings  
✅ Display statistics (min/max/avg)  
✅ Display historical data table  
✅ Display line chart  
✅ Fetch from existing backend API  
✅ No backend modifications  
✅ No database changes  
✅ Windows-only target  
✅ Maintainable code  

---

## 🎉 Ready to Go!

Your IOT Desktop Dashboard is **complete and ready to use**.

**Choose your path**:
- 🏃 **Quick Start**: [`QUICK_START.md`](QUICK_START.md) → 3 minutes
- 📘 **Full Docs**: [`README.md`](README.md) → Everything you need
- 🏗️ **Architecture**: [`ARCHITECTURE.md`](ARCHITECTURE.md) → Design decisions

**Or just run**:
```bash
dotnet run
```

---

**Happy Monitoring!** 🌡️💧📊

**Built with ❤️ for IoT Students**

