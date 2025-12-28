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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            pnlMain = new Panel();
            pnlTopControls = new Panel();
            pnlTop = new Panel();
            pnlTempCard = new Panel();
            lblTempTitle = new Label();
            lblTemperatureValue = new Label();
            lblTempWarning = new Label();
            pnlHumidCard = new Panel();
            lblHumidTitle = new Label();
            lblHumidityValue = new Label();
            pnlButtons = new Panel();
            btnRefresh = new Button();
            btnTestConnection = new Button();
            btnMuteSound = new Button();
            lblTotalReadings = new Label();
            grpStats = new GroupBox();
            lblTempStatsTitle = new Label();
            lblTempMin = new Label();
            lblTempMax = new Label();
            lblTempAvg = new Label();
            lblHumidStatsTitle = new Label();
            lblHumidMin = new Label();
            lblHumidMax = new Label();
            lblHumidAvg = new Label();
            splitContainer = new SplitContainer();
            grpChart = new GroupBox();
            chartSensors = new Chart();
            grpHistory = new GroupBox();
            dgvHistory = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            pnlStatusBar = new Panel();
            lblStatus = new Label();
            lblLastUpdate = new Label();
            pnlMain.SuspendLayout();
            pnlTopControls.SuspendLayout();
            pnlTop.SuspendLayout();
            pnlTempCard.SuspendLayout();
            pnlHumidCard.SuspendLayout();
            pnlButtons.SuspendLayout();
            grpStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            grpChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartSensors).BeginInit();
            grpHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            pnlStatusBar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(236, 240, 241);
            pnlMain.Controls.Add(pnlTopControls);
            pnlMain.Controls.Add(splitContainer);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new Padding(18, 10, 18, 19);
            pnlMain.Size = new Size(1225, 975);
            pnlMain.TabIndex = 0;
            // 
            // pnlTopControls
            // 
            pnlTopControls.BackColor = Color.Transparent;
            pnlTopControls.Controls.Add(pnlTop);
            pnlTopControls.Controls.Add(grpStats);
            pnlTopControls.Dock = DockStyle.Top;
            pnlTopControls.Location = new Point(18, 10);
            pnlTopControls.Name = "pnlTopControls";
            pnlTopControls.Size = new Size(1189, 219);
            pnlTopControls.TabIndex = 0;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(pnlTempCard);
            pnlTop.Controls.Add(pnlHumidCard);
            pnlTop.Controls.Add(pnlButtons);
            pnlTop.Controls.Add(lblTotalReadings);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 82);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1189, 137);
            pnlTop.TabIndex = 0;
            // 
            // pnlTempCard
            // 
            pnlTempCard.BackColor = Color.FromArgb(231, 76, 60);
            pnlTempCard.Controls.Add(lblTempTitle);
            pnlTempCard.Controls.Add(lblTemperatureValue);
            pnlTempCard.Controls.Add(lblTempWarning);
            pnlTempCard.Location = new Point(18, 0);
            pnlTempCard.Name = "pnlTempCard";
            pnlTempCard.Padding = new Padding(13, 14, 13, 14);
            pnlTempCard.Size = new Size(256, 141);
            pnlTempCard.TabIndex = 0;
            // 
            // lblTempTitle
            // 
            lblTempTitle.AutoSize = true;
            lblTempTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTempTitle.ForeColor = Color.White;
            lblTempTitle.Location = new Point(13, 14);
            lblTempTitle.Name = "lblTempTitle";
            lblTempTitle.Size = new Size(105, 19);
            lblTempTitle.TabIndex = 0;
            lblTempTitle.Text = "TEMPERATURE";
            // 
            // lblTemperatureValue
            // 
            lblTemperatureValue.AutoSize = true;
            lblTemperatureValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
            lblTemperatureValue.ForeColor = Color.White;
            lblTemperatureValue.Location = new Point(13, 42);
            lblTemperatureValue.Name = "lblTemperatureValue";
            lblTemperatureValue.Size = new Size(114, 65);
            lblTemperatureValue.TabIndex = 1;
            lblTemperatureValue.Text = "--°C";
            // 
            // lblTempWarning
            // 
            lblTempWarning.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblTempWarning.AutoSize = true;
            lblTempWarning.BackColor = Color.Transparent;
            lblTempWarning.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblTempWarning.ForeColor = Color.White;
            lblTempWarning.Location = new Point(13, 113);
            lblTempWarning.Name = "lblTempWarning";
            lblTempWarning.Size = new Size(83, 13);
            lblTempWarning.TabIndex = 2;
            lblTempWarning.Text = "⚠️ HIGH TEMP";
            lblTempWarning.TextAlign = ContentAlignment.MiddleCenter;
            lblTempWarning.Visible = false;
            // 
            // pnlHumidCard
            // 
            pnlHumidCard.BackColor = Color.FromArgb(52, 152, 219);
            pnlHumidCard.Controls.Add(lblHumidTitle);
            pnlHumidCard.Controls.Add(lblHumidityValue);
            pnlHumidCard.Location = new Point(297, 0);
            pnlHumidCard.Name = "pnlHumidCard";
            pnlHumidCard.Padding = new Padding(13, 14, 13, 14);
            pnlHumidCard.Size = new Size(282, 141);
            pnlHumidCard.TabIndex = 1;
            // 
            // lblHumidTitle
            // 
            lblHumidTitle.AutoSize = true;
            lblHumidTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblHumidTitle.ForeColor = Color.White;
            lblHumidTitle.Location = new Point(13, 14);
            lblHumidTitle.Name = "lblHumidTitle";
            lblHumidTitle.Size = new Size(78, 19);
            lblHumidTitle.TabIndex = 0;
            lblHumidTitle.Text = "HUMIDITY";
            // 
            // lblHumidityValue
            // 
            lblHumidityValue.AutoSize = true;
            lblHumidityValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
            lblHumidityValue.ForeColor = Color.White;
            lblHumidityValue.Location = new Point(13, 42);
            lblHumidityValue.Name = "lblHumidityValue";
            lblHumidityValue.Size = new Size(108, 65);
            lblHumidityValue.TabIndex = 1;
            lblHumidityValue.Text = "--%";
            // 
            // pnlButtons
            // 
            pnlButtons.BackColor = Color.White;
            pnlButtons.Controls.Add(btnRefresh);
            pnlButtons.Controls.Add(btnTestConnection);
            pnlButtons.Controls.Add(btnMuteSound);
            pnlButtons.Location = new Point(592, 0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(9);
            pnlButtons.Size = new Size(262, 170);
            pnlButtons.TabIndex = 2;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(9, 9);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(245, 42);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "🔄 Refresh Now";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnTestConnection
            // 
            btnTestConnection.BackColor = Color.FromArgb(149, 165, 166);
            btnTestConnection.Cursor = Cursors.Hand;
            btnTestConnection.FlatAppearance.BorderSize = 0;
            btnTestConnection.FlatStyle = FlatStyle.Flat;
            btnTestConnection.Font = new Font("Segoe UI", 10F);
            btnTestConnection.ForeColor = Color.White;
            btnTestConnection.Location = new Point(9, 61);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(245, 38);
            btnTestConnection.TabIndex = 1;
            btnTestConnection.Text = "🔌 Test Connection";
            btnTestConnection.UseVisualStyleBackColor = false;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // btnMuteSound
            // 
            btnMuteSound.BackColor = Color.FromArgb(46, 204, 113);
            btnMuteSound.Cursor = Cursors.Hand;
            btnMuteSound.FlatAppearance.BorderSize = 0;
            btnMuteSound.FlatStyle = FlatStyle.Flat;
            btnMuteSound.Font = new Font("Segoe UI", 10F);
            btnMuteSound.ForeColor = Color.White;
            btnMuteSound.Location = new Point(9, 109);
            btnMuteSound.Name = "btnMuteSound";
            btnMuteSound.Size = new Size(245, 38);
            btnMuteSound.TabIndex = 2;
            btnMuteSound.Text = "🔊 Sound: ON";
            btnMuteSound.UseVisualStyleBackColor = false;
            // 
            // lblTotalReadings
            // 
            lblTotalReadings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTotalReadings.AutoSize = true;
            lblTotalReadings.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblTotalReadings.ForeColor = Color.FromArgb(127, 140, 141);
            lblTotalReadings.Location = new Point(1094, 5);
            lblTotalReadings.Name = "lblTotalReadings";
            lblTotalReadings.Size = new Size(95, 15);
            lblTotalReadings.TabIndex = 3;
            lblTotalReadings.Text = "0 readings today";
            lblTotalReadings.TextAlign = ContentAlignment.TopRight;
            // 
            // grpStats
            // 
            grpStats.BackColor = Color.White;
            grpStats.Controls.Add(lblTempStatsTitle);
            grpStats.Controls.Add(lblTempMin);
            grpStats.Controls.Add(lblTempMax);
            grpStats.Controls.Add(lblTempAvg);
            grpStats.Controls.Add(lblHumidStatsTitle);
            grpStats.Controls.Add(lblHumidMin);
            grpStats.Controls.Add(lblHumidMax);
            grpStats.Controls.Add(lblHumidAvg);
            grpStats.Dock = DockStyle.Top;
            grpStats.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            grpStats.Location = new Point(0, 0);
            grpStats.Name = "grpStats";
            grpStats.Padding = new Padding(9);
            grpStats.Size = new Size(1189, 82);
            grpStats.TabIndex = 1;
            grpStats.TabStop = false;
            grpStats.Text = "📊 STATISTICS (Today)";
            // 
            // lblTempStatsTitle
            // 
            lblTempStatsTitle.AutoSize = true;
            lblTempStatsTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTempStatsTitle.Location = new Point(18, 28);
            lblTempStatsTitle.Name = "lblTempStatsTitle";
            lblTempStatsTitle.Size = new Size(99, 19);
            lblTempStatsTitle.TabIndex = 0;
            lblTempStatsTitle.Text = "Temperature:";
            // 
            // lblTempMin
            // 
            lblTempMin.AutoSize = true;
            lblTempMin.Font = new Font("Segoe UI", 9F);
            lblTempMin.Location = new Point(18, 52);
            lblTempMin.Name = "lblTempMin";
            lblTempMin.Size = new Size(57, 15);
            lblTempMin.TabIndex = 1;
            lblTempMin.Text = "Min: --°C";
            // 
            // lblTempMax
            // 
            lblTempMax.AutoSize = true;
            lblTempMax.Font = new Font("Segoe UI", 9F);
            lblTempMax.Location = new Point(105, 52);
            lblTempMax.Name = "lblTempMax";
            lblTempMax.Size = new Size(58, 15);
            lblTempMax.TabIndex = 2;
            lblTempMax.Text = "Max: --°C";
            // 
            // lblTempAvg
            // 
            lblTempAvg.AutoSize = true;
            lblTempAvg.Font = new Font("Segoe UI", 9F);
            lblTempAvg.Location = new Point(192, 52);
            lblTempAvg.Name = "lblTempAvg";
            lblTempAvg.Size = new Size(57, 15);
            lblTempAvg.TabIndex = 3;
            lblTempAvg.Text = "Avg: --°C";
            // 
            // lblHumidStatsTitle
            // 
            lblHumidStatsTitle.AutoSize = true;
            lblHumidStatsTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblHumidStatsTitle.Location = new Point(350, 28);
            lblHumidStatsTitle.Name = "lblHumidStatsTitle";
            lblHumidStatsTitle.Size = new Size(75, 19);
            lblHumidStatsTitle.TabIndex = 4;
            lblHumidStatsTitle.Text = "Humidity:";
            // 
            // lblHumidMin
            // 
            lblHumidMin.AutoSize = true;
            lblHumidMin.Font = new Font("Segoe UI", 9F);
            lblHumidMin.Location = new Point(350, 52);
            lblHumidMin.Name = "lblHumidMin";
            lblHumidMin.Size = new Size(54, 15);
            lblHumidMin.TabIndex = 5;
            lblHumidMin.Text = "Min: --%";
            // 
            // lblHumidMax
            // 
            lblHumidMax.AutoSize = true;
            lblHumidMax.Font = new Font("Segoe UI", 9F);
            lblHumidMax.Location = new Point(438, 52);
            lblHumidMax.Name = "lblHumidMax";
            lblHumidMax.Size = new Size(55, 15);
            lblHumidMax.TabIndex = 6;
            lblHumidMax.Text = "Max: --%";
            // 
            // lblHumidAvg
            // 
            lblHumidAvg.AutoSize = true;
            lblHumidAvg.Font = new Font("Segoe UI", 9F);
            lblHumidAvg.Location = new Point(525, 52);
            lblHumidAvg.Name = "lblHumidAvg";
            lblHumidAvg.Size = new Size(54, 15);
            lblHumidAvg.TabIndex = 7;
            lblHumidAvg.Text = "Avg: --%";
            // 
            // splitContainer
            // 
            splitContainer.BackColor = Color.FromArgb(189, 195, 199);
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(18, 10);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(grpChart);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(grpHistory);
            splitContainer.Size = new Size(1189, 946);
            splitContainer.SplitterDistance = 670;
            splitContainer.SplitterWidth = 5;
            splitContainer.TabIndex = 2;
            // 
            // grpChart
            // 
            grpChart.BackColor = Color.White;
            grpChart.Controls.Add(chartSensors);
            grpChart.Dock = DockStyle.Fill;
            grpChart.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            grpChart.Location = new Point(0, 0);
            grpChart.Name = "grpChart";
            grpChart.Padding = new Padding(9);
            grpChart.Size = new Size(1189, 670);
            grpChart.TabIndex = 0;
            grpChart.TabStop = false;
            grpChart.Text = "📈 SENSOR TRENDS (Today)";
            // 
            // chartSensors
            // 
            chartSensors.BackColor = Color.White;
            chartSensors.Dock = DockStyle.Fill;
            chartSensors.Location = new Point(9, 29);
            chartSensors.Name = "chartSensors";
            chartSensors.Size = new Size(1171, 632);
            chartSensors.TabIndex = 0;
            chartSensors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // grpHistory
            // 
            grpHistory.BackColor = Color.White;
            grpHistory.Controls.Add(dgvHistory);
            grpHistory.Dock = DockStyle.Fill;
            grpHistory.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            grpHistory.Location = new Point(0, 0);
            grpHistory.Name = "grpHistory";
            grpHistory.Padding = new Padding(9);
            grpHistory.Size = new Size(1189, 271);
            grpHistory.TabIndex = 0;
            grpHistory.TabStop = false;
            grpHistory.Text = "📋 HISTORICAL DATA (Last 50 Records)";
            // 
            // dgvHistory
            // 
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 245, 245);
            dgvHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4 });
            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.Location = new Point(9, 29);
            dgvHistory.MultiSelect = false;
            dgvHistory.Name = "dgvHistory";
            dgvHistory.ReadOnly = true;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.Size = new Size(1171, 233);
            dgvHistory.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "ID";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Temperature (°C)";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Humidity (%)";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Measured At";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // pnlStatusBar
            // 
            pnlStatusBar.BackColor = Color.FromArgb(52, 73, 94);
            pnlStatusBar.Controls.Add(lblStatus);
            pnlStatusBar.Controls.Add(lblLastUpdate);
            pnlStatusBar.Dock = DockStyle.Bottom;
            pnlStatusBar.Location = new Point(0, 975);
            pnlStatusBar.Name = "pnlStatusBar";
            pnlStatusBar.Padding = new Padding(9, 7, 9, 7);
            pnlStatusBar.Size = new Size(1225, 33);
            pnlStatusBar.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(9, 7);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(39, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Ready";
            // 
            // lblLastUpdate
            // 
            lblLastUpdate.AutoSize = true;
            lblLastUpdate.Dock = DockStyle.Right;
            lblLastUpdate.Font = new Font("Segoe UI", 9F);
            lblLastUpdate.ForeColor = Color.LightGray;
            lblLastUpdate.Location = new Point(1148, 7);
            lblLastUpdate.Name = "lblLastUpdate";
            lblLastUpdate.Size = new Size(68, 15);
            lblLastUpdate.TabIndex = 1;
            lblLastUpdate.Text = "No data yet";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1225, 1008);
            Controls.Add(pnlMain);
            Controls.Add(pnlStatusBar);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "IOT Dashboard - Temperature & Humidity Monitor";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            pnlMain.ResumeLayout(false);
            pnlTopControls.ResumeLayout(false);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlTempCard.ResumeLayout(false);
            pnlTempCard.PerformLayout();
            pnlHumidCard.ResumeLayout(false);
            pnlHumidCard.PerformLayout();
            pnlButtons.ResumeLayout(false);
            grpStats.ResumeLayout(false);
            grpStats.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            grpChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartSensors).EndInit();
            grpHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            pnlStatusBar.ResumeLayout(false);
            pnlStatusBar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        // Control declarations
        private Label lblTemperatureValue;
        private Label lblHumidityValue;
        private Label lblTempWarning; // ✅ NEW: Warning label
        private Panel pnlTempCard; // ✅ Need reference for color changes
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
        private Button btnMuteSound;
        private Chart chartSensors;
        private DataGridView dgvHistory;
        private Panel pnlMain;
        private Panel pnlTopControls;
        private Panel pnlTop;
        private Label lblTempTitle;
        private Panel pnlHumidCard;
        private Label lblHumidTitle;
        private Panel pnlButtons;
        private GroupBox grpStats;
        private Label lblTempStatsTitle;
        private Label lblHumidStatsTitle;
        private SplitContainer splitContainer;
        private GroupBox grpChart;
        private GroupBox grpHistory;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Panel pnlStatusBar;
    }
}

