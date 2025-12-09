import mqtt from 'mqtt';
import SensorData from '../models/SensorData.js';
import dotenv from 'dotenv';

dotenv.config();

const client = mqtt.connect(process.env.MQTT_URL);

client.on('connect', () => {
  console.log('[MQTT] Connected');
  client.subscribe('iot/sensor');
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

    await SensorData.create({
      temperature,
      humidity
    });

    console.log(`[DB] Saved: T=${temperature} | H=${humidity}`);

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
