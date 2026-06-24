using System;
using System.Windows.Forms;

namespace projectucp1
{
    public partial class FormKasirMenu : Form
    {
        private readonly string username;
        private readonly string role;

        public FormKasirMenu(string user, string role)
        {
            InitializeComponent();
            username = user;
            this.role = role;
        }

        private void FormKasirMenu_Load(object sender, EventArgs e)
        {
            LblUser.Text = $"{username} ({role})";
        }

        private void btnTransaksi_Click(object sender, EventArgs e)
        {
            new FormTransaksi(username, role).Show();
            this.Hide();
        }

        private void btnProduk_Click(object sender, EventArgs e)
        {
            new FormProduk(true, username, role).Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            new FormLogin().Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new FormProduk(true, username, role).Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            new FormTransaksi(username, role).Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
                new FormLogin().Show();
            }
        }

        private void FormKasirMenu_FormClosing(object sender, FormClosingEventArgs e)
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