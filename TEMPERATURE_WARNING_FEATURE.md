# 🌡️ Temperature Warning Feature - Complete Guide

## 📋 **OVERVIEW**

**Feature**: Automatic warnings when temperature exceeds **30°C**

**Implementation**: Multi-layer warning system across the entire IoT stack

---

## 🏗️ **ARCHITECTURE**

The warning system is implemented at **3 layers** for maximum effectiveness:

```
┌─────────────────────────────────────────────────────────┐
│                  LAYER 1: ESP8266                       │
│  ➤ Activates built-in LED when temp > 30°C            │
│  ➤ Serial logs warning message                         │
│  ➤ Physical notification on device                     │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│                  LAYER 2: Backend                       │
│  ➤ Detects high temperature in MQTT handler            │
│  ➤ Logs warning to console                             │
│  ➤ Emits Socket.IO event with warning flag             │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│                LAYER 3: Desktop App                     │
│  ➤ Temperature card turns darker red                   │
│  ➤ Shows "⚠️ HIGH TEMP WARNING" label                  │
│  ➤ Status bar displays warning message                 │
│  ➤ Popup notification (once per session)               │
└─────────────────────────────────────────────────────────┘
```

---

## 📍 **CODE LOCATIONS**

### **LAYER 1: ESP8266 Device**

**File**: `IOT-Website/iot-backend-mvc/public/IOT_FINAL/IOT_FINAL.ino`

#### **Configuration (Lines 31-34)**
```cpp
// Temperature warning LED (optional - can use built-in LED)
#define TEMP_WARNING_LED LED_BUILTIN  // GPIO2 (built-in LED)
#define WARNING_TEMP 30.0  // Temperature threshold in °C
```

**What it does**: Defines the warning LED pin and temperature threshold

---

#### **Setup (Lines 150-152)**
```cpp
// Setup warning LED
pinMode(TEMP_WARNING_LED, OUTPUT);
digitalWrite(TEMP_WARNING_LED, HIGH); // OFF (inverted logic)
```

**What it does**: Initializes the warning LED as output

---

#### **Warning Logic (Lines 192-198)**
```cpp
// Temperature warning LED control
if (t > WARNING_TEMP) {
  digitalWrite(TEMP_WARNING_LED, LOW);  // ON (inverted)
  Serial.print("🔥 HIGH TEMP WARNING: "); 
  Serial.print(t); 
  Serial.println("°C");
} else {
  digitalWrite(TEMP_WARNING_LED, HIGH); // OFF
}
```

**What it does**:
- Turns on built-in LED when temp > 30°C
- Logs warning to Serial monitor
- Automatically turns off when temp drops below threshold

**How to customize**:
- Change `WARNING_TEMP` to your desired threshold
- Change `TEMP_WARNING_LED` to use external LED (e.g., `LED1` = GPIO14)

---

### **LAYER 2: Backend (Node.js)**

**File**: `IOT-Website/iot-backend-mvc/src/services/mqttService.js`

#### **Configuration (Line 62)**
```javascript
// Temperature warning threshold
const WARNING_TEMP = 30.0; // °C
const isHighTemp = temperature > WARNING_TEMP;
```

**What it does**: Defines the backend warning threshold

---

#### **Warning Detection (Lines 72-75)**
```javascript
// Log temperature warning
if (isHighTemp) {
  console.warn(`[WARNING] 🔥 High temperature detected: ${temperature}°C (threshold: ${WARNING_TEMP}°C)`);
}
```

**What it does**: Logs warning to backend console

---

#### **Socket.IO Event (Lines 77-87)**
```javascript
io.emit('sensor:update', {
  temperature,
  humidity,
  measured_at: sensorRecord.measured_at,
  warning: isHighTemp ? {
    type: 'high_temperature',
    message: `Temperature exceeds ${WARNING_TEMP}°C`,
    value: temperature
  } : null
});
```

**What it does**: Emits warning data to real-time web clients (if you add web frontend)

---

### **LAYER 3: Desktop App (C# WinForms)**

**File**: `IOT-Desktop-App/Forms/MainForm.cs`

#### **Configuration & State (Lines 16-17)**
```csharp
private bool _highTempWarningShown = false; // Track if popup shown
```

**What it does**: Prevents spam by showing popup only once per session

---

