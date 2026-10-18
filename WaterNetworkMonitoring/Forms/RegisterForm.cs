using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring
{
    public partial class RegisterForm : Form
    {
        private readonly int? accountId;
        public bool Saved { get; private set; }

        public RegisterForm() : this(null) { }

        public RegisterForm(int? accountId)
        {
            InitializeComponent();
            btnRegister.Left = (ClientSize.Width - btnRegister.Width) / 2;
            Resize += (_, _) => btnRegister.Left = Math.Max(0, (ClientSize.Width - btnRegister.Width) / 2);
            this.accountId = accountId;
            cmbRole.SelectedItem = "User";
            if (accountId.HasValue)
            {
                using var db = new AppDbContext();
                var account = db.Accounts.Single(a => a.Id == accountId.Value);
                txtUsername.Text = account.Username;
                txtEmail.Text = account.Email;
                cmbRole.SelectedItem = account.Role;
                btnRegister.Text = "Save";
                Text = "Edit Account";
                var hint = new Label
                {
                    Text = "Leave passwords empty to keep the current password.",
                    AutoSize = true,
                    Location = new Point(90, 20)
                };
                Controls.Add(hint);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                (!accountId.HasValue && string.IsNullOrWhiteSpace(password)))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }
            if (password != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }
            string? role = cmbRole.SelectedItem as string;
            if (role != "User" && role != "Admin")
            {
                MessageBox.Show("Please select User or Admin.");
                return;
            }
            try
            {
                using var db = new AppDbContext();
                if (db.Accounts.Any(a => a.Id != (accountId ?? 0) && (a.Username == username || a.Email == email)))
                {
                    MessageBox.Show("Username or email already exists.");
                    return;
                }
                var account = accountId.HasValue ? db.Accounts.Find(accountId.Value) : new Account();
                if (account == null)
                {
                    MessageBox.Show("This account no longer exists.");
                    return;
                }
                if (account.Role == "Admin" && role != "Admin" && accountId.HasValue &&
                    !db.Accounts.Any(a => a.Id != account.Id && a.Role == "Admin"))
                {
                    MessageBox.Show("At least one Admin account must remain.");
                    return;
                }
                account.Username = username;
                account.Email = email;
                account.Role = role;
                if (!string.IsNullOrEmpty(password))
                    account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                if (!accountId.HasValue) db.Accounts.Add(account);
                db.SaveChanges();
                Saved = true;
                Close();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Could not save the account. Check the connection and that username/email are unique.");
            }
        }
    }
}

