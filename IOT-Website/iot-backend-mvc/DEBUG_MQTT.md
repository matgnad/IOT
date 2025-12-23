# 🔍 MQTT System Debugging Guide

## 🚨 **ROOT CAUSE FOUND**

Your sensor data stopped updating due to **THREE CRITICAL BUGS**:

### **Bug #1: ESP8266 - Light Sensor Failure Blocks All Data** ❌
**Problem**: If BH1750 light sensor is disconnected, ESP8266 stops publishing temperature/humidity even though DHT11 works fine.

**Fix**: Changed to only require temp/humid, light is optional.

---

### **Bug #2: Backend - Missing MQTT Error Handlers** ❌
**Problem**: No error/offline/reconnect handlers = silent failures

**Fix**: Added comprehensive error handlers with emoji logging.

---

### **Bug #3: Backend - Database Errors Logged as "JSON Error"** ❌
**Problem**: Database failures were misleadingly logged as JSON errors

**Fix**: Separated JSON vs Database error logging.

---

## ✅ **FIXES APPLIED**

### **1. ESP8266 Code (`IOT_FINAL.ino`)**

**Changes**:
- ✅ Only require temp/humid (DHT11) - light (BH1750) is optional
- ✅ Enhanced logging with emojis for visibility
- ✅ Better error messages showing MQTT state
- ✅ Don't stop publishing if only light sensor fails

---

### **2. Backend Code (`mqttService.js`)**

**Changes**:
- ✅ Added `on('error')` handler
- ✅ Added `on('offline')` handler
- ✅ Added `on('reconnect')` handler
- ✅ Added `on('close')` handler
- ✅ Enhanced message logging with topic + payload
- ✅ Separated JSON errors from database errors
- ✅ Added type validation for temp/humid
- ✅ Better console logging with emojis

---

## 🧪 **DEBUGGING STEPS - Execute in Order**

### **LAYER 1️⃣: MQTT BROKER**

#### **Test 1: Check if Mosquitto is Running**

**Windows**:
```powershell
# Check if mosquitto service is running
Get-Service mosquitto

# Or check process
Get-Process mosquitto -ErrorAction SilentlyContinue
```

**Linux/Mac**:
```bash
sudo systemctl status mosquitto
# or
ps aux | grep mosquitto
```

**Expected**: Service is running

---

#### **Test 2: Subscribe to MQTT Topic**

Open a **NEW terminal** and run:

```bash
mosquitto_sub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -v
```

**Expected Output** (every 2 seconds):
```
esp8266/sensors {"temp":25.5,"humid":60.2,"light":123.45}
esp8266/sensors {"temp":25.6,"humid":60.1,"light":124.00}
```

**If you see messages**: ✅ ESP8266 is publishing correctly → Problem is in backend

**If NO messages**: ❌ ESP8266 is not publishing → Check ESP8266 layer

---

### **LAYER 2️⃣: ESP8266 DEVICE**

#### **Test 3: Check Serial Monitor**

1. **Connect ESP8266 to computer via USB**
2. **Open Arduino IDE Serial Monitor** (115200 baud)
3. **Reset ESP8266** (press RST button)

**Expected Output**:
```
Đang kết nối WiFi: Thanhhai
....
WiFi đã kết nối!
IP: 172.20.10.X

Đang kết nối MQTT (auth)...Đã kết nối!
PUB ACK -> esp8266/devices/ack : {"id":1,"status":"OFF","actionBy":"System"} [OK]
PUB ACK -> esp8266/devices/ack : {"id":2,"status":"OFF","actionBy":"System"} [OK]
PUB ACK -> esp8266/devices/ack : {"id":3,"status":"OFF","actionBy":"System"} [OK]

📡 Publishing to esp8266/sensors: {"temp":25.5,"humid":60.2,"light":123.45} ✅ [SUCCESS]
📡 Publishing to esp8266/sensors: {"temp":25.6,"humid":60.1,"light":124.00} ✅ [SUCCESS]
```

**Common Issues**:

| Serial Output | Problem | Solution |
|---------------|---------|----------|
| `Sensor read error` repeating | All sensors failing | Check DHT11 wiring |
| `❌ [FAILED]` | MQTT publish failed | Check MQTT connection |
| `MQTT publish failed! Client state: -2` | MQTT disconnected | Check broker IP/credentials |
| `⚠️ BH1750 light sensor error` | Light sensor disconnected | **OK - now publishes anyway** |

---

#### **Test 4: Upload Fixed ESP8266 Code**

1. **Open Arduino IDE**
2. **Open**: `C:\UP\iot\IOT-Website\iot-backend-mvc\public\IOT_FINAL\IOT_FINAL.ino`
3. **Verify fixes are present**:
   - Line ~179: Should check `if (isnan(h) || isnan(t))` (NOT lux)
   - Line ~197: Should have emoji logging
