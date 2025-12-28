import nodemailer from 'nodemailer';
import dotenv from 'dotenv';

dotenv.config();

// =============================
// Alert Service
// Handles email alerts with cooldown to prevent spam
// =============================

// Configuration from environment variables
const ALERT_CONFIG = {
  // Thresholds (configurable via env)
  TEMP_THRESHOLD: parseFloat(process.env.TEMP_THRESHOLD) || 35.0,
  HUMIDITY_THRESHOLD: parseFloat(process.env.HUMIDITY_THRESHOLD) || 80.0,
  
  // Email configuration
  GMAIL_USER: process.env.GMAIL_USER,
  GMAIL_APP_PASSWORD: process.env.GMAIL_APP_PASSWORD,
  ALERT_EMAIL_TO: process.env.ALERT_EMAIL_TO || process.env.GMAIL_USER,
  
  // Cooldown period in milliseconds (default: 15 minutes)
  EMAIL_COOLDOWN_MS: parseInt(process.env.EMAIL_COOLDOWN_MS) || 15 * 60 * 1000,
};

// Cooldown tracking: { alertType: lastSentTimestamp }
const cooldownTracker = {
  temperature: null,
  humidity: null,
};

// Email transporter (initialized lazily)
let transporter = null;

/**
 * Initialize email transporter
 * @returns {Promise<Object>} Nodemailer transporter
 */
function getTransporter() {
  if (transporter) {
    return transporter;
  }

  // Validate Gmail configuration
  if (!ALERT_CONFIG.GMAIL_USER || !ALERT_CONFIG.GMAIL_APP_PASSWORD) {
    console.warn('[ALERT] ⚠️ Gmail credentials not configured. Email alerts disabled.');
    return null;
  }

  transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
      user: ALERT_CONFIG.GMAIL_USER,
      pass: ALERT_CONFIG.GMAIL_APP_PASSWORD,
    },
  });

  return transporter;
}

/**
 * Check if alert should be sent (cooldown logic)
 * @param {string} alertType - 'temperature' or 'humidity'
 * @returns {boolean} True if alert should be sent
 */
function shouldSendAlert(alertType) {
  const lastSent = cooldownTracker[alertType];
  
  if (lastSent === null) {
    // First time alert
    return true;
  }

  const timeSinceLastAlert = Date.now() - lastSent;
  
  if (timeSinceLastAlert >= ALERT_CONFIG.EMAIL_COOLDOWN_MS) {
    // Cooldown expired
    return true;
  }

  // Still in cooldown period
  const remainingMinutes = Math.ceil((ALERT_CONFIG.EMAIL_COOLDOWN_MS - timeSinceLastAlert) / 60000);
  console.log(`[ALERT] ⏳ Alert suppressed: ${alertType} alert sent ${remainingMinutes} minute(s) ago (cooldown: ${ALERT_CONFIG.EMAIL_COOLDOWN_MS / 60000} min)`);
  return false;
}

/**
 * Update cooldown tracker
 * @param {string} alertType - 'temperature' or 'humidity'
 */
function updateCooldown(alertType) {
  cooldownTracker[alertType] = Date.now();
}

/**
 * Send email alert
 * @param {Object} alertData - Alert information
 * @param {string} alertData.type - 'temperature' or 'humidity'
 * @param {number} alertData.value - Current sensor value
 * @param {number} alertData.threshold - Threshold that was exceeded
 * @param {number} alertData.temperature - Current temperature
 * @param {number} alertData.humidity - Current humidity
 * @param {string} alertData.level - 'warning' or 'critical'
 * @returns {Promise<boolean>} True if email was sent successfully
 */
