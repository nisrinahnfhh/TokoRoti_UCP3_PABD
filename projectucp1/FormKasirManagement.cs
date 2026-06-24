using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace projectucp1
{
    public partial class FormKasirManagement : Form
    {
        private readonly string con = DAL.GetConnectionString();
        private readonly string adminUsername;
        private BindingSource bindingSource;
        private bool updatingSelection = false;
        private System.Threading.CancellationTokenSource loadCts;
        private bool isFirstLoad = true;

        public FormKasirManagement(string admin)
        {
            InitializeComponent();
            adminUsername = admin;
            bindingSource = new BindingSource();
        }

        private async void FormKasirManagement_Load(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            loadCts?.Cancel(); loadCts?.Dispose();
            loadCts = new System.Threading.CancellationTokenSource();
            try
            {
                await LoadKasirAsync(loadCts.Token);
            }
            catch (OperationCanceledException) { }

            this.FormClosed -= FormKasirManagement_FormClosed;
            this.FormClosed += FormKasirManagement_FormClosed;
        }

        private void SetupBinding()
        {
            // Reset Data Source komponen UI
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            // Bersihkan dan pasang ulang DataBindings txtUsername
            txtUsername.DataBindings.Clear();
            var usernameBinding = new Binding("Text", bindingSource, "username", true, DataSourceUpdateMode.OnPropertyChanged);
            usernameBinding.NullValue = string.Empty;
            usernameBinding.BindingComplete += (s, ev) =>
            {
                if (ev.Exception != null) ev.Cancel = true;
            };
            txtUsername.DataBindings.Add(usernameBinding);

            // Pasang ulang event untuk sinkronisasi seleksi baris Grid
            bindingSource.PositionChanged -= BindingSource_PositionChanged;
            bindingSource.PositionChanged += BindingSource_PositionChanged;

            // Trigger pembaruan seleksi pertama kali
            UpdateSelection();
        }

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            if (bindingSource == null || bindingSource.Position < 0)
            {
                dataGridView1.ClearSelection();
                return;
            }

            if (updatingSelection) return;

            try
            {
                updatingSelection = true;
                int pos = bindingSource.Position;
                if (dataGridView1.Rows.Count > 0 && pos >= 0 && pos < dataGridView1.Rows.Count)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[pos].Selected = true;

                    if (dataGridView1.Rows[pos].Cells.Count > 0 &&
                        (dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.RowIndex != pos))
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (dataGridView1.Rows.Count > pos && dataGridView1.Rows[pos].Cells.Count > 0)
                            {
                                dataGridView1.CurrentCell = dataGridView1.Rows[pos].Cells[0];
                            }
                        }));
                    }
                }
                else
                {
                    dataGridView1.ClearSelection();
                }
            }
            catch { }
            finally
            {
                updatingSelection = false;
            }
        }

        private async System.Threading.Tasks.Task LoadKasirAsync(System.Threading.CancellationToken ct)
        {
            try
            {
                int prevPos = bindingSource != null ? bindingSource.Position : -1;

                DataTable dt = await System.Threading.Tasks.Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    using (SqlConnection conn = new SqlConnection(con))
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT loginID, username, role FROM dbo.vw_kasir", conn))
                    {
                        DataTable local = new DataTable();
                        da.Fill(local);
                        return local;
                    }
                }, ct);

                ct.ThrowIfCancellationRequested();
                bindingSource.DataSource = dt;

                if (isFirstLoad)
                {
                    SetupBinding();
                    isFirstLoad = false;
                }
                else
                {
                    bindingSource.ResetBindings(false);
                }

                if (dt.Rows.Count > 0)
                {
                    if (prevPos >= 0 && prevPos < bindingSource.Count)
                        bindingSource.Position = prevPos;
                    else
                        bindingSource.Position = 0;

                    UpdateSelection();
                }
                else
                {
                    dataGridView1.ClearSelection();
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username tidak boleh kosong", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password tidak boleh kosong", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Username minimal 3 karakter", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtPassword.Text.Length < 3)
            {
                MessageBox.Show("Password minimal 3 karakter", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            // JANGAN hapus DataBindings di sini agar relasi textbox dan grid tidak putus.
            // Cukup bersihkan isinya atau kosongkan password saja.
            txtPassword.Clear();
            if (bindingSource == null || bindingSource.Count == 0)
            {
                txtUsername.Clear();
                dataGridView1.ClearSelection();
            }
        }

        private async void btnTambah_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(con))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_InsertKasir", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50) { Value = txtUsername.Text });
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 50) { Value = txtPassword.Text });
                    cmd.Parameters.Add(new SqlParameter("@outLoginID", SqlDbType.Int) { Direction = ParameterDirection.Output });

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kasir berhasil ditambah", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await TriggerRefreshAsync();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null)
            {
                MessageBox.Show("Pilih kasir yang akan diupdate", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
                return;

            try
            {
                DataRowView rowView = (DataRowView)bindingSource.Current;
                int id = (int)rowView["loginID"];

                using (SqlConnection conn = new SqlConnection(con))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_UpdateKasir", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@loginID", SqlDbType.Int) { Value = id });
                    cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50) { Value = txtUsername.Text });
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 50) { Value = txtPassword.Text });
                    cmd.Parameters.Add(new SqlParameter("@outRows", SqlDbType.Int) { Direction = ParameterDirection.Output });

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kasir berhasil diupdate", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await TriggerRefreshAsync();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnHapus_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null)
            {
                MessageBox.Show("Pilih kasir yang akan dihapus", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Yakin ingin menghapus kasir ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            bindingSource.PositionChanged -= BindingSource_PositionChanged;

            try
            {
                DataRowView rowView = (DataRowView)bindingSource.Current;
                int id = (int)rowView["loginID"];

                using (SqlConnection conn = new SqlConnection(con))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_DeleteKasir", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@loginID", SqlDbType.Int) { Value = id });
                    cmd.Parameters.Add(new SqlParameter("@outRows", SqlDbType.Int) { Direction = ParameterDirection.Output });

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kasir berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await TriggerRefreshAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearInputs();
                SetupBinding();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                bindingSource.Position = e.RowIndex;
                txtPassword.Clear(); // Mengosongkan password saat baris dipilih demi keamanan
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
            loadCts = new System.Threading.CancellationTokenSource();
            _ = LoadKasirAsync(loadCts.Token);
        }

        private async System.Threading.Tasks.Task TriggerRefreshAsync()
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
            loadCts = new System.Threading.CancellationTokenSource();

            await LoadKasirAsync(loadCts.Token);
        }

        private void FormKasirManagement_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
            new FormAdminMenu(adminUsername, "admin").Show();
        }
    }
}