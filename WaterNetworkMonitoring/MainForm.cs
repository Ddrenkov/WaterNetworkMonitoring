using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring
{
    public partial class MainForm : Form
    {
        private readonly Account currentAccount;

        public MainForm(Account account)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            currentAccount = account;

            SetupUserAccess();
        }

        private void SetupUserAccess()
        {
            btnAccountManagement.Visible =
                currentAccount.Role == "Admin";
        }
       
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnDashboard_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
