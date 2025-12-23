using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IOT_Dashboard.Models
{
    // Response for /api/sensors/latest
    public class LatestSensorResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public SensorDataDto Data { get; set; }
    }

    // Response for /api/sensors (list with pagination)
    public class SensorListResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public List<SensorDataDto> Data { get; set; }

        [JsonProperty("pagination")]
        public PaginationInfo Pagination { get; set; }
    }

    // Response for /api/sensors/today
    public class TodayDataResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public List<SensorDataDto> Data { get; set; }
    }

    public class SensorDataDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("temperature")]
        public float Temperature { get; set; }

        [JsonProperty("humidity")]
        public float Humidity { get; set; }

        [JsonProperty("measured_at")]
        public DateTime MeasuredAt { get; set; }
    }

    public class PaginationInfo
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
    }
}

