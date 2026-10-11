using WaterNetworkMonitoring.Data;

namespace WaterNetworkMonitoring
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }

            using AppDbContext db = new AppDbContext();

            var account = db.Accounts
                .FirstOrDefault(a => a.Username == username);

            if (account == null)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }

            bool correctPassword =
                BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);

            if (!correctPassword)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }

            using MainForm mainForm = new MainForm(account);

            this.Hide();

            try
            {
                mainForm.ShowDialog();
            }
            finally
            {
                this.Show();
            }
        }

    }

}
    