async function sendEmailAlert(alertData) {
  const emailTransporter = getTransporter();
  
  if (!emailTransporter) {
    return false;
  }

  const { type, value, threshold, temperature, humidity, level } = alertData;
  
  const subject = `🚨 IoT Alert: ${type === 'temperature' ? 'Temperature' : 'Humidity'} ${level === 'critical' ? 'CRITICAL' : 'Warning'}`;
  
  const htmlContent = `
    <!DOCTYPE html>
    <html>
    <head>
      <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .alert-box { 
          background-color: ${level === 'critical' ? '#ff4444' : '#ffaa00'}; 
          color: white; 
          padding: 20px; 
          border-radius: 8px; 
          margin: 20px 0;
        }
        .data-table { 
          background-color: #f5f5f5; 
          padding: 15px; 
          border-radius: 5px; 
          margin: 15px 0;
        }
        .data-row { 
          display: flex; 
          justify-content: space-between; 
          padding: 8px 0; 
          border-bottom: 1px solid #ddd;
        }
        .data-label { font-weight: bold; }
        .footer { 
          margin-top: 30px; 
          padding-top: 20px; 
          border-top: 1px solid #ddd; 
          font-size: 12px; 
          color: #666;
        }
      </style>
    </head>
    <body>
      <div class="container">
        <div class="alert-box">
          <h2>${level === 'critical' ? '🔥 CRITICAL ALERT' : '⚠️ WARNING ALERT'}</h2>
          <p><strong>${type === 'temperature' ? 'Temperature' : 'Humidity'} exceeded safe threshold!</strong></p>
        </div>
        
        <div class="data-table">
          <div class="data-row">
            <span class="data-label">Alert Type:</span>
            <span>${type === 'temperature' ? 'Temperature' : 'Humidity'}</span>
          </div>
          <div class="data-row">
            <span class="data-label">Current Value:</span>
            <span><strong>${value}${type === 'temperature' ? '°C' : '%'}</strong></span>
          </div>
          <div class="data-row">
            <span class="data-label">Threshold:</span>
            <span>${threshold}${type === 'temperature' ? '°C' : '%'}</span>
          </div>
          <div class="data-row">
            <span class="data-label">Temperature:</span>
            <span>${temperature}°C</span>
          </div>
          <div class="data-row">
            <span class="data-label">Humidity:</span>
            <span>${humidity}%</span>
          </div>
          <div class="data-row">
            <span class="data-label">Time:</span>
            <span>${new Date().toLocaleString()}</span>
          </div>
          <div class="data-row">
            <span class="data-label">Warning Level:</span>
            <span><strong>${level.toUpperCase()}</strong></span>
          </div>
        </div>
        
        <div class="footer">
          <p>This is an automated alert from your IoT monitoring system.</p>
          <p>Please check your sensor and take appropriate action.</p>
        </div>
      </div>
    </body>
    </html>
  `;

  const textContent = `
🚨 IoT Alert: ${type === 'temperature' ? 'Temperature' : 'Humidity'} ${level === 'critical' ? 'CRITICAL' : 'Warning'}

${level === 'critical' ? '🔥 CRITICAL ALERT' : '⚠️ WARNING ALERT'}
${type === 'temperature' ? 'Temperature' : 'Humidity'} exceeded safe threshold!

Alert Type: ${type === 'temperature' ? 'Temperature' : 'Humidity'}
Current Value: ${value}${type === 'temperature' ? '°C' : '%'}
Threshold: ${threshold}${type === 'temperature' ? '°C' : '%'}
Temperature: ${temperature}°C
Humidity: ${humidity}%
Time: ${new Date().toLocaleString()}
Warning Level: ${level.toUpperCase()}

This is an automated alert from your IoT monitoring system.
Please check your sensor and take appropriate action.
  `;

  try {
    const info = await emailTransporter.sendMail({
      from: `"IoT Monitoring System" <${ALERT_CONFIG.GMAIL_USER}>`,
      to: ALERT_CONFIG.ALERT_EMAIL_TO,
      subject: subject,
      text: textContent,
      html: htmlContent,
    });

    console.log(`[ALERT] ✅ Email sent successfully: ${info.messageId}`);
    return true;
  } catch (error) {
    // Do NOT crash the backend if email fails
    console.error('[ALERT] ❌ Email send failed:', error.message);
    console.error('[ALERT] Error details:', {
      code: error.code,
      command: error.command,
    });
    return false;
  }
}

/**
 * Check thresholds and trigger alerts if needed
 * @param {Object} sensorData - Sensor data
 * @param {number} sensorData.temperature - Temperature value
 * @param {number} sensorData.humidity - Humidity value
 * @param {Object} socketIO - Socket.IO instance for real-time notifications
 * @returns {Promise<Array>} Array of triggered alerts
 */
