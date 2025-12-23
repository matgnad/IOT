# Architecture & Design Decisions

## 🎯 Project Goal

Replace the web-based frontend (HTML/CSS/JavaScript) with a native Windows desktop application while keeping the existing backend infrastructure unchanged.

---

## 📊 System Architecture

### Overall Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      IOT DASHBOARD SYSTEM                       │
└─────────────────────────────────────────────────────────────────┘

┌─────────────┐         MQTT          ┌────────────────┐
│  ESP8266    │ ──────────────────────>│  MQTT Broker   │
│  Sensor     │   (esp8266/sensors)    │  (Mosquitto)   │
│             │   {"temp": 25.5,       └────────────────┘
│  DHT22      │    "humid": 60.2}              │
└─────────────┘                                │
                                               │ Subscribe
                                               ▼
                                    ┌──────────────────────┐
                                    │   Node.js Backend    │
                                    │   ─────────────────  │
                                    │   • Express Server   │
                                    │   • MQTT Client      │
                                    │   • REST API         │
                                    │   • MySQL Storage    │
                                    └──────────────────────┘
                                               │
                                               │ REST API
                                               │ (HTTP GET)
                                               │
                        ┌──────────────────────┴──────────────────────┐
                        │                                             │
                        ▼                                             ▼
              ┌─────────────────┐                           ┌─────────────────┐
              │  WEB FRONTEND   │                           │ DESKTOP APP     │
              │  (OLD - REMOVE) │                           │ (NEW - THIS)    │
              │                 │                           │                 │
              │  • HTML/CSS/JS  │                           │  • C# WinForms  │
              │  • Socket.IO    │                           │  • REST Client  │
              │  • Browser      │                           │  • Native UI    │
              └─────────────────┘                           └─────────────────┘
