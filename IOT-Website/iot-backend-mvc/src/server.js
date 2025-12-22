import 'dotenv/config';
import http from 'http';
import { Server } from 'socket.io';
import app from './app.js';
import { syncModels, Device } from './models/index.js';
import mqttService, { setSocketIO } from './services/mqttService.js';

const server = http.createServer(app);
const io = new Server(server, {
  cors: {
    origin: "*",
    methods: ["GET", "POST"]
  }
});

// Inject Socket.IO into mqttService
setSocketIO(io);

io.on('connection', (socket) => {
  console.log('[Socket.IO] Client connected:', socket.id);
  
  socket.on('disconnect', () => {
    console.log('[Socket.IO] Client disconnected:', socket.id);
  });
});

(async () => {
  await syncModels();

  const defaults = [{ name: 'LIGHT BULB' }, { name: 'FAN' }, { name: 'AIR CONDITIONER' }];
  for (const d of defaults) {
    const found = await Device.findOne({ where: { name: d.name } });
    if (!found) await Device.create(d);
  }

  const port = process.env.PORT || 3000;
  server.listen(port, () =>
    console.log(`Server running http://localhost:${port}`)
  );
})();
