# 🚨 Alert System - Complete Guide

## 📋 Overview

The upgraded alert system provides **real-time notifications** and **email alerts** when sensor values exceed configured thresholds. The system is designed to be **production-ready** with proper error handling, cooldown mechanisms, and non-blocking operations.

---

## 🏗️ Architecture

```
ESP8266 (DHT Sensor)
    ↓ MQTT
MQTT Broker
    ↓
Backend (Node.js)
    ├─→ Database (MySQL) - Store sensor data
    ├─→ Alert Service - Check thresholds
    │   ├─→ Email Alert (with cooldown)
    │   └─→ Socket.IO Alert (real-time)
    │
Frontend (Dashboard)
    ├─→ Socket.IO Listener
    ├─→ Audio Alert (beep sound)
    └─→ Visual Warning (toast + card highlight)
```

---

## ⚙️ Configuration

### Environment Variables

Copy `.env.example` to `.env` and configure:

```env
# Thresholds
TEMP_THRESHOLD=35.0          # Temperature alert threshold (°C)
HUMIDITY_THRESHOLD=80.0      # Humidity alert threshold (%)

# Email Cooldown (milliseconds)
EMAIL_COOLDOWN_MS=900000     # 15 minutes (prevents spam)

# Gmail Configuration
GMAIL_USER=your_email@gmail.com
GMAIL_APP_PASSWORD=your_16_char_app_password
ALERT_EMAIL_TO=recipient@gmail.com
```

### Gmail Setup

**IMPORTANT**: You need a **Gmail App Password**, not your regular password!

