#nullable disable
namespace WaterNetworkMonitoring
{
    partial class WaterMeters
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                loadCancellation?.Cancel();
                loadCancellation?.Dispose();
                loadCancellation = null;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            page = new TableLayoutPanel();
            consumptionToolbar = new FlowLayoutPanel();
            consumptionTitle = new Label();
            reloadConsumptionButton = new Button();
            consumptionStatus = new Label();
            consumptionPanel = new TableLayoutPanel();
            consumptionGrid = new DataGridView();
            buildingColumn = new DataGridViewTextBoxColumn();
            meterIdColumn = new DataGridViewTextBoxColumn();
            monthlyUseColumn = new DataGridViewTextBoxColumn();
            consumptionChart = new DashboardChart();
            consumptionNote = new Label();
            mapPanel = new Panel();
            map = new MapControl();
            toolbar = new FlowLayoutPanel();
            zoneSelector = new ComboBox();
            zoomOutButton = new Button();
            zoomInButton = new Button();
            resetButton = new Button();
            numberLabel = new Label();
            buildingNumber = new NumericUpDown();
            findButton = new Button();
            statusLabel = new Label();
            attributionLabel = new Label();
            refreshTimer = new System.Windows.Forms.Timer(components);
            page.SuspendLayout();
            consumptionToolbar.SuspendLayout();
            consumptionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)consumptionGrid).BeginInit();
            mapPanel.SuspendLayout();
            toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)buildingNumber).BeginInit();
            SuspendLayout();
            // page
            page.Name = "page";
            page.Location = new Point(24, 24);
            page.Size = new Size(935, 742);
            page.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            page.Margin = Padding.Empty;
            page.ColumnCount = 1;
            page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            page.RowCount = 4;
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 360));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 310));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            page.Controls.Add(mapPanel, 0, 0);
            page.Controls.Add(consumptionToolbar, 0, 1);
            page.Controls.Add(consumptionPanel, 0, 2);
            page.Controls.Add(consumptionNote, 0, 3);
            consumptionToolbar.Name = "consumptionToolbar";
            consumptionToolbar.Dock = DockStyle.Fill;
            consumptionToolbar.Margin = Padding.Empty;
            consumptionToolbar.WrapContents = false;
            consumptionToolbar.AutoScroll = true;
            consumptionToolbar.Controls.Add(consumptionTitle);
            consumptionToolbar.Controls.Add(reloadConsumptionButton);
            consumptionToolbar.Controls.Add(consumptionStatus);
            consumptionTitle.Name = "consumptionTitle";
            consumptionTitle.Text = "Monthly consumption (L)";
            consumptionTitle.AutoSize = true;
            consumptionTitle.Margin = new Padding(0, 8, 16, 0);
            reloadConsumptionButton.Name = "reloadConsumptionButton";
            reloadConsumptionButton.Text = "Refresh";
            reloadConsumptionButton.Size = new Size(80, 28);
            reloadConsumptionButton.Click += reloadConsumptionButton_Click;
            consumptionStatus.Name = "consumptionStatus";
            consumptionStatus.AutoSize = true;
            consumptionStatus.Margin = new Padding(12, 8, 0, 0);
            consumptionStatus.ForeColor = Color.FromArgb(85, 95, 108);
            consumptionPanel.Name = "consumptionPanel";
            consumptionPanel.Dock = DockStyle.Fill;
            consumptionPanel.Margin = Padding.Empty;
            consumptionPanel.ColumnCount = 2;
            consumptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            consumptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            consumptionPanel.RowCount = 1;
            consumptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            consumptionPanel.Controls.Add(consumptionGrid, 0, 0);
            consumptionPanel.Controls.Add(consumptionChart, 1, 0);
            buildingColumn.Name = "Building";
            buildingColumn.HeaderText = "Building";
            buildingColumn.ValueType = typeof(int);
            buildingColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            buildingColumn.Width = 74;
            meterIdColumn.Name = "MeterId";
            meterIdColumn.HeaderText = "Meter ID";
            meterIdColumn.ValueType = typeof(int);
            meterIdColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            meterIdColumn.Width = 94;
            monthlyUseColumn.Name = "MonthlyUse";
            monthlyUseColumn.HeaderText = "Consumption (L)";
            monthlyUseColumn.ValueType = typeof(decimal);
            monthlyUseColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            monthlyUseColumn.MinimumWidth = 112;
            monthlyUseColumn.DefaultCellStyle.Format = "N0";
            monthlyUseColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            consumptionGrid.Name = "consumptionGrid";
            consumptionGrid.Columns.AddRange(new DataGridViewColumn[] { buildingColumn, meterIdColumn, monthlyUseColumn });
            consumptionGrid.Dock = DockStyle.Fill;
            consumptionGrid.ReadOnly = true;
            consumptionGrid.AllowUserToAddRows = false;
            consumptionGrid.AllowUserToDeleteRows = false;
            consumptionGrid.AllowUserToResizeRows = false;
            consumptionGrid.MultiSelect = false;
            consumptionGrid.RowHeadersVisible = false;
            consumptionGrid.AutoGenerateColumns = false;
            consumptionGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            consumptionGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            consumptionGrid.BackgroundColor = Color.White;
            consumptionGrid.BorderStyle = BorderStyle.FixedSingle;
            consumptionGrid.GridColor = Color.FromArgb(225, 228, 232);
            consumptionGrid.EnableHeadersVisualStyles = false;
            consumptionGrid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            consumptionGrid.ColumnHeadersHeight = 30;
            consumptionGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            consumptionGrid.RowTemplate.Height = 26;
            consumptionGrid.Margin = new Padding(0, 0, 10, 0);
            consumptionGrid.CurrentCellChanged += consumptionGrid_CurrentCellChanged;
            consumptionGrid.CellFormatting += consumptionGrid_CellFormatting;
            consumptionChart.Name = "consumptionChart";
            consumptionChart.Dock = DockStyle.Fill;
            consumptionChart.Margin = Padding.Empty;
            consumptionChart.Heading = "Select a meter";
            consumptionChart.Unit = "L/month";
            consumptionChart.NumberFormat = "N0";
            consumptionChart.DateFormat = "MMM yyyy";
            consumptionNote.Name = "consumptionNote";
            consumptionNote.Dock = DockStyle.Fill;
            consumptionNote.Font = new Font("Segoe UI", 8F);
            consumptionNote.ForeColor = Color.FromArgb(85, 95, 108);
            consumptionNote.Margin = new Padding(0, 6, 0, 0);
            consumptionNote.Text = "≈ Estimated from readings around midnight. — Not enough readings. Select a row or building to see its chart.";
            // mapPanel
            mapPanel.BackColor = Color.White;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Location = new Point(0, 0);
            mapPanel.Name = "mapPanel";
            mapPanel.Dock = DockStyle.Fill;
            mapPanel.Margin = new Padding(0, 0, 0, 12);
            mapPanel.Size = new Size(935, 348);
            mapPanel.Controls.Add(map);
            mapPanel.Controls.Add(statusLabel);
            mapPanel.Controls.Add(attributionLabel);
            mapPanel.Controls.Add(toolbar);
            // map
            map.Name = "map";
            map.Dock = DockStyle.Fill;
            map.MinZoom = 12;
            map.MaxZoom = 19;
            map.CanDragMap = true;
            map.DragButton = MouseButtons.Left;
            map.MarkersEnabled = true;
            map.PolygonsEnabled = false;
            map.ShowCenter = false;
            // toolbar
            toolbar.Name = "toolbar";
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 48;
            toolbar.Padding = new Padding(10, 5, 10, 5);
            toolbar.BackColor = Color.White;
            toolbar.WrapContents = false;
            toolbar.AutoScroll = true;
            toolbar.Controls.Add(zoneSelector);
            toolbar.Controls.Add(zoomOutButton);
            toolbar.Controls.Add(zoomInButton);
            toolbar.Controls.Add(resetButton);
            toolbar.Controls.Add(numberLabel);
            toolbar.Controls.Add(buildingNumber);
            toolbar.Controls.Add(findButton);
            // zoneSelector
            zoneSelector.Name = "zoneSelector";
            zoneSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            zoneSelector.Size = new Size(210, 28);
            zoneSelector.Margin = new Padding(0, 5, 12, 0);
            zoneSelector.SelectedIndexChanged += zoneSelector_SelectedIndexChanged;
            // zoomOutButton
            zoomOutButton.Name = "zoomOutButton";
            zoomOutButton.Text = "−";
            zoomOutButton.Size = new Size(36, 32);
            zoomOutButton.Click += zoomOutButton_Click;
            // zoomInButton
            zoomInButton.Name = "zoomInButton";
            zoomInButton.Text = "+";
            zoomInButton.Size = new Size(36, 32);
            zoomInButton.Click += zoomInButton_Click;
            // resetButton
            resetButton.Name = "resetButton";
            resetButton.Text = "Reset view";
            resetButton.Size = new Size(106, 32);
            resetButton.Click += resetButton_Click;
            // numberLabel
            numberLabel.Name = "numberLabel";
            numberLabel.Text = "Building #";
            numberLabel.AutoSize = true;
            numberLabel.Margin = new Padding(14, 9, 4, 0);
            // buildingNumber
            buildingNumber.Name = "buildingNumber";
            buildingNumber.Minimum = 1;
            buildingNumber.Maximum = 1;
            buildingNumber.Value = 1;
            buildingNumber.Size = new Size(80, 28);
            buildingNumber.Margin = new Padding(0, 5, 6, 0);
            // findButton
            findButton.Name = "findButton";
            findButton.Text = "Go";
            findButton.Size = new Size(48, 32);
            findButton.Click += findButton_Click;
            // statusLabel
            statusLabel.Name = "statusLabel";
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 32;
            statusLabel.BackColor = Color.FromArgb(240, 246, 255);
            statusLabel.ForeColor = Color.FromArgb(25, 65, 110);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            statusLabel.Padding = new Padding(10, 0, 0, 0);
            statusLabel.AutoEllipsis = true;
            // attributionLabel
            attributionLabel.Name = "attributionLabel";
            attributionLabel.Text = "© OpenStreetMap contributors";
            attributionLabel.Dock = DockStyle.Bottom;
            attributionLabel.Height = 28;
            attributionLabel.TextAlign = ContentAlignment.MiddleRight;
            attributionLabel.Font = new Font("Segoe UI", 8F);
            // refreshTimer
            refreshTimer.Interval = 120;
            // WaterMeters
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 800);
            BackColor = Color.FromArgb(245, 247, 251);
            Font = new Font("Segoe UI", 9F);
            AutoScroll = true;
            Controls.Add(page);
            Name = "WaterMeters";
            Text = "Water Meters";
            Load += WaterMeters_Load;
            ((System.ComponentModel.ISupportInitialize)consumptionGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)buildingNumber).EndInit();
            consumptionPanel.ResumeLayout(false);
            consumptionToolbar.ResumeLayout(false);
            consumptionToolbar.PerformLayout();
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            mapPanel.ResumeLayout(false);
            page.ResumeLayout(false);
            page.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private TableLayoutPanel page;
        private FlowLayoutPanel consumptionToolbar;
        private Label consumptionTitle;
        private Button reloadConsumptionButton;
        private Label consumptionStatus;
        private TableLayoutPanel consumptionPanel;
        private DataGridView consumptionGrid;
        private DataGridViewTextBoxColumn buildingColumn;
        private DataGridViewTextBoxColumn meterIdColumn;
        private DataGridViewTextBoxColumn monthlyUseColumn;
        private DashboardChart consumptionChart;
        private Label consumptionNote;
        private Panel mapPanel;
        private GMap.NET.WindowsForms.GMapControl map;
        private FlowLayoutPanel toolbar;
        private ComboBox zoneSelector;
        private Button zoomOutButton;
        private Button zoomInButton;
        private Button resetButton;
        private Label numberLabel;
        private NumericUpDown buildingNumber;
        private Button findButton;
        private Label statusLabel;
        private Label attributionLabel;
        private System.Windows.Forms.Timer refreshTimer;
    }
}
