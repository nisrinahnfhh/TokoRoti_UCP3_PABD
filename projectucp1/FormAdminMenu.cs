using System;
using System.Windows.Forms;

namespace projectucp1
{
    public partial class FormAdminMenu : Form
    {
        private readonly string username;
        private readonly string role;

        public FormAdminMenu(string user, string role)
        {
            InitializeComponent();
            username = user;
            this.role = role;
        }

        private void FormAdminMenu_Load(object sender, EventArgs e)
        {
            LblUser.Text = $"{username} ({role})";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new FormProduk(false, username, role).Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            new FormKasirManagement(username).Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            new FormLaporan(username, role).Show();
            this.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
                new FormLogin().Show();
            }
        }
    }
}