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
            if (disposing) components?.Dispose();
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
            mapPanel = new Panel();
            map = new GMap.NET.WindowsForms.GMapControl();
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
            mapPanel.SuspendLayout();
            toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)buildingNumber).BeginInit();
            SuspendLayout();
            // mapPanel
            mapPanel.BackColor = Color.White;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Location = new Point(24, 36);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new Size(1100, 620);
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
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 692);
            BackColor = Color.FromArgb(245, 247, 251);
            Controls.Add(mapPanel);
            Name = "WaterMeters";
            Text = "Water Meters";
            Load += WaterMeters_Load;
            ((System.ComponentModel.ISupportInitialize)buildingNumber).EndInit();
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            mapPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

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
