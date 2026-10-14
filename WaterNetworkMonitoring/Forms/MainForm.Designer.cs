namespace WaterNetworkMonitoring
{
    partial class MainForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            panelSidebar = new Panel();
            btnLogout = new Button();
            btnAccountManagement = new Button();
            btnReports = new Button();
            btnWaterMeters = new Button();
            btnLoggers = new Button();
            btnDashboard = new Button();
            lblWaterNetwork = new Label();
            panelContent = new Panel();
            panelSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.Controls.Add(btnLogout);
            panelSidebar.Controls.Add(btnAccountManagement);
            panelSidebar.Controls.Add(btnReports);
            panelSidebar.Controls.Add(btnWaterMeters);
            panelSidebar.Controls.Add(btnLoggers);
            panelSidebar.Controls.Add(btnDashboard);
            panelSidebar.Controls.Add(lblWaterNetwork);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(220, 450);
            panelSidebar.TabIndex = 1;
            // 
            // btnLogout
            // 
            btnLogout.Dock = DockStyle.Top;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Location = new Point(0, 407);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(220, 50);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnAccountManagement
            // 
            btnAccountManagement.Dock = DockStyle.Top;
            btnAccountManagement.FlatAppearance.BorderSize = 0;
            btnAccountManagement.FlatStyle = FlatStyle.Flat;
            btnAccountManagement.Location = new Point(0, 357);
            btnAccountManagement.Name = "btnAccountManagement";
            btnAccountManagement.Size = new Size(220, 50);
            btnAccountManagement.TabIndex = 1;
            btnAccountManagement.Text = "Account Management";
            btnAccountManagement.UseVisualStyleBackColor = true;
            btnAccountManagement.Click += btnAccountManagement_Click;
            // 
            // btnReports
            // 
            btnReports.Dock = DockStyle.Top;
            btnReports.FlatAppearance.BorderSize = 0;
            btnReports.FlatStyle = FlatStyle.Flat;
            btnReports.Location = new Point(0, 307);
            btnReports.Name = "btnReports";
            btnReports.Size = new Size(220, 50);
            btnReports.TabIndex = 2;
            btnReports.Text = "Reports";
            btnReports.UseVisualStyleBackColor = true;
            // 
            // btnWaterMeters
            // 
            btnWaterMeters.Dock = DockStyle.Top;
            btnWaterMeters.FlatAppearance.BorderSize = 0;
            btnWaterMeters.FlatStyle = FlatStyle.Flat;
            btnWaterMeters.Location = new Point(0, 257);
            btnWaterMeters.Name = "btnWaterMeters";
            btnWaterMeters.Size = new Size(220, 50);
            btnWaterMeters.TabIndex = 3;
            btnWaterMeters.Text = "Water Meters";
            btnWaterMeters.UseVisualStyleBackColor = true;
            // 
            // btnLoggers
            // 
            btnLoggers.Dock = DockStyle.Top;
            btnLoggers.FlatAppearance.BorderSize = 0;
            btnLoggers.FlatStyle = FlatStyle.Flat;
            btnLoggers.Location = new Point(0, 207);
            btnLoggers.Name = "btnLoggers";
            btnLoggers.Size = new Size(220, 50);
            btnLoggers.TabIndex = 4;
            btnLoggers.Text = "Loggers";
            btnLoggers.UseVisualStyleBackColor = true;
            // 
            // btnDashboard
            // 
            btnDashboard.Dock = DockStyle.Top;
            btnDashboard.FlatAppearance.BorderSize = 0;
            btnDashboard.FlatStyle = FlatStyle.Flat;
            btnDashboard.Location = new Point(0, 157);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Size = new Size(220, 50);
            btnDashboard.TabIndex = 5;
            btnDashboard.Text = "Dashboard";
            btnDashboard.UseVisualStyleBackColor = true;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // lblWaterNetwork
            // 
            lblWaterNetwork.Dock = DockStyle.Top;
            lblWaterNetwork.Location = new Point(0, 0);
            lblWaterNetwork.Name = "lblWaterNetwork";
            lblWaterNetwork.Size = new Size(220, 157);
            lblWaterNetwork.TabIndex = 6;
            lblWaterNetwork.Text = "Water Network";
            lblWaterNetwork.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelContent
            // 
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(220, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(580, 450);
            panelContent.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelContent);
            Controls.Add(panelSidebar);
            Name = "MainForm";
            Text = "Water Network Monitoring\n";
            Load += MainForm_Load;
            panelSidebar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnLoggers;
        private System.Windows.Forms.Button btnWaterMeters;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnAccountManagement;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblWaterNetwork;
    }
}