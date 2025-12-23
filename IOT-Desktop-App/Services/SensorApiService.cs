using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IOT_Dashboard.Models;

namespace IOT_Dashboard.Services
{
    public class SensorApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SensorApiService(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        /// <summary>
        /// Fetch the latest sensor reading
        /// GET /api/sensors/latest
        /// </summary>
        public async Task<SensorData> GetLatestAsync()
        {
            try
            {
                string url = $"{_baseUrl}/api/sensors/latest";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<LatestSensorResponse>(json);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return new SensorData
                    {
                        Id = apiResponse.Data.Id,
                        Temperature = apiResponse.Data.Temperature,
                        Humidity = apiResponse.Data.Humidity,
                        MeasuredAt = apiResponse.Data.MeasuredAt
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] GetLatest: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Fetch historical sensor data (paginated)
        /// GET /api/sensors?page=1&limit=100&order=DESC
        /// </summary>
        public async Task<List<SensorData>> GetHistoryAsync(int page = 1, int limit = 100, string order = "DESC")
        {
            try
            {
                string url = $"{_baseUrl}/api/sensors?page={page}&limit={limit}&order={order}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<SensorListResponse>(json);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data.Select(dto => new SensorData
                    {
                        Id = dto.Id,
                        Temperature = dto.Temperature,
                        Humidity = dto.Humidity,
                        MeasuredAt = dto.MeasuredAt
                    }).ToList();
                }

                return new List<SensorData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] GetHistory: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Fetch today's sensor data
        /// GET /api/sensors/today
        /// </summary>
        public async Task<List<SensorData>> GetTodayDataAsync()
        {
            try
            {
                string url = $"{_baseUrl}/api/sensors/today";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<TodayDataResponse>(json);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data.Select(dto => new SensorData
                    {
                        Id = dto.Id,
                        Temperature = dto.Temperature,
                        Humidity = dto.Humidity,
                        MeasuredAt = dto.MeasuredAt
                    }).ToList();
                }

                return new List<SensorData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] GetTodayData: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Calculate statistics from a list of sensor data (client-side)
        /// </summary>
        public Statistics CalculateStatistics(List<SensorData> data)
        {
            if (data == null || data.Count == 0)
            {
                return new Statistics();
            }

            return new Statistics
            {
                MinTemperature = data.Min(d => d.Temperature),
                MaxTemperature = data.Max(d => d.Temperature),
                AvgTemperature = data.Average(d => d.Temperature),
                MinHumidity = data.Min(d => d.Humidity),
                MaxHumidity = data.Max(d => d.Humidity),
                AvgHumidity = data.Average(d => d.Humidity),
                TotalReadings = data.Count
            };
        }

        /// <summary>
        /// Test connection to backend
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                string url = $"{_baseUrl}/api/sensors/latest";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