4. **Upload** to ESP8266
5. **Open Serial Monitor** to verify new logs

---

### **LAYER 3️⃣: BACKEND**

#### **Test 5: Check Backend Logs**

1. **Stop backend** (Ctrl+C if running)
2. **Start with fresh logs**:
   ```bash
   cd C:\UP\iot\IOT-Website\iot-backend-mvc
   npm start
   ```

**Expected Output**:
```
Server running http://localhost:3000
[MQTT] ✅ Connected to broker: mqtt://172.20.10.2:1883
[MQTT] ✅ Subscribed to: esp8266/sensors

[MQTT] 📨 Received on [esp8266/sensors]: {"temp":25.5,"humid":60.2,"light":123.45}
[DB] ✅ Saved: ID=123 T=25.5°C H=60.2%
[Socket.IO] ✅ Emitted sensor:update

[MQTT] 📨 Received on [esp8266/sensors]: {"temp":25.6,"humid":60.1}
[DB] ✅ Saved: ID=124 T=25.6°C H=60.1%
```

**Common Issues**:

| Log Message | Problem | Solution |
|-------------|---------|----------|
| `[MQTT] ❌ Connection error` | Can't connect to broker | Check `MQTT_URL` in `.env` |
| `[MQTT] ⚠️ Client went offline` | Connection lost | Check network/broker |
| `[MQTT] ⚠️ Invalid data` | Wrong JSON format | Check ESP8266 payload |
| `[DB] ❌ Database error` | Can't insert to DB | Check database connection |
| No `📨 Received` messages | Not receiving MQTT | Check ESP8266 is publishing |

---

#### **Test 6: Check .env Configuration**

```bash
cd C:\UP\iot\IOT-Website\iot-backend-mvc
type .env
```

**Required**:
```env
MQTT_URL=mqtt://ThanhHai:thanhhai2004@172.20.10.2:1883

DB_HOST=localhost
DB_PORT=3306
DB_NAME=iot_db
DB_USER=root
DB_PASS=your_password
```

**Verify**:
- ✅ MQTT_URL matches ESP8266's mqtt_server (172.20.10.2)
- ✅ MQTT_URL includes credentials
- ✅ Database credentials are correct

---

### **LAYER 4️⃣: DATABASE**

#### **Test 7: Verify Database Connection**

```bash
mysql -u root -p
```

```sql
USE iot_db;

-- Check table exists
SHOW TABLES;

-- Check schema
DESCRIBE sensors;

-- Check latest data
SELECT * FROM sensors ORDER BY id DESC LIMIT 10;

-- Check if new data is being inserted (run twice, 5 seconds apart)
SELECT COUNT(*), MAX(measured_at) FROM sensors;
```

**Expected**:
- `sensors` table exists
- Columns: `id`, `temperature`, `humidity`, `measured_at`
- COUNT increases every few seconds
- `measured_at` is recent (not old timestamps)

---

#### **Test 8: Manual Insert Test**

```sql
-- Try manual insert
INSERT INTO sensors (temperature, humidity) VALUES (99.9, 88.8);

-- Verify
SELECT * FROM sensors ORDER BY id DESC LIMIT 1;
```

**If manual insert works**: ✅ Database is fine → Problem is MQTT/Backend

**If manual insert fails**: ❌ Database connection issue → Fix database

---

## 🔧 **CONFIGURATION CHECKLIST**

### **ESP8266 Configuration**

| Setting | Current Value | Verify |
|---------|---------------|--------|
| WiFi SSID | `Thanhhai` | Match your WiFi |
| WiFi Password | `Thanhhai2004@` | Match your WiFi |
| MQTT Server | `172.20.10.2` | Your computer's IP |
| MQTT User | `ThanhHai` | Match broker config |
| MQTT Pass | `thanhhai2004` | Match broker config |
| Publish Interval | `2000ms` (2s) | ✅ OK |
| Topic | `esp8266/sensors` | ✅ Match backend |

---

### **Backend Configuration (.env)**

| Setting | Required Format | Example |
|---------|----------------|---------|
| MQTT_URL | `mqtt://user:pass@ip:port` | `mqtt://ThanhHai:thanhhai2004@172.20.10.2:1883` |
| DB_HOST | Database host | `localhost` |
| DB_NAME | Database name | `iot_db` |

---

## 🎯 **VERIFICATION CHECKLIST**

After fixes, verify:

- [ ] **ESP8266**: Serial monitor shows `✅ [SUCCESS]` every 2 seconds
- [ ] **ESP8266**: WiFi connected (shows IP address)
- [ ] **ESP8266**: MQTT connected (no repeated reconnect attempts)
- [ ] **MQTT Broker**: `mosquitto_sub` shows messages
- [ ] **Backend**: Log shows `📨 Received` messages
- [ ] **Backend**: Log shows `✅ Saved` messages
- [ ] **Database**: `SELECT COUNT(*)` increases over time
- [ ] **Database**: `measured_at` has recent timestamps
- [ ] **Dashboard**: Desktop app shows live data
- [ ] **Dashboard**: Statistics update
- [ ] **Dashboard**: Chart displays trends

