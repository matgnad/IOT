using System;
using System.Threading.Tasks;
using IOT_Dashboard.Models;

namespace IOT_Dashboard.Services
{
    /// <summary>
    /// Alert Service - Main alert logic with threshold checking and cooldown
    /// </summary>
    public class AlertService
    {
        private readonly EmailService _emailService;
        private readonly SoundService _soundService;
        
        // Configuration
        private readonly float _tempThreshold;
        private readonly float _humidityThreshold;
        private readonly int _emailCooldownMinutes;
        
        // Cooldown tracking
        private DateTime? _lastTempEmailSent = null;
        private DateTime? _lastHumidityEmailSent = null;
        
        // Alert state tracking (for hysteresis - prevent flickering)
        private bool _tempAlertActive = false;
        private bool _humidityAlertActive = false;
        
        // Event for UI updates
        public event EventHandler<AlertEventArgs> AlertTriggered;
        
        public AlertService(EmailService emailService, SoundService soundService, 
            float tempThreshold, float humidityThreshold, int emailCooldownMinutes)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _soundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
            _tempThreshold = tempThreshold;
            _humidityThreshold = humidityThreshold;
            _emailCooldownMinutes = emailCooldownMinutes;
        }
        
        /// <summary>
        /// Check sensor data and trigger alerts if thresholds are exceeded
        /// </summary>
        public async Task CheckAlertsAsync(SensorData data)
        {
            if (data == null) return;
            
            // Check temperature threshold
            if (data.Temperature >= _tempThreshold)
            {
                await HandleTemperatureAlertAsync(data);
            }
            else
            {
                // Reset alert state when value returns to safe range (with hysteresis)
                if (_tempAlertActive && data.Temperature < _tempThreshold - 2.0f)
                {
                    _tempAlertActive = false;
                    _lastTempEmailSent = null; // Reset cooldown when alert clears
                    Console.WriteLine($"[Alert] ✅ Temperature returned to safe range: {data.Temperature:F1}°C");
                }
            }
            
            // Check humidity threshold
            if (data.Humidity >= _humidityThreshold)
            {
                await HandleHumidityAlertAsync(data);
            }
            else
            {
                // Reset alert state when value returns to safe range
                if (_humidityAlertActive && data.Humidity < _humidityThreshold - 5.0f)
                {
                    _humidityAlertActive = false;
                    _lastHumidityEmailSent = null;
                    Console.WriteLine($"[Alert] ✅ Humidity returned to safe range: {data.Humidity:F1}%");
                }
            }
        }
        
        private async Task HandleTemperatureAlertAsync(SensorData data)
        {
            bool isNewAlert = !_tempAlertActive;
            _tempAlertActive = true;
            
            // Determine alert level
            string level = data.Temperature >= _tempThreshold + 5.0f ? "CRITICAL" : "WARNING";
            
            Console.WriteLine($"[Alert] 🔥 Temperature Alert ({level}): {data.Temperature:F1}°C >= {_tempThreshold}°C");
            
            // Play sound (always, no cooldown)
            if (isNewAlert)
            {
                _soundService.PlayAlertSound(level == "CRITICAL");
                Console.WriteLine("[Alert] 🔊 Sound alert played");
            }
            
            // Send email (with cooldown)
            bool emailSent = false;
            if (ShouldSendEmail(_lastTempEmailSent))
            {
                try
                {
                    emailSent = await _emailService.SendAlertEmailAsync(
                        alertType: "Temperature",
                        value: data.Temperature,
                        threshold: _tempThreshold,
                        level: level,
                        timestamp: data.MeasuredAt,
                        humidity: data.Humidity
                    );
                    
                    if (emailSent)
                    {
                        _lastTempEmailSent = DateTime.Now;
                        Console.WriteLine("[Alert] ✅ Email sent successfully");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Alert] ❌ Email send failed: {ex.Message}");
                    // Don't crash - continue processing
                }
            }
            else
            {
                TimeSpan timeSinceLastEmail = DateTime.Now - _lastTempEmailSent.Value;
                int remainingMinutes = _emailCooldownMinutes - (int)timeSinceLastEmail.TotalMinutes;
                Console.WriteLine($"[Alert] ⏳ Email suppressed (cooldown: {remainingMinutes} min remaining)");
            }
            
            // Raise event for UI updates
            AlertTriggered?.Invoke(this, new AlertEventArgs
            {
                Type = "Temperature",
                Value = data.Temperature,
                Threshold = _tempThreshold,
                Level = level,
                Message = $"Temperature {data.Temperature:F1}°C exceeded threshold {_tempThreshold}°C",
                EmailSent = emailSent,
                IsNewAlert = isNewAlert
            });
        }
        
        private async Task HandleHumidityAlertAsync(SensorData data)
        {
            bool isNewAlert = !_humidityAlertActive;
            _humidityAlertActive = true;
            
            string level = "WARNING"; // Humidity alerts are typically warnings
            
            Console.WriteLine($"[Alert] 💧 Humidity Alert ({level}): {data.Humidity:F1}% >= {_humidityThreshold}%");
            
            // Play sound (always, no cooldown)
            if (isNewAlert)
            {
                _soundService.PlayAlertSound(false); // Warning sound
                Console.WriteLine("[Alert] 🔊 Sound alert played");
            }
            
            // Send email (with cooldown)
            bool emailSent = false;
            if (ShouldSendEmail(_lastHumidityEmailSent))
            {
                try
                {
                    emailSent = await _emailService.SendAlertEmailAsync(
                        alertType: "Humidity",
                        value: data.Humidity,
                        threshold: _humidityThreshold,
                        level: level,
                        timestamp: data.MeasuredAt,
                        temperature: data.Temperature
                    );
                    
                    if (emailSent)
                    {
                        _lastHumidityEmailSent = DateTime.Now;
                        Console.WriteLine("[Alert] ✅ Email sent successfully");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Alert] ❌ Email send failed: {ex.Message}");
                }
            }
            else
            {
                TimeSpan timeSinceLastEmail = DateTime.Now - _lastHumidityEmailSent.Value;
                int remainingMinutes = _emailCooldownMinutes - (int)timeSinceLastEmail.TotalMinutes;
                Console.WriteLine($"[Alert] ⏳ Email suppressed (cooldown: {remainingMinutes} min remaining)");
            }
            
            // Raise event for UI updates
            AlertTriggered?.Invoke(this, new AlertEventArgs
            {
                Type = "Humidity",
                Value = data.Humidity,
                Threshold = _humidityThreshold,
                Level = level,
                Message = $"Humidity {data.Humidity:F1}% exceeded threshold {_humidityThreshold}%",
                EmailSent = emailSent,
                IsNewAlert = isNewAlert
            });
        }
        
        private bool ShouldSendEmail(DateTime? lastSent)
        {
            if (lastSent == null) return true; // First time
            
            TimeSpan timeSinceLastEmail = DateTime.Now - lastSent.Value;
            return timeSinceLastEmail.TotalMinutes >= _emailCooldownMinutes;
        }
    }
    
    /// <summary>
    /// Event arguments for alert events
    /// </summary>
    public class AlertEventArgs : EventArgs
    {
        public string Type { get; set; } // "Temperature" or "Humidity"
        public float Value { get; set; }
        public float Threshold { get; set; }
        public string Level { get; set; } // "WARNING" or "CRITICAL"
        public string Message { get; set; }
        public bool EmailSent { get; set; }
        public bool IsNewAlert { get; set; }
    }
}

