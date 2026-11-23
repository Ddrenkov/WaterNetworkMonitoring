#nullable disable
namespace WaterNetworkMonitoring
{
    partial class WasteWater
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
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            page = new Panel();
            contentPanel = new Panel();
            mapPanel = new Panel();
            toolbar = new FlowLayoutPanel();
            maintenancePanel = new Panel();
            maintenanceTitleLabel = new Label();
            maintenanceDetailsLabel = new Label();
            flowChangeLabel = new Label();
            levelChangeLabel = new Label();
            checkedAtLabel = new Label();
            recentReadingsLabel = new Label();
            maintenanceGrid = new DataGridView();
            titleLabel = new Label();
            resetButton = new Button();
            refreshLoggersButton = new Button();
            zoomOutButton = new Button();
            zoomInButton = new Button();
            numbersCheckBox = new CheckBox();
            map = new MapControl();
            footerPanel = new Panel();
            legendLabel = new Label();
            statusLabel = new Label();
            attributionLabel = new LinkLabel();
            mapPanel.SuspendLayout();
            toolbar.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            page.Name = "page";
            page.Dock = DockStyle.Fill;
            page.AutoScroll = true;
            page.Controls.Add(contentPanel);
            contentPanel.Name = "contentPanel";
            contentPanel.Controls.Add(mapPanel);
            contentPanel.Controls.Add(maintenancePanel);
            mapPanel.Name = "mapPanel";
            mapPanel.BackColor = Color.White;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Controls.Add(map);
            mapPanel.Controls.Add(footerPanel);
            mapPanel.Controls.Add(attributionLabel);
            mapPanel.Controls.Add(toolbar);
            toolbar.Name = "toolbar";
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 42;
            toolbar.Padding = new Padding(10, 5, 10, 0);
            toolbar.WrapContents = false;
            toolbar.Controls.Add(titleLabel);
            toolbar.Controls.Add(zoomOutButton);
            toolbar.Controls.Add(zoomInButton);
            toolbar.Controls.Add(resetButton);
            toolbar.Controls.Add(refreshLoggersButton);
            toolbar.Controls.Add(numbersCheckBox);
            titleLabel.Name = "titleLabel";
            titleLabel.Text = "Waste Water · Zapad";
            titleLabel.AutoSize = true;
            titleLabel.Margin = new Padding(0, 7, 18, 0);
            zoomOutButton.Name = "zoomOutButton";
            zoomOutButton.Text = "−";
            zoomOutButton.Size = new Size(34, 28);
            zoomOutButton.Click += zoomOutButton_Click;
            zoomInButton.Name = "zoomInButton";
            zoomInButton.Text = "+";
            zoomInButton.Size = new Size(34, 28);
            zoomInButton.Click += zoomInButton_Click;
            resetButton.Name = "resetButton";
            resetButton.Text = "Reset view";
            resetButton.Size = new Size(92, 28);
            resetButton.Click += resetButton_Click;
            refreshLoggersButton.Name = "refreshLoggersButton";
            refreshLoggersButton.Text = "Refresh";
            refreshLoggersButton.Size = new Size(78, 28);
            refreshLoggersButton.Click += refreshLoggersButton_Click;
            numbersCheckBox.Name = "numbersCheckBox";
            numbersCheckBox.Text = "Building numbers";
            numbersCheckBox.AutoSize = true;
            numbersCheckBox.Margin = new Padding(14, 8, 0, 0);
            numbersCheckBox.CheckedChanged += numbersCheckBox_CheckedChanged;
            maintenancePanel.Name = "maintenancePanel";
            maintenancePanel.Padding = new Padding(12);
            maintenancePanel.BackColor = Color.White;
            maintenancePanel.BorderStyle = BorderStyle.FixedSingle;
            maintenancePanel.Visible = false;
            maintenancePanel.Controls.Add(maintenanceGrid);
            maintenancePanel.Controls.Add(recentReadingsLabel);
            maintenancePanel.Controls.Add(checkedAtLabel);
            maintenancePanel.Controls.Add(levelChangeLabel);
            maintenancePanel.Controls.Add(flowChangeLabel);
            maintenancePanel.Controls.Add(maintenanceDetailsLabel);
            maintenancePanel.Controls.Add(maintenanceTitleLabel);
            maintenanceTitleLabel.Name = "maintenanceTitleLabel";
            maintenanceTitleLabel.Dock = DockStyle.Top;
            maintenanceTitleLabel.Height = 28;
            maintenanceTitleLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            maintenanceTitleLabel.ForeColor = Color.Firebrick;
            maintenanceDetailsLabel.Name = "maintenanceDetailsLabel";
            maintenanceDetailsLabel.Dock = DockStyle.Top;
            maintenanceDetailsLabel.Height = 24;
            maintenanceDetailsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            flowChangeLabel.Name = "flowChangeLabel";
            flowChangeLabel.Dock = DockStyle.Top;
            flowChangeLabel.Height = 24;
            levelChangeLabel.Name = "levelChangeLabel";
            levelChangeLabel.Dock = DockStyle.Top;
            levelChangeLabel.Height = 24;
            checkedAtLabel.Name = "checkedAtLabel";
            checkedAtLabel.Dock = DockStyle.Top;
            checkedAtLabel.Height = 24;
            checkedAtLabel.Font = new Font("Segoe UI", 8F);
            checkedAtLabel.ForeColor = Color.DimGray;
            recentReadingsLabel.Name = "recentReadingsLabel";
            recentReadingsLabel.Text = "Recent readings";
            recentReadingsLabel.Dock = DockStyle.Top;
            recentReadingsLabel.Height = 24;
            recentReadingsLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            maintenanceGrid.Name = "maintenanceGrid";
            maintenanceGrid.Dock = DockStyle.Fill;
            maintenanceGrid.ReadOnly = true;
            maintenanceGrid.AllowUserToAddRows = false;
            maintenanceGrid.AllowUserToDeleteRows = false;
            maintenanceGrid.AllowUserToResizeRows = false;
            maintenanceGrid.RowHeadersVisible = false;
            maintenanceGrid.MultiSelect = false;
            maintenanceGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            maintenanceGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            maintenanceGrid.ScrollBars = ScrollBars.Vertical;
            maintenanceGrid.BackgroundColor = Color.White;
            maintenanceGrid.BorderStyle = BorderStyle.None;
            maintenanceGrid.GridColor = Color.FromArgb(222, 226, 230);
            maintenanceGrid.EnableHeadersVisualStyles = false;
            maintenanceGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 243, 245);
            maintenanceGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8F);
            maintenanceGrid.ColumnHeadersHeight = 30;
            maintenanceGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            maintenanceGrid.DefaultCellStyle.Font = new Font("Segoe UI", 8.25F);
            maintenanceGrid.RowTemplate.Height = 23;
            maintenanceGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Logger", HeaderText = "Logger", FillWeight = 45, MinimumWidth = 38, Visible = false });
            maintenanceGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Time", HeaderText = "Time", FillWeight = 130, MinimumWidth = 90,
                ValueType = typeof(DateTime), DefaultCellStyle = new DataGridViewCellStyle { Format = "dd MMM HH:mm" } });
            maintenanceGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Flow", HeaderText = "Flow (m³/h)", FillWeight = 85, MinimumWidth = 72,
                ValueType = typeof(decimal), DefaultCellStyle = new DataGridViewCellStyle { Format = "0.000", Alignment = DataGridViewContentAlignment.MiddleRight } });
            maintenanceGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Level", HeaderText = "Level (m)", FillWeight = 75, MinimumWidth = 58,
                ValueType = typeof(decimal), DefaultCellStyle = new DataGridViewCellStyle { Format = "0.000", Alignment = DataGridViewContentAlignment.MiddleRight } });
            maintenanceGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rain", HeaderText = "Rain", FillWeight = 45, MinimumWidth = 38 });
            foreach (DataGridViewColumn column in maintenanceGrid.Columns) column.SortMode = DataGridViewColumnSortMode.NotSortable;
            map.Name = "map";
            map.Dock = DockStyle.Fill;
            map.Position = new GMap.NET.PointLatLng(42.0172, 23.0826);
            map.Zoom = 16;
            map.MinZoom = 14;
            map.MaxZoom = 19;
            map.ShowCenter = false;
            map.CanDragMap = true;
            map.DragButton = MouseButtons.Left;
            map.RoutesEnabled = true;
            map.MarkersEnabled = true;
            map.PolygonsEnabled = false;
            map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            footerPanel.Name = "footerPanel";
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 48;
            footerPanel.Controls.Add(statusLabel);
            footerPanel.Controls.Add(legendLabel);
            legendLabel.Name = "legendLabel";
            legendLabel.Text = "Brown: street pipes    Dashed: building connections    W1–W3: loggers";
            legendLabel.Dock = DockStyle.Top;
            legendLabel.Height = 22;
            legendLabel.Padding = new Padding(10, 3, 0, 0);
            legendLabel.Font = new Font("Segoe UI", 8F);
            statusLabel.Name = "statusLabel";
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Padding = new Padding(10, 2, 0, 0);
            statusLabel.Font = new Font("Segoe UI", 8F);
            statusLabel.AutoEllipsis = true;
            attributionLabel.Name = "attributionLabel";
            attributionLabel.Text = "© OpenStreetMap contributors";
            attributionLabel.Dock = DockStyle.Bottom;
            attributionLabel.Height = 24;
            attributionLabel.TextAlign = ContentAlignment.MiddleRight;
            attributionLabel.Padding = new Padding(0, 0, 8, 0);
            attributionLabel.Font = new Font("Segoe UI", 8F);
            attributionLabel.LinkClicked += attributionLabel_LinkClicked;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F);
            ClientSize = new Size(1248, 808);
            BackColor = Color.FromArgb(245, 247, 251);
            Name = "WasteWater";
            Text = "Waste Water";
            Controls.Add(page);
            Load += WasteWater_Load;
            footerPanel.ResumeLayout(false);
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            mapPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        private Panel page;
        private Panel contentPanel;
        private Panel mapPanel;
        private FlowLayoutPanel toolbar;
        private Panel maintenancePanel;
        private Label maintenanceTitleLabel;
        private Label maintenanceDetailsLabel;
        private Label flowChangeLabel;
        private Label levelChangeLabel;
        private Label checkedAtLabel;
        private Label recentReadingsLabel;
        private DataGridView maintenanceGrid;
        private Label titleLabel;
        private Button zoomOutButton;
        private Button zoomInButton;
        private Button resetButton;
        private Button refreshLoggersButton;
        private CheckBox numbersCheckBox;
        private GMap.NET.WindowsForms.GMapControl map;
        private Panel footerPanel;
        private Label legendLabel;
        private Label statusLabel;
        private LinkLabel attributionLabel;
    }
}