---

## 🚨 **COMMON FAILURE SCENARIOS**

### **Scenario 1: ESP8266 Connected but No Publish**

**Symptoms**: Serial shows MQTT connected, but no publish messages

**Causes**:
- Sensors not working (DHT11 disconnected)
- `isnan()` check failing
- MQTT buffer full

**Fix**:
1. Check DHT11 wiring
2. Verify sensor readings in serial monitor
3. With new code, light sensor failure won't block publishes

---

### **Scenario 2: Backend Connected but No Messages**

**Symptoms**: Backend says `✅ Connected` but no `📨 Received`

**Causes**:
- Wrong topic subscription
- MQTT broker not receiving from ESP8266
- Firewall blocking

**Fix**:
1. Use `mosquitto_sub` to verify ESP8266 publishes
2. Check topic name matches exactly: `esp8266/sensors`
3. Disable firewall temporarily for testing

---

### **Scenario 3: Messages Received but Not Saved**

**Symptoms**: Backend shows `📨 Received` but no `✅ Saved`

**Causes**:
- Database connection failed
- Invalid JSON format
- Missing temp/humid fields

**Fix**:
1. Check backend logs for `❌ Database error`
2. Verify `.env` database credentials
3. Check JSON payload has `temp` and `humid`

---

## 🔬 **ADVANCED DEBUGGING**

### **Enable MQTT Debug Logging**

Add to `mqttService.js` after client creation:

```javascript
client.on('packetreceive', (packet) => {
  console.log('[MQTT DEBUG] Packet received:', packet.cmd);
});

client.on('packetsend', (packet) => {
  console.log('[MQTT DEBUG] Packet sent:', packet.cmd);
});
```

---

### **Test MQTT Manually**

**Publish test message**:
```bash
mosquitto_pub -h 172.20.10.2 -t "esp8266/sensors" -u ThanhHai -P thanhhai2004 -m '{"temp":99.9,"humid":88.8}'
```

Backend should immediately log:
```
[MQTT] 📨 Received on [esp8266/sensors]: {"temp":99.9,"humid":88.8}
[DB] ✅ Saved: ID=XXX T=99.9°C H=88.8%
```

---

### **Check Network Connectivity**

**From your PC**:
```bash
# Can you reach MQTT broker?
ping 172.20.10.2

# Can you connect to MQTT port?
telnet 172.20.10.2 1883
```

---

## 📊 **MONITORING DASHBOARD**

Create a monitoring script `monitor.sql`:

```sql
-- Real-time monitoring query (run repeatedly)
SELECT 
    COUNT(*) as total_records,
    MAX(measured_at) as last_update,
    TIMESTAMPDIFF(SECOND, MAX(measured_at), NOW()) as seconds_ago,
    AVG(temperature) as avg_temp,
    AVG(humidity) as avg_humid
FROM sensors
WHERE measured_at > DATE_SUB(NOW(), INTERVAL 1 HOUR);
```

**Expected**: `seconds_ago` should be < 5 when system is healthy

---

## ✅ **SUCCESS CRITERIA**

System is **HEALTHY** when:

1. **ESP8266 Serial**: Shows publish success every 2 seconds
2. **MQTT Broker**: `mosquitto_sub` receives messages
3. **Backend Logs**: Shows received + saved messages
4. **Database**: New rows appear every 2 seconds
5. **Desktop App**: Live data updates
6. **No Error Logs**: No ❌ or ⚠️ in backend console

---

## 📞 **TROUBLESHOOTING MATRIX**

| Symptom | Layer | Check | Fix |
|---------|-------|-------|-----|
| No Serial output | ESP8266 | USB connection | Check cable, reopen serial monitor |
| WiFi not connecting | ESP8266 | SSID/password | Update credentials, check WiFi range |
| MQTT connect fails | ESP8266 | Broker IP/port | Verify broker is running, check IP |
| Sensor read error | ESP8266 | DHT11 wiring | Check connections (VCC, GND, DATA) |
| Publish fails | ESP8266 | MQTT state | Check serial for client.state() code |
| mosquitto_sub empty | Broker | ESP8266 publish | Use `-d` flag for debug |
| Backend no receive | Backend | Topic mismatch | Verify "esp8266/sensors" exactly |
| Database error | Backend | DB credentials | Check .env, test MySQL connection |
| Old timestamps | All | System stopped | Restart ESP8266 + Backend |

---

**Your system should now be fully operational!** 🎉

If issues persist, check logs at each layer and compare with expected output above.

