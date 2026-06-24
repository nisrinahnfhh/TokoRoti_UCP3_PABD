using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ExcelDataReader;

namespace projectucp1
{
    public partial class FormProduk : Form
    {
        private readonly string con = "Data Source=LAPTOP-1SH0I1SH\\NANA;Initial Catalog=TOKO_ROTIku;Integrated Security=True";
        private readonly bool readOnly;
        private readonly string username;
        private readonly string role;
        private bool updatingSelection = false;
        private System.Threading.CancellationTokenSource loadCts;


        public FormProduk(bool readOnly, string user, string role)
        {
            InitializeComponent();
            this.readOnly = readOnly;
            this.username = user;
            this.role = role;
            
        }

        private async void FormProduk_Load(object sender, EventArgs e)
        {
            SetupBinding();
            loadCts?.Cancel(); loadCts?.Dispose();
            loadCts = new System.Threading.CancellationTokenSource();
            try
            {
                await LoadDataAsync(loadCts.Token);
            }
            catch (OperationCanceledException) { }

            if (readOnly || role == "kasir")
            {
                btnTambah.Visible = false;
                btnUpdate.Visible = false;
                btnHapus.Visible = false;
                btnUpload.Visible = false;
                btnRefresh.Visible = false;
            }
            this.FormClosed -= FormProduk_FormClosed;
            this.FormClosed += FormProduk_FormClosed;
            //ClearInputs();
        }

        private void FormProduk_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
        }

        private void SetupBinding()
        {
            dataGridView1.DataSource = vwprodukBindingSource;
            bindingNavigator1.BindingSource = vwprodukBindingSource;

            txtNamaProduk.DataBindings.Clear();
            txtHarga.DataBindings.Clear();
            txtStok.DataBindings.Clear();
            txtNamaProduk.DataBindings.Add("Text", vwprodukBindingSource, "namaProduk", true, System.Windows.Forms.DataSourceUpdateMode.Never);
            txtHarga.DataBindings.Add("Text", vwprodukBindingSource, "harga", true, System.Windows.Forms.DataSourceUpdateMode.Never);
            txtStok.DataBindings.Add("Text", vwprodukBindingSource, "stok", true, System.Windows.Forms.DataSourceUpdateMode.Never);

            vwprodukBindingSource.PositionChanged -= VwprodukBindingSource_PositionChanged;
            vwprodukBindingSource.PositionChanged += VwprodukBindingSource_PositionChanged;
        }

        private void VwprodukBindingSource_PositionChanged(object sender, EventArgs e)
        {
            UpdateGridSelection();

            if (vwprodukBindingSource.Current != null)
            {
                txtNamaProduk.DataBindings["Text"]?.ReadValue();
                txtHarga.DataBindings["Text"]?.ReadValue();
                txtStok.DataBindings["Text"]?.ReadValue();
            }
        }

