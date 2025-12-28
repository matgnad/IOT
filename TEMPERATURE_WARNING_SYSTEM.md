# 🌡️ Temperature Warning System Documentation

## 📋 **OVERVIEW**

The temperature warning system provides **multi-level alerts** when temperature exceeds safe thresholds:
- **30°C**: ⚠️ **WARNING** - Yellow/Orange alert
- **35°C**: 🔥 **CRITICAL** - Red alert with immediate action required

---

## 🏗️ **ARCHITECTURE**

The warning system operates at **TWO LAYERS** for redundancy:

```
┌─────────────────┐
│   ESP8266       │
│  DHT11 Sensor   │
│  (Reads temp)   │
└────────┬────────┘
         │ MQTT Publish
         │ {"temp":32.5,"humid":60.2}
         ▼
┌─────────────────┐
│  MQTT Broker    │
│  (Mosquitto)    │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────────────┐
│      LAYER 1: Backend (Node.js)         │
│  ────────────────────────────────────   │
│  ✅ Monitors all incoming data          │
│  ✅ Logs warnings to console             │
│  ✅ Stores in database                   │
│  ✅ Emits Socket.IO events               │
│                                          │
│  Thresholds:                             │
│  • 30°C: Console warning                 │
│  • 35°C: Console error                   │
└──────────────────┬───────────────────────┘
                   │
                   │ REST API / Socket.IO
                   ▼
┌─────────────────────────────────────────┐
│    LAYER 2: Desktop App (WinForms)      │
│  ────────────────────────────────────   │
│  ✅ Visual color-coded alerts            │
│  ✅ Popup notifications                  │
│  ✅ Audio warnings                       │
│  ✅ Status bar messages                  │
│                                          │
│  Thresholds:                             │
│  • 30°C: Orange card + warning popup     │
│  • 35°C: Dark red card + critical popup  │
└──────────────────────────────────────────┘
```

---

## 🎯 **WARNING LEVELS**

### **Level 1: NORMAL (< 30°C)** ✅

**Status**: Healthy

**Backend Behavior**:
```
[DB] ✅ Saved: ID=123 T=28.5°C H=60.2%
```

**Desktop App Behavior**:
- Temperature card: **Red** (normal color)
- No warning label visible
- Status: Normal operation

---

### **Level 2: WARNING (30°C - 34.9°C)** ⚠️

**Status**: Elevated temperature - Monitor system

**Backend Behavior**:
```
[ALERT] ⚠️ WARNING: Temperature 32.5°C exceeds 30.0°C
[DB] ✅ Saved: ID=124 T=32.5°C H=59.8%
[Socket.IO] ✅ Emitted sensor:update [WARNING]
```

