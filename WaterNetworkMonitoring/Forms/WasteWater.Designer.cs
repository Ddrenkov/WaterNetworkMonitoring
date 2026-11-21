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
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            mapPanel = new Panel();
            toolbar = new FlowLayoutPanel();
            titleLabel = new Label();
            resetButton = new Button();
            zoomOutButton = new Button();
            zoomInButton = new Button();
            numbersCheckBox = new CheckBox();
            map = new GMap.NET.WindowsForms.GMapControl();
            footerPanel = new Panel();
            legendLabel = new Label();
            statusLabel = new Label();
            attributionLabel = new LinkLabel();
            mapPanel.SuspendLayout();
            toolbar.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
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
            numbersCheckBox.Name = "numbersCheckBox";
            numbersCheckBox.Text = "Building numbers";
            numbersCheckBox.AutoSize = true;
            numbersCheckBox.Margin = new Padding(14, 8, 0, 0);
            numbersCheckBox.CheckedChanged += numbersCheckBox_CheckedChanged;
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
            legendLabel.Text = "Brown: street pipes    Dashed: building connections";
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
            Controls.Add(mapPanel);
            Load += WasteWater_Load;
            footerPanel.ResumeLayout(false);
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            mapPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        private Panel mapPanel;
        private FlowLayoutPanel toolbar;
        private Label titleLabel;
        private Button zoomOutButton;
        private Button zoomInButton;
        private Button resetButton;
        private CheckBox numbersCheckBox;
        private GMap.NET.WindowsForms.GMapControl map;
        private Panel footerPanel;
        private Label legendLabel;
        private Label statusLabel;
        private LinkLabel attributionLabel;
    }
}
