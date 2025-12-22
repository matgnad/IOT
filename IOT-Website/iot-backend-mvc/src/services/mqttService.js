import mqtt from 'mqtt';
import SensorData from '../models/SensorData.js';
import dotenv from 'dotenv';

dotenv.config();

// Socket.IO will be injected after server initialization
let io = null;

export function setSocketIO(socketIO) {
  io = socketIO;
}

const client = mqtt.connect(process.env.MQTT_URL);

client.on('connect', () => {
  console.log('[MQTT] Connected');
  client.subscribe('esp8266/sensors');
  console.log('[MQTT] Subscribed to: esp8266/sensors');
});

client.on('message', async (topic, message) => {
  try {
    const data = JSON.parse(message.toString());

    const temperature = data.temp;
    const humidity = data.humid;

    if (temperature == null || humidity == null) {
      console.log('[MQTT] Invalid data =>', message.toString());
      return;
    }

    const sensorRecord = await SensorData.create({
      temperature,
      humidity
    });

    console.log(`[DB] Saved: T=${temperature} | H=${humidity}`);

    // Emit Socket.IO event for real-time updates
    if (io) {
      io.emit('sensor:update', {
        temperature,
        humidity,
        measured_at: sensorRecord.measured_at
      });
      console.log(`[Socket.IO] Emitted sensor:update`);
    }

  } catch (e) {
    console.error('[MQTT] JSON error:', e.message);
  }
});

// 👉 THÊM HÀM NÀY NẾU WEB CẦN BẬT TẮT THIẾT BỊ
export function publishDeviceCommand(device, action) {
  const payload = JSON.stringify({ device, action });
  client.publish('iot/device/control', payload);
  console.log("[MQTT] Sent device command:", payload);
}

export default client;
