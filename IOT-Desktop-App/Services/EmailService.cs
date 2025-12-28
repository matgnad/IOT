using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace IOT_Dashboard.Services
{
    /// <summary>
    /// Email Service - Sends alert emails via Gmail SMTP
    /// </summary>
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _gmailUser;
        private readonly string _gmailAppPassword;
        private readonly string _recipientEmail;
        private readonly bool _isConfigured;
        
        public EmailService()
        {
            // Load configuration from App.config
            _gmailUser = ConfigurationManager.AppSettings["GmailUser"] ?? "";
            _gmailAppPassword = ConfigurationManager.AppSettings["GmailAppPassword"] ?? "";
            _recipientEmail = ConfigurationManager.AppSettings["AlertEmailTo"] ?? _gmailUser;
            
            _isConfigured = !string.IsNullOrWhiteSpace(_gmailUser) && 
                           !string.IsNullOrWhiteSpace(_gmailAppPassword);
            
            if (!_isConfigured)
            {
                Console.WriteLine("[Email] ⚠️ Gmail credentials not configured. Email alerts disabled.");
                Console.WriteLine("[Email] Configure GmailUser and GmailAppPassword in App.config");
            }
            else
            {
                Console.WriteLine($"[Email] ✅ Email service configured for: {_recipientEmail}");
            }
        }
        
        /// <summary>
        /// Send alert email via Gmail SMTP
        /// </summary>
        public async Task<bool> SendAlertEmailAsync(string alertType, float value, float threshold, 
            string level, DateTime timestamp, float? temperature = null, float? humidity = null)
        {
            if (!_isConfigured)
            {
                Console.WriteLine("[Email] ⚠️ Email not sent: Gmail not configured");
                return false;
            }
            
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_gmailUser, _gmailAppPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 10000; // 10 seconds timeout
                    
                    var mail = new MailMessage
                    {
                        From = new MailAddress(_gmailUser, "IoT Monitoring System"),
                        Subject = $"🚨 IoT Alert: {alertType} {level}",
                        Body = BuildEmailBody(alertType, value, threshold, level, timestamp, temperature, humidity),
                        IsBodyHtml = true,
                        Priority = level == "CRITICAL" ? MailPriority.High : MailPriority.Normal
                    };
                    
                    mail.To.Add(_recipientEmail);
                    
                    await Task.Run(() => client.Send(mail));
                    
                    Console.WriteLine($"[Email] ✅ Alert email sent to {_recipientEmail}");
                    return true;
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[Email] ❌ SMTP Error: {ex.Message}");
                Console.WriteLine($"[Email] Status Code: {ex.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email] ❌ Error sending email: {ex.Message}");
                Console.WriteLine($"[Email] Stack: {ex.StackTrace}");
                return false;
            }
        }
        
        private string BuildEmailBody(string alertType, float value, float threshold, 
            string level, DateTime timestamp, float? temperature, float? humidity)
        {
            string unit = alertType == "Temperature" ? "°C" : "%";
            string bgColor = level == "CRITICAL" ? "#ff4444" : "#ffaa00";
            
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .alert-box {{ 
            background-color: {bgColor}; 
            color: white; 
            padding: 20px; 
            border-radius: 8px; 
            margin: 20px 0;
        }}
        .data-table {{ 
            background-color: #f5f5f5; 
            padding: 15px; 
            border-radius: 5px; 
            margin: 15px 0;
        }}
        .data-row {{ 
            display: flex; 
            justify-content: space-between; 
            padding: 8px 0; 
            border-bottom: 1px solid #ddd;
        }}
        .data-label {{ font-weight: bold; }}
        .footer {{ 
            margin-top: 30px; 
            padding-top: 20px; 
            border-top: 1px solid #ddd; 
            font-size: 12px; 
            color: #666;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""alert-box"">
            <h2>{level} ALERT</h2>
            <p><strong>{alertType} exceeded safe threshold!</strong></p>
        </div>
        
        <div class=""data-table"">
            <div class=""data-row"">
                <span class=""data-label"">Alert Type:</span>
                <span>{alertType}</span>
            </div>
            <div class=""data-row"">
                <span class=""data-label"">Current Value:</span>
                <span><strong>{value:F1}{unit}</strong></span>
            </div>
            <div class=""data-row"">
                <span class=""data-label"">Threshold:</span>
                <span>{threshold:F1}{unit}</span>
            </div>
            {(temperature.HasValue ? $@"
            <div class=""data-row"">
                <span class=""data-label"">Temperature:</span>
                <span>{temperature.Value:F1}°C</span>
            </div>" : "")}
            {(humidity.HasValue ? $@"
            <div class=""data-row"">
                <span class=""data-label"">Humidity:</span>
                <span>{humidity.Value:F1}%</span>
            </div>" : "")}
            <div class=""data-row"">
                <span class=""data-label"">Time:</span>
                <span>{timestamp:yyyy-MM-dd HH:mm:ss}</span>
            </div>
            <div class=""data-row"">
                <span class=""data-label"">Warning Level:</span>
                <span><strong>{level}</strong></span>
            </div>
        </div>
        
        <div class=""footer"">
            <p>This is an automated alert from your IoT monitoring system.</p>
            <p>Please check your sensor and take appropriate action.</p>
        </div>
    </div>
</body>
</html>";
        }
        
        public bool IsConfigured => _isConfigured;
    }
}

