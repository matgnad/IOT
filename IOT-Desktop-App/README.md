# IOT Desktop Dashboard - Windows Forms Application

A modern Windows Forms desktop application for monitoring temperature and humidity data from ESP8266 IoT sensors. This application replaces the web-based frontend and communicates with the existing Node.js backend via REST API.

---

## 📋 Table of Contents
- [Features](#features)
- [Architecture Decision](#architecture-decision)
- [System Requirements](#system-requirements)
- [Project Structure](#project-structure)
- [Installation & Build](#installation--build)
- [Configuration](#configuration)
- [Usage](#usage)
- [How It Works](#how-it-works)
- [Troubleshooting](#troubleshooting)

---

## ✨ Features

### Dashboard Features
- **Real-time Monitoring**: Display latest temperature and humidity readings
- **Statistics**: Show min/max/average for today's data
- **Historical Data**: View last 50 sensor readings in a data grid
- **Line Chart**: Visualize temperature and humidity trends over time
- **Auto-refresh**: Automatically updates every 10 seconds
- **Manual Refresh**: Force refresh with a button click
- **Connection Test**: Test backend API connectivity

### UI Components
- Modern, color-coded dashboard with cards
- Interactive chart with dual Y-axis (Temperature & Humidity)
- Sortable data grid for historical records
- Status bar showing connection status and last update time
- Responsive layout with split panels

---

## 🏗️ Architecture Decision

### Language Choice: **C# (Winner)**

| Aspect | C# | C |
|--------|----|----|
| **WinForms Support** | ✅ Native, built-in | ❌ Manual Win32 API |
| **UI Controls** | ✅ DataGridView, Chart, etc. | ❌ Must code from scratch |
| **HTTP Client** | ✅ HttpClient built-in | ❌ Low-level sockets |
| **JSON Parsing** | ✅ Newtonsoft.Json | ❌ External library (cJSON) |
| **Async Programming** | ✅ async/await | ❌ Not supported |
| **Development Speed** | ✅ Fast with Visual Studio | ❌ Slow, manual coding |
| **Maintainability** | ✅ High-level, readable | ❌ Complex, verbose |

**Verdict**: C# is the **clear winner** for Windows Forms development. Using C would be like building a car from raw metal instead of assembling pre-made parts.

---

### Communication Strategy: **REST API (Winner)**

| Aspect | REST API | MQTT Direct |
|--------|----------|-------------|
| **Simplicity** | ✅ Simple HTTP GET | ❌ Broker connection required |
| **Backend Reuse** | ✅ Uses existing `/api/sensors/*` | ❌ Bypasses backend |
| **Statistics** | ✅ Backend handles calculation | ❌ Must calculate locally |
| **Historical Data** | ✅ Paginated API available | ❌ Must query DB directly |
| **Real-time** | ⚠️ Polling (10s) - acceptable | ✅ True real-time |
| **Maintainability** | ✅ Centralized logic | ❌ Duplicated logic |

**Verdict**: **REST API** is better because:
- Your backend already has well-designed APIs (`/api/sensors/latest`, `/api/sensors`, `/api/sensors/today`)
- No need to modify backend or database schema
- Statistics can be added to backend without changing desktop app
- Simpler for students to understand and maintain
- 10-second polling is sufficient for IoT sensor monitoring

---

## 🖥️ System Requirements

- **OS**: Windows 10/11 (64-bit)
- **Framework**: .NET 8.0 SDK or later
- **IDE** (recommended): Visual Studio 2022 or VS Code with C# extension
- **Backend**: Node.js backend must be running on `http://localhost:3000` (or configured URL)

---

## 📁 Project Structure

```
IOT-Desktop-App/
│
├── IOT-Dashboard.csproj       # Project configuration
├── Program.cs                  # Application entry point
├── App.config                  # Configuration file (API URL, refresh interval)
│
├── Forms/
│   ├── MainForm.cs             # Main dashboard logic
│   └── MainForm.Designer.cs    # UI controls & layout
│
├── Services/
│   └── SensorApiService.cs     # REST API communication service
│
├── Models/
│   ├── SensorData.cs           # Sensor data model
│   ├── ApiResponse.cs          # API response DTOs
│   └── Statistics.cs           # Statistics model
│
└── README.md                   # This file
```

---

## 🛠️ Installation & Build

### Option 1: Using Visual Studio 2022

1. **Open the project**:
   ```
   File → Open → Project/Solution → Select IOT-Dashboard.csproj
   ```

2. **Restore NuGet packages** (automatic):
   - Newtonsoft.Json (JSON parsing)
   - System.Windows.Forms.DataVisualization (Chart control)

3. **Build the solution**:
   ```
   Build → Build Solution (Ctrl+Shift+B)
   ```

4. **Run the application**:
   ```
   Debug → Start Without Debugging (Ctrl+F5)
   ```

### Option 2: Using .NET CLI

1. **Navigate to the project directory**:
   ```bash
   cd C:\UP\iot\IOT-Desktop-App
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

### Option 3: Build Executable

To create a standalone executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

The executable will be in `bin\Release\net8.0-windows\win-x64\publish\IOT-Dashboard.exe`

---

## ⚙️ Configuration

### App.config Settings

Edit `App.config` to configure the application:

```xml
<appSettings>
  <!-- Backend API Base URL -->
  <add key="ApiBaseUrl" value="http://localhost:3000" />
  
  <!-- Refresh interval in seconds -->
  <add key="RefreshInterval" value="10" />
</appSettings>
```

**Common Configurations**:
- Local backend: `http://localhost:3000`
- Network backend: `http://192.168.1.100:3000`
- Cloud backend: `http://your-server.com:3000`

---

## 🚀 Usage

### Starting the Application

1. **Ensure the backend is running**:
   ```bash
   cd C:\UP\iot\IOT-Website\iot-backend-mvc
   npm start
   ```
   The backend should be running on `http://localhost:3000`

2. **Start the desktop app**:
   - Double-click `IOT-Dashboard.exe`, or
   - Run via Visual Studio, or
   - Run `dotnet run` in terminal

3. **Test connection**:
   - Click "🔌 Test Connection" button
   - If successful, you'll see a "Connected" message
   - If failed, check backend is running and `App.config` URL is correct

4. **View data**:
   - The app automatically fetches data on startup
   - Data refreshes every 10 seconds
   - Click "🔄 Refresh Now" for manual refresh

### Dashboard Sections

**1. Current Values (Top Cards)**
- 🌡️ **Temperature Card**: Shows latest temperature reading in °C
- 💧 **Humidity Card**: Shows latest humidity reading in %

**2. Statistics (Middle Section)**
- Min/Max/Average temperature (today)
- Min/Max/Average humidity (today)
- Total number of readings today

**3. Sensor Trends Chart**
- Line chart showing temperature (red) and humidity (blue) over time
- X-axis: Time (HH:mm format)
- Y-axis: Sensor values
- Displays today's data

**4. Historical Data Table**
- Shows last 50 sensor readings
- Columns: ID, Temperature, Humidity, Measured At
- Newest records at the top

**5. Status Bar (Bottom)**
- Left: Connection status and last update time
- Right: Last sensor reading timestamp

---

## 🔧 How It Works

### Architecture Overview

```
┌─────────────┐         MQTT          ┌────────────────┐
│  ESP8266    │ ──────────────────────>│  MQTT Broker   │
│  Sensor     │   (esp8266/sensors)    └────────────────┘
└─────────────┘                                │
                                               │ MQTT Subscribe
                                               ▼
                                        ┌────────────────┐
                                        │   Node.js      │
                                        │   Backend      │
                                        │   + MySQL      │
                                        └────────────────┘
                                               │
                                               │ REST API
                                               │ (HTTP GET)
                                               ▼
                                        ┌────────────────┐
                                        │  WinForms App  │
                                        │  (C# Desktop)  │
                                        └────────────────┘
```

### Communication Flow

1. **ESP8266** publishes sensor data to MQTT broker on topic `esp8266/sensors`
   ```json
   { "temp": 25.5, "humid": 60.2 }
   ```

2. **Node.js Backend** subscribes to MQTT, stores data in MySQL, and exposes REST APIs:
   - `GET /api/sensors/latest` - Latest sensor reading
   - `GET /api/sensors?page=1&limit=50` - Paginated history
   - `GET /api/sensors/today` - Today's data

3. **Desktop App** (this application):
   - Polls the REST API every 10 seconds using `HttpClient`
   - Parses JSON responses using `Newtonsoft.Json`
   - Updates UI controls on the main thread
   - Calculates statistics from fetched data
   - Renders charts and tables

### Key Classes

**SensorApiService.cs**
- `GetLatestAsync()` - Fetch latest reading
- `GetHistoryAsync()` - Fetch paginated history
- `GetTodayDataAsync()` - Fetch today's data
- `CalculateStatistics()` - Calculate min/max/avg

**MainForm.cs**
- `RefreshAllDataAsync()` - Main update loop
- `UpdateLatestDisplay()` - Update current value cards
- `UpdateStatistics()` - Update statistics section
- `UpdateHistoryTable()` - Update data grid
- `UpdateChart()` - Update line chart

---

## 🐛 Troubleshooting

### Common Issues

**1. "Failed to connect to backend API"**
- **Cause**: Backend server is not running
- **Solution**: 
  ```bash
  cd C:\UP\iot\IOT-Website\iot-backend-mvc
  npm start
  ```
- **Verify**: Open browser to `http://localhost:3000/api/sensors/latest`

**2. "Error: Connection refused"**
- **Cause**: Wrong API URL in `App.config`
- **Solution**: Check `ApiBaseUrl` matches your backend address

**3. "No sensor data available"**
- **Cause**: No data in database yet
- **Solution**: Ensure ESP8266 is publishing data to MQTT broker

**4. Chart not displaying**
- **Cause**: No data for today
- **Solution**: Wait for ESP8266 to publish data, or check database has records for today

**5. Build errors about missing packages**
- **Cause**: NuGet packages not restored
- **Solution**: 
  ```bash
  dotnet restore
  ```

**6. "System.Windows.Forms.DataVisualization not found"**
- **Cause**: Chart package not installed
- **Solution**: Ensure `.csproj` has the package reference and run `dotnet restore`

### Backend API Endpoints

Test these endpoints in a browser or Postman:

- **Latest**: `http://localhost:3000/api/sensors/latest`
  ```json
  {
    "success": true,
    "data": {
      "id": 123,
      "temperature": 25.5,
      "humidity": 60.2,
      "measured_at": "2025-12-22T21:30:00.000Z"
    }
  }
  ```

- **History**: `http://localhost:3000/api/sensors?page=1&limit=10`
  ```json
  {
    "success": true,
    "data": [...],
    "pagination": { "page": 1, "limit": 10, "total": 100, "totalPages": 10 }
  }
  ```

- **Today**: `http://localhost:3000/api/sensors/today`
  ```json
  {
    "success": true,
    "data": [...]
  }
  ```

---

## 📊 Database Schema

The backend uses this MySQL table:

```sql
CREATE TABLE sensors (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  temperature FLOAT NOT NULL,
  humidity FLOAT NOT NULL,
  measured_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

**No changes required** - the desktop app uses the existing schema.

---

## 🎯 Future Enhancements

Potential improvements for students:

1. **Add statistics API to backend** instead of calculating in desktop app
2. **Export data to CSV/Excel** for analysis
3. **Set threshold alerts** (e.g., notify if temp > 30°C)
4. **Add date range picker** to view historical data by date
5. **Display connection status indicator** (green/red dot)
6. **Add device control** (if backend has `/api/devices` endpoints)
7. **Implement WebSocket** instead of polling for true real-time updates
8. **Dark mode theme** toggle

---

## 📝 License

This is a student IoT project. Feel free to use and modify.

---

## 👨‍💻 Development Notes

### Why REST API over MQTT?

**REST API Advantages**:
- ✅ Backend already has well-designed APIs
- ✅ No MQTT broker credentials needed in desktop app
- ✅ Centralized business logic (statistics, pagination)
- ✅ Easier debugging (can test in browser)
- ✅ Better separation of concerns

**MQTT Direct Drawbacks**:
- ❌ Would bypass backend completely
- ❌ Desktop app would need to query database directly
- ❌ Statistics calculation duplicated
- ❌ No pagination for historical data
- ❌ More complex error handling

### Auto-Refresh Implementation

```csharp
// Timer triggers every 10 seconds
_refreshTimer = new System.Windows.Forms.Timer();
_refreshTimer.Interval = 10000; // 10 seconds
_refreshTimer.Tick += RefreshTimer_Tick;

private async void RefreshTimer_Tick(object sender, EventArgs e)
{
    await RefreshAllDataAsync();
}
```

### Thread Safety

UI updates are performed on the main thread using `async/await`:

```csharp
// Good: async/await automatically handles thread marshalling
private async Task RefreshAllDataAsync()
{
    var data = await _apiService.GetLatestAsync();
    lblTemperatureValue.Text = $"{data.Temperature}°C"; // Safe!
}
```

---

## 📞 Support

For issues or questions:
1. Check the [Troubleshooting](#troubleshooting) section
2. Verify backend is running: `http://localhost:3000/api/sensors/latest`
3. Check `App.config` settings
4. Review backend logs for errors

---

**Built with ❤️ for IoT Students**

