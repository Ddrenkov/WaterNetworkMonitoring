using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring
{
    public partial class AccountManagementForm : Form
    {
        private Account currentAccount = new Account();



        public event EventHandler? CurrentAccountChanged;

        public AccountManagementForm()
        {
            InitializeComponent();
            footerPanel.Resize += (_, _) => CenterFooterButton();
            CenterFooterButton();
            accountsList.ClientSizeChanged += (_, _) => ResizeAccountRows();
        }

        public AccountManagementForm(Account currentAccount) : this()
        {
            this.currentAccount = currentAccount;
        }

        private void AccountManagementForm_Load(object? sender, EventArgs e)
        {
            if (currentAccount.Id != 0) LoadAccounts();
        }

        private void CenterFooterButton()
        {
            addButton.Left = Math.Max(0, (footerPanel.ClientSize.Width - addButton.Width) / 2);
        }

        private void addButton_Click(object? sender, EventArgs e)
        {
            OpenEditor(null);
        }
        private bool IsAdmin()
        {
            using var db = new AppDbContext();
            return db.Accounts.Any(a => a.Id == currentAccount.Id && a.Role == "Admin");
        }

        private void LoadAccounts()
        {
            try
            {
                if (!IsAdmin()) return;
                using var db = new AppDbContext();
                var accounts = db.Accounts.AsNoTracking().OrderBy(a => a.CreatedAt).ThenBy(a => a.Id).ToList();
                accountsList.SuspendLayout();
                try
                {
                    foreach (Control row in accountsList.Controls.Cast<Control>().ToArray()) row.Dispose();
                    foreach (var account in accounts)
                    {
                        var row = new AccountRowControl(account);
                        row.EditRequested += (_, _) => OpenEditor(account.Id);
                        row.DeleteRequested += (_, _) => DeleteAccount(account.Id);
                        accountsList.Controls.Add(row);
                    }
                    ResizeAccountRows();
                }
                finally { accountsList.ResumeLayout(); }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not load accounts. Check your database connection.");
            }
        }

        private void ResizeAccountRows()
        {
            int width = Math.Max(720, accountsList.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
            foreach (Control row in accountsList.Controls) row.Width = width;
        }
        private void DeleteAccount(int id)
        {
            try
            {
                if (!IsAdmin()) return;
                if (id == currentAccount.Id)
                {
                    MessageBox.Show("You cannot delete the account you are currently using.");
                    return;
                }
                using var db = new AppDbContext();
                var account = db.Accounts.Find(id);
                if (account == null) { LoadAccounts(); return; }
                if (account.Role == "Admin" && !db.Accounts.Any(a => a.Id != id && a.Role == "Admin"))
                {
                    MessageBox.Show("At least one Admin account must remain.");
                    return;
                }
                db.Accounts.Remove(account);
                db.SaveChanges();
                LoadAccounts();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not delete the account. Check the database connection and related records.");
            }
        }

        private void OpenEditor(int? id)
        {
            if (!IsAdmin()) return;
            var editor = new RegisterForm(id)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual
            };
            var host = new Panel { Dock = DockStyle.Fill };
            void CenterEditor() => editor.Location = new Point(Math.Max(0, (host.ClientSize.Width - editor.Width) / 2), Math.Max(0, (host.ClientSize.Height - editor.Height) / 2));
            host.Resize += (_, _) => CenterEditor();
            accountsList.Visible = false;
            addButton.Visible = false;
            host.Controls.Add(editor);
            var backPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            var back = new Button { Text = "Back to Accounts", Size = new Size(160, 36), Location = new Point(0, 7) };
            backPanel.Controls.Add(back);
            backPanel.Resize += (_, _) => back.Left = Math.Max(0, (backPanel.ClientSize.Width - back.Width) / 2);
            host.Controls.Add(backPanel);
            back.Click += (_, _) => editor.Close();
            Controls.Add(host);
            host.BringToFront();
            editor.FormClosed += (_, _) =>
            {
                bool saved = editor.Saved;
                BeginInvoke(new Action(() =>
                {
                    host.Dispose();
                    accountsList.Visible = true;
                    addButton.Visible = true;
                    if (saved && id == currentAccount.Id)
                    {
                        using var db = new AppDbContext();
                        var updated = db.Accounts.AsNoTracking().Single(a => a.Id == currentAccount.Id);
                        currentAccount.Username = updated.Username;
                        currentAccount.Email = updated.Email;
                        currentAccount.Role = updated.Role;
                        CurrentAccountChanged?.Invoke(this, EventArgs.Empty);
                    }
                    if (!IsDisposed && currentAccount.Role == "Admin") LoadAccounts();
                }));
            };
            editor.Show();
            CenterEditor();
        }
    }
}





