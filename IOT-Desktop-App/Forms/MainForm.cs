using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;
using IOT_Dashboard.Models;
using IOT_Dashboard.Services;

namespace IOT_Dashboard.Forms
{
    public partial class MainForm : Form
    {
        private readonly SensorApiService _apiService;
        private readonly AlertService _alertService;
        private readonly EmailService _emailService;
        private readonly SoundService _soundService;
        private readonly MqttService _mqttService;
        
        private System.Windows.Forms.Timer _refreshTimer;
        private bool _isUpdating = false;
        private bool _useMqtt = false;

        // ✅ FIX #4: Panning state variables
        private bool _isPanning = false;
        private Point _panStartPoint = Point.Empty;
        private double _panStartX = 0;
        private double _panStartY = 0;

        public MainForm()
        {
            InitializeComponent();

            // Initialize services
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:3000";
            _apiService = new SensorApiService(baseUrl);
            
            // Initialize alert services
            _emailService = new EmailService();
            _soundService = new SoundService();
            
            // Load alert configuration
            float tempThreshold = float.Parse(ConfigurationManager.AppSettings["TemperatureThreshold"] ?? "35.0");
            float humidityThreshold = float.Parse(ConfigurationManager.AppSettings["HumidityThreshold"] ?? "80.0");
            int emailCooldownMinutes = int.Parse(ConfigurationManager.AppSettings["EmailCooldownMinutes"] ?? "15");
            
            _alertService = new AlertService(_emailService, _soundService, tempThreshold, humidityThreshold, emailCooldownMinutes);
            _alertService.AlertTriggered += AlertService_AlertTriggered;
            
            // Initialize MQTT service (optional)
            _mqttService = new MqttService();
            _useMqtt = bool.Parse(ConfigurationManager.AppSettings["UseMqtt"] ?? "false");
            
            // Setup auto-refresh timer (only if not using MQTT)
            _refreshTimer = new System.Windows.Forms.Timer();
            int refreshInterval = int.Parse(ConfigurationManager.AppSettings["RefreshInterval"] ?? "10");
            _refreshTimer.Interval = refreshInterval * 1000;
            _refreshTimer.Tick += RefreshTimer_Tick;

            // Setup chart
            SetupChart();
            
            // Setup mute button (will be initialized after InitializeComponent)
            // Hook up in MainForm_Load instead
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Setup mute button
            if (btnMuteSound != null)
            {
                btnMuteSound.Click += BtnMuteSound_Click;
                UpdateMuteButtonText();
            }
            
            // Connect to MQTT if enabled
            if (_useMqtt)
            {
                lblStatus.Text = "Connecting to MQTT...";
                lblStatus.ForeColor = Color.Orange;
                
                bool mqttConnected = await _mqttService.ConnectAsync();
                if (mqttConnected)
                {
                    _mqttService.SensorDataReceived += MqttService_SensorDataReceived;
                    lblStatus.Text = "Connected via MQTT - Real-time alerts enabled";
                    lblStatus.ForeColor = Color.Green;
                    Console.WriteLine("[MainForm] ✅ MQTT connected - Real-time alerts active");
                }
                else
                {
                    lblStatus.Text = "MQTT failed - Falling back to REST API";
                    lblStatus.ForeColor = Color.Orange;
                    _useMqtt = false; // Fallback to REST API
                    _refreshTimer.Start();
                }
            }
            else
            {
                // Use REST API polling
                await RefreshAllDataAsync();
                _refreshTimer.Start();
                lblStatus.Text = $"Connected - Auto-refresh every {_refreshTimer.Interval / 1000}s";
                lblStatus.ForeColor = Color.Green;
            }
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
            if (data == null) return;
            
            // Update UI
            lblTemperatureValue.Text = $"{data.Temperature:F1}°C";
            lblHumidityValue.Text = $"{data.Humidity:F1}%";
            lblLastUpdate.Text = $"Last reading: {data.MeasuredAt:yyyy-MM-dd HH:mm:ss}";

            // 🚨 ALERT SYSTEM: Check thresholds and trigger alerts
            // This will handle sound, email, and UI updates via events
            _ = _alertService.CheckAlertsAsync(data); // Fire and forget (non-blocking)
        }
        
