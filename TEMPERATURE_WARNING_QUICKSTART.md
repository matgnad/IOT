# 🌡️ Temperature Warning - Quick Reference

## ✅ **FEATURE STATUS**

The temperature warning system is **FULLY IMPLEMENTED** and **PRODUCTION-READY**.

---

## 📍 **WHERE IT'S LOCATED**

### **Backend (Node.js)**
```
File: IOT-Website/iot-backend-mvc/src/services/mqttService.js
Lines: 72-108
```

### **Desktop App (WinForms)**
```
File: IOT-Desktop-App/Forms/MainForm.cs
Lines: 111-176
```

---

## 🎯 **HOW IT WORKS**

### **Two Threshold Levels**:

| Temperature | Level | Backend | Desktop App |
|-------------|-------|---------|-------------|
| < 30°C | ✅ **NORMAL** | No alerts | Red card (normal) |
| 30-34.9°C | ⚠️ **WARNING** | Console warning | Orange card + popup |
| ≥ 35°C | 🔥 **CRITICAL** | Console error | Dark red + critical popup |

---

## 🔧 **CHANGE THRESHOLDS**

### **Backend** (`mqttService.js`):
```javascript
const TEMP_WARNING = 30.0;   // ← Change this
const TEMP_CRITICAL = 35.0;  // ← Change this
```

### **Desktop App** (`MainForm.cs`):
```csharp
const float WARNING_TEMP = 30.0f;   // ← Change this
const float CRITICAL_TEMP = 35.0f;  // ← Change this
```

**⚠️ Keep both synchronized!**

---

## 🧪 **TEST IT**

### **Quick Test**:
```bash
# Simulate 32°C (warning)
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":32.0,"humid":60.0}'

# Simulate 36°C (critical)
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":36.0,"humid":55.0}'
```

---

## 📊 **WHAT YOU'LL SEE**

### **At 32°C (Warning)**:

**Backend Console**:
```
[ALERT] ⚠️ WARNING: Temperature 32.0°C exceeds 30.0°C
```

**Desktop App**:
- Orange temperature card
- "⚠️ WARNING" label visible
- Popup: "High temperature detected!"
- Sound: Windows exclamation

---

### **At 36°C (Critical)**:

**Backend Console**:
```
[ALERT] 🔥 CRITICAL: Temperature 36.0°C exceeds 35.0°C!
```

**Desktop App**:
- Dark red temperature card
- "🔥 CRITICAL" label visible
- Popup: "CRITICAL TEMPERATURE ALERT!"
- Sound: Windows error (more urgent)

---

## ✅ **FEATURES**

- ✅ Multi-level warnings (warning + critical)
- ✅ Visual color-coded alerts
- ✅ One-time popups (no spam)
- ✅ Audio notifications
- ✅ Hysteresis (2°C) to prevent flickering
- ✅ Status bar messages
- ✅ Backend logging for monitoring
- ✅ Socket.IO real-time updates

---

## 📚 **FULL DOCUMENTATION**

See `TEMPERATURE_WARNING_SYSTEM.md` for complete details:
- Architecture diagrams
- Code explanations
- Testing procedures
- Troubleshooting guide
- Database queries
- Future enhancements

---

**Temperature Warning System is Ready to Use!** 🎉

