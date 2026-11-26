namespace WaterNetworkMonitoring
{
    partial class DashboardControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            page = new TableLayoutPanel();
            headerPanel = new TableLayoutPanel();
            titleLabel = new Label();
            periodSelector = new ComboBox();
            refreshButton = new Button();
            mapPanel = new Panel();
            mapToolbar = new FlowLayoutPanel();
            mapTitle = new Label();
            resetMapButton = new Button();
            numbersCheckBox = new CheckBox();
            pipesCheckBox = new CheckBox();
            mapFooter = new TableLayoutPanel();
            mapHint = new Label();
            attributionLabel = new LinkLabel();
            map = new MapControl();
            leakPanel = new Panel();
            leakContent = new TableLayoutPanel();
            leakTextPanel = new TableLayoutPanel();
            leakStatus = new Label();
            leakDetails = new Label();
            leakLocation = new Label();
            lossPanel = new TableLayoutPanel();
            leakRate = new Label();
            leakRateCaption = new Label();
            repairLabel = new Label();
            comparisonHeader = new TableLayoutPanel();
            comparisonTitle = new Label();
            comparisonSelector = new ComboBox();
            graphGrid = new TableLayoutPanel();
            flowChart = new DashboardChart();
            lossChart = new DashboardChart();
            statusLabel = new Label();
            SuspendLayout();
            comparisonHeader.SuspendLayout();
            page.SuspendLayout();
            headerPanel.SuspendLayout();
            mapPanel.SuspendLayout();
            mapToolbar.SuspendLayout();
            mapFooter.SuspendLayout();
            leakPanel.SuspendLayout();
            leakContent.SuspendLayout();
            leakTextPanel.SuspendLayout();
            lossPanel.SuspendLayout();
            graphGrid.SuspendLayout();

            page.Name = "page";
            page.Location = new Point(24, 24);
            page.Size = new Size(952, 798);
            page.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            page.ColumnCount = 1;
            page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            page.RowCount = 7;
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 340));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 230));
            page.Controls.Add(headerPanel, 0, 0);
            page.Controls.Add(statusLabel, 0, 1);
            page.Controls.Add(mapPanel, 0, 2);
            page.Controls.Add(leakPanel, 0, 3);
            page.Controls.Add(repairLabel, 0, 4);
            page.Controls.Add(comparisonHeader, 0, 5);
            page.Controls.Add(graphGrid, 0, 6);

            headerPanel.Name = "headerPanel";
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Margin = Padding.Empty;
            headerPanel.ColumnCount = 3;
            headerPanel.RowCount = 1;
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            headerPanel.Controls.Add(titleLabel, 0, 0);
            headerPanel.Controls.Add(periodSelector, 1, 0);
            headerPanel.Controls.Add(refreshButton, 2, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Text = "Dashboard";
            titleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Margin = Padding.Empty;
            periodSelector.Name = "periodSelector";
            periodSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            periodSelector.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            periodSelector.Margin = new Padding(0, 0, 10, 10);
            periodSelector.Enabled = false;
            periodSelector.SelectedIndexChanged += periodSelector_SelectedIndexChanged;
            refreshButton.Name = "refreshButton";
            refreshButton.Text = "Refresh";
            refreshButton.Size = new Size(78, 26);
            refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            refreshButton.Margin = Padding.Empty;
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;

            mapPanel.Name = "mapPanel";
            mapPanel.Dock = DockStyle.Fill;
            mapPanel.BackColor = Color.White;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Margin = new Padding(0, 0, 0, 14);
            mapPanel.Controls.Add(map);
            mapPanel.Controls.Add(mapFooter);
            mapPanel.Controls.Add(mapToolbar);
            mapToolbar.Name = "mapToolbar";
            mapToolbar.Dock = DockStyle.Top;
            mapToolbar.Height = 36;
            mapToolbar.Padding = new Padding(10, 5, 0, 0);
            mapToolbar.WrapContents = false;
            mapToolbar.Controls.Add(mapTitle);
            mapToolbar.Controls.Add(resetMapButton);
            mapToolbar.Controls.Add(numbersCheckBox);
            mapToolbar.Controls.Add(pipesCheckBox);
            mapTitle.Name = "mapTitle";
            mapTitle.Text = "Zapad";
            mapTitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            mapTitle.AutoSize = true;
            mapTitle.Margin = new Padding(0, 5, 20, 0);
            resetMapButton.Name = "resetMapButton";
            resetMapButton.Text = "Reset";
            resetMapButton.Size = new Size(70, 25);
            resetMapButton.Margin = new Padding(0, 0, 16, 0);
            resetMapButton.UseVisualStyleBackColor = true;
            resetMapButton.Click += resetMapButton_Click;
            numbersCheckBox.Name = "numbersCheckBox";
            numbersCheckBox.Text = "Numbers";
            numbersCheckBox.AutoSize = true;
            numbersCheckBox.Checked = false;
            numbersCheckBox.Margin = new Padding(0, 6, 0, 0);
            numbersCheckBox.CheckedChanged += numbersCheckBox_CheckedChanged;
            pipesCheckBox.Name = "pipesCheckBox";
            pipesCheckBox.Text = "Proposed pipes";
            pipesCheckBox.AutoSize = true;
            pipesCheckBox.Checked = true;
            pipesCheckBox.Margin = new Padding(12, 6, 0, 0);
            pipesCheckBox.CheckedChanged += pipesCheckBox_CheckedChanged;
            mapFooter.Name = "mapFooter";
            mapFooter.Dock = DockStyle.Bottom;
            mapFooter.Height = 26;
            mapFooter.ColumnCount = 2;
            mapFooter.RowCount = 1;
            mapFooter.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mapFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            mapFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            mapFooter.Controls.Add(mapHint, 0, 0);
            mapFooter.Controls.Add(attributionLabel, 1, 0);
            mapHint.Name = "mapHint";
            mapHint.AutoEllipsis = true;
            mapHint.Text = "Zapad buildings";
            mapHint.Dock = DockStyle.Fill;
            mapHint.Font = new Font("Segoe UI", 9F);
            mapHint.ForeColor = Color.FromArgb(99, 110, 122);
            mapHint.TextAlign = ContentAlignment.MiddleLeft;
            mapHint.Margin = new Padding(10, 0, 0, 0);
            attributionLabel.Name = "attributionLabel";
            attributionLabel.Text = "© OpenStreetMap contributors";
            attributionLabel.LinkColor = Color.FromArgb(55, 91, 131);
            attributionLabel.Dock = DockStyle.Fill;
            attributionLabel.Font = new Font("Segoe UI", 8F);
            attributionLabel.TextAlign = ContentAlignment.MiddleRight;
            attributionLabel.Margin = new Padding(0, 0, 8, 0);
            attributionLabel.LinkClicked += attributionLabel_LinkClicked;
            map.Name = "map";
            map.Dock = DockStyle.Fill;
            map.Position = new GMap.NET.PointLatLng(42.0172, 23.0826);
            map.MinZoom = 12;
            map.MaxZoom = 19;
            map.Zoom = 16;
            map.CanDragMap = true;
            map.DragButton = MouseButtons.Left;
            map.ShowCenter = false;
            map.EmptyTileColor = Color.FromArgb(236, 240, 243);
            map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;

            leakPanel.Name = "leakPanel";
            leakPanel.BackColor = Color.White;
            leakPanel.Dock = DockStyle.Fill;
            leakPanel.Padding = new Padding(10, 6, 10, 6);
            leakPanel.Margin = new Padding(0, 0, 0, 10);
            leakPanel.Controls.Add(leakContent);
            leakContent.Name = "leakContent";
            leakContent.Dock = DockStyle.Fill;
            leakContent.ColumnCount = 2;
            leakContent.RowCount = 1;
            leakContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            leakContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
            leakContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            leakContent.Controls.Add(leakTextPanel, 0, 0);
            leakContent.Controls.Add(lossPanel, 1, 0);
            leakTextPanel.Name = "leakTextPanel";
            leakTextPanel.Dock = DockStyle.Fill;
            leakTextPanel.Margin = Padding.Empty;
            leakTextPanel.ColumnCount = 1;
            leakTextPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            leakTextPanel.RowCount = 3;
            leakTextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            leakTextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            leakTextPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            leakTextPanel.Controls.Add(leakStatus, 0, 0);
            leakTextPanel.Controls.Add(leakDetails, 0, 1);
            leakTextPanel.Controls.Add(leakLocation, 0, 2);
            leakStatus.Name = "leakStatus";
            leakStatus.Text = "Waiting for readings";
            leakStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            leakStatus.Dock = DockStyle.Fill;
            leakStatus.Margin = Padding.Empty;
            leakDetails.Name = "leakDetails";
            leakDetails.Text = "";
            leakDetails.Dock = DockStyle.Fill;
            leakDetails.Font = new Font("Segoe UI", 8F);
            leakDetails.ForeColor = Color.FromArgb(85, 95, 108);
            leakDetails.Margin = Padding.Empty;
            leakLocation.Name = "leakLocation";
            leakLocation.Dock = DockStyle.Fill;
            leakLocation.Font = new Font("Segoe UI", 8F);
            leakLocation.ForeColor = Color.FromArgb(85, 95, 108);
            leakLocation.Margin = Padding.Empty;
            lossPanel.Name = "lossPanel";
            lossPanel.Dock = DockStyle.Fill;
            lossPanel.ColumnCount = 1;
            lossPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            lossPanel.RowCount = 2;
            lossPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            lossPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            lossPanel.Controls.Add(leakRate, 0, 0);
            lossPanel.Controls.Add(leakRateCaption, 0, 1);
            leakRate.Name = "leakRate";
            leakRate.Text = "—";
            leakRate.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            leakRate.Dock = DockStyle.Fill;
            leakRate.TextAlign = ContentAlignment.BottomRight;
            leakRate.Margin = Padding.Empty;
            leakRateCaption.Name = "leakRateCaption";
            leakRateCaption.Text = "Difference";
            leakRateCaption.Dock = DockStyle.Fill;
            leakRateCaption.Font = new Font("Segoe UI", 8F);
            leakRateCaption.ForeColor = Color.FromArgb(99, 110, 122);
            leakRateCaption.TextAlign = ContentAlignment.TopRight;
            leakRateCaption.Margin = Padding.Empty;

            repairLabel.Name = "repairLabel";
            repairLabel.Text = "Latest clean-water repair";
            repairLabel.Dock = DockStyle.Fill;
            repairLabel.Font = new Font("Segoe UI", 8F);
            repairLabel.ForeColor = Color.FromArgb(55, 65, 78);
            repairLabel.Margin = new Padding(10, 0, 10, 8);

            comparisonHeader.Name = "comparisonHeader";
            comparisonHeader.Dock = DockStyle.Fill;
            comparisonHeader.Margin = new Padding(4, 0, 4, 0);
            comparisonHeader.ColumnCount = 2;
            comparisonHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));
            comparisonHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            comparisonHeader.RowCount = 1;
            comparisonHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            comparisonHeader.Controls.Add(comparisonTitle, 0, 0);
            comparisonHeader.Controls.Add(comparisonSelector, 1, 0);
            comparisonTitle.Name = "comparisonTitle";
            comparisonTitle.Text = "Area";
            comparisonTitle.Dock = DockStyle.Fill;
            comparisonTitle.TextAlign = ContentAlignment.MiddleLeft;
            comparisonTitle.Margin = Padding.Empty;
            comparisonSelector.Name = "comparisonSelector";
            comparisonSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            comparisonSelector.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comparisonSelector.Margin = new Padding(0, 0, 0, 4);
            comparisonSelector.Enabled = false;
            comparisonSelector.SelectedIndexChanged += comparisonSelector_SelectedIndexChanged;

            graphGrid.Name = "graphGrid";
            graphGrid.Dock = DockStyle.Fill;
            graphGrid.Margin = Padding.Empty;
            graphGrid.ColumnCount = 2;
            graphGrid.RowCount = 1;
            graphGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            graphGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            graphGrid.Controls.Add(flowChart, 0, 0);
            graphGrid.Controls.Add(lossChart, 1, 0);
            flowChart.Name = "flowChart";
            flowChart.Dock = DockStyle.Fill;
            flowChart.Heading = "Supply vs meter use";
            flowChart.Unit = "m³ / interval";
            flowChart.Margin = new Padding(0, 0, 6, 0);
            lossChart.Name = "lossChart";
            lossChart.Dock = DockStyle.Fill;
            lossChart.Heading = "Difference";
            lossChart.Unit = "L/h";
            lossChart.Margin = new Padding(6, 0, 0, 0);


            statusLabel.Name = "statusLabel";
            statusLabel.Text = "Loading readings…";
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Height = 26;
            statusLabel.Font = new Font("Segoe UI", 8F);
            statusLabel.ForeColor = Color.FromArgb(99, 110, 122);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            statusLabel.Margin = Padding.Empty;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.White;
            ForeColor = Color.FromArgb(36, 45, 56);
            Font = new Font("Segoe UI", 9F);
            Size = new Size(1000, 846);
            Name = "DashboardControl";
            Controls.Add(page);
            comparisonHeader.ResumeLayout(false);
            graphGrid.ResumeLayout(false);
            lossPanel.ResumeLayout(false);
            leakTextPanel.ResumeLayout(false);
            leakContent.ResumeLayout(false);
            leakPanel.ResumeLayout(false);
            mapFooter.ResumeLayout(false);
            mapToolbar.ResumeLayout(false);
            mapToolbar.PerformLayout();
            mapPanel.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            page.ResumeLayout(false);
            page.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel page;
        private TableLayoutPanel headerPanel;
        private Label titleLabel;
        private ComboBox periodSelector;
        private Button refreshButton;
        private Panel mapPanel;
        private FlowLayoutPanel mapToolbar;
        private Label mapTitle;
        private Button resetMapButton;
        private CheckBox numbersCheckBox;
        private CheckBox pipesCheckBox;
        private TableLayoutPanel mapFooter;
        private Label mapHint;
        private LinkLabel attributionLabel;
        private GMap.NET.WindowsForms.GMapControl map;
        private Panel leakPanel;
        private TableLayoutPanel leakContent;
        private TableLayoutPanel leakTextPanel;
        private Label leakStatus;
        private Label leakDetails;
        private Label leakLocation;
        private TableLayoutPanel lossPanel;
        private Label leakRate;
        private Label leakRateCaption;
        private Label repairLabel;
        private TableLayoutPanel comparisonHeader;
        private Label comparisonTitle;
        private ComboBox comparisonSelector;
        private TableLayoutPanel graphGrid;
        private DashboardChart flowChart;
        private DashboardChart lossChart;
        private Label statusLabel;
    }
}
