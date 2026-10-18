#nullable disable
namespace WaterNetworkMonitoring
{
    partial class AccountManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            accountsGrid = new DataGridView();
            editColumn = new DataGridViewButtonColumn();
            deleteColumn = new DataGridViewButtonColumn();
            usernameColumn = new DataGridViewTextBoxColumn();
            emailColumn = new DataGridViewTextBoxColumn();
            roleColumn = new DataGridViewTextBoxColumn();
            createdColumn = new DataGridViewTextBoxColumn();
            addButton = new Button();
            footerPanel = new Panel();
            ((System.ComponentModel.ISupportInitialize)accountsGrid).BeginInit();
            SuspendLayout();
            // accountsGrid
            accountsGrid.AllowUserToAddRows = false;
            accountsGrid.AllowUserToDeleteRows = false;
            accountsGrid.AutoGenerateColumns = false;
            accountsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            accountsGrid.BackgroundColor = SystemColors.Window;
            accountsGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            accountsGrid.Columns.AddRange(new DataGridViewColumn[] { usernameColumn, emailColumn, roleColumn, createdColumn, editColumn, deleteColumn });
            accountsGrid.Dock = DockStyle.Fill;
            accountsGrid.Location = new Point(24, 24);
            accountsGrid.MultiSelect = false;
            accountsGrid.Name = "accountsGrid";
            accountsGrid.ReadOnly = true;
            accountsGrid.RowHeadersVisible = false;
            accountsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            accountsGrid.Size = new Size(952, 507);
            accountsGrid.TabIndex = 0;
            accountsGrid.CellContentClick += AccountAction;
            // columns
            editColumn.Name = "Edit";
            editColumn.HeaderText = "";
            editColumn.Text = "Edit";
            editColumn.UseColumnTextForButtonValue = true;
            editColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            editColumn.Width = 75;
            deleteColumn.Name = "Delete";
            deleteColumn.HeaderText = "";
            deleteColumn.Text = "Delete";
            deleteColumn.UseColumnTextForButtonValue = true;
            deleteColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            deleteColumn.Width = 75;
            usernameColumn.Name = "usernameColumn";
            usernameColumn.HeaderText = "Username";
            usernameColumn.DataPropertyName = "Username";
            emailColumn.Name = "emailColumn";
            emailColumn.HeaderText = "Email";
            emailColumn.DataPropertyName = "Email";
            roleColumn.Name = "roleColumn";
            roleColumn.HeaderText = "Role";
            roleColumn.DataPropertyName = "Role";
            createdColumn.Name = "createdColumn";
            createdColumn.HeaderText = "Created";
            createdColumn.DataPropertyName = "CreatedAt";
            // addButton
            addButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            addButton.Location = new Point(0, 10);
            addButton.Name = "addButton";
            addButton.Size = new Size(160, 36);
            addButton.TabIndex = 1;
            addButton.Text = "Add New Account";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            footerPanel.Name = "footerPanel";
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 56;
            footerPanel.Controls.Add(addButton);
            // AccountManagementForm
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(accountsGrid);
            Controls.Add(footerPanel);
            Name = "AccountManagementForm";
            Padding = new Padding(24);
            Text = "Account Management";
            Load += AccountManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)accountsGrid).EndInit();
            ResumeLayout(false);
        }

        private DataGridView accountsGrid;
        private DataGridViewButtonColumn editColumn;
        private DataGridViewButtonColumn deleteColumn;
        private DataGridViewTextBoxColumn usernameColumn;
        private DataGridViewTextBoxColumn emailColumn;
        private DataGridViewTextBoxColumn roleColumn;
        private DataGridViewTextBoxColumn createdColumn;
        private Button addButton;
        private Panel footerPanel;
    }
}