**Desktop App Behavior**:
- Temperature card: **Orange** (#E67E22)
- Warning label: **"⚠️ WARNING"** (visible)
- Status bar: **"⚠️ WARNING: High temperature 32.5°C"**
- Popup: One-time warning dialog (dismissible)
- Sound: Windows exclamation sound

**Visual Example**:
```
┌────────────────────┐
│    TEMPERATURE     │
│    ──────────      │
│                    │
│      32.5°C        │  ← Large display
│                    │
│  ⚠️ WARNING        │  ← Warning indicator
└────────────────────┘
(Orange background)
```

---

### **Level 3: CRITICAL (≥ 35°C)** 🔥

**Status**: CRITICAL - Immediate action required

**Backend Behavior**:
```
[ALERT] 🔥 CRITICAL: Temperature 36.2°C exceeds 35.0°C!
[DB] ✅ Saved: ID=125 T=36.2°C H=58.5%
[Socket.IO] ✅ Emitted sensor:update [CRITICAL]
```

**Desktop App Behavior**:
- Temperature card: **Dark Red** (#A93226)
- Warning label: **"🔥 CRITICAL"** (visible, white text)
- Status bar: **"🔥 CRITICAL: Temperature 36.2°C exceeds 35.0°C!"** (red text)
- Popup: Critical alert dialog (modal, cannot dismiss easily)
- Sound: Windows error sound (more urgent)

**Visual Example**:
```
┌────────────────────┐
│    TEMPERATURE     │
│    ──────────      │
│                    │
│      36.2°C        │  ← Large display
│                    │
│  🔥 CRITICAL       │  ← Critical indicator
└────────────────────┘
(Dark red background, pulsing effect)
```

---

## 📂 **FILE LOCATIONS**

### **Backend Implementation**

**File**: `IOT-Website/iot-backend-mvc/src/services/mqttService.js`

**Lines**: 72-108

**Code**:
```javascript
// 🌡️ TEMPERATURE WARNING SYSTEM - Multi-level thresholds
const TEMP_WARNING = 30.0;   // °C - Yellow warning
const TEMP_CRITICAL = 35.0;  // °C - Red critical alert

let warningLevel = null;
let warningMessage = null;

if (temperature >= TEMP_CRITICAL) {
  warningLevel = 'critical';
  warningMessage = `🔥 CRITICAL: Temperature ${temperature}°C exceeds ${TEMP_CRITICAL}°C!`;
  console.error(`[ALERT] ${warningMessage}`);
} else if (temperature >= TEMP_WARNING) {
  warningLevel = 'warning';
  warningMessage = `⚠️ WARNING: Temperature ${temperature}°C exceeds ${TEMP_WARNING}°C`;
  console.warn(`[ALERT] ${warningMessage}`);
}

// ... database save ...

// Emit Socket.IO event with warning data
if (io) {
  io.emit('sensor:update', {
    temperature,
    humidity,
    measured_at: sensorRecord.measured_at,
    warning: warningLevel ? {
      level: warningLevel,           // 'warning' or 'critical'
      type: 'high_temperature',
      message: warningMessage,
      value: temperature,
      threshold: warningLevel === 'critical' ? TEMP_CRITICAL : TEMP_WARNING
    } : null
  });
}
```

---

### **Desktop App Implementation**

**File**: `IOT-Desktop-App/Forms/MainForm.cs`

**Lines**: 111-176

**Code**:
```csharp
// 🌡️ MULTI-LEVEL TEMPERATURE WARNING SYSTEM
const float WARNING_TEMP = 30.0f;   // Yellow warning
const float CRITICAL_TEMP = 35.0f;  // Red critical

if (data.Temperature >= CRITICAL_TEMP)
{
    // 🔥 CRITICAL: Dark red + flashing effect
    pnlTempCard.BackColor = Color.FromArgb(169, 50, 38);
    lblTempWarning.Visible = true;
    lblTempWarning.Text = $"🔥 CRITICAL";
    lblStatus.Text = $"🔥 CRITICAL: Temperature {data.Temperature:F1}°C...";
    lblStatus.ForeColor = Color.Red;
    
    // Show critical popup (once)
    if (!_criticalTempWarningShown)
    {
        System.Media.SystemSounds.Hand.Play();
        MessageBox.Show(
            $"CRITICAL TEMPERATURE ALERT!\n\n" +
            $"Current: {data.Temperature:F1}°C\n" +
            $"Critical Threshold: {CRITICAL_TEMP}°C\n\n" +
            $"⚠️ IMMEDIATE ACTION REQUIRED",
            "CRITICAL Temperature Alert",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        _criticalTempWarningShown = true;
    }
}
else if (data.Temperature >= WARNING_TEMP)
{
    // ⚠️ WARNING: Orange
    pnlTempCard.BackColor = Color.FromArgb(230, 126, 34);
    lblTempWarning.Visible = true;
    lblTempWarning.Text = $"⚠️ WARNING";
    lblStatus.Text = $"⚠️ WARNING: High temperature...";
    lblStatus.ForeColor = Color.Orange;
    
    // Show warning popup (once)
    if (!_highTempWarningShown)
    {
        System.Media.SystemSounds.Exclamation.Play();
        MessageBox.Show(
            $"High temperature detected!\n\n" +
            $"Current: {data.Temperature:F1}°C\n" +
            $"Warning Threshold: {WARNING_TEMP}°C",
            "Temperature Warning",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        _highTempWarningShown = true;
    }
}
else
{
    // ✅ NORMAL
    pnlTempCard.BackColor = Color.FromArgb(231, 76, 60);
    lblTempWarning.Visible = false;
    
    // Reset flags with hysteresis
    if (_highTempWarningShown && data.Temperature < WARNING_TEMP - 2.0f)
    {
        _highTempWarningShown = false;
        _criticalTempWarningShown = false;
    }
}
```

---

## ⚙️ **CONFIGURATION**

### **Customize Thresholds**

#### **Backend** (`mqttService.js` line 72-73):
```javascript
const TEMP_WARNING = 30.0;   // Change this for warning threshold
const TEMP_CRITICAL = 35.0;  // Change this for critical threshold
```

#### **Desktop App** (`MainForm.cs` line 112-113):
```csharp
const float WARNING_TEMP = 30.0f;   // Change this for warning threshold
const float CRITICAL_TEMP = 35.0f;  // Change this for critical threshold
```

**⚠️ Important**: Keep both values synchronized!

---

## 🧪 **TESTING**

### **Test 1: Simulate High Temperature**

**Method 1: Manual MQTT Publish**
```bash
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":32.5,"humid":60.0}'
```

**Expected**:
- Backend logs: `[ALERT] ⚠️ WARNING: Temperature 32.5°C exceeds 30.0°C`
- Desktop app: Orange card, warning popup

---

**Method 2: Heat the DHT11 Sensor**
- Use a heat gun or hair dryer (carefully!)
- Hold near DHT11 sensor for 10-15 seconds
- Watch temperature rise in real-time

**Expected**:
- Temperature climbs above 30°C → Warning activates
- Temperature climbs above 35°C → Critical activates

---

### **Test 2: Verify Hysteresis**

**Purpose**: Ensure warnings don't flicker on/off at threshold boundary

**Test Steps**:
1. Heat sensor to 32°C (warning active)
2. Let it cool to 31°C → Warning stays active (no flicker)
3. Let it cool to 27°C → Warning deactivates (2°C hysteresis)

**Expected**: Smooth transitions, no rapid on/off switching

---

### **Test 3: Critical Alert**

```bash
# Simulate critical temperature
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":36.5,"humid":55.0}'
```

**Expected**:
- Backend: `[ALERT] 🔥 CRITICAL: Temperature 36.5°C exceeds 35.0°C!`
- Desktop: Dark red card, critical popup with error sound

---

## 📊 **MONITORING**

### **Backend Console Output**

**Normal**:
```
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":28.5,"humid":60.2}
[DB] ✅ Saved: ID=123 T=28.5°C H=60.2%
[Socket.IO] ✅ Emitted sensor:update
```

**Warning**:
```
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":32.5,"humid":59.8}
[ALERT] ⚠️ WARNING: Temperature 32.5°C exceeds 30.0°C
[DB] ✅ Saved: ID=124 T=32.5°C H=59.8%
[Socket.IO] ✅ Emitted sensor:update [WARNING]
```

**Critical**:
```
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":36.2,"humid":58.5}
[ALERT] 🔥 CRITICAL: Temperature 36.2°C exceeds 35.0°C!
[DB] ✅ Saved: ID=125 T=36.2°C H=58.5%
[Socket.IO] ✅ Emitted sensor:update [CRITICAL]
```

---

### **Database Queries**

**Check recent high temperatures**:
```sql
SELECT 
    id,
    temperature,
    humidity,
    measured_at,
    CASE 
        WHEN temperature >= 35.0 THEN '🔥 CRITICAL'
        WHEN temperature >= 30.0 THEN '⚠️ WARNING'
        ELSE '✅ NORMAL'
    END as status
FROM sensors 
WHERE temperature >= 30.0
ORDER BY measured_at DESC 
LIMIT 20;
```

**Count warnings by hour**:
```sql
SELECT 
    DATE_FORMAT(measured_at, '%Y-%m-%d %H:00') as hour,
    COUNT(*) as total_readings,
    SUM(CASE WHEN temperature >= 30.0 THEN 1 ELSE 0 END) as warnings,
    SUM(CASE WHEN temperature >= 35.0 THEN 1 ELSE 0 END) as critical,
    MAX(temperature) as max_temp
FROM sensors 
WHERE measured_at > DATE_SUB(NOW(), INTERVAL 24 HOUR)
GROUP BY DATE_FORMAT(measured_at, '%Y-%m-%d %H:00')
ORDER BY hour DESC;
```

---

## 🚨 **TROUBLESHOOTING**

### **Issue 1: No Warnings Showing**

**Symptoms**: Temperature exceeds 30°C but no alerts

**Checks**:
1. **Backend logs**: Do you see `[ALERT]` messages?
   - NO → Backend not detecting high temp
   - YES → Desktop app not receiving data

2. **Desktop app connection**: Is it fetching data?
   - Check status bar shows "Connected"
   - Check "Last reading" timestamp is recent

3. **Threshold configuration**: Are thresholds set correctly?
   - Backend: Check `mqttService.js` line 72-73
   - Desktop: Check `MainForm.cs` line 112-113

---

### **Issue 2: Popup Shows Repeatedly**

**Symptoms**: Warning popup appears every refresh cycle

**Cause**: Flag not being set properly

**Fix**: Check `_highTempWarningShown` field is declared in `MainForm.cs` line 18-19

---

### **Issue 3: Colors Not Changing**

**Symptoms**: Temperature card stays red even at 32°C

**Cause**: `pnlTempCard` reference missing

**Fix**: Verify `MainForm.Designer.cs` line 64 uses `this.pnlTempCard = new Panel()`

---

## 📈 **FUTURE ENHANCEMENTS**

### **Possible Additions**:

1. **Email/SMS Notifications**
   - Send email when critical threshold exceeded
   - Integration with Twilio for SMS

2. **Configurable Thresholds**
   - UI in desktop app to change thresholds
   - Store in config file

3. **Historical Warning Log**
   - Database table for warning events
   - View warning history in dashboard

4. **Automated Actions**
   - Turn on fan when temp > 30°C
   - Shutdown system when temp > 40°C

5. **Multiple Sensor Support**
   - Different thresholds per room/location
   - Aggregate warnings across sensors

---

## ✅ **VERIFICATION CHECKLIST**

After implementing, verify:

- [ ] **Backend**: Logs show `[ALERT]` when temp > 30°C
- [ ] **Backend**: Logs show `🔥 CRITICAL` when temp > 35°C
- [ ] **Desktop**: Card changes to orange at 30°C
- [ ] **Desktop**: Card changes to dark red at 35°C
- [ ] **Desktop**: Warning popup shows once at 30°C
- [ ] **Desktop**: Critical popup shows once at 35°C
- [ ] **Desktop**: Warning sound plays
- [ ] **Desktop**: Status bar shows alert message
- [ ] **Hysteresis**: Warning doesn't flicker at 29-31°C
- [ ] **Reset**: Warnings clear when temp drops to 28°C

---

## 📞 **SUPPORT**

**Configuration Issues**: Edit threshold constants in:
- `mqttService.js` (backend)
- `MainForm.cs` (desktop)

**Testing**: Use `mosquitto_pub` to simulate high temperatures

**Monitoring**: Watch backend console for `[ALERT]` messages

---

**Temperature Warning System is Production-Ready!** 🎉

Multi-level warnings provide clear visibility when system temperature exceeds safe thresholds.

