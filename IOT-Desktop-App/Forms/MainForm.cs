using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using IOT_Dashboard.Models;
using IOT_Dashboard.Services;

namespace IOT_Dashboard.Forms
{
    public partial class MainForm : Form
    {
        private readonly SensorApiService _apiService;
        private System.Windows.Forms.Timer _refreshTimer;
        private bool _isUpdating = false;

        public MainForm()
        {
            InitializeComponent();
            
            // Initialize API service (get base URL from App.config or hardcode)
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:3000";
            _apiService = new SensorApiService(baseUrl);

            // Setup auto-refresh timer (every 10 seconds)
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 10000; // 10 seconds
            _refreshTimer.Tick += RefreshTimer_Tick;

            // Setup chart
            SetupChart();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Initial data load
            await RefreshAllDataAsync();

            // Start auto-refresh
            _refreshTimer.Start();
            lblStatus.Text = "Connected - Auto-refresh every 10s";
            lblStatus.ForeColor = Color.Green;
        }

        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            await RefreshAllDataAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshAllDataAsync();
        }

        private async Task RefreshAllDataAsync()
        {
            if (_isUpdating) return;

            _isUpdating = true;
            btnRefresh.Enabled = false;
            lblStatus.Text = "Updating...";
            lblStatus.ForeColor = Color.Orange;

            try
            {
                // 1. Fetch latest sensor data
                var latest = await _apiService.GetLatestAsync();
                if (latest != null)
                {
                    UpdateLatestDisplay(latest);
                }

                // 2. Fetch today's data for statistics and chart
                var todayData = await _apiService.GetTodayDataAsync();
                if (todayData.Count > 0)
                {
                    UpdateStatistics(todayData);
                    UpdateChart(todayData);
                }

                // 3. Fetch historical data for table (last 50 records)
                var history = await _apiService.GetHistoryAsync(page: 1, limit: 50, order: "DESC");
                UpdateHistoryTable(history);

                lblStatus.Text = $"Last updated: {DateTime.Now:HH:mm:ss}";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to fetch data:\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isUpdating = false;
                btnRefresh.Enabled = true;
            }
        }

        private void UpdateLatestDisplay(SensorData data)
        {
            lblTemperatureValue.Text = $"{data.Temperature:F1}°C";
            lblHumidityValue.Text = $"{data.Humidity:F1}%";
            lblLastUpdate.Text = $"Last reading: {data.MeasuredAt:yyyy-MM-dd HH:mm:ss}";
        }

        private void UpdateStatistics(List<SensorData> data)
        {
            var stats = _apiService.CalculateStatistics(data);

            lblTempMin.Text = $"{stats.MinTemperature:F1}°C";
            lblTempMax.Text = $"{stats.MaxTemperature:F1}°C";
            lblTempAvg.Text = $"{stats.AvgTemperature:F1}°C";

            lblHumidMin.Text = $"{stats.MinHumidity:F1}%";
            lblHumidMax.Text = $"{stats.MaxHumidity:F1}%";
            lblHumidAvg.Text = $"{stats.AvgHumidity:F1}%";

            lblTotalReadings.Text = $"{stats.TotalReadings} readings today";
        }

        private void UpdateHistoryTable(List<SensorData> data)
        {
            dgvHistory.Rows.Clear();

            foreach (var record in data)
            {
                dgvHistory.Rows.Add(
                    record.Id,
                    $"{record.Temperature:F1}",
                    $"{record.Humidity:F1}",
                    record.MeasuredAt.ToString("yyyy-MM-dd HH:mm:ss")
                );
            }
        }

        private void SetupChart()
        {
            chartSensors.Series.Clear();
            chartSensors.ChartAreas.Clear();

            // Create chart area
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Time";
            chartArea.AxisX.LabelStyle.Format = "HH:mm";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Auto;
            chartArea.AxisY.Title = "Value";
            chartArea.AxisY.LabelStyle.Format = "F1";
            chartSensors.ChartAreas.Add(chartArea);

            // Temperature series
            Series tempSeries = new Series("Temperature");
            tempSeries.ChartType = SeriesChartType.Line;
            tempSeries.Color = Color.Red;
            tempSeries.BorderWidth = 2;
            tempSeries.XValueType = ChartValueType.DateTime;
            chartSensors.Series.Add(tempSeries);

            // Humidity series
            Series humidSeries = new Series("Humidity");
            humidSeries.ChartType = SeriesChartType.Line;
            humidSeries.Color = Color.Blue;
            humidSeries.BorderWidth = 2;
            humidSeries.XValueType = ChartValueType.DateTime;
            chartSensors.Series.Add(humidSeries);

            // Enable legend
            chartSensors.Legends.Clear();
            Legend legend = new Legend("Legend");
            legend.Docking = Docking.Top;
            chartSensors.Legends.Add(legend);
        }

        private void UpdateChart(List<SensorData> data)
        {
            chartSensors.Series["Temperature"].Points.Clear();
            chartSensors.Series["Humidity"].Points.Clear();

            // Sort by time ascending for proper chart display
            var sortedData = data.OrderBy(d => d.MeasuredAt).ToList();

            foreach (var record in sortedData)
            {
                chartSensors.Series["Temperature"].Points.AddXY(record.MeasuredAt, record.Temperature);
                chartSensors.Series["Humidity"].Points.AddXY(record.MeasuredAt, record.Humidity);
            }

            // Auto-scale axes
            chartSensors.ChartAreas[0].RecalculateAxesScale();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop timer when closing
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            btnTestConnection.Enabled = false;
            lblStatus.Text = "Testing connection...";
            lblStatus.ForeColor = Color.Orange;

            try
            {
                bool connected = await _apiService.TestConnectionAsync();
                if (connected)
                {
                    MessageBox.Show("Successfully connected to backend API!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblStatus.Text = "Connection OK";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    MessageBox.Show("Failed to connect to backend API.\nPlease check if the server is running.", 
                        "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Connection failed";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection test failed:\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Connection error";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnTestConnection.Enabled = true;
            }
        }
    }
}

