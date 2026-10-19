using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring
{
    public partial class AccountRowControl : UserControl
    {
        public int AccountId { get; private set; }
        public event EventHandler? EditRequested;
        public event EventHandler? DeleteRequested;

        public AccountRowControl()
        {
            InitializeComponent();
            editButton.Click += (_, _) => EditRequested?.Invoke(this, EventArgs.Empty);
            deleteButton.Click += (_, _) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        public AccountRowControl(Account account) : this()
        {
            AccountId = account.Id;
            usernameTextBox.Text = account.Username;
            roleTextBox.Text = account.Role;
            emailTextBox.Text = account.Email;
            createdTextBox.Text = account.CreatedAt.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
