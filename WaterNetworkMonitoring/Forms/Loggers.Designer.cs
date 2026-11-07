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
            mapPanel = new Panel();
            map = new GMap.NET.WindowsForms.GMapControl();
            headerPanel = new Panel();
            titleLabel = new Label();
            resetButton = new Button();
            selectionLabel = new Label();
            attributionLabel = new Label();
            toolbar = new FlowLayoutPanel();
            zoneSelector = new ComboBox();
            zoomInButton = new Button();
            zoomOutButton = new Button();
            labelsCheckBox = new CheckBox();
            mapPanel.SuspendLayout();
            headerPanel.SuspendLayout();
            SuspendLayout();
            // mapPanel
            mapPanel.BackColor = Color.White;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Location = new Point(24, 24);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new Size(1100, 620);
            mapPanel.Controls.Add(map);
            mapPanel.Controls.Add(selectionLabel);
            mapPanel.Controls.Add(attributionLabel);
            mapPanel.Controls.Add(toolbar);
            mapPanel.Controls.Add(headerPanel);
            // map
            map.Name = "map";
            map.Dock = DockStyle.Fill;
            map.MinZoom = 2;
            map.MaxZoom = 19;
            map.CanDragMap = true;
            map.DragButton = MouseButtons.Left;
            map.PolygonsEnabled = true;
            map.MarkersEnabled = true;
            map.ShowCenter = false;
            // headerPanel
            headerPanel.Name = "headerPanel";
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 48;
            headerPanel.Visible = false;
            headerPanel.BackColor = Color.FromArgb(245, 248, 253);
            headerPanel.Controls.Add(titleLabel);

            // titleLabel
            titleLabel.Name = "titleLabel";
            titleLabel.Text = "Loggers  /  Blagoevgrad regions";
            titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.Padding = new Padding(10, 0, 0, 0);
            // resetButton
            resetButton.Name = "resetButton";
            resetButton.Text = "Reset view";
            resetButton.Size = new Size(106, 32);

            resetButton.Click += resetButton_Click;
            // toolbar
            toolbar.Name = "toolbar";
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 48;
            toolbar.Padding = new Padding(10, 5, 10, 5);
            toolbar.BackColor = Color.White;
            toolbar.WrapContents = false;
            toolbar.Controls.Add(zoneSelector);
            toolbar.Controls.Add(zoomOutButton);
            toolbar.Controls.Add(zoomInButton);
            toolbar.Controls.Add(resetButton);
            toolbar.Controls.Add(labelsCheckBox);
            zoneSelector.Name = "zoneSelector";
            zoneSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            zoneSelector.Size = new Size(210, 28);
            zoneSelector.Margin = new Padding(0, 5, 12, 0);
            zoneSelector.SelectedIndexChanged += zoneSelector_SelectedIndexChanged;
            zoomOutButton.Name = "zoomOutButton";
            zoomOutButton.Text = "−";
            zoomOutButton.Size = new Size(36, 32);
            zoomOutButton.Click += zoomOutButton_Click;
            zoomInButton.Name = "zoomInButton";
            zoomInButton.Text = "+";
            zoomInButton.Size = new Size(36, 32);
            zoomInButton.Click += zoomInButton_Click;
            labelsCheckBox.Name = "labelsCheckBox";
            labelsCheckBox.Text = "Show names";
            labelsCheckBox.Checked = true;
            labelsCheckBox.AutoSize = true;
            labelsCheckBox.Margin = new Padding(14, 9, 0, 0);
            labelsCheckBox.CheckedChanged += labelsCheckBox_CheckedChanged;
            // selectionLabel
            selectionLabel.Name = "selectionLabel";
            selectionLabel.Text = "Select a region to see its details.";
            selectionLabel.Dock = DockStyle.Bottom;
            selectionLabel.Height = 44;
            selectionLabel.Visible = false;
            selectionLabel.BackColor = Color.FromArgb(240, 246, 255);
            selectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            selectionLabel.Padding = new Padding(10, 0, 0, 0);
            selectionLabel.ForeColor = Color.FromArgb(25, 65, 110);
            // attributionLabel
            attributionLabel.Name = "attributionLabel";
            attributionLabel.Text = "© OpenStreetMap contributors";
            attributionLabel.Dock = DockStyle.Bottom;
            attributionLabel.Height = 28;
            attributionLabel.TextAlign = ContentAlignment.MiddleRight;
            attributionLabel.Font = new Font("Segoe UI", 8F);
            // Loggers
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 692);
            BackColor = Color.FromArgb(245, 247, 251);
            Controls.Add(mapPanel);
            Name = "Loggers";
            Text = "Loggers";
            Load += Loggers_Load;
            headerPanel.ResumeLayout(false);
            mapPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

        private Panel mapPanel;
        private GMap.NET.WindowsForms.GMapControl map;
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
    }
}


