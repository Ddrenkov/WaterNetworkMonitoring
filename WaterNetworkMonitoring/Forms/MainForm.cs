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
            panelContent.Resize += panelContent_Resize;

            currentAccount = account;

            SetupUserAccess();
            btnDashboard_Click(this, EventArgs.Empty);
        }

        private void SetupUserAccess()
        {
            btnAccountManagement.Visible =
                currentAccount.Role == "Admin";
        }
       
        private void btnAccountManagement_Click(object sender, EventArgs e)
        {
            if (currentAccount.Role != "Admin")
            {
                return;
            }

            foreach (Control content in panelContent.Controls.Cast<Control>().ToArray())
            {
                content.Dispose();
            }

            RegisterForm registerForm = new RegisterForm
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.None,
                StartPosition = FormStartPosition.Manual
            };

            panelContent.Controls.Add(registerForm);
            registerForm.Show();
            CenterContent();
        }
        private void panelContent_Resize(object? sender, EventArgs e)
        {
            CenterContent();
        }

        private void CenterContent()
        {
            foreach (Control content in panelContent.Controls)
            {
                if (content.Dock != DockStyle.None)
                {
                    continue;
                }

                content.Location = new Point(
                    Math.Max(0, (panelContent.ClientSize.Width - content.Width) / 2),
                    Math.Max(0, (panelContent.ClientSize.Height - content.Height) / 2));
            }
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            foreach (Control content in panelContent.Controls.Cast<Control>().ToArray())
            {
                content.Dispose();
            }

            DashboardControl dashboard = new DashboardControl
            {
                Dock = DockStyle.Fill
            };

            panelContent.Controls.Add(dashboard);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
