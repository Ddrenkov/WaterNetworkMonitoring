#nullable disable
namespace WaterNetworkMonitoring
{
    partial class Loggers
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
                var cancellation = loadCancellation;
                loadCancellation = null;
                cancellation?.Cancel();
                cancellation?.Dispose();
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
            mapPanel = new Panel();
            map = new MapControl();
            selectionLabel = new Label();
            attributionLabel = new Label();
            toolbar = new FlowLayoutPanel();
            zoneSelector = new ComboBox();
            zoomOutButton = new Button();
            zoomInButton = new Button();
            resetButton = new Button();
            labelsCheckBox = new CheckBox();
            headerPanel = new Panel();
            titleLabel = new Label();
            chartsPanel = new TableLayoutPanel();
            chartHeader = new Panel();
            periodSelector = new ComboBox();
            refreshReadingsButton = new Button();
            flowChart = new DashboardChart();
            pressureChart = new DashboardChart();
            mapPanel.SuspendLayout();
            toolbar.SuspendLayout();
            headerPanel.SuspendLayout();
            chartsPanel.SuspendLayout();
            chartHeader.SuspendLayout();
            SuspendLayout();
            // 
            // mapPanel
            // 
            mapPanel.BackColor = Color.White;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Controls.Add(map);
            mapPanel.Controls.Add(selectionLabel);
            mapPanel.Controls.Add(attributionLabel);
            mapPanel.Controls.Add(toolbar);
            mapPanel.Controls.Add(headerPanel);
            mapPanel.Location = new Point(24, 24);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new Size(734, 644);
            mapPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mapPanel.TabIndex = 0;
            // 
            // map
            // 
            map.Bearing = 0F;
            map.CanDragMap = true;
            map.Dock = DockStyle.Fill;
            map.EmptyTileColor = Color.Navy;
            map.GrayScaleMode = false;
            map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            map.LevelsKeepInMemory = 5;
            map.Location = new Point(0, 96);
            map.MarkersEnabled = true;
            map.MaxZoom = 19;
            map.MinZoom = 2;
            map.MouseWheelZoomEnabled = true;
            map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            map.Name = "map";
            map.NegativeMode = false;
            map.PolygonsEnabled = true;
            map.RetryLoadTile = 0;
            map.RoutesEnabled = true;
            map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            map.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            map.ShowTileGridLines = false;
            map.Size = new Size(1098, 450);
            map.TabIndex = 0;
            map.Zoom = 0D;
            // 
            // selectionLabel
            // 
            selectionLabel.BackColor = Color.FromArgb(240, 246, 255);
            selectionLabel.Dock = DockStyle.Bottom;
            selectionLabel.ForeColor = Color.FromArgb(25, 65, 110);
            selectionLabel.Location = new Point(0, 546);
            selectionLabel.Name = "selectionLabel";
            selectionLabel.Padding = new Padding(10, 0, 0, 0);
            selectionLabel.Size = new Size(1098, 44);
            selectionLabel.TabIndex = 1;
            selectionLabel.Text = "Select a region to see its details.";
            selectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            selectionLabel.Visible = false;
            // 
            // attributionLabel
            // 
            attributionLabel.Dock = DockStyle.Bottom;
            attributionLabel.Font = new Font("Segoe UI", 8F);
            attributionLabel.Location = new Point(0, 590);
            attributionLabel.Name = "attributionLabel";
            attributionLabel.Size = new Size(1098, 28);
            attributionLabel.TabIndex = 2;
            attributionLabel.Text = "© OpenStreetMap contributors";
            attributionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // toolbar
            // 
            toolbar.BackColor = Color.White;
            toolbar.Controls.Add(zoneSelector);
            toolbar.Controls.Add(zoomOutButton);
            toolbar.Controls.Add(zoomInButton);
            toolbar.Controls.Add(resetButton);
            toolbar.Controls.Add(labelsCheckBox);
            toolbar.Dock = DockStyle.Top;
            toolbar.Location = new Point(0, 48);
            toolbar.Name = "toolbar";
            toolbar.Padding = new Padding(10, 5, 10, 5);
            toolbar.Size = new Size(1098, 48);
            toolbar.TabIndex = 3;
            toolbar.WrapContents = false;
            // 
            // zoneSelector
            // 
            zoneSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            zoneSelector.Location = new Point(10, 10);
            zoneSelector.Margin = new Padding(0, 5, 12, 0);
            zoneSelector.Name = "zoneSelector";
            zoneSelector.Size = new Size(210, 28);
            zoneSelector.TabIndex = 0;
            zoneSelector.SelectedIndexChanged += zoneSelector_SelectedIndexChanged;
            // 
            // zoomOutButton
            // 
            zoomOutButton.Location = new Point(235, 8);
            zoomOutButton.Name = "zoomOutButton";
            zoomOutButton.Size = new Size(36, 32);
            zoomOutButton.TabIndex = 1;
            zoomOutButton.Text = "−";
            zoomOutButton.Click += zoomOutButton_Click;
            // 
            // zoomInButton
            // 
            zoomInButton.Location = new Point(277, 8);
            zoomInButton.Name = "zoomInButton";
            zoomInButton.Size = new Size(36, 32);
            zoomInButton.TabIndex = 2;
            zoomInButton.Text = "+";
            zoomInButton.Click += zoomInButton_Click;
            // 
            // resetButton
            // 
            resetButton.Location = new Point(319, 8);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(106, 32);
            resetButton.TabIndex = 3;
            resetButton.Text = "Reset view";
            resetButton.Click += resetButton_Click;
            // 
            // labelsCheckBox
            // 
            labelsCheckBox.AutoSize = true;
            labelsCheckBox.Checked = true;
            labelsCheckBox.CheckState = CheckState.Checked;
            labelsCheckBox.Location = new Point(442, 14);
            labelsCheckBox.Margin = new Padding(14, 9, 0, 0);
            labelsCheckBox.Name = "labelsCheckBox";
            labelsCheckBox.Size = new Size(114, 24);
            labelsCheckBox.TabIndex = 4;
            labelsCheckBox.Text = "Show names";
            labelsCheckBox.CheckedChanged += labelsCheckBox_CheckedChanged;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(245, 248, 253);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1098, 48);
            headerPanel.TabIndex = 4;
            headerPanel.Visible = false;
            // 
            // titleLabel
            // 
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            titleLabel.Location = new Point(0, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Padding = new Padding(10, 0, 0, 0);
            titleLabel.Size = new Size(1098, 48);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Loggers  /  Blagoevgrad regions";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chartsPanel
            // 
            chartsPanel.ColumnCount = 1;
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            chartsPanel.Controls.Add(chartHeader, 0, 0);
            chartsPanel.Controls.Add(flowChart, 0, 1);
            chartsPanel.Controls.Add(pressureChart, 0, 2);
            chartsPanel.Location = new Point(774, 24);
            chartsPanel.Name = "chartsPanel";
            chartsPanel.RowCount = 3;
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            chartsPanel.Size = new Size(350, 644);
            chartsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            chartsPanel.TabIndex = 1;
            // 
            // chartHeader
            // 
            chartHeader.Controls.Add(periodSelector);
            chartHeader.Controls.Add(refreshReadingsButton);
            chartHeader.Dock = DockStyle.Fill;
            chartHeader.Location = new Point(0, 0);
            chartHeader.Margin = new Padding(0);
            chartHeader.Name = "chartHeader";
            chartHeader.Size = new Size(350, 36);
            chartHeader.TabIndex = 0;
            // 
            // periodSelector
            // 
            periodSelector.Dock = DockStyle.Fill;
            periodSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            periodSelector.Location = new Point(0, 0);
            periodSelector.Name = "periodSelector";
            periodSelector.Size = new Size(274, 28);
            periodSelector.TabIndex = 0;
            periodSelector.SelectedIndexChanged += periodSelector_SelectedIndexChanged;
            // 
            // refreshReadingsButton
            // 
            refreshReadingsButton.Dock = DockStyle.Right;
            refreshReadingsButton.Location = new Point(274, 0);
            refreshReadingsButton.Name = "refreshReadingsButton";
            refreshReadingsButton.Size = new Size(76, 36);
            refreshReadingsButton.TabIndex = 1;
            refreshReadingsButton.Text = "Refresh";
            refreshReadingsButton.Click += refreshReadingsButton_Click;
            // 
            // flowChart
            // 
            flowChart.BackColor = Color.White;
            flowChart.Dock = DockStyle.Fill;
            flowChart.ForeColor = Color.FromArgb(36, 45, 56);
            flowChart.Heading = "Daily flow";
            flowChart.Location = new Point(0, 36);
            flowChart.Margin = new Padding(0, 0, 0, 10);
            flowChart.Name = "flowChart";
            flowChart.Size = new Size(350, 294);
            flowChart.TabIndex = 1;
            flowChart.Unit = "m³/h";
            // 
            // pressureChart
            // 
            pressureChart.BackColor = Color.White;
            pressureChart.Dock = DockStyle.Fill;
            pressureChart.ForeColor = Color.FromArgb(36, 45, 56);
            pressureChart.Heading = "Daily pressure";
            pressureChart.Location = new Point(0, 340);
            pressureChart.Margin = new Padding(0);
            pressureChart.Name = "pressureChart";
            pressureChart.Size = new Size(350, 304);
            pressureChart.StartAtZero = false;
            pressureChart.TabIndex = 2;
            pressureChart.Unit = "bar";
            // 
            // Loggers
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1148, 692);
            Controls.Add(mapPanel);
            Controls.Add(chartsPanel);
            Name = "Loggers";
            Text = "Loggers";
            Load += Loggers_Load;
            mapPanel.ResumeLayout(false);
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            headerPanel.ResumeLayout(false);
            chartsPanel.ResumeLayout(false);
            chartHeader.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

        private Panel mapPanel;
        private Panel headerPanel;
        private Label titleLabel;
        private Button resetButton;
        private Label selectionLabel;
        private Label attributionLabel;
        private FlowLayoutPanel toolbar;
        private ComboBox zoneSelector;
        private Button zoomInButton;
        private Button zoomOutButton;
        private CheckBox labelsCheckBox;
        private TableLayoutPanel chartsPanel;
        private Panel chartHeader;
        private ComboBox periodSelector;
        private Button refreshReadingsButton;
        private DashboardChart flowChart;
        private DashboardChart pressureChart;
        private MapControl map;
    }
}