        private void UpdateGridSelection()
        {
            if (vwprodukBindingSource == null || vwprodukBindingSource.Position < 0)
            {
                dataGridView1.ClearSelection();
                return;
            }

            if (updatingSelection) return;

            try
            {
                updatingSelection = true;
                int pos = vwprodukBindingSource.Position;
                if (dataGridView1.Rows.Count > 0 && pos >= 0 && pos < dataGridView1.Rows.Count)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[pos].Selected = true;
                    if (dataGridView1.Rows[pos].Cells.Count > 0 &&
                        (dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.RowIndex != pos))
                    {
                        this.BeginInvoke(new Action(() =>
                            dataGridView1.CurrentCell = dataGridView1.Rows[pos].Cells[0]));
                    }
                }
                else
                {
                    dataGridView1.ClearSelection();
                }
            }
            finally
            {
                updatingSelection = false;
            }
        }

        private async System.Threading.Tasks.Task LoadDataAsync(System.Threading.CancellationToken ct)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(con))
                {
                    int prevPos = vwprodukBindingSource != null ? vwprodukBindingSource.Position : -1;

                    // 1. Ambil data dari database di Background Thread (Task.Run)
                    DataTable dataTable = await System.Threading.Tasks.Task.Run(() =>
                    {
                        ct.ThrowIfCancellationRequested();
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_produk ORDER BY produkID", conn);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }, ct);

                    // 2. Kode di bawah ini dijalankan AMAN di UI Thread secara otomatis berkat 'await'
                    ct.ThrowIfCancellationRequested();

                    vwprodukBindingSource.DataSource = dataTable;
                    vwprodukBindingSource.ResetBindings(false);

                    if (dataTable.Rows.Count > 0)
                    {
                        if (prevPos >= 0 && prevPos < vwprodukBindingSource.Count)
                            vwprodukBindingSource.Position = prevPos;
                        else
                            vwprodukBindingSource.Position = 0;
                    }
                    else
                    {
                        dataGridView1.ClearSelection();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Diabaikan jika pembatalan sengaja dipicu
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTambah_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            if (vwprodukBindingSource.DataSource is DataTable dt)
            {
                string namaBaru = txtNamaProduk.Text.Trim().ToLower();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["namaProduk"] != DBNull.Value && row["namaProduk"].ToString().Trim().ToLower() == namaBaru)
                    {
                        MessageBox.Show("nama produk tidak boleh sama", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }
                }
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(con))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_InsertProduk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter pName = new SqlParameter("@namaProduk", SqlDbType.VarChar, 100);
                    pName.Value = txtNamaProduk.Text;
                    cmd.Parameters.Add(pName);

                    SqlParameter pHarga = new SqlParameter("@harga", SqlDbType.Decimal);
                    pHarga.Value = decimal.Parse(txtHarga.Text);
                    cmd.Parameters.Add(pHarga);

                    SqlParameter pStok = new SqlParameter("@stok", SqlDbType.Int);
                    pStok.Value = int.Parse(txtStok.Text);
                    cmd.Parameters.Add(pStok);

                    SqlParameter pOutID = new SqlParameter("@outProdukID", SqlDbType.Int);
                    pOutID.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pOutID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Produk berhasil ditambah", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { loadCts?.Cancel(); } catch { }
                    try { loadCts?.Dispose(); } catch { }
                    loadCts = new System.Threading.CancellationTokenSource();
                    _ = LoadDataAsync(loadCts.Token);
                    ClearInputs();
                    await LoadDataAsync(loadCts.Token);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ClearInputs();
            SetupBinding();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                vwprodukBindingSource.Position = e.RowIndex;
            }
        }

        private void TxtStok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (vwprodukBindingSource.Current == null)
            {
                MessageBox.Show("Pilih satu produk yang akan diupdate", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
                return;

            try
            {
                DataRowView rowView = (DataRowView)vwprodukBindingSource.Current;
                int id = (int)rowView["produkID"];

                using (SqlConnection conn = new SqlConnection(con))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_UpdateProduk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter pID = new SqlParameter("@produkID", SqlDbType.Int);
                    pID.Value = id;
                    cmd.Parameters.Add(pID);

                    SqlParameter pName = new SqlParameter("@namaProduk", SqlDbType.VarChar, 100);
                    pName.Value = txtNamaProduk.Text;
                    cmd.Parameters.Add(pName);

                    SqlParameter pHarga = new SqlParameter("@harga", SqlDbType.Decimal);
                    pHarga.Value = decimal.Parse(txtHarga.Text);
                    cmd.Parameters.Add(pHarga);

                    SqlParameter pStok = new SqlParameter("@stok", SqlDbType.Int);
                    pStok.Value = int.Parse(txtStok.Text);
                    cmd.Parameters.Add(pStok);

                    SqlParameter pOutRows = new SqlParameter("@outRows", SqlDbType.Int);
                    pOutRows.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pOutRows);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Produk berhasil diupdate", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    try { loadCts?.Cancel(); } catch { }
                    try { loadCts?.Dispose(); } catch { }
                    loadCts = new System.Threading.CancellationTokenSource();
                    _ = LoadDataAsync(loadCts.Token);
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (vwprodukBindingSource.Current == null)
            {
                MessageBox.Show("Pilih minimal 1 data untuk dihapus", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string message = "Yakin ingin menghapus produk ini?";
            DialogResult result = MessageBox.Show(message, "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            // --- STRATEGI ANTI FREEZE 1: Putus event listener agar grid tidak mencoba menyeleksi data yang sedang dihapus ---
            vwprodukBindingSource.PositionChanged -= VwprodukBindingSource_PositionChanged;
            dataGridView1.DataSource = null; // Putus sementara datasource agar grid tidak lag saat data berubah

            try
            {
                DataRowView rowView = (DataRowView)vwprodukBindingSource.Current;
                int id = (int)rowView["produkID"];

                using (SqlConnection conn = new SqlConnection(con))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_DeleteProduk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter pID = new SqlParameter("@produkID", SqlDbType.Int) { Value = id };
                    cmd.Parameters.Add(pID);

                    SqlParameter pOutRows = new SqlParameter("@outRows", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pOutRows);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Produk berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { loadCts?.Cancel(); } catch { }
                    try { loadCts?.Dispose(); } catch { }
                    loadCts = new System.Threading.CancellationTokenSource();

                    // Ambil data baru secara asinkronus (menunggu hingga selesai tanpa memicu event UI)
                    await LoadDataAsync(loadCts.Token);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // --- STRATEGI ANTI FREEZE 2: Sambungkan kembali setelah semuanya aman dan stabil ---
                ClearInputs();
                SetupBinding(); // Ini akan mengikat ulang datasource dan memasang kembali event PositionChanged
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
            loadCts = new System.Threading.CancellationTokenSource();

            ClearInputs();
            await LoadDataAsync(loadCts.Token);
            SetupBinding();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
            {
                MessageBox.Show("Nama produk tidak boleh kosong", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtHarga.Text, out decimal harga) || harga <= 0)
            {
                MessageBox.Show("Harga harus berupa angka positif", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtStok.Text, out int stok) || stok < 0)
            {
                MessageBox.Show("Stok harus berupa angka positif atau nol", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            //txtNamaProduk.Clear();
            //txtHarga.Clear();
            //txtStok.Clear();
            //if (vwprodukBindingSource.Count > 0)
            //    vwprodukBindingSource.Position = 0;

            txtNamaProduk.DataBindings.Clear();
            txtHarga.DataBindings.Clear();
            txtStok.DataBindings.Clear();

            txtNamaProduk.Clear();
            txtHarga.Clear();
            txtStok.Clear();

            dataGridView1.ClearSelection();
        }

        private void BtnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
            if (role == "admin")
            {
                new FormAdminMenu(username, role).Show();
            }
            else if (role == "kasir")
            {
                new FormKasirMenu(username, role).Show();
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            // 1. Inisialisasi provider encoding
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Pastikan DataSource saat ini adalah DataTable yang valid dari database
            if (!(vwprodukBindingSource.DataSource is DataTable dtUtama))
            {
                MessageBox.Show("Gagal mengambil data lama dari grid. Pastikan data database sudah termuat.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Buka dialog untuk memilih file Excel
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Pilih File Excel Produk";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        DataTable dtExcel = new DataTable();

                        // 3. Membaca file Excel ke DataTable Sementara (dtExcel)
                        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true // Menggunakan baris pertama sebagai nama kolom
                                    }
                                });

                                dtExcel = result.Tables[0];
                            }
                        }

                        // Validasi jika Excel kosong
                        if (dtExcel == null || dtExcel.Rows.Count == 0)
                        {
                            MessageBox.Show("File Excel kosong atau tidak valid.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int dataBerhasilDiinput = 0;

                        // 4. Looping data Excel untuk disimpan SATU PER SATU ke Database via SQL
                        using (SqlConnection conn = new SqlConnection(con))
                        {
                            await conn.OpenAsync();

                            foreach (DataRow row in dtExcel.Rows)
                    {
                                // Skip jika baris kosong
                                if (row["namaProduk"] == DBNull.Value || string.IsNullOrWhiteSpace(row["namaProduk"].ToString()))
                                    continue;

                                // Cek Duplikasi: Lewati jika nama produk sudah ada di database/grid lama
                                string namaBaru = row["namaProduk"].ToString().Trim().ToLower();
                                bool IsDuplikat = false;
                                foreach (DataRow drUtama in dtUtama.Rows)
                                {
                                    if (drUtama["namaProduk"] != DBNull.Value && drUtama["namaProduk"].ToString().Trim().ToLower() == namaBaru)
                                    {
                                        IsDuplikat = true;
                                        break;
                                    }
                                }

                                if (IsDuplikat) continue; // Skip jika nama produk sudah ada

                                // Eksekusi Stored Procedure Insert Anda
                                using (SqlCommand cmd = new SqlCommand("dbo.sp_InsertProduk", conn))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.Add(new SqlParameter("@namaProduk", SqlDbType.VarChar, 100) { Value = row["namaProduk"] });
                                    cmd.Parameters.Add(new SqlParameter("@harga", SqlDbType.Decimal) { Value = decimal.Parse(row["harga"].ToString()) });
                                    cmd.Parameters.Add(new SqlParameter("@stok", SqlDbType.Int) { Value = int.Parse(row["stok"].ToString()) });

                                    SqlParameter pOutID = new SqlParameter("@outProdukID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                                    cmd.Parameters.Add(pOutID);

                                    await cmd.ExecuteNonQueryAsync();
                                    dataBerhasilDiinput++;
                                }
                            }
                        }

                        // 5. REFRESH DATA GRID: Panggil kembali data terbaru secara asinkronus dari database
                        if (dataBerhasilDiinput > 0)
                        {
                            MessageBox.Show($"Berhasil menyimpan {dataBerhasilDiinput} data baru dari Excel ke Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            try { loadCts?.Cancel(); } catch { }
                            try { loadCts?.Dispose(); } catch { }
                            loadCts = new System.Threading.CancellationTokenSource();

                            // Ambil ulang data dari database agar ID baru sinkron dan masuk di jajaran paling bawah
                            await LoadDataAsync(loadCts.Token);
                        }
                        else
                        {
                            MessageBox.Show("Tidak ada data baru yang dimasukkan (Semua data mungkin duplikat atau kosong).", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal memproses file Excel ke Database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}