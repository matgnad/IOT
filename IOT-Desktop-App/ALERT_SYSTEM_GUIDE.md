# 🚨 Alert System - Complete Implementation Guide

## 📋 Overview

The alert system provides **real-time notifications** (sound + email) when sensor values exceed configured thresholds. The system is designed for **24/7 operation** with proper error handling, cooldown mechanisms, and non-blocking operations.

---

## 🏗️ Architecture

```
ESP8266 (DHT Sensor)
    ↓ MQTT
MQTT Broker
    ↓
Desktop App (WinForms C#)
    ├─→ MqttService (real-time data) OR
    ├─→ SensorApiService (REST API polling - fallback)
    │
    ├─→ AlertService (threshold checking + cooldown)
    │   ├─→ SoundService (Windows system sounds)
    │   └─→ EmailService (Gmail SMTP)
    │
    └─→ MainForm (UI updates)
```

---

## ⚙️ Configuration

### App.config Setup

Edit `App.config` with your settings:

```xml
<appSettings>
  <!-- Alert Thresholds -->
  <add key="TemperatureThreshold" value="35.0" />
  <add key="HumidityThreshold" value="80.0" />
  
  <!-- Email Cooldown (minutes) -->
  <add key="EmailCooldownMinutes" value="15" />
  
  <!-- Gmail Configuration -->
  <add key="GmailUser" value="your_email@gmail.com" />
  <add key="GmailAppPassword" value="your_16_char_app_password" />
  <add key="AlertEmailTo" value="your_email@gmail.com" />
  
  <!-- MQTT Configuration (Optional but Recommended) -->
  <add key="UseMqtt" value="true" />
  <add key="MqttBrokerUrl" value="mqtt://your_broker:1883" />
  <add key="MqttUsername" value="" />
  <add key="MqttPassword" value="" />
  <add key="MqttTopic" value="esp8266/sensors" />
</appSettings>
```

### Gmail Setup

**CRITICAL**: You need a **Gmail App Password**, NOT your regular password!

