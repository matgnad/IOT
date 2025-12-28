using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using IOT_Dashboard.Models;
using System.Configuration;

namespace IOT_Dashboard.Services
{
    /// <summary>
    /// MQTT Service - Subscribes to MQTT broker for real-time sensor data
    /// This provides faster alerts than REST API polling
    /// </summary>
    public class MqttService
    {
        private IMqttClient _mqttClient;
        private readonly string _brokerUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly string _topic;
        private bool _isConnected = false;
        
        // Event for real-time sensor data
        public event EventHandler<SensorData> SensorDataReceived;
        
        public MqttService()
        {
            // Load configuration from App.config
            _brokerUrl = ConfigurationManager.AppSettings["MqttBrokerUrl"] ?? "";
            _username = ConfigurationManager.AppSettings["MqttUsername"] ?? "";
            _password = ConfigurationManager.AppSettings["MqttPassword"] ?? "";
            _topic = ConfigurationManager.AppSettings["MqttTopic"] ?? "esp8266/sensors";
            
            if (string.IsNullOrWhiteSpace(_brokerUrl))
            {
                Console.WriteLine("[MQTT] ⚠️ MQTT broker URL not configured. MQTT service disabled.");
                Console.WriteLine("[MQTT] Configure MqttBrokerUrl in App.config to enable real-time alerts");
            }
        }
        
        /// <summary>
        /// Connect to MQTT broker and subscribe to sensor topic
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (string.IsNullOrWhiteSpace(_brokerUrl))
            {
                Console.WriteLine("[MQTT] ⚠️ Cannot connect: Broker URL not configured");
                return false;
            }
            
            try
            {
                var factory = new MqttFactory();
                _mqttClient = factory.CreateMqttClient();
                
                // Setup event handlers
                _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceived;
                _mqttClient.ConnectedAsync += HandleConnected;
                _mqttClient.DisconnectedAsync += HandleDisconnected;
                
                // Parse broker URL (format: mqtt://host:port or mqtt://user:pass@host:port)
                var uri = new Uri(_brokerUrl);
                string host = uri.Host;
                int port = uri.Port > 0 ? uri.Port : 1883;
                
                // Build options
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(host, port)
                    .WithClientId($"IoT-Dashboard-{Guid.NewGuid().ToString().Substring(0, 8)}")
                    .WithCleanSession();
                
                // Add authentication if provided
                if (!string.IsNullOrWhiteSpace(_username))
                {
                    options.WithCredentials(_username, _password);
                }
                
                // Connect
                var result = await _mqttClient.ConnectAsync(options.Build());
                
                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine($"[MQTT] ✅ Connected to {host}:{port}");
                    
                    // Subscribe to sensor topic
                    var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(_topic)
                        .Build();
                    
                    await _mqttClient.SubscribeAsync(subscribeOptions);
                    Console.WriteLine($"[MQTT] ✅ Subscribed to topic: {_topic}");
                    
                    _isConnected = true;
                    return true;
                }
                else
                {
                    Console.WriteLine($"[MQTT] ❌ Connection failed: {result.ResultCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MQTT] ❌ Connection error: {ex.Message}");
                Console.WriteLine($"[MQTT] Stack: {ex.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// Disconnect from MQTT broker
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_mqttClient != null && _isConnected)
            {
                try
                {
                    await _mqttClient.DisconnectAsync();
                    Console.WriteLine("[MQTT] ✅ Disconnected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MQTT] ❌ Disconnect error: {ex.Message}");
                }
                finally
                {
                    _isConnected = false;
                }
            }
        }
        
        private Task HandleConnected(MqttClientConnectedEventArgs args)
        {
            Console.WriteLine("[MQTT] ✅ Connection established");
            _isConnected = true;
            return Task.CompletedTask;
        }
        
        private Task HandleDisconnected(MqttClientDisconnectedEventArgs args)
        {
            Console.WriteLine($"[MQTT] ⚠️ Disconnected: {args.Reason}");
            _isConnected = false;
            return Task.CompletedTask;
        }
        
        private Task HandleMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            try
            {
                string payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"[MQTT] 📨 Received: {payload}");
                
                // Parse JSON payload
                var data = JsonConvert.DeserializeObject<dynamic>(payload);
                
                if (data != null && data.temp != null && data.humid != null)
                {
                    var sensorData = new SensorData
                    {
                        Temperature = (float)data.temp,
                        Humidity = (float)data.humid,
                        MeasuredAt = DateTime.Now // Use current time if not provided
                    };
                    
                    // Raise event for real-time processing
                    SensorDataReceived?.Invoke(this, sensorData);
                }
                else
                {
                    Console.WriteLine("[MQTT] ⚠️ Invalid payload format");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MQTT] ❌ Error processing message: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }
        
        public bool IsConnected => _isConnected;
    }
}

