import mqtt from 'mqtt';
import SensorData from '../models/SensorData.js';
import dotenv from 'dotenv';
import { checkAndTriggerAlerts } from './alertService.js';

dotenv.config();

// Socket.IO will be injected after server initialization
let io = null;

export function setSocketIO(socketIO) {
  io = socketIO;
}

const client = mqtt.connect(process.env.MQTT_URL);

// ✅ FIX: Add connection event handler
client.on('connect', () => {
  console.log('[MQTT] ✅ Connected to broker:', process.env.MQTT_URL);
  client.subscribe('esp8266/sensors', (err) => {
    if (err) {
      console.error('[MQTT] ❌ Subscribe failed:', err);
    } else {
      console.log('[MQTT] ✅ Subscribed to: esp8266/sensors');
    }
  });
});

// ✅ FIX: Add error handler
client.on('error', (err) => {
  console.error('[MQTT] ❌ Connection error:', err.message);
});

// ✅ FIX: Add offline handler
client.on('offline', () => {
  console.warn('[MQTT] ⚠️ Client went offline');
});

// ✅ FIX: Add reconnect handler
client.on('reconnect', () => {
  console.log('[MQTT] 🔄 Attempting to reconnect...');
});

// ✅ FIX: Add close handler
client.on('close', () => {
  console.warn('[MQTT] ⚠️ Connection closed');
});

// ✅ FIX: Enhanced message handler with better error handling
client.on('message', async (topic, message) => {
  const rawMessage = message.toString();
  console.log(`[MQTT] 📨 Received on [${topic}]: ${rawMessage}`);

  try {
    // Parse JSON
    const data = JSON.parse(rawMessage);

    const temperature = data.temp;
    const humidity = data.humid;

    // Validate required fields
    if (temperature == null || humidity == null) {
      console.warn('[MQTT] ⚠️ Invalid data (missing temp or humid):', rawMessage);
      return;
    }

    // ✅ FIX: Add type validation
    if (typeof temperature !== 'number' || typeof humidity !== 'number') {
      console.warn('[MQTT] ⚠️ Invalid data types:', { temperature, humidity });
      return;
    }

    // Insert into database (non-blocking)
    let sensorRecord = null;
    try {
      sensorRecord = await SensorData.create({
        temperature,
        humidity
      });

      console.log(`[DB] ✅ Saved: ID=${sensorRecord.id} T=${temperature}°C H=${humidity}%`);
    } catch (dbErr) {
      // ✅ FIX: Separate database errors
      console.error('[DB] ❌ Database error:', dbErr.message);
      console.error('[DB] Stack:', dbErr.stack);
      // Continue processing even if DB fails (for alerts)
    }

    // 🚨 ALERT SYSTEM: Check thresholds and trigger alerts (non-blocking)
    // This runs asynchronously and won't block MQTT processing
    checkAndTriggerAlerts(
      { temperature, humidity },
      io
    ).catch((alertErr) => {
      // Do NOT crash backend if alert system fails
      console.error('[ALERT] ❌ Alert processing error:', alertErr.message);
    });

    // Emit Socket.IO event for real-time updates (always emit, even if DB failed)
    if (io) {
      io.emit('sensor:update', {
        temperature,
        humidity,
        measured_at: sensorRecord?.measured_at || new Date().toISOString(),
      });
      console.log(`[Socket.IO] ✅ Emitted sensor:update`);
    }

  } catch (jsonErr) {
    // ✅ FIX: Better JSON error logging
    console.error('[MQTT] ❌ JSON parse error:', jsonErr.message);
    console.error('[MQTT] Raw message:', rawMessage);
  }
});

// 👉 THÊM HÀM NÀY NẾU WEB CẦN BẬT TẮT THIẾT BỊ
export function publishDeviceCommand(device, action) {
  const payload = JSON.stringify({ device, action });
  client.publish('iot/device/control', payload);
  console.log("[MQTT] Sent device command:", payload);
}

export default client;