```

---

## 🤔 Key Design Decisions

### 1. Language Selection: **C# ✅**

**Options Considered**:
- C# with Windows Forms
- C with Win32 API

**Decision**: **C# was chosen**

**Rationale**:

| Criteria | C# | C |
|----------|----|----|
| **Development Speed** | Fast (days) | Slow (weeks) |
| **UI Framework** | WinForms built-in | Manual Win32 API |
| **HTTP Client** | `HttpClient` class | Low-level sockets |
| **JSON Parsing** | `Newtonsoft.Json` | External library (cJSON) |
| **Charting** | `System.Windows.Forms.DataVisualization.Charting` | Must implement from scratch |
| **Async Programming** | `async/await` support | Not available |
| **Learning Curve** | Moderate (suitable for students) | Steep (low-level) |
| **Maintainability** | High (readable, documented) | Low (verbose, complex) |
| **Community Support** | Excellent | Limited for GUI |

**Conclusion**: C# provides the right balance of power, simplicity, and productivity for an IoT student project.

---

### 2. Communication Strategy: **REST API ✅**

**Options Considered**:
- Option A: HTTP REST API (polling)
- Option B: MQTT Direct Subscription (real-time)

**Decision**: **REST API was chosen**

**Rationale**:

| Criteria | REST API | MQTT Direct |
|----------|----------|-------------|
| **Complexity** | Simple HTTP GET | Requires MQTT client library |
| **Backend Reuse** | ✅ Uses existing APIs | ❌ Bypasses backend |
| **Business Logic** | ✅ Centralized in backend | ❌ Duplicated in desktop app |
| **Statistics** | ✅ Backend calculates | ❌ Desktop app calculates |
| **Historical Data** | ✅ Paginated API | ❌ Must query DB directly |
| **Security** | ✅ Single point (backend) | ❌ Needs MQTT credentials |
| **Real-time Updates** | ⚠️ Polling (10s delay) | ✅ Instant |
| **Backend Modification** | ✅ None required | ❌ Would need changes |
| **Database Coupling** | ✅ Decoupled | ❌ Tightly coupled |

**Why REST API wins**:

1. **Existing Infrastructure**: Backend already has well-designed REST APIs:
   - `GET /api/sensors/latest` - Current values
   - `GET /api/sensors?page=1&limit=50` - Historical data
   - `GET /api/sensors/today` - Today's statistics

2. **Separation of Concerns**: Backend handles all business logic
   - Data validation
   - Statistics calculation
   - Database queries
   - Error handling

3. **Maintainability**: Changes to data processing only require backend updates

4. **Simplicity**: HTTP GET requests are easier for students to understand than MQTT protocols

5. **Acceptable Latency**: 10-second polling is sufficient for temperature/humidity monitoring (not critical real-time)

**When MQTT would be better**:
- Critical real-time requirements (< 1 second latency)
- High-frequency sensor updates (> 1 Hz)
- Bi-directional communication needed (sending commands to ESP8266)
- No backend available

---

### 3. UI Update Strategy: **Timer-based Polling**

**Implementation**:
```csharp
// Refresh every 10 seconds
_refreshTimer = new System.Windows.Forms.Timer();
_refreshTimer.Interval = 10000; // 10 seconds
_refreshTimer.Tick += RefreshTimer_Tick;
```

**Why 10 seconds?**
- Temperature/humidity changes slowly (not critical timing)
- Reduces server load
- Prevents excessive database queries
- Provides acceptable user experience

**Alternative considered**: WebSocket for push notifications
- More complex to implement
- Requires backend modification
- Overkill for temperature monitoring

---

## 🏗️ Application Architecture

### Layered Architecture

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  ─────────────────────────────────────  │
│  MainForm.cs (UI Logic)                 │
│  MainForm.Designer.cs (UI Controls)     │
│  • Labels, Buttons, Chart, DataGrid    │
│  • Event handlers                       │
│  • Timer-based refresh                  │
└─────────────────────────────────────────┘
                  │
                  │ Calls
                  ▼
┌─────────────────────────────────────────┐
│          Service Layer                  │
│  ─────────────────────────────────────  │
│  SensorApiService.cs                    │
│  • GetLatestAsync()                     │
│  • GetHistoryAsync()                    │
│  • GetTodayDataAsync()                  │
│  • CalculateStatistics()                │
│  • TestConnectionAsync()                │
└─────────────────────────────────────────┘
                  │
                  │ Uses
                  ▼
┌─────────────────────────────────────────┐
│          Model Layer                    │
│  ─────────────────────────────────────  │
│  SensorData.cs                          │
│  ApiResponse.cs (DTOs)                  │
│  Statistics.cs                          │
└─────────────────────────────────────────┘
                  │
                  │ Communicates via
                  ▼
┌─────────────────────────────────────────┐
│          Backend (Node.js)              │
│  ─────────────────────────────────────  │
│  REST API Endpoints                     │
│  • /api/sensors/latest                  │
│  • /api/sensors                         │
│  • /api/sensors/today                   │
└─────────────────────────────────────────┘
```

### Design Patterns Used

1. **Service Layer Pattern**: `SensorApiService` encapsulates all HTTP communication
2. **DTO (Data Transfer Objects)**: `ApiResponse.cs` models match backend JSON
3. **Model-View Separation**: Models are independent of UI
4. **Async/Await Pattern**: Non-blocking UI updates
5. **Timer Pattern**: Periodic data refresh

---

## 🔄 Data Flow

### Startup Sequence

```
1. Program.cs
   └─> Create MainForm

2. MainForm.InitializeComponent()
   └─> Create all UI controls (labels, buttons, chart, grid)

3. MainForm.Load event
   ├─> Initialize SensorApiService
   ├─> Call RefreshAllDataAsync()
   └─> Start 10-second timer

4. RefreshAllDataAsync()
   ├─> Fetch latest sensor data
   ├─> Fetch today's data
   ├─> Fetch historical data
   └─> Update all UI components
```

### Refresh Cycle (Every 10 seconds)

```
Timer Tick
   │
   ├─> 1. Call /api/sensors/latest
   │      └─> Update temperature & humidity cards
   │
   ├─> 2. Call /api/sensors/today
   │      ├─> Calculate statistics (min/max/avg)
   │      ├─> Update statistics labels
   │      └─> Update line chart
   │
   └─> 3. Call /api/sensors?page=1&limit=50
          └─> Update DataGridView with latest 50 records
```