        /// <summary>
        /// Handle alert events from AlertService
        /// </summary>
        private void AlertService_AlertTriggered(object sender, AlertEventArgs e)
        {
            // Update UI on UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleAlertUI(e)));
            }
            else
            {
                HandleAlertUI(e);
            }
        }
        
        /// <summary>
        /// Handle MQTT real-time sensor data
        /// </summary>
        private void MqttService_SensorDataReceived(object sender, SensorData data)
        {
            // Update UI
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateLatestDisplay(data)));
            }
            else
            {
                UpdateLatestDisplay(data);
            }
            
            // Also update chart and statistics (optional - can be done less frequently)
            // For now, just update latest display and trigger alerts
        }
        
        private void HandleAlertUI(AlertEventArgs e)
        {
            // Update status bar
            string statusText = $"{e.Level}: {e.Message}";
            if (e.EmailSent)
            {
                statusText += " (Email sent)";
            }
            lblStatus.Text = statusText;
            lblStatus.ForeColor = e.Level == "CRITICAL" ? Color.Red : Color.Orange;
            
            // Update sensor card based on alert type
            if (e.Type == "Temperature")
            {
                if (e.Level == "CRITICAL")
                {
                    pnlTempCard.BackColor = Color.FromArgb(169, 50, 38); // Dark red
                    lblTempWarning.Visible = true;
                    lblTempWarning.Text = "🔥 CRITICAL";
                    lblTempWarning.ForeColor = Color.White;
                    lblTempWarning.BackColor = Color.FromArgb(169, 50, 38);
                }
                else
                {
                    pnlTempCard.BackColor = Color.FromArgb(230, 126, 34); // Orange
                    lblTempWarning.Visible = true;
                    lblTempWarning.Text = "⚠️ WARNING";
                    lblTempWarning.ForeColor = Color.White;
                    lblTempWarning.BackColor = Color.FromArgb(230, 126, 34);
                }
                
                // Show popup only for new alerts
                if (e.IsNewAlert)
                {
                    MessageBox.Show(
                        $"{e.Level} ALERT: {e.Message}\n\n" +
                        $"Current: {e.Value:F1}°C\n" +
                        $"Threshold: {e.Threshold:F1}°C\n\n" +
                        $"{(e.Level == "CRITICAL" ? "⚠️ IMMEDIATE ACTION REQUIRED" : "Please monitor your system.")}",
                        $"{e.Level} {e.Type} Alert",
                        MessageBoxButtons.OK,
                        e.Level == "CRITICAL" ? MessageBoxIcon.Error : MessageBoxIcon.Warning);
                }
            }
            else if (e.Type == "Humidity")
            {
                // Update humidity card if needed (add similar logic)
                // For now, just show in status bar
            }
        }
        
        /// <summary>
        /// Mute/Unmute sound button handler
        /// </summary>
        private void BtnMuteSound_Click(object sender, EventArgs e)
        {
            _soundService.SoundEnabled = !_soundService.SoundEnabled;
            UpdateMuteButtonText();
        }
        
        private void UpdateMuteButtonText()
        {
            if (btnMuteSound != null)
            {
                btnMuteSound.Text = _soundService.SoundEnabled ? "🔊 Sound: ON" : "🔇 Sound: OFF";
                btnMuteSound.BackColor = _soundService.SoundEnabled ? 
                    Color.FromArgb(46, 204, 113) : Color.FromArgb(149, 165, 166);
            }
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
            try
            {
                // Clear existing chart
                chartSensors.Series.Clear();
                chartSensors.ChartAreas.Clear();
                chartSensors.Legends.Clear();

                // Create chart area with single Y-axis for both Temperature and Humidity
                ChartArea chartArea = new ChartArea("MainArea");

                // ✅ OPTIMIZE: Maximize vertical plotting area by reducing bottom whitespace
                chartArea.Position.Auto = false;
                chartArea.Position.X = 3; // Small left margin for Y-axis labels
                chartArea.Position.Y = 5; // Small top margin for legend
                chartArea.Position.Width = 95; // Use 95% width (leaves space for Y-axis on right)
                chartArea.Position.Height = 95; // Use 92% height (reduces bottom whitespace)

                // ✅ OPTIMIZE: Reduce Chart control bottom padding
                chartSensors.Padding = new Padding(0, 0, 0, 0);

                // ===== X-AXIS (TIME) CONFIGURATION =====
                chartArea.AxisX.Title = "Time";
                chartArea.AxisX.LabelStyle.Format = "HH:mm";
                chartArea.AxisX.IntervalType = DateTimeIntervalType.Auto;
                chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
                
                // ✅ OPTIMIZE: Reduce X-axis label spacing to sit closer to bottom
                chartArea.AxisX.LabelStyle.IsEndLabelVisible = true;
                chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None; // Prevent auto-fitting that adds space
                chartArea.AxisX.TitleAlignment = StringAlignment.Center;
                chartArea.AxisX.TitleFont = new Font("Segoe UI", 9f, FontStyle.Regular); // Smaller title font
                
                // ✅ OPTIMIZE: Reduce margin below X-axis labels
                chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 8f); // Smaller label font
                chartArea.AxisX.MajorTickMark.Enabled = true;
                chartArea.AxisX.MajorTickMark.Size = 3; // Smaller tick marks

                // ✅ FIX #4: Enable Zoom & Pan on X-Axis with scroll bar
                chartArea.CursorX.IsUserEnabled = true;
                chartArea.CursorX.IsUserSelectionEnabled = true; // Allow selection for zoom
                chartArea.CursorX.AutoScroll = true;
                chartArea.AxisX.ScaleView.Zoomable = true;
                chartArea.AxisX.ScrollBar.IsPositionedInside = true;
                chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                chartArea.AxisX.ScrollBar.Size = 15;
                chartArea.AxisX.ScrollBar.Enabled = true; // ✅ FIX #4: Enable scroll bar

                // ===== PRIMARY Y-AXIS (SINGLE AXIS FOR BOTH TEMP & HUMIDITY) =====
                chartArea.AxisY.Title = "Value";
                chartArea.AxisY.LabelStyle.Format = "F1";
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.LineColor = Color.Gray;
                
                // ✅ OPTIMIZE: Reduce Y-axis label spacing
                chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 8f); // Smaller label font
                chartArea.AxisY.TitleFont = new Font("Segoe UI", 9f, FontStyle.Regular); // Smaller title font

                // Auto-scale Y-axis (can start from 0 or auto based on data)
                chartArea.AxisY.IsStartedFromZero = false;

                // Enable Y-axis zoom (vertical zoom)
                chartArea.CursorY.IsUserEnabled = true;
                chartArea.CursorY.IsUserSelectionEnabled = true;
                chartArea.AxisY.ScaleView.Zoomable = true;

                // Disable Secondary Y-Axis
                chartArea.AxisY2.Enabled = AxisEnabled.False;

                chartSensors.ChartAreas.Add(chartArea);

                // ===== TEMPERATURE SERIES (Red, Primary Y-Axis) =====
                Series tempSeries = new Series("Temperature");
                tempSeries.ChartType = SeriesChartType.Line;
                tempSeries.Color = Color.FromArgb(231, 76, 60); // Red (matches Temperature card)
                tempSeries.BorderWidth = 3;
                tempSeries.XValueType = ChartValueType.DateTime;
                tempSeries.YAxisType = AxisType.Primary; // Single Y-axis
                tempSeries.IsVisibleInLegend = true;
                tempSeries.MarkerStyle = MarkerStyle.Circle;
                tempSeries.MarkerSize = 6;
                tempSeries.MarkerColor = Color.FromArgb(192, 57, 43);
                tempSeries.ToolTip = "Time: #VALX{HH:mm:ss}\nTemperature: #VALY{F1}°C";
                chartSensors.Series.Add(tempSeries);

                // ===== HUMIDITY SERIES (Blue, Primary Y-Axis) =====
                Series humidSeries = new Series("Humidity");
                humidSeries.ChartType = SeriesChartType.Line;
                humidSeries.Color = Color.FromArgb(52, 152, 219); // Blue (matches Humidity card)
                humidSeries.BorderWidth = 3;
                humidSeries.XValueType = ChartValueType.DateTime;
                humidSeries.YAxisType = AxisType.Primary; // Single Y-axis (same as Temperature)
                humidSeries.IsVisibleInLegend = true;
                humidSeries.MarkerStyle = MarkerStyle.Circle;
                humidSeries.MarkerSize = 6;
                humidSeries.MarkerColor = Color.FromArgb(41, 128, 185);
                humidSeries.ToolTip = "Time: #VALX{HH:mm:ss}\nHumidity: #VALY{F1}%";
                chartSensors.Series.Add(humidSeries);

                // Enable legend (shows both series)
                Legend legend = new Legend("Legend");
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Center;
                legend.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                chartSensors.Legends.Add(legend);

                // ✅ FIX: Enable MouseWheel zoom and improved interactions
                chartSensors.MouseWheel += ChartSensors_MouseWheel;
                chartSensors.MouseDown += ChartSensors_MouseDown;

                // ✅ FIX #4: Enable panning with mouse drag
                chartSensors.MouseMove += ChartSensors_MouseMove;
                chartSensors.MouseUp += ChartSensors_MouseUp;

                // Add context menu for chart
                var contextMenu = new ContextMenuStrip();
                var resetZoomItem = new ToolStripMenuItem("🔄 Reset Zoom");
                resetZoomItem.Click += (s, e) =>
                {
                    chartSensors.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chartSensors.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    Console.WriteLine("[Chart] Zoom reset via context menu");
                };
                contextMenu.Items.Add(resetZoomItem);
                chartSensors.ContextMenuStrip = contextMenu;

                Console.WriteLine("[Chart Setup] ✅ Single Y-axis chart configured:");
                Console.WriteLine("  - Both Temperature and Humidity on Primary Y-axis");
                Console.WriteLine("  - Auto-scaling Y-axis");
                Console.WriteLine("  - Optimized layout: 92% height, minimal bottom padding");
                Console.WriteLine("  - MouseWheel zoom (scroll up/down to zoom in/out)");
                Console.WriteLine("  - Click-drag selection to zoom into time range");
                Console.WriteLine("  - Double-click or right-click menu to reset zoom");
                Console.WriteLine("  - Scroll bar for panning when zoomed");
                Console.WriteLine("  - Tooltips on hover");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chart Setup Error] ❌ {ex.Message}");
                MessageBox.Show($"Chart initialization error: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Chart Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== MOUSEWHEEL ZOOM HANDLER =====
        private void ChartSensors_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                var chart = (Chart)sender;
                var chartArea = chart.ChartAreas[0];
                var xAxis = chartArea.AxisX;
                var yAxis = chartArea.AxisY;

                // Determine zoom direction: scroll up = zoom in, scroll down = zoom out
                double zoomFactor = e.Delta > 0 ? 0.9 : 1.1; // 10% zoom per scroll

                // Get mouse position relative to chart area
                var pos = e.Location;
                var results = chart.HitTest(pos.X, pos.Y, false, ChartElementType.PlottingArea);

                if (results.Count() > 0 && results[0].ChartArea != null)
                {
                    // Zoom X-axis (time)
                    double xMin = xAxis.ScaleView.ViewMinimum;
                    double xMax = xAxis.ScaleView.ViewMaximum;
                    double xRange = xMax - xMin;
                    double xCenter = (xMin + xMax) / 2;

                    double newXRange = xRange * zoomFactor;
                    double newXMin = xCenter - newXRange / 2;
                    double newXMax = xCenter + newXRange / 2;

                    // Apply X-axis zoom if within data bounds
                    if (newXRange > 0 && newXMin >= xAxis.Minimum && newXMax <= xAxis.Maximum)
                    {
                        xAxis.ScaleView.Zoom(newXMin, newXMax);
                    }
                    else if (zoomFactor > 1) // Zooming out - reset to full view
                    {
                        xAxis.ScaleView.ZoomReset();
                        yAxis.ScaleView.ZoomReset();
                    }

                    Console.WriteLine($"[MouseWheel] Zoom {(e.Delta > 0 ? "IN" : "OUT")} - X Range: {newXMin:F0} to {newXMax:F0}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MouseWheel Error] {ex.Message}");
            }
        }

        // ===== MOUSE DOWN HANDLER (Double-click to reset zoom, Right-click/Ctrl+Click to pan) =====
        private void ChartSensors_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var chart = (Chart)sender;
                var chartArea = chart.ChartAreas[0];

                // Double-click with any button resets zoom
                if (e.Clicks >= 2)
                {
                    chartArea.AxisX.ScaleView.ZoomReset();
                    chartArea.AxisY.ScaleView.ZoomReset();
                    Console.WriteLine("[Chart] Zoom reset via double-click");
                    return;
                }

                // ✅ FIX #4: Start panning on right-click or Ctrl+left-click
                if (e.Button == MouseButtons.Right || (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control))
                {
                    _isPanning = true;
                    _panStartPoint = e.Location;

                    // Store current view position (use ViewMinimum if zoomed, otherwise Minimum)
                    _panStartX = chartArea.AxisX.ScaleView.IsZoomed ? chartArea.AxisX.ScaleView.ViewMinimum : chartArea.AxisX.Minimum;
                    _panStartY = chartArea.AxisY.ScaleView.IsZoomed ? chartArea.AxisY.ScaleView.ViewMinimum : chartArea.AxisY.Minimum;

                    chart.Cursor = Cursors.Hand; // Show hand cursor while panning
                    Console.WriteLine("[Chart] Panning started (Right-click or Ctrl+Click)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MouseDown Error] {ex.Message}");
            }
        }

        // ✅ FIX #4: Mouse move handler for panning
        private void ChartSensors_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!_isPanning) return;

                var chart = (Chart)sender;
                var chartArea = chart.ChartAreas[0];

                // Calculate pixel delta
                int deltaX = e.X - _panStartPoint.X;
                int deltaY = e.Y - _panStartPoint.Y;

                // Convert pixel delta to value delta using axis scale
                if (chartArea.AxisX.ScaleView.IsZoomed)
                {
                    double xRange = chartArea.AxisX.ScaleView.ViewMaximum - chartArea.AxisX.ScaleView.ViewMinimum;
                    double xPixelRange = chartArea.AxisX.ValueToPixelPosition(chartArea.AxisX.ScaleView.ViewMaximum) -
                                       chartArea.AxisX.ValueToPixelPosition(chartArea.AxisX.ScaleView.ViewMinimum);

                    if (xPixelRange != 0)
                    {
                        double deltaXValue = -(deltaX * xRange / xPixelRange); // Negative for natural pan direction
                        double newXMin = _panStartX + deltaXValue;
                        double newXMax = newXMin + xRange;

                        // Clamp to data bounds
                        if (newXMin >= chartArea.AxisX.Minimum && newXMax <= chartArea.AxisX.Maximum)
                        {
                            chartArea.AxisX.ScaleView.Position = newXMin;
                        }
                    }
                }

                // Apply panning to Y-axis
                if (chartArea.AxisY.ScaleView.IsZoomed)
                {
                    double yRange = chartArea.AxisY.ScaleView.ViewMaximum - chartArea.AxisY.ScaleView.ViewMinimum;
                    double yPixelRange = chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.ScaleView.ViewMaximum) -
                                       chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.ScaleView.ViewMinimum);

                    if (yPixelRange != 0)
                    {
                        double deltaYValue = (deltaY * yRange / yPixelRange); // Positive for natural pan direction (Y is inverted)
                        double newYMin = _panStartY - deltaYValue;
                        double newYMax = newYMin + yRange;

                        // Clamp to data bounds
                        if (newYMin >= chartArea.AxisY.Minimum && newYMax <= chartArea.AxisY.Maximum)
                        {
                            chartArea.AxisY.ScaleView.Position = newYMin;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MouseMove Error] {ex.Message}");
            }
        }

        // ✅ FIX #4: Mouse up handler to stop panning
        private void ChartSensors_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (_isPanning)
                {
                    _isPanning = false;
                    var chart = (Chart)sender;
                    chart.Cursor = Cursors.Default; // Restore default cursor
                    Console.WriteLine("[Chart] Panning stopped");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MouseUp Error] {ex.Message}");
            }
        }

        private void UpdateChart(List<SensorData> data)
        {
            try
            {
                // Handle null or empty data
                if (data == null || data.Count == 0)
                {
                    Console.WriteLine("[Chart Update] ⚠️ No data available");

                    // Clear existing data but don't throw error
                    if (chartSensors.Series.Count >= 2)
                    {
                        chartSensors.Series["Temperature"].Points.Clear();
                        chartSensors.Series["Humidity"].Points.Clear();
                    }
                    return;
                }

                // Clear previous data
                chartSensors.Series["Temperature"].Points.Clear();
                chartSensors.Series["Humidity"].Points.Clear();

                // Sort by time ascending for proper chart display
                var sortedData = data.OrderBy(d => d.MeasuredAt).ToList();

                Console.WriteLine($"[Chart Update] 📊 Binding {sortedData.Count} records to chart");

                // Add data points to both series
                int tempPoints = 0;
                int humidPoints = 0;

                foreach (var record in sortedData)
                {
                    // Add Temperature point (Primary Y-axis)
                    chartSensors.Series["Temperature"].Points.AddXY(record.MeasuredAt, record.Temperature);
                    tempPoints++;

                    // Add Humidity point (Primary Y-axis - same as Temperature)
                    chartSensors.Series["Humidity"].Points.AddXY(record.MeasuredAt, record.Humidity);
                    humidPoints++;
                }

                Console.WriteLine($"[Chart Update] ✅ Temperature: {tempPoints} points, Humidity: {humidPoints} points");

                // ✅ FIX #1: Calculate Y-axis range from ACTUAL DATA (not current axis state)
                // This prevents the axis from expanding indefinitely on each refresh
                var chartArea = chartSensors.ChartAreas[0];

                // Find min/max from actual data points (both Temperature and Humidity)
                double dataMin = Math.Min(
                    sortedData.Min(x => x.Temperature),
                    sortedData.Min(x => x.Humidity)
                );
                double dataMax = Math.Max(
                    sortedData.Max(x => x.Temperature),
                    sortedData.Max(x => x.Humidity)
                );

                // Add buffer to prevent data clipping at edges
                double dataRange = dataMax - dataMin;
                double yBuffer = Math.Max(2.0, dataRange * 0.1); // 10% buffer or minimum 2 units

                // Set axis range based on data (not current axis state)
                chartArea.AxisY.Minimum = dataMin - yBuffer;
                chartArea.AxisY.Maximum = dataMax + yBuffer;

                Console.WriteLine($"[Chart Update] 📊 Data range: {dataMin:F1} to {dataMax:F1}");
                Console.WriteLine($"[Chart Update] 📊 Y-axis: {chartArea.AxisY.Minimum:F1} to {chartArea.AxisY.Maximum:F1} (buffer added)");

                // Verify both series are visible
                if (chartSensors.Series["Temperature"].Points.Count == 0)
                {
                    Console.WriteLine("[Chart Update] ⚠️ Temperature series has no points!");
                }

                if (chartSensors.Series["Humidity"].Points.Count == 0)
                {
                    Console.WriteLine("[Chart Update] ⚠️ Humidity series has no points!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chart Error] ❌ {ex.Message}");
                Console.WriteLine($"[Chart Error] Stack: {ex.StackTrace}");

                // Show error in status bar
                lblStatus.Text = $"Chart error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop timer when closing
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            
            // Disconnect MQTT
            if (_mqttService != null && _useMqtt)
            {
                await _mqttService.DisconnectAsync();
            }
            
            // Cleanup sound service
            _soundService?.Dispose();
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