1. Go to [Google Account Security](https://myaccount.google.com/)
2. Enable **2-Step Verification** (if not already enabled)
3. Go to **App passwords** section
4. Generate a new app password for **Mail**
5. Use the 16-character password in `GMAIL_APP_PASSWORD`

---

## 🔄 Alert Workflow

### Step 1: Sensor Data Arrives

When ESP8266 publishes sensor data via MQTT:

```json
{
  "temp": 38.2,
  "humid": 65.0
}
```

### Step 2: Backend Processing

**File**: `src/services/mqttService.js`

1. **Parse & Validate** MQTT message
2. **Save to Database** (non-blocking)
3. **Check Thresholds** via `alertService.js` (non-blocking)
4. **Emit Socket.IO** events for real-time updates

### Step 3: Alert Detection

**File**: `src/services/alertService.js`

For each threshold violation:

1. **Check Cooldown**: Has email been sent recently?
   - If YES → Skip email, log suppression
   - If NO → Proceed to send email

2. **Send Email** (async, non-blocking):
   - HTML email with sensor data
   - Error handling (won't crash backend)

3. **Emit Socket.IO Alert** (always, no cooldown):
   ```javascript
   io.emit('alert', {
     type: 'temperature',
     value: 38.2,
     threshold: 35.0,
     level: 'critical',
     message: 'Temperature 38.2°C exceeded threshold 35.0°C',
     timestamp: '2024-01-15T10:30:00Z'
   });
   ```

### Step 4: Frontend Response

**File**: `public/dashboard.html`

1. **Socket.IO Listener** receives `alert` event
2. **Play Beep Sound** (Web Audio API, 800Hz, 0.3s)
3. **Show Toast Notification**:
   - Red background for critical
   - Orange background for warning
   - Auto-dismiss after 10 seconds
4. **Highlight Sensor Card**:
   - Red border + glow for critical
   - Orange border + glow for warning
   - Auto-remove after 5 seconds

---

## 📧 Email Alert Format

### Subject
```
🚨 IoT Alert: Temperature CRITICAL
```

### Content
- **Alert Type**: Temperature or Humidity
- **Current Value**: 38.2°C
- **Threshold**: 35.0°C
- **Temperature**: 38.2°C
- **Humidity**: 65.0%
- **Time**: 2024-01-15 10:30:00
- **Warning Level**: CRITICAL

---

## 🎵 Sound Alert

- **Technology**: Web Audio API
- **Frequency**: 800 Hz
- **Duration**: 0.3 seconds
- **Volume**: 30% (adjustable)
- **Mute Control**: Button in dashboard (top-right)

**Features**:
- ✅ No infinite loops
- ✅ User can mute/unmute
- ✅ Works across modern browsers
- ✅ Non-blocking (doesn't freeze UI)

---

## 🛡️ Safety Features

### 1. Non-Blocking Operations
- Email sending is **async** and won't block MQTT processing
- Database errors don't stop alert processing
- Alert errors don't crash the backend

### 2. Cooldown System
- Prevents email spam
- Configurable cooldown period (default: 15 minutes)
- Separate cooldown for temperature and humidity
- Socket.IO alerts are **always sent** (no cooldown)

### 3. Error Handling
- Email failures are logged but don't crash backend
- Missing Gmail config disables email (logs warning)
- Invalid sensor data is rejected gracefully

### 4. Logging
All alert events are logged:
```
[ALERT] 🔥 Temperature alert triggered: 38.2°C >= 35.0°C (critical)
[ALERT] ✅ Socket.IO alert emitted: temperature
[ALERT] ✅ Email sent successfully: <message-id>
[ALERT] ⏳ Alert suppressed: temperature alert sent 5 minute(s) ago
```

---

## 📁 File Structure

```
IOT-Website/iot-backend-mvc/
├── src/
│   ├── services/
│   │   ├── alertService.js      # Email alerts + cooldown logic
│   │   └── mqttService.js        # MQTT handler + alert integration
│   └── server.js                 # Socket.IO setup
├── public/
│   └── dashboard.html            # Frontend with alert handling
├── package.json                  # Dependencies (nodemailer added)
└── .env.example                  # Configuration template
```

---

## 🧪 Testing

### Test Temperature Alert

```bash
# Simulate high temperature (38°C)
mosquitto_pub -h YOUR_MQTT_BROKER -t "esp8266/sensors" \
  -m '{"temp":38.0,"humid":60.0}'
```

**Expected Results**:
1. ✅ Backend logs: `[ALERT] 🔥 Temperature alert triggered`
2. ✅ Email sent (if configured)
3. ✅ Frontend plays beep sound
4. ✅ Toast notification appears
5. ✅ Temperature card highlighted

### Test Humidity Alert

```bash
# Simulate high humidity (85%)
mosquitto_pub -h YOUR_MQTT_BROKER -t "esp8266/sensors" \
  -m '{"temp":25.0,"humid":85.0}'
```

### Test Cooldown

1. Send first alert → Email sent ✅
2. Send second alert within 15 minutes → Email suppressed ⏳
3. Wait 15+ minutes → Email sent again ✅

---

## 🔧 Troubleshooting

### Email Not Sending

1. **Check Gmail credentials**:
   ```bash
   # Verify .env file has correct values
   cat .env | grep GMAIL
   ```

2. **Verify App Password**:
   - Must be 16 characters
   - Generated from Google Account → App passwords
   - NOT your regular Gmail password

3. **Check logs**:
   ```
   [ALERT] ⚠️ Gmail credentials not configured
   [ALERT] ❌ Email send failed: ...
   ```

### Sound Not Playing

1. **Check browser console** for errors
2. **Verify Web Audio API** is supported
3. **Check mute button** state
4. **Browser permissions** may block autoplay (user interaction required)

### Alerts Not Triggering

1. **Verify thresholds** in `.env`:
   ```env
   TEMP_THRESHOLD=35.0
   HUMIDITY_THRESHOLD=80.0
   ```

2. **Check MQTT data** format:
   ```json
   {"temp": 38.0, "humid": 60.0}
   ```

3. **Check backend logs** for alert detection

---

## 📊 Monitoring

### Backend Logs

Watch for these log patterns:

```
✅ Normal operation:
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":25.0,"humid":60.0}
[DB] ✅ Saved: ID=123 T=25.0°C H=60.0%
[Socket.IO] ✅ Emitted sensor:update

✅ Alert triggered:
[ALERT] 🔥 Temperature alert triggered: 38.2°C >= 35.0°C (critical)
[ALERT] ✅ Socket.IO alert emitted: temperature
[ALERT] ✅ Email sent successfully: <message-id>

⏳ Cooldown active:
[ALERT] ⏳ Alert suppressed: temperature alert sent 5 minute(s) ago
```

### Frontend Console

```javascript
// Socket.IO connection
[Socket.IO] Connected to server

// Alert received
[ALERT] Received alert: {type: "temperature", value: 38.2, ...}
```

---

## 🚀 Production Checklist

- [ ] Configure `.env` with real Gmail credentials
- [ ] Set appropriate thresholds for your use case
- [ ] Test email delivery
- [ ] Test sound alerts in target browsers
- [ ] Verify cooldown period is appropriate
- [ ] Monitor backend logs for errors
- [ ] Set up log rotation/management
- [ ] Consider adding alert history/analytics

---

## 📝 Summary

✅ **Real-time alerts** via Socket.IO (no cooldown)  
✅ **Email alerts** with cooldown (prevents spam)  
✅ **Sound alerts** on frontend (mutable)  
✅ **Visual warnings** (toast + card highlight)  
✅ **Non-blocking** operations (won't crash backend)  
✅ **Production-ready** error handling  
✅ **Configurable** thresholds and cooldown  

The alert system is **fully integrated** and **ready for production use**!

