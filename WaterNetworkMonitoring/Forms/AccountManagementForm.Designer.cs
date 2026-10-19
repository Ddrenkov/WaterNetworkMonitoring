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
            accountsList = new FlowLayoutPanel();
            footerPanel = new Panel();
            addButton = new Button();
            accountRowPreview = new AccountRowControl();
            accountsList.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            accountsList.Name = "accountsList";
            accountsList.Dock = DockStyle.Fill;
            accountsList.AutoScroll = true;
            accountsList.FlowDirection = FlowDirection.TopDown;
            accountsList.WrapContents = false;
            accountsList.BackColor = SystemColors.Window;
            accountsList.Controls.Add(accountRowPreview);
            accountRowPreview.Name = "accountRowPreview";
            footerPanel.Name = "footerPanel";
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 56;
            footerPanel.Controls.Add(addButton);
            addButton.Name = "addButton";
            addButton.Text = "Add New Account";
            addButton.Size = new Size(160, 36);
            addButton.Location = new Point(396, 10);
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.AddRange(new Control[] { accountsList, footerPanel });
            Padding = new Padding(24);
            Name = "AccountManagementForm";
            Text = "Account Management";
            Load += AccountManagementForm_Load;
            footerPanel.ResumeLayout(false);
            accountsList.ResumeLayout(false);
            ResumeLayout(false);
        }
        private FlowLayoutPanel accountsList;
        private Panel footerPanel;
        private Button addButton;
        private AccountRowControl accountRowPreview;
    }
}