1. Go to [Google Account Security](https://myaccount.google.com/)
2. Enable **2-Step Verification** (if not already enabled)
3. Go to **App passwords** section
4. Generate a new app password for **Mail**
5. Use the 16-character password in `GmailAppPassword`

---

## 🔄 Alert Workflow

### Step 1: Data Source

**Option A: MQTT (Recommended)**
- Real-time data (instant alerts)
- Lower latency
- More efficient
- Configure `UseMqtt=true` in App.config

**Option B: REST API Polling (Fallback)**
- Polls backend every X seconds
- More reliable if MQTT unavailable
- Configure `UseMqtt=false` in App.config

### Step 2: Alert Detection

When sensor data arrives:

1. **AlertService.CheckAlertsAsync()** is called
2. Checks if temperature >= threshold OR humidity >= threshold
3. Determines alert level (WARNING or CRITICAL)

### Step 3: Alert Actions

For each threshold violation:

1. **Sound Alert** (always, no cooldown):
   - Plays Windows system sound
   - Critical = Error sound (Hand)
   - Warning = Exclamation sound
   - User can mute via button

2. **Email Alert** (with cooldown):
   - Checks cooldown period (default: 15 minutes)
   - If cooldown expired → Send email
   - If cooldown active → Suppress (log message)
   - HTML formatted email with sensor data

3. **UI Updates**:
   - Status bar shows alert message
   - Sensor card highlighted (red/orange)
   - Popup dialog for new alerts
   - Mute button updates

### Step 4: Cooldown Reset

Cooldown resets when:
- Value returns to safe range (with hysteresis)
- Temperature: Must drop 2°C below threshold
- Humidity: Must drop 5% below threshold

---

## 📧 Email Alert Format

### Subject
```
🚨 IoT Alert: Temperature CRITICAL
```

### Content (HTML)
- **Alert Type**: Temperature or Humidity
- **Current Value**: 38.2°C
- **Threshold**: 35.0°C
- **Temperature**: 38.2°C
- **Humidity**: 65.0%
- **Time**: 2024-01-15 10:30:00
- **Warning Level**: CRITICAL

---

## 🎵 Sound Alert

- **Technology**: Windows System Sounds
- **Critical**: `SystemSounds.Hand` (Error sound)
- **Warning**: `SystemSounds.Exclamation` (Warning sound)
- **Mute Control**: Button in UI (top-right)
- **Features**:
  - ✅ No infinite loops
  - ✅ User can mute/unmute
  - ✅ Non-blocking (doesn't freeze UI)

---

## 🛡️ Safety Features

### 1. Non-Blocking Operations
- Email sending is **async** and won't block UI
- Alert checking is **fire-and-forget**
- MQTT processing is **event-driven**

### 2. Cooldown System
- Prevents email spam
- Configurable cooldown period (default: 15 minutes)
- Separate cooldown for temperature and humidity
- Sound alerts are **always played** (no cooldown)

### 3. Error Handling
- Email failures are logged but don't crash app
- Missing Gmail config disables email (logs warning)
- Invalid sensor data is rejected gracefully
- MQTT failures fallback to REST API

### 4. Hysteresis
- Prevents alert flickering
- Temperature: 2°C below threshold to reset
- Humidity: 5% below threshold to reset

---

## 📁 File Structure

```
IOT-Desktop-App/
├── Services/
│   ├── AlertService.cs          # Main alert logic + cooldown
│   ├── EmailService.cs          # Gmail SMTP email sending
│   ├── SoundService.cs          # Windows sound alerts
│   ├── MqttService.cs            # MQTT real-time data (optional)
│   └── SensorApiService.cs      # REST API polling (fallback)
├── Forms/
│   ├── MainForm.cs              # UI + alert integration
│   └── MainForm.Designer.cs      # UI controls
├── App.config                    # Configuration
└── IOT-Dashboard.csproj          # Project file (NuGet packages)
```

---

## 🧪 Testing

### Test Temperature Alert

1. **Configure threshold** in App.config:
   ```xml
   <add key="TemperatureThreshold" value="30.0" />
   ```

2. **Send test data** via MQTT:
   ```bash
   mosquitto_pub -h YOUR_BROKER -t "esp8266/sensors" \
     -m '{"temp":32.0,"humid":60.0}'
   ```

3. **Expected Results**:
   - ✅ Sound plays
   - ✅ Email sent (if configured)
   - ✅ UI updates (status bar, card highlight)
   - ✅ Popup dialog appears

### Test Cooldown

1. Send first alert → Email sent ✅
2. Send second alert within 15 minutes → Email suppressed ⏳
3. Wait 15+ minutes → Email sent again ✅

### Test Mute Button

1. Click "🔊 Sound: ON" button
2. Button changes to "🔇 Sound: OFF"
3. Alerts no longer play sound
4. Click again to unmute

---

## 🔧 Troubleshooting

### Email Not Sending

1. **Check Gmail credentials** in App.config
2. **Verify App Password** (must be 16 characters, not regular password)
3. **Check console logs**:
   ```
   [Email] ⚠️ Gmail credentials not configured
   [Email] ❌ SMTP Error: ...
   ```

### Sound Not Playing

1. **Check mute button** state
2. **Verify Windows sound** is enabled
3. **Check console logs**:
   ```
   [Sound] 🔊 Alert sound played
   [Sound] ❌ Error playing sound: ...
   ```

### Alerts Not Triggering

1. **Verify thresholds** in App.config:
   ```xml
   <add key="TemperatureThreshold" value="35.0" />
   <add key="HumidityThreshold" value="80.0" />
   ```

2. **Check data source**:
   - MQTT: Check connection status in console
   - REST API: Check if backend is running

3. **Check console logs**:
   ```
   [Alert] 🔥 Temperature Alert (CRITICAL): 38.2°C >= 35.0°C
   [Alert] ✅ Email sent successfully
   ```

### MQTT Not Connecting

1. **Check broker URL** in App.config:
   ```xml
   <add key="MqttBrokerUrl" value="mqtt://your_broker:1883" />
   ```

2. **Verify credentials** (if required):
   ```xml
   <add key="MqttUsername" value="username" />
   <add key="MqttPassword" value="password" />
   ```

3. **Check console logs**:
   ```
   [MQTT] ✅ Connected to broker:port
   [MQTT] ❌ Connection failed: ...
   ```

---

## 📊 Monitoring

### Console Logs

Watch for these log patterns:

```
✅ Normal operation:
[MQTT] 📨 Received: {"temp":25.0,"humid":60.0}
[Alert] ✅ Temperature returned to safe range: 24.5°C

✅ Alert triggered:
[Alert] 🔥 Temperature Alert (CRITICAL): 38.2°C >= 35.0°C
[Alert] 🔊 Sound alert played
[Alert] ✅ Email sent successfully

⏳ Cooldown active:
[Alert] ⏳ Email suppressed (cooldown: 5 min remaining)
```

---

## 🚀 Production Checklist

- [ ] Configure `App.config` with real Gmail credentials
- [ ] Set appropriate thresholds for your use case
- [ ] Test email delivery
- [ ] Test sound alerts
- [ ] Verify cooldown period is appropriate
- [ ] Test MQTT connection (or use REST API fallback)
- [ ] Monitor console logs for errors
- [ ] Set up log file rotation (optional)
- [ ] Test 24/7 operation

---

## 📝 Summary

✅ **Real-time alerts** via MQTT (or REST API fallback)  
✅ **Email alerts** with cooldown (prevents spam)  
✅ **Sound alerts** on desktop (mutable)  
✅ **Visual warnings** (status bar + card highlight + popup)  
✅ **Non-blocking** operations (won't freeze UI)  
✅ **Production-ready** error handling  
✅ **Configurable** thresholds and cooldown  
✅ **Hysteresis** prevents alert flickering  

The alert system is **fully integrated** and **ready for 24/7 production use**!

---

## 🎯 Why This Architecture?

### MQTT vs REST API

**MQTT (Recommended)**:
- ✅ Real-time (instant alerts)
- ✅ Lower latency
- ✅ More efficient (push vs pull)
- ✅ Standard for IoT

**REST API (Fallback)**:
- ✅ More reliable if MQTT unavailable
- ✅ Simpler setup
- ✅ Works through firewalls
- ✅ Already implemented

**Decision**: Use MQTT for real-time alerts, fallback to REST API if MQTT fails.

### Desktop App vs Web Dashboard

**Desktop App (This Implementation)**:
- ✅ Runs 24/7 without browser
- ✅ Native Windows integration
- ✅ System sounds
- ✅ Better for monitoring stations

**Web Dashboard**:
- ✅ Accessible from anywhere
- ✅ Multiple users
- ✅ Mobile-friendly

**Decision**: Desktop app is better for dedicated monitoring stations that run continuously.

---

## 📚 Dependencies

### NuGet Packages

- **MQTTnet** (v4.3.3.952) - MQTT client
- **Newtonsoft.Json** (v13.0.3) - JSON parsing
- **System.Configuration.ConfigurationManager** (v9.0.0) - App.config reading

All packages are automatically restored when building the project.

---

## 🔐 Security Notes

1. **Gmail App Password**: Never commit to version control
2. **MQTT Credentials**: Store securely in App.config (not in code)
3. **Email Content**: Contains sensor data - ensure recipient is authorized
4. **Network**: MQTT broker should be on secure network

---

## 📞 Support

For issues or questions:
1. Check console logs for error messages
2. Verify App.config settings
3. Test individual services (email, sound, MQTT)
4. Review this guide for troubleshooting steps