export async function checkAndTriggerAlerts(sensorData, socketIO) {
  const { temperature, humidity } = sensorData;
  const triggeredAlerts = [];

  // Check temperature threshold
  if (temperature >= ALERT_CONFIG.TEMP_THRESHOLD) {
    const level = temperature >= ALERT_CONFIG.TEMP_THRESHOLD ? 'critical' : 'warning';
    
    console.log(`[ALERT] 🔥 Temperature alert triggered: ${temperature}°C >= ${ALERT_CONFIG.TEMP_THRESHOLD}°C (${level})`);

    // Emit Socket.IO alert event (real-time, no cooldown)
    if (socketIO) {
      socketIO.emit('alert', {
        type: 'temperature',
        value: temperature,
        threshold: ALERT_CONFIG.TEMP_THRESHOLD,
        level: level,
        message: `Temperature ${temperature}°C exceeded threshold ${ALERT_CONFIG.TEMP_THRESHOLD}°C`,
        timestamp: new Date().toISOString(),
      });
      console.log('[ALERT] ✅ Socket.IO alert emitted: temperature');
    }

    // Send email (with cooldown)
    if (shouldSendAlert('temperature')) {
      const emailSent = await sendEmailAlert({
        type: 'temperature',
        value: temperature,
        threshold: ALERT_CONFIG.TEMP_THRESHOLD,
        temperature: temperature,
        humidity: humidity,
        level: level,
      });

      if (emailSent) {
        updateCooldown('temperature');
        triggeredAlerts.push({ type: 'temperature', level, emailSent: true });
      } else {
        triggeredAlerts.push({ type: 'temperature', level, emailSent: false });
      }
    } else {
      triggeredAlerts.push({ type: 'temperature', level, emailSent: false, suppressed: true });
    }
  }

  // Check humidity threshold
  if (humidity >= ALERT_CONFIG.HUMIDITY_THRESHOLD) {
    const level = 'warning'; // Humidity alerts are typically warnings
    
    console.log(`[ALERT] 💧 Humidity alert triggered: ${humidity}% >= ${ALERT_CONFIG.HUMIDITY_THRESHOLD}% (${level})`);

    // Emit Socket.IO alert event (real-time, no cooldown)
    if (socketIO) {
      socketIO.emit('alert', {
        type: 'humidity',
        value: humidity,
        threshold: ALERT_CONFIG.HUMIDITY_THRESHOLD,
        level: level,
        message: `Humidity ${humidity}% exceeded threshold ${ALERT_CONFIG.HUMIDITY_THRESHOLD}%`,
        timestamp: new Date().toISOString(),
      });
      console.log('[ALERT] ✅ Socket.IO alert emitted: humidity');
    }

    // Send email (with cooldown)
    if (shouldSendAlert('humidity')) {
      const emailSent = await sendEmailAlert({
        type: 'humidity',
        value: humidity,
        threshold: ALERT_CONFIG.HUMIDITY_THRESHOLD,
        temperature: temperature,
        humidity: humidity,
        level: level,
      });

      if (emailSent) {
        updateCooldown('humidity');
        triggeredAlerts.push({ type: 'humidity', level, emailSent: true });
      } else {
        triggeredAlerts.push({ type: 'humidity', level, emailSent: false });
      }
    } else {
      triggeredAlerts.push({ type: 'humidity', level, emailSent: false, suppressed: true });
    }
  }

  return triggeredAlerts;
}

/**
 * Get current alert configuration
 * @returns {Object} Alert configuration
 */
export function getAlertConfig() {
  return {
    tempThreshold: ALERT_CONFIG.TEMP_THRESHOLD,
    humidityThreshold: ALERT_CONFIG.HUMIDITY_THRESHOLD,
    emailCooldownMinutes: ALERT_CONFIG.EMAIL_COOLDOWN_MS / 60000,
    emailConfigured: !!(ALERT_CONFIG.GMAIL_USER && ALERT_CONFIG.GMAIL_APP_PASSWORD),
  };
}

