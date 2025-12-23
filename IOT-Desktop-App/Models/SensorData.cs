using System;

namespace IOT_Dashboard.Models
{
    public class SensorData
    {
        public long Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public DateTime MeasuredAt { get; set; }

        public override string ToString()
        {
            return $"Temp: {Temperature}°C, Humidity: {Humidity}%, Time: {MeasuredAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}

