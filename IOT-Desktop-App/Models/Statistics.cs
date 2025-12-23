namespace IOT_Dashboard.Models
{
    public class Statistics
    {
        public float MinTemperature { get; set; }
        public float MaxTemperature { get; set; }
        public float AvgTemperature { get; set; }

        public float MinHumidity { get; set; }
        public float MaxHumidity { get; set; }
        public float AvgHumidity { get; set; }

        public int TotalReadings { get; set; }
    }
}