### API Request Flow

```
MainForm (UI Thread)
   │
   │ async call
   ▼
SensorApiService.GetLatestAsync()
   │
   │ HTTP GET
   ▼
HttpClient.GetAsync("http://localhost:3000/api/sensors/latest")
   │
   │ Network request
   ▼
Node.js Backend
   │
   │ Database query
   ▼
MySQL Database
   │
   │ JSON response
   ▼
{"success": true, "data": {"temperature": 25.5, "humidity": 60.2}}
   │
   │ Deserialize
   ▼
SensorData object
   │
   │ Return to UI
   ▼
MainForm.UpdateLatestDisplay()
   │
   └─> lblTemperatureValue.Text = "25.5°C"
```

---

## 📦 Project Structure Explained

```
IOT-Desktop-App/
│
├── IOT-Dashboard.csproj         # .NET project configuration
│   • Target framework: net8.0-windows
│   • NuGet packages: Newtonsoft.Json, Chart
│
├── Program.cs                    # Application entry point
│   • [STAThread] Main method
│   • Creates and runs MainForm
│
├── App.config                    # Runtime configuration
│   • ApiBaseUrl setting
│   • RefreshInterval setting
│
├── Forms/
│   ├── MainForm.cs               # UI logic & event handlers
│   │   • Timer setup
│   │   • Async data fetching
│   │   • UI update methods
│   │
│   └── MainForm.Designer.cs      # UI controls & layout
│       • Control declarations
│       • Visual styling
│       • Event wire-up
│
├── Services/
│   └── SensorApiService.cs       # HTTP client wrapper
│       • RESTful API calls
│       • JSON deserialization
│       • Error handling
│
├── Models/
│   ├── SensorData.cs             # Domain model
│   │   • Id, Temperature, Humidity, MeasuredAt
│   │
│   ├── ApiResponse.cs            # DTOs for JSON responses
│   │   • LatestSensorResponse
│   │   • SensorListResponse
│   │   • TodayDataResponse
│   │
│   └── Statistics.cs             # Statistics data model
│       • Min/Max/Avg for temp & humidity
│
└── Documentation/
    ├── README.md                 # Full documentation
    ├── QUICK_START.md            # 3-minute setup guide
    └── ARCHITECTURE.md           # This file
```

---

## 🎨 UI Design Philosophy

### Color Scheme

- **Temperature Card**: Red (#E74C3C) - warm color for heat
- **Humidity Card**: Blue (#3498DB) - cool color for water
- **Refresh Button**: Green (#2ECC71) - positive action
- **Title Bar**: Professional blue (#2980B9)
- **Background**: Light gray (#ECF0F1) - reduces eye strain

### Layout Strategy

1. **Top Section**: Most important data (current values) - large, visible
2. **Middle Section**: Statistics - compact, informative
3. **Bottom Section**: Historical data - detailed, scrollable
4. **Status Bar**: Connection status - always visible

### User Experience Goals

- **At-a-glance information**: Large cards for current values
- **Minimal clicks**: Auto-refresh eliminates manual updates
- **Visual feedback**: Status bar shows connection state
- **Error handling**: Clear messages for connection failures
- **Responsive**: Non-blocking async operations

---

## 🔐 Security Considerations

### Current Implementation (Basic)

- No authentication required for sensor data
- HTTP (not HTTPS) communication
- No input validation needed (read-only)

### Production Recommendations

1. **Use HTTPS**: Encrypt API communication
2. **Add Authentication**: API keys or JWT tokens
3. **Rate Limiting**: Prevent API abuse
4. **Input Validation**: Sanitize any user inputs
5. **Error Logging**: Don't expose internal errors to users

---

## 📈 Scalability Considerations

### Current Limitations

- Single user (desktop app)
- Polling creates constant server load
- No data caching

### Future Improvements

1. **WebSocket**: Replace polling for true real-time updates
2. **Local Caching**: Store recent data locally to reduce API calls
3. **Batch Requests**: Combine multiple API calls into one
4. **Offline Mode**: Cache data for viewing when backend is down
5. **Multi-sensor Support**: Extend to handle multiple ESP8266 devices

---

## 🧪 Testing Strategy

### Manual Testing Checklist

- [ ] Application starts without errors
- [ ] Connects to backend successfully
- [ ] Displays latest temperature and humidity
- [ ] Shows correct statistics (min/max/avg)
- [ ] Chart renders with correct data
- [ ] Historical table populates
- [ ] Auto-refresh updates data every 10 seconds
- [ ] Manual refresh button works
- [ ] Connection test button works
- [ ] Handles backend offline gracefully
- [ ] Status bar updates correctly

### Integration Testing

- Backend running: Normal operation
- Backend stopped: Error messages displayed
- Backend slow: Timeout handling
- Invalid JSON: Parse error handling
- Empty database: "No data" message

---

## 🚀 Deployment Options

### Option 1: Framework-Dependent (.NET 6 required)

```bash
dotnet publish -c Release
```
Size: ~500 KB (requires .NET 6 runtime on target PC)

### Option 2: Self-Contained (No .NET required)

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```
Size: ~70 MB (includes .NET runtime)

### Option 3: Single File Executable

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
Size: ~70 MB (single .exe file)

---

## 📚 Learning Outcomes

Students working on this project will learn:

1. **C# Programming**: Classes, async/await, LINQ
2. **Windows Forms**: UI design, event handling, threading
3. **REST APIs**: HTTP clients, JSON parsing, error handling
4. **Architecture**: Layered design, separation of concerns
5. **IoT Integration**: How sensors connect to applications
6. **Data Visualization**: Charts, tables, real-time updates

---

## 🔧 Alternative Approaches Considered

### 1. Web App with Electron
- **Pros**: Cross-platform, web technologies
- **Cons**: Large bundle size (~150 MB), slower startup
- **Verdict**: ❌ Overkill for simple dashboard

### 2. WPF (Windows Presentation Foundation)
- **Pros**: Modern XAML UI, better graphics
- **Cons**: Steeper learning curve than WinForms
- **Verdict**: ⚠️ Good alternative, but WinForms is simpler

### 3. UWP (Universal Windows Platform)
- **Pros**: Modern Windows 10/11 app
- **Cons**: Complex deployment, Windows Store required
- **Verdict**: ❌ Too restrictive for student project

### 4. Console Application
- **Pros**: Very simple
- **Cons**: No charts, no visual appeal
- **Verdict**: ❌ Doesn't meet requirements

---

## 📊 Performance Metrics

**Expected Performance**:
- **Startup Time**: < 2 seconds
- **API Response Time**: 100-500 ms (depends on network)
- **UI Update Time**: < 100 ms
- **Memory Usage**: ~50-80 MB
- **CPU Usage**: < 5% (idle), ~10% (updating)

**Optimization Opportunities**:
- Cache API responses for 5 seconds
- Reduce chart rendering frequency
- Virtualize large data grids
- Compress API responses (gzip)

---

## ✅ Requirements Met

| Requirement | Status | Implementation |
|-------------|--------|----------------|
| Display latest temp/humidity | ✅ | Large cards with live values |
| Display statistics (min/max/avg) | ✅ | Statistics section with calculations |
| Display historical data table | ✅ | DataGridView with last 50 records |
| Display line chart | ✅ | Dual-line chart (temp + humidity) |
| Fetch from backend API | ✅ | HTTP GET via REST API |
| No web browser/HTML/JS | ✅ | Native C# WinForms application |
| No backend modifications | ✅ | Uses existing APIs unchanged |
| No database changes | ✅ | Uses existing schema |
| Windows-only | ✅ | .NET 6 Windows target |
| Maintainable code | ✅ | Layered architecture, documented |

---

**Architecture designed for clarity, maintainability, and student learning** 🎓

