using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace projectucp1
{
    public partial class FormTransaksi : Form
    {
        private readonly string con = DAL.GetConnectionString();
        private readonly string username;
        private readonly string role;
        DataTable keranjang = new DataTable();
        private BindingSource keranjangBindingSource = new BindingSource();
        DataTable detailTable;
        private int lastTransactionID = 0;
        private decimal lastTotal = 0;
        private decimal lastBayar = 0;

        private PrintDocument printDocumentStruk;
        private string strukText;
        private BindingSource detailBindingSource = new BindingSource();
        private System.Threading.CancellationTokenSource loadCts;

        public FormTransaksi(string user, string role)
        {
            InitializeComponent();
            username = user;
            this.role = role;
            this.detailTable = new DataTable();
        }

        private async void FormTransaksi_Load(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            lblNamaKasir.Text = $"{role}: {username}";
            cmbProduk.DataSource = null;
            dgvKeranjang.DataSource = null;
            keranjang.Columns.Clear();
            keranjang.Columns.Add("produkID");
            keranjang.Columns.Add("namaProduk");
            keranjang.Columns.Add("harga");
            keranjang.Columns.Add("stok");

            // bind keranjang through a BindingSource so the BindingNavigator can be kept in sync
            keranjangBindingSource.DataSource = keranjang;
            dgvKeranjang.DataSource = keranjangBindingSource;

            // bind the navigator at runtime so it adapts when keranjang is empty
            if (bindingNavigator1 != null)
                bindingNavigator1.BindingSource = keranjangBindingSource;

            // setup detail view (not used by navigator)
            SetupDetailBinding();
            loadCts?.Cancel(); loadCts?.Dispose();
            loadCts = new System.Threading.CancellationTokenSource();
            try
            {
                await LoadProdukAsync(loadCts.Token);
            }
            catch (OperationCanceledException) { }
            UpdateTotal();
        }

        private void SetupDetailBinding()
        {
            detailTable = new DataTable();
            detailTable.Columns.Add("namaProduk", typeof(string));
            detailTable.Columns.Add("jumlah", typeof(string));
            detailTable.Columns.Add("hargaSatuan", typeof(string));

            detailBindingSource.DataSource = detailTable;
            dgvDetailTransaksi.DataSource = detailBindingSource;
            UpdateNavigatorButtons();
        }

        private void UpdateNavigatorButtons()
        {
            var source = keranjangBindingSource;
            bool hasItems = source != null && source.Count > 0;
            bindingNavigatorMoveFirstItem.Enabled = hasItems;
            bindingNavigatorMovePreviousItem.Enabled = hasItems;
            bindingNavigatorMoveNextItem.Enabled = hasItems;
            bindingNavigatorMoveLastItem.Enabled = hasItems;
            bindingNavigatorPositionItem.ReadOnly = !hasItems;
            bindingNavigatorCountItem.Text = $"of { (source?.Count ?? 0) }";
        }

        private async System.Threading.Tasks.Task LoadProdukAsync(System.Threading.CancellationToken ct)
        {
            try
            {
                string currentSelection = cmbProduk.SelectedItem?.ToString();

                var items = await System.Threading.Tasks.Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    var list = new System.Collections.Generic.List<string>();
                    using (SqlConnection conn = new SqlConnection(con))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT produkID, namaProduk FROM produk WHERE stok > 0 ORDER BY namaProduk", conn);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                list.Add(r["produkID"] + " - " + r["namaProduk"]);
                            }
                        }
                    }
                    ct.ThrowIfCancellationRequested();
                    return list;
                }, ct);

                cmbProduk.BeginUpdate();
                try
                {
                    cmbProduk.Items.Clear();
                    foreach (var it in items) cmbProduk.Items.Add(it);
                    if (!string.IsNullOrEmpty(currentSelection) && cmbProduk.Items.Contains(currentSelection))
                        cmbProduk.SelectedItem = currentSelection;
                    else
                        cmbProduk.SelectedIndex = -1;
                }
                finally
                {
                    cmbProduk.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException))
                    MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetProductStock(int produkID)
        {
            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT stok FROM produk WHERE produkID=@id", conn);
                cmd.Parameters.AddWithValue("@id", produkID);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (cmbProduk.SelectedIndex < 0)
            {
                MessageBox.Show("Pilih produk terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQty.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Jumlah harus berupa angka positif", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string[] split = cmbProduk.Text.Split('-');
                int id = int.Parse(split[0].Trim());
                int availableStock = GetProductStock(id);
                if (qty > availableStock)
                {
                    MessageBox.Show($"Stok tidak cukup. Stok tersedia: {availableStock}", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT namaProduk, harga FROM produk WHERE produkID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            keranjang.Rows.Add(
                                id,
                                r["namaProduk"],
                                r["harga"],
                                qty
                            );
                                // notify binding source so navigator updates
                                keranjangBindingSource.ResetBindings(false);
                                // move position to the newly added row
                                if (keranjangBindingSource.Count > 0)
                                    keranjangBindingSource.Position = keranjangBindingSource.Count - 1;
                        }
                    }
                }

                txtQty.Clear();
                cmbProduk.SelectedIndex = -1;
                UpdateTotal();
                MessageBox.Show("Produk berhasil ditambah ke keranjang", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (keranjang.Rows.Count == 0)
            {
                MessageBox.Show("Keranjang kosong, tambah produk terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtBayar.Text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CreateSpecificCulture("id-ID"), out decimal bayar) || bayar <= 0)
            {
                MessageBox.Show("Jumlah pembayaran tidak valid", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = 0;
            foreach (DataRow row in keranjang.Rows)
            {
                //total += Convert.ToDecimal(row["harga"]) * Convert.ToInt32(row["qty"]);

                // UBAH: dari "qty" menjadi "stok"
                total += Convert.ToDecimal(row["harga"]) * Convert.ToInt32(row["stok"]);
            }

            if (bayar < total)
            {
                MessageBox.Show($"Pembayaran kurang. Total: {total:C}, Bayar: {bayar:C}", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Lanjutkan transaksi?\nTotal: {total:C}\nBayar: {bayar:C}", "Konfirmasi Transaksi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO transaksi(tanggal, totalHarga, kasirID) OUTPUT INSERTED.transaksiID VALUES(GETDATE(), @total, @kasirID)",
                        conn, trans);

                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@kasirID", 1);

                    object transaksiIDObj = cmd.ExecuteScalar();
                    if (transaksiIDObj == null || transaksiIDObj is DBNull)
                    {
                        trans.Rollback();
                        MessageBox.Show("Gagal membuat nomor transaksi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int transaksiID = Convert.ToInt32(transaksiIDObj);

                    foreach (DataRow row in keranjang.Rows)
                    {
                        object produkIDObj = row["produkID"];
                        if (produkIDObj == null || produkIDObj is DBNull)
                        {
                            trans.Rollback();
                            MessageBox.Show("produkID tidak valid di keranjang", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        int produkID = Convert.ToInt32(produkIDObj);

                        // UBAH: dari "qty" menjadi "stok"
                        object qtyObj = row["stok"];
                        if (qtyObj == null || qtyObj is DBNull)
                        {
                            trans.Rollback();
                            MessageBox.Show("Qty tidak valid di keranjang", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        int qty = Convert.ToInt32(qtyObj);

                        object hargaObj = row["harga"];
                        if (hargaObj == null || hargaObj is DBNull)
                        {
                            trans.Rollback();
                            MessageBox.Show("Harga tidak valid di keranjang", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        decimal hargaSatuan = Convert.ToDecimal(hargaObj);
                        decimal subtotal = hargaSatuan * qty;

                        SqlCommand d = new SqlCommand(
                            "INSERT INTO detailTransaksi (transaksiID, produkID, jumlah, hargaSatuan, total) VALUES (@tid, @pid, @qty, @harga, @subtotal)",
                            conn, trans);

                        d.Parameters.AddWithValue("@tid", transaksiID);
                        d.Parameters.AddWithValue("@pid", produkID);
                        d.Parameters.AddWithValue("@qty", qty);
                        d.Parameters.AddWithValue("@harga", hargaSatuan);
                        d.Parameters.AddWithValue("@subtotal", subtotal);
                        d.ExecuteNonQuery();

                        SqlCommand checkStock = new SqlCommand("SELECT stok FROM produk WHERE produkID=@id", conn, trans);
                        checkStock.Parameters.AddWithValue("@id", produkID);
                        object stockObj = checkStock.ExecuteScalar();

                        if (stockObj == null || stockObj is DBNull)
                        {
                            trans.Rollback();
                            MessageBox.Show($"Produk ID {produkID} tidak ditemukan atau stok tidak valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int currentStock = Convert.ToInt32(stockObj);

                        if (currentStock < qty)
                        {
                            trans.Rollback();
                            MessageBox.Show($"Stok produk ID {produkID} tidak cukup. Stok tersedia: {currentStock}, diminta: {qty}", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        SqlCommand u = new SqlCommand(
                            "UPDATE produk SET stok = stok - @q WHERE produkID=@id",
                            conn, trans);

                        u.Parameters.AddWithValue("@q", qty);
                        u.Parameters.AddWithValue("@id", produkID);
                        u.ExecuteNonQuery();
                    }

                    trans.Commit();

                    lastTransactionID = transaksiID;
                    lastTotal = total;
                    lastBayar = bayar;

                    try
                    {
                        lblNamaKasir.Text = username;
                        detailTable.Clear();
                        foreach (DataRow row in keranjang.Rows)
                        {
                            string nama = row["nama"].ToString();
                            // UBAH: dari "qty" menjadi "stok"
                            string jumlahStr = row["stok"].ToString();
                            string hargaStr = Convert.ToDecimal(row["harga"]).ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("id-ID"));
                            detailTable.Rows.Add(nama, jumlahStr, hargaStr);
                        }
                        UpdateNavigatorButtons();
                    }
                    catch { }

                    MessageBox.Show("Transaksi berhasil disimpan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetForm();
                }
                catch (Exception ex)
                {
                    try
                    {
                        trans.Rollback();
                    }
                    catch { }
                    MessageBox.Show("Gagal transaksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbProduk_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduk.SelectedIndex >= 0)
            {
                string[] split = cmbProduk.Text.Split('-');
                if (split.Length > 0 && int.TryParse(split[0].Trim(), out int id))
                {
                    using (SqlConnection conn = new SqlConnection(con))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT harga FROM produk WHERE produkID=@id", conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        var harga = cmd.ExecuteScalar();
                        if (harga != null)
                        {
                            txtHarga.Text = harga.ToString();
                        }
                    }

                    try
                    {
                        int stok = GetProductStock(id);
                        txtStokTersedia.Text = stok.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal mengambil data stok: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                txtHarga.Clear();
                txtStokTersedia.Clear();
            }
        }

        private void txtBayar_TextChanged(object sender, EventArgs e)
        {
            string original = txtBayar.Text;
            string cleanText = System.Text.RegularExpressions.Regex.Replace(original, "[^0-9]", "");
            if (cleanText.Length > 0)
            {
                txtBayar.TextChanged -= txtBayar_TextChanged;
                decimal value = 0;
                decimal.TryParse(cleanText, out value);
                txtBayar.Text = value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("id-ID"));
                txtBayar.SelectionStart = txtBayar.Text.Length;
                txtBayar.SelectionLength = 0;
                txtBayar.TextChanged += txtBayar_TextChanged;
            }
            UpdateTotal();
        }

        private void txtBayar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void UpdateTotal()
        {
            decimal total = 0;

            foreach (DataRow row in keranjang.Rows)
            {
                decimal harga = Convert.ToDecimal(row["harga"]);
                //int qty = Convert.ToInt32(row["qty"]);

                // UBAH: dari "qty" menjadi "stok"
                int qty = Convert.ToInt32(row["stok"]);
                total += harga * qty;
            }

            lblTotal.Text = $"Total: {total:C}";

            if (decimal.TryParse(txtBayar.Text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CreateSpecificCulture("id-ID"), out decimal bayar))
            {
                decimal kembalian = bayar - total;
                lblKembalian.Text = $"Kembalian: {kembalian:C}";
            }
            else
            {
                lblKembalian.Text = "Kembalian: 0";
            }
        }

        private void ResetForm()
        {
            keranjang.Clear();
            if (keranjangBindingSource != null)
            {
                keranjangBindingSource.ResetBindings(false);
                try { keranjangBindingSource.Position = -1; } catch { }
            }
            if (detailTable != null)
                detailTable.Clear();
            cmbProduk.Items.Clear();
            cmbProduk.SelectedIndex = -1;
            txtHarga.Clear();
            txtQty.Clear();
            txtBayar.Clear();
            // refresh product list asynchronously; do not block UI
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
            loadCts = new System.Threading.CancellationTokenSource();
            _ = LoadProdukAsync(loadCts.Token);
            UpdateNavigatorButtons();
            UpdateTotal();
        }

        private void BtnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
            new FormKasirMenu(username, "kasir").Show();
        }

        private string GenerateStrukText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TOKO ROTI RAN");
            sb.AppendLine("No. Transaksi: " + lastTransactionID.ToString());
            sb.AppendLine("Tanggal: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("Kasir: " + lblNamaKasir.Text);
            sb.AppendLine(new string('-', 40));
            sb.AppendLine(string.Format("{0,-20}{1,6}{2,12}", "Produk", "Qty", "Harga"));
            sb.AppendLine(new string('-', 40));
            bool printedAny = false;
            if (dgvDetailTransaksi != null && dgvDetailTransaksi.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvDetailTransaksi.Rows)
                {
                    if (row.IsNewRow) continue;
                    string nama = row.Cells["namaProduk"].Value?.ToString() ?? string.Empty;
                    string jumlah = row.Cells["jumlah"].Value?.ToString() ?? "0";
                    decimal harga = 0m;
                    decimal.TryParse(row.Cells["hargaSatuan"].Value?.ToString() ?? "0", System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CreateSpecificCulture("id-ID"), out harga);

                    sb.AppendLine(string.Format("{0,-20}{1,6}{2,12}",
                        nama.Length > 20 ? nama.Substring(0, 17) + "..." : nama,
                        jumlah,
                        harga.ToString("N0")
                    ));
                    printedAny = true;
                }
            }

            if (!printedAny && keranjang != null && keranjang.Rows.Count > 0)
            {
                foreach (DataRow row in keranjang.Rows)
                {
                    string nama = row["namaProduk"].ToString();
                    string jumlah = row["stok"].ToString(); // UBAH: dari "qty" menjadi "stok"
                    decimal harga = 0m;
                    try { harga = Convert.ToDecimal(row["harga"]); } catch { harga = 0m; }

                    sb.AppendLine(string.Format("{0,-20}{1,6}{2,12}",
                        nama.Length > 20 ? nama.Substring(0, 17) + "..." : nama,
                        jumlah,
                        harga.ToString("N0")
                    ));
                }
            }

            sb.AppendLine(new string('-', 40));
            sb.AppendLine(lblTotal.Text);
            sb.AppendLine("Bayar: " + txtBayar.Text);
            sb.AppendLine(lblKembalian.Text);
            sb.AppendLine("Terima kasih!");
            return sb.ToString();
        }

        private void btnCetakStruk_Click(object sender, EventArgs e)
        {
            strukText = GenerateStrukText();
            printDocumentStruk = new PrintDocument();
            printDocumentStruk.PrintPage += PrintDocumentStruk_PrintPage;
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDocumentStruk;
            preview.ShowDialog();
        }

        private void PrintDocumentStruk_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(strukText, new Font("Consolas", 10), Brushes.Black, 20, 20);
        }

        private void BtnCetakStruk_Click(object sender, EventArgs e)
        {
            btnCetakStruk_Click(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblNamaKasir_Click(object sender, EventArgs e)
        {

        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
            loadCts = new System.Threading.CancellationTokenSource();
            try
            {
                await LoadProdukAsync(loadCts.Token);
            }
            catch (OperationCanceledException) { }
            UpdateTotal();
        }

        private void txtHarga_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}