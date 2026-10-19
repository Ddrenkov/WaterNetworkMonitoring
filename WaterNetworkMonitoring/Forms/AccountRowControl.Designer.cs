#nullable disable
namespace WaterNetworkMonitoring
{
    partial class AccountRowControl
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
        private void InitializeComponent()
        {
            rowLayout = new TableLayoutPanel();
            deleteButton = new Button();
            editButton = new Button();
            usernamePanel = new Panel();
            usernameLabel = new Label();
            usernameTextBox = new TextBox();
            rolePanel = new Panel();
            roleLabel = new Label();
            roleTextBox = new TextBox();
            emailPanel = new Panel();
            emailLabel = new Label();
            emailTextBox = new TextBox();
            createdPanel = new Panel();
            createdLabel = new Label();
            createdTextBox = new TextBox();
            SuspendLayout();
            rowLayout.Dock = DockStyle.Fill;
            rowLayout.ColumnCount = 6;
            rowLayout.RowCount = 1;
            rowLayout.Padding = new Padding(8);
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            deleteButton.Text = "Delete";
            deleteButton.Size = new Size(72, 32);
            deleteButton.Anchor = AnchorStyles.Top;
            deleteButton.Margin = new Padding(3, 24, 3, 3);
            editButton.Text = "Edit";
            editButton.Size = new Size(72, 32);
            editButton.Anchor = AnchorStyles.Top;
            editButton.Margin = new Padding(3, 24, 3, 3);
            rowLayout.Controls.Add(deleteButton, 5, 0);
            rowLayout.Controls.Add(editButton, 4, 0);
            usernamePanel.Dock = DockStyle.Fill;
            usernamePanel.Padding = new Padding(4, 0, 4, 0);
            usernameLabel.Text = "Username";
            usernameLabel.Dock = DockStyle.Top;
            usernameLabel.Height = 24;
            usernameTextBox.Dock = DockStyle.Top;
            usernameTextBox.ReadOnly = true;
            usernameTextBox.Enabled = true;
            usernameTextBox.TabStop = false;
            usernameTextBox.BorderStyle = BorderStyle.FixedSingle;
            usernameTextBox.BackColor = SystemColors.Window;
            usernamePanel.Controls.Add(usernameTextBox);
            usernamePanel.Controls.Add(usernameLabel);
            rowLayout.Controls.Add(usernamePanel, 0, 0);
            rolePanel.Dock = DockStyle.Fill;
            rolePanel.Padding = new Padding(4, 0, 4, 0);
            roleLabel.Text = "Role";
            roleLabel.Dock = DockStyle.Top;
            roleLabel.Height = 24;
            roleTextBox.Dock = DockStyle.Top;
            roleTextBox.ReadOnly = true;
            roleTextBox.Enabled = true;
            roleTextBox.TabStop = false;
            roleTextBox.BorderStyle = BorderStyle.FixedSingle;
            roleTextBox.BackColor = SystemColors.Window;
            rolePanel.Controls.Add(roleTextBox);
            rolePanel.Controls.Add(roleLabel);
            rowLayout.Controls.Add(rolePanel, 1, 0);
            emailPanel.Dock = DockStyle.Fill;
            emailPanel.Padding = new Padding(4, 0, 4, 0);
            emailLabel.Text = "Email";
            emailLabel.Dock = DockStyle.Top;
            emailLabel.Height = 24;
            emailTextBox.Dock = DockStyle.Top;
            emailTextBox.ReadOnly = true;
            emailTextBox.Enabled = true;
            emailTextBox.TabStop = false;
            emailTextBox.BorderStyle = BorderStyle.FixedSingle;
            emailTextBox.BackColor = SystemColors.Window;
            emailPanel.Controls.Add(emailTextBox);
            emailPanel.Controls.Add(emailLabel);
            rowLayout.Controls.Add(emailPanel, 2, 0);
            createdPanel.Dock = DockStyle.Fill;
            createdPanel.Padding = new Padding(4, 0, 4, 0);
            createdLabel.Text = "Created At";
            createdLabel.Dock = DockStyle.Top;
            createdLabel.Height = 24;
            createdTextBox.Dock = DockStyle.Top;
            createdTextBox.ReadOnly = true;
            createdTextBox.Enabled = true;
            createdTextBox.TabStop = false;
            createdTextBox.BorderStyle = BorderStyle.FixedSingle;
            createdTextBox.BackColor = SystemColors.Window;
            createdPanel.Controls.Add(createdTextBox);
            createdPanel.Controls.Add(createdLabel);
            rowLayout.Controls.Add(createdPanel, 3, 0);
            Controls.Add(rowLayout);
            BorderStyle = BorderStyle.None;
            Margin = new Padding(0, 0, 0, 10);
            Size = new Size(930, 86);
            Name = "AccountRowControl";
            ResumeLayout(false);
        }
        private TableLayoutPanel rowLayout;
        private Button deleteButton;
        private Button editButton;
        private Panel usernamePanel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Panel rolePanel;
        private Label roleLabel;
        private TextBox roleTextBox;
        private Panel emailPanel;
        private Label emailLabel;
        private TextBox emailTextBox;
        private Panel createdPanel;
        private Label createdLabel;
        private TextBox createdTextBox;
    }
}


