using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IOT_Dashboard.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Text = "IOT Dashboard - Temperature & Humidity Monitor";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);

            // Title Panel
            Panel pnlTitle = new Panel();
            pnlTitle.BackColor = Color.FromArgb(41, 128, 185);
            pnlTitle.Dock = DockStyle.Top;
            pnlTitle.Height = 80;

            Label lblTitle = new Label();
            lblTitle.Text = "IOT SENSOR DASHBOARD";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = false;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Dock = DockStyle.Fill;

            pnlTitle.Controls.Add(lblTitle);
            this.Controls.Add(pnlTitle);

            // Main container panel
            Panel pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Padding = new Padding(20);
            pnlMain.BackColor = Color.FromArgb(236, 240, 241);

            // ===== TOP SECTION: Current Values Cards =====
            Panel pnlTop = new Panel();
            pnlTop.Height = 150;
            pnlTop.Dock = DockStyle.Top;

            // Temperature Card
            Panel pnlTempCard = new Panel();
            pnlTempCard.Size = new Size(300, 130);
            pnlTempCard.Location = new Point(20, 10);
            pnlTempCard.BackColor = Color.FromArgb(231, 76, 60);
            pnlTempCard.Padding = new Padding(15);

            Label lblTempTitle = new Label();
            lblTempTitle.Text = "TEMPERATURE";
            lblTempTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTempTitle.ForeColor = Color.White;
            lblTempTitle.Location = new Point(15, 15);
            lblTempTitle.AutoSize = true;

            this.lblTemperatureValue = new Label();
            this.lblTemperatureValue.Text = "--°C";
            this.lblTemperatureValue.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            this.lblTemperatureValue.ForeColor = Color.White;
            this.lblTemperatureValue.Location = new Point(15, 45);
            this.lblTemperatureValue.AutoSize = true;

            pnlTempCard.Controls.Add(lblTempTitle);
            pnlTempCard.Controls.Add(this.lblTemperatureValue);
            pnlTop.Controls.Add(pnlTempCard);

            // Humidity Card
            Panel pnlHumidCard = new Panel();
            pnlHumidCard.Size = new Size(300, 130);
            pnlHumidCard.Location = new Point(340, 10);
            pnlHumidCard.BackColor = Color.FromArgb(52, 152, 219);
            pnlHumidCard.Padding = new Padding(15);

            Label lblHumidTitle = new Label();
            lblHumidTitle.Text = "HUMIDITY";
            lblHumidTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblHumidTitle.ForeColor = Color.White;
            lblHumidTitle.Location = new Point(15, 15);
            lblHumidTitle.AutoSize = true;

            this.lblHumidityValue = new Label();
            this.lblHumidityValue.Text = "--%";
            this.lblHumidityValue.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            this.lblHumidityValue.ForeColor = Color.White;
            this.lblHumidityValue.Location = new Point(15, 45);
            this.lblHumidityValue.AutoSize = true;

            pnlHumidCard.Controls.Add(lblHumidTitle);
            pnlHumidCard.Controls.Add(this.lblHumidityValue);
            pnlTop.Controls.Add(pnlHumidCard);

            // Control buttons panel
            Panel pnlButtons = new Panel();
            pnlButtons.Location = new Point(660, 10);
            pnlButtons.Size = new Size(300, 130);
            pnlButtons.BackColor = Color.White;
            pnlButtons.Padding = new Padding(10);

            this.btnRefresh = new Button();
            this.btnRefresh.Text = "🔄 Refresh Now";
            this.btnRefresh.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.btnRefresh.Size = new Size(280, 45);
            this.btnRefresh.Location = new Point(10, 10);
            this.btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Cursor = Cursors.Hand;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.btnTestConnection = new Button();
            this.btnTestConnection.Text = "🔌 Test Connection";
            this.btnTestConnection.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.btnTestConnection.Size = new Size(280, 40);
            this.btnTestConnection.Location = new Point(10, 65);
            this.btnTestConnection.BackColor = Color.FromArgb(149, 165, 166);
            this.btnTestConnection.ForeColor = Color.White;
            this.btnTestConnection.FlatStyle = FlatStyle.Flat;
            this.btnTestConnection.FlatAppearance.BorderSize = 0;
            this.btnTestConnection.Cursor = Cursors.Hand;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);

            pnlButtons.Controls.Add(this.btnRefresh);
            pnlButtons.Controls.Add(this.btnTestConnection);
            pnlTop.Controls.Add(pnlButtons);

            pnlMain.Controls.Add(pnlTop);

            // ===== MIDDLE SECTION: Statistics =====
            GroupBox grpStats = new GroupBox();
            grpStats.Text = "📊 STATISTICS (Today)";
            grpStats.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grpStats.Height = 120;
            grpStats.Dock = DockStyle.Top;
            grpStats.BackColor = Color.White;
            grpStats.Padding = new Padding(10);

            // Temperature stats
            Label lblTempStatsTitle = new Label();
            lblTempStatsTitle.Text = "Temperature:";
            lblTempStatsTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTempStatsTitle.Location = new Point(20, 30);
            lblTempStatsTitle.AutoSize = true;

            this.lblTempMin = new Label();
            this.lblTempMin.Text = "Min: --°C";
            this.lblTempMin.Font = new Font("Segoe UI", 9);
            this.lblTempMin.Location = new Point(20, 55);
            this.lblTempMin.AutoSize = true;

            this.lblTempMax = new Label();
            this.lblTempMax.Text = "Max: --°C";
            this.lblTempMax.Font = new Font("Segoe UI", 9);
            this.lblTempMax.Location = new Point(120, 55);
            this.lblTempMax.AutoSize = true;

            this.lblTempAvg = new Label();
            this.lblTempAvg.Text = "Avg: --°C";
            this.lblTempAvg.Font = new Font("Segoe UI", 9);
            this.lblTempAvg.Location = new Point(220, 55);
            this.lblTempAvg.AutoSize = true;

            // Humidity stats
            Label lblHumidStatsTitle = new Label();
            lblHumidStatsTitle.Text = "Humidity:";
            lblHumidStatsTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblHumidStatsTitle.Location = new Point(400, 30);
            lblHumidStatsTitle.AutoSize = true;

            this.lblHumidMin = new Label();
            this.lblHumidMin.Text = "Min: --%";
            this.lblHumidMin.Font = new Font("Segoe UI", 9);
            this.lblHumidMin.Location = new Point(400, 55);
            this.lblHumidMin.AutoSize = true;

            this.lblHumidMax = new Label();
            this.lblHumidMax.Text = "Max: --%";
            this.lblHumidMax.Font = new Font("Segoe UI", 9);
            this.lblHumidMax.Location = new Point(500, 55);
            this.lblHumidMax.AutoSize = true;

            this.lblHumidAvg = new Label();
            this.lblHumidAvg.Text = "Avg: --%";
            this.lblHumidAvg.Font = new Font("Segoe UI", 9);
            this.lblHumidAvg.Location = new Point(600, 55);
            this.lblHumidAvg.AutoSize = true;

            this.lblTotalReadings = new Label();
            this.lblTotalReadings.Text = "0 readings today";
            this.lblTotalReadings.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            this.lblTotalReadings.Location = new Point(20, 80);
            this.lblTotalReadings.AutoSize = true;

            grpStats.Controls.Add(lblTempStatsTitle);
            grpStats.Controls.Add(this.lblTempMin);
            grpStats.Controls.Add(this.lblTempMax);
            grpStats.Controls.Add(this.lblTempAvg);
            grpStats.Controls.Add(lblHumidStatsTitle);
            grpStats.Controls.Add(this.lblHumidMin);
            grpStats.Controls.Add(this.lblHumidMax);
            grpStats.Controls.Add(this.lblHumidAvg);
            grpStats.Controls.Add(this.lblTotalReadings);

            pnlMain.Controls.Add(grpStats);

            // ===== BOTTOM SECTION: Split between Chart and Table =====
            SplitContainer splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 300;
            splitContainer.BackColor = Color.FromArgb(189, 195, 199);

            // Chart panel
            GroupBox grpChart = new GroupBox();
            grpChart.Text = "📈 SENSOR TRENDS (Today)";
            grpChart.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grpChart.Dock = DockStyle.Fill;
            grpChart.BackColor = Color.White;
            grpChart.Padding = new Padding(10);

            this.chartSensors = new Chart();
            this.chartSensors.Dock = DockStyle.Fill;
            this.chartSensors.BackColor = Color.White;

            grpChart.Controls.Add(this.chartSensors);
            splitContainer.Panel1.Controls.Add(grpChart);

            // History table panel
            GroupBox grpHistory = new GroupBox();
            grpHistory.Text = "📋 HISTORICAL DATA (Last 50 Records)";
            grpHistory.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grpHistory.Dock = DockStyle.Fill;
            grpHistory.BackColor = Color.White;
            grpHistory.Padding = new Padding(10);

            this.dgvHistory = new DataGridView();
            this.dgvHistory.Dock = DockStyle.Fill;
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.BackgroundColor = Color.White;
            this.dgvHistory.BorderStyle = BorderStyle.None;
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Define columns
            this.dgvHistory.Columns.Add("colId", "ID");
            this.dgvHistory.Columns.Add("colTemp", "Temperature (°C)");
            this.dgvHistory.Columns.Add("colHumid", "Humidity (%)");
            this.dgvHistory.Columns.Add("colTime", "Measured At");

            this.dgvHistory.Columns[0].FillWeight = 10;
            this.dgvHistory.Columns[1].FillWeight = 20;
            this.dgvHistory.Columns[2].FillWeight = 20;
            this.dgvHistory.Columns[3].FillWeight = 50;

            grpHistory.Controls.Add(this.dgvHistory);
            splitContainer.Panel2.Controls.Add(grpHistory);

            pnlMain.Controls.Add(splitContainer);
            this.Controls.Add(pnlMain);

            // ===== STATUS BAR =====
            Panel pnlStatusBar = new Panel();
            pnlStatusBar.Dock = DockStyle.Bottom;
            pnlStatusBar.Height = 35;
            pnlStatusBar.BackColor = Color.FromArgb(52, 73, 94);
            pnlStatusBar.Padding = new Padding(10, 7, 10, 7);

            this.lblStatus = new Label();
            this.lblStatus.Text = "Ready";
            this.lblStatus.Font = new Font("Segoe UI", 9);
            this.lblStatus.ForeColor = Color.White;
            this.lblStatus.Dock = DockStyle.Left;
            this.lblStatus.AutoSize = true;

            this.lblLastUpdate = new Label();
            this.lblLastUpdate.Text = "No data yet";
            this.lblLastUpdate.Font = new Font("Segoe UI", 9);
            this.lblLastUpdate.ForeColor = Color.LightGray;
            this.lblLastUpdate.Dock = DockStyle.Right;
            this.lblLastUpdate.AutoSize = true;

            pnlStatusBar.Controls.Add(this.lblStatus);
            pnlStatusBar.Controls.Add(this.lblLastUpdate);
            this.Controls.Add(pnlStatusBar);
        }

        #endregion

        // Control declarations
        private Label lblTemperatureValue;
        private Label lblHumidityValue;
        private Label lblTempMin;
        private Label lblTempMax;
        private Label lblTempAvg;
        private Label lblHumidMin;
        private Label lblHumidMax;
        private Label lblHumidAvg;
        private Label lblTotalReadings;
        private Label lblStatus;
        private Label lblLastUpdate;
        private Button btnRefresh;
        private Button btnTestConnection;
        private Chart chartSensors;
        private DataGridView dgvHistory;
    }
}