#### **Warning Logic (Lines 96-126)**
```csharp
private void UpdateLatestDisplay(SensorData data)
{
    lblTemperatureValue.Text = $"{data.Temperature:F1}°C";
    lblHumidityValue.Text = $"{data.Humidity:F1}%";
    
    // Temperature warning at 30°C
    const float WARNING_TEMP = 30.0f;
    if (data.Temperature > WARNING_TEMP)
    {
        // 1. Change card color to darker red
        pnlTempCard.BackColor = Color.FromArgb(192, 57, 43);
        
        // 2. Show warning label
        lblTempWarning.Visible = true;
        lblTempWarning.Text = $"⚠️ HIGH TEMP WARNING";
        
        // 3. Update status bar
        lblStatus.Text = $"⚠️ WARNING: Temperature {data.Temperature:F1}°C exceeds {WARNING_TEMP}°C!";
        lblStatus.ForeColor = Color.Orange;
        
        // 4. Show popup notification (once)
        if (!_highTempWarningShown)
        {
            MessageBox.Show(
                $"High temperature detected!\n\nCurrent: {data.Temperature:F1}°C\nThreshold: {WARNING_TEMP}°C",
                "Temperature Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _highTempWarningShown = true;
        }
    }
    else
    {
        // Reset to normal appearance
        pnlTempCard.BackColor = Color.FromArgb(231, 76, 60);
        lblTempWarning.Visible = false;
        
        // Reset popup flag with hysteresis (2°C buffer)
        if (_highTempWarningShown && data.Temperature < WARNING_TEMP - 2.0f)
        {
            _highTempWarningShown = false;
        }
    }
}
```

**What it does**:
1. **Visual**: Changes temperature card color from red to dark red
2. **Label**: Shows "⚠️ HIGH TEMP WARNING" inside the card
3. **Status Bar**: Displays warning message with orange text
4. **Popup**: Shows MessageBox once per session (with 2°C hysteresis)

---

**File**: `IOT-Desktop-App/Forms/MainForm.Designer.cs`

#### **UI Control Declaration (Lines 88-95)**
```csharp
// Temperature warning label
this.lblTempWarning = new Label();
this.lblTempWarning.Text = "⚠️ HIGH TEMP";
this.lblTempWarning.Font = new Font("Segoe UI", 9, FontStyle.Bold);
this.lblTempWarning.ForeColor = Color.Yellow;
this.lblTempWarning.Location = new Point(15, 105);
this.lblTempWarning.Visible = false; // Hidden by default
```

**What it does**: Creates the warning label UI control

---

## 🎨 **VISUAL BEHAVIOR**

### **Normal Temperature (≤ 30°C)**
```
┌─────────────────────┐
│   TEMPERATURE       │ ← Regular red background
│                     │   (RGB: 231, 76, 60)
│      25.5°C         │
│                     │
└─────────────────────┘
```

### **High Temperature (> 30°C)**
```
┌─────────────────────┐
│   TEMPERATURE       │ ← Darker red background
│                     │   (RGB: 192, 57, 43)
│      32.8°C         │
│ ⚠️ HIGH TEMP WARNING│ ← Yellow warning label
└─────────────────────┘

[Status Bar] ⚠️ WARNING: Temperature 32.8°C exceeds 30.0°C!

[Popup - First Time Only]
╔══════════════════════════╗
║  Temperature Warning     ║
╠══════════════════════════╣
║ High temperature         ║
║ detected!                ║
║                          ║
║ Current: 32.8°C         ║
║ Threshold: 30.0°C       ║
║                          ║
║        [  OK  ]          ║
╚══════════════════════════╝
```

---

## ⚙️ **CUSTOMIZATION**

### **Change Warning Threshold**

**To change from 30°C to 35°C:**

1. **ESP8266** (`IOT_FINAL.ino` line 34):
   ```cpp
   #define WARNING_TEMP 35.0
   ```

2. **Backend** (`mqttService.js` line 62):
   ```javascript
   const WARNING_TEMP = 35.0;
   ```

3. **Desktop App** (`MainForm.cs` line 102):
   ```csharp
   const float WARNING_TEMP = 35.0f;
   ```

---

### **Use External LED on ESP8266**

**Instead of built-in LED, use GPIO14 (D5):**

```cpp
// Change line 33 from:
#define TEMP_WARNING_LED LED_BUILTIN

// To:
#define TEMP_WARNING_LED LED1  // GPIO14 (D5)

// Note: LED1 uses normal logic (HIGH = ON, LOW = OFF)
// So change lines 196-197:
if (t > WARNING_TEMP) {
  digitalWrite(TEMP_WARNING_LED, HIGH);  // ON (normal logic)
} else {
  digitalWrite(TEMP_WARNING_LED, LOW);   // OFF
}
```

---

### **Disable Popup Notification**

**In `MainForm.cs`, comment out lines 116-124:**

```csharp
// if (!_highTempWarningShown)
// {
//     MessageBox.Show(...);
//     _highTempWarningShown = true;
// }
```

**Result**: Visual warnings only (no popup dialog)

---

### **Add Email/SMS Alerts**

**In backend `mqttService.js`, add after line 75:**

```javascript
if (isHighTemp) {
  console.warn(`[WARNING] 🔥 High temperature: ${temperature}°C`);
  
  // Add email alert
  sendEmailAlert(temperature);
  
  // Add SMS alert
  sendSMSAlert(temperature);
}
```

---

## 🧪 **TESTING**

### **Test 1: Simulate High Temperature**

**Option A: Manual MQTT Message**
```bash
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":35.0,"humid":60.0}'
```

**Expected**:
- Backend logs: `[WARNING] 🔥 High temperature detected: 35.0°C`
- Desktop app: Red card, warning label, status bar message, popup

---

**Option B: Heat the Sensor**
```
1. Gently heat DHT11 sensor (e.g., warm breath, hand heat)
2. Wait for temperature to rise above 30°C
3. Observe all three layers
```

**Expected**:
- ESP8266: Built-in LED turns on, serial shows "🔥 HIGH TEMP WARNING"
- Backend: Console shows warning
- Desktop app: Visual warnings appear

---

### **Test 2: Verify Reset Behavior**

**Procedure**:
```
1. Trigger high temp warning (temp > 30°C)
2. Observe popup appears
3. Let temperature drop below 28°C (hysteresis)
4. Heat sensor again to > 30°C
5. Popup should appear again (flag reset)
```

---

### **Test 3: Verify LED on ESP8266**

**Serial Monitor Output (when temp > 30°C)**:
```
📡 Publishing to esp8266/sensors: {"temp":32.5,"humid":60.2} ✅ [SUCCESS]
🔥 HIGH TEMP WARNING: 32.5°C
```

**Physical Check**:
- Built-in LED on ESP8266 should be **ON** (glowing)

---

## 📊 **MONITORING**

### **Backend Logs**

**Normal Operation**:
```
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":25.5,"humid":60.2}
[DB] ✅ Saved: ID=123 T=25.5°C H=60.2%
[Socket.IO] ✅ Emitted sensor:update
```

**Warning Triggered**:
```
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":32.8,"humid":60.2}
[WARNING] 🔥 High temperature detected: 32.8°C (threshold: 30.0°C)
[DB] ✅ Saved: ID=124 T=32.8°C H=60.2%
[Socket.IO] ✅ Emitted sensor:update [WITH WARNING]
```

---

### **ESP8266 Serial Monitor**

**Normal**:
```
📡 Publishing to esp8266/sensors: {"temp":25.5,"humid":60.2} ✅ [SUCCESS]
```

**Warning**:
```
🔥 HIGH TEMP WARNING: 32.8°C
📡 Publishing to esp8266/sensors: {"temp":32.8,"humid":60.2} ✅ [SUCCESS]
```

---

## 🔧 **TROUBLESHOOTING**

| Issue | Cause | Fix |
|-------|-------|-----|
| LED doesn't turn on | Wrong pin or inverted logic | Check `TEMP_WARNING_LED` definition |
| No backend warning | Threshold not reached | Lower `WARNING_TEMP` for testing |
| No desktop app warning | Stale code | Rebuild: `dotnet build` |
| Popup shows repeatedly | Flag not set | Check `_highTempWarningShown` variable |
| Card color doesn't change | `pnlTempCard` not defined | Check Designer.cs has `this.pnlTempCard` |

---

## 📋 **FEATURE CHECKLIST**

After deploying, verify:

- [ ] **ESP8266**: Built-in LED turns on when hot
- [ ] **ESP8266**: Serial shows "🔥 HIGH TEMP WARNING"
- [ ] **Backend**: Console shows warning with 🔥 emoji
- [ ] **Desktop**: Temperature card turns darker red
- [ ] **Desktop**: Warning label appears in card
- [ ] **Desktop**: Status bar shows warning
- [ ] **Desktop**: Popup appears (first time)
- [ ] **Desktop**: Popup doesn't spam (after first time)
- [ ] **Desktop**: Warnings clear when temp drops
- [ ] **Desktop**: Popup can appear again after temp normalizes

---

## 🚀 **DEPLOYMENT**

### **Step 1: Upload ESP8266 Code**
```bash
1. Open Arduino IDE
2. Open: IOT-Website/iot-backend-mvc/public/IOT_FINAL/IOT_FINAL.ino
3. Upload to ESP8266
4. Open Serial Monitor
```

### **Step 2: Restart Backend**
```bash
cd C:\UP\iot\IOT-Website\iot-backend-mvc
npm start
```

### **Step 3: Rebuild Desktop App**
```bash
cd C:\UP\iot\IOT-Desktop-App
dotnet build
dotnet run
```

### **Step 4: Test**
```bash
# Simulate high temp
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":35.0,"humid":60.0}'
```

---

## 📊 **SUMMARY**

| Layer | Location | Warning Type | Trigger |
|-------|----------|--------------|---------|
| **ESP8266** | `IOT_FINAL.ino` lines 192-198 | LED + Serial log | temp > 30°C |
| **Backend** | `mqttService.js` lines 62-87 | Console log + Socket.IO | temp > 30°C |
| **Desktop** | `MainForm.cs` lines 96-126 | Visual + Popup | temp > 30°C |

**All warnings are synchronized** - they all trigger at the same 30°C threshold.

---

## 🎯 **BENEFITS**

✅ **Multi-layer redundancy**: If one layer fails, others still alert  
✅ **Physical notification**: LED visible on device itself  
✅ **Remote monitoring**: Desktop app shows warnings instantly  
✅ **Logging**: Backend records all warning events  
✅ **User-friendly**: Visual indicators + popup notification  
✅ **Non-intrusive**: Popup appears only once per session  
✅ **Smart reset**: Hysteresis prevents flapping warnings  

---

**Temperature warning system is now fully operational!** 🎉

For questions or customization, refer to the code locations above.

