using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace projectucp1
{
    public partial class FormLaporan : Form
    {
        private readonly string con = DAL.GetConnectionString();
        private readonly string username;
        private readonly string role;

        private PrintDocument printDocumentLaporan;
        private string laporanText;
        private BindingSource bindingSource;
        private bool updatingSelection = false;
        private System.Threading.CancellationTokenSource loadCts;

        public FormLaporan(string user, string role)
        {
            InitializeComponent();
            this.username = user;
            this.role = role;
            this.bindingSource = new BindingSource();
        }

        private async void FormLaporan_Load(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            SetupBinding();
            loadCts?.Cancel();
            loadCts?.Dispose();
            loadCts = new System.Threading.CancellationTokenSource();
            try
            {
                await LoadLaporanAsync(loadCts.Token);
            }
            catch (OperationCanceledException) { }
        }

        private void SetupBinding()
        {
            dataGridView1.DataSource = bindingSource;
            if (bindingNavigator1 != null)
                bindingNavigator1.BindingSource = bindingSource;
            bindingSource.PositionChanged -= BindingSource_PositionChanged;
            bindingSource.PositionChanged += BindingSource_PositionChanged;
        }

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            UpdateGridSelection();
        }

        private void UpdateGridSelection()
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

        private async System.Threading.Tasks.Task LoadLaporanAsync(System.Threading.CancellationToken ct)
        {
            try
            {
                var dt = await System.Threading.Tasks.Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    DataTable table = new DataTable();
                    using (SqlConnection conn = new SqlConnection(con))
                    {
                        // PERUBAHAN QUERY: Menggunakan STRING_AGG dan GROUP BY agar baris produk menyatu menyamping
                        string queryGrid = @"
                            SELECT 
                                NomorTransaksi, 
                                Tanggal, 
                                TotalTransaksi, 
                                STRING_AGG(NamaProduk, ', ') AS NamaProduk, 
                                SUM(Jumlah) AS Jumlah 
                            FROM dbo.vw_transaksi_detail 
                            GROUP BY NomorTransaksi, Tanggal, TotalTransaksi
                            ORDER BY Tanggal DESC";

                        SqlDataAdapter adapter = new SqlDataAdapter(queryGrid, conn);
                        adapter.Fill(table);
                    }
                    ct.ThrowIfCancellationRequested();
                    return table;
                }, ct);

                bindingSource.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGridView1.DataSource = bindingSource;

                if (dt.Rows.Count > 0)
                {
                    DataGridViewColumn col;
                    col = FindColumnByDataPropertyName("NomorTransaksi"); if (col != null) col.Width = 90;
                    col = FindColumnByDataPropertyName("Tanggal"); if (col != null) col.Width = 120;
                    col = FindColumnByDataPropertyName("TotalTransaksi"); if (col != null) col.Width = 100;
                    col = FindColumnByDataPropertyName("NamaProduk"); if (col != null) col.Width = 220;
                    col = FindColumnByDataPropertyName("Jumlah"); if (col != null) col.Width = 60;

                    // Sembunyikan kolom HargaSatuan dan Subtotal karena data sudah dikelompokkan (Grouped)
                    col = FindColumnByDataPropertyName("HargaSatuan"); if (col != null) col.Visible = false;
                    col = FindColumnByDataPropertyName("Subtotal"); if (col != null) col.Visible = false;

                    lblStatus.Text = $"Total: {dt.Rows.Count} transaksi";
                    if (bindingSource.Count > 0)
                        bindingSource.Position = 0;
                }
                else
                {
                    lblStatus.Text = "Belum ada data transaksi";
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            try { loadCts?.Cancel(); } catch { }
            try { loadCts?.Dispose(); } catch { }
            loadCts = new System.Threading.CancellationTokenSource();
            try
            {
                await LoadLaporanAsync(loadCts.Token);
            }
            catch (OperationCanceledException) { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            loadCts?.Cancel();
            loadCts?.Dispose();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bindingSource.Position = e.RowIndex;
            }
        }

        private void BtnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
            new FormAdminMenu(username, role).Show();
        }

        private string GenerateLaporanText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("LAPORAN DETAIL TRANSAKSI");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(string.Format("{0,-10}{1,-16}{2,-30}{3,6}{4,12}", "ID", "Tanggal", "Produk", "Qty", "Total"));
            sb.AppendLine(new string('-', 80));

            using (SqlConnection conn = new SqlConnection(con))
            {
                // PERUBAHAN QUERY: Menyelaraskan teks cetak struk laporan dengan format pengelompokan yang baru
                string queryPrint = @"
                    SELECT 
                        NomorTransaksi, 
                        Tanggal, 
                        TotalTransaksi,
                        STRING_AGG(NamaProduk, ', ') AS NamaProduk, 
                        SUM(Jumlah) AS Jumlah 
                    FROM dbo.vw_transaksi_detail
                    GROUP BY NomorTransaksi, Tanggal, TotalTransaksi
                    ORDER BY Tanggal DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(queryPrint, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string id = row["NomorTransaksi"].ToString();
                    string tanggal = Convert.ToDateTime(row["Tanggal"]).ToString("dd/MM/yyyy");
                    string produk = row["NamaProduk"].ToString();
                    string jumlah = row["Jumlah"].ToString();
                    decimal total = row["TotalTransaksi"] != DBNull.Value ? Convert.ToDecimal(row["TotalTransaksi"]) : 0m;

                    sb.AppendLine(string.Format("{0,-10}{1,-16}{2,-30}{3,6}{4,12}",
                        id,
                        tanggal,
                        produk.Length > 30 ? produk.Substring(0, 27) + "..." : produk,
                        jumlah,
                        total.ToString("N0")
                    ));
                }
            }
            return sb.ToString();
        }

        private void btnCetakLaporan_Click(object sender, EventArgs e)
        {
            laporanText = GenerateLaporanText();
            printDocumentLaporan = new PrintDocument();
            printDocumentLaporan.PrintPage += PrintDocumentLaporan_PrintPage;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDocumentLaporan;
            preview.ShowDialog();
        }

        private void BtnCetakLaporan_Click(object sender, EventArgs e)
        {
            btnCetakLaporan_Click(sender, e);
        }

        private void PrintDocumentLaporan_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(laporanText, new Font("Consolas", 9), Brushes.Black, 20, 20);
        }

        private DataGridViewColumn FindColumnByDataPropertyName(string dataPropertyName)
        {
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                if (string.Equals(c.DataPropertyName, dataPropertyName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.HeaderText, dataPropertyName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.Name, dataPropertyName, StringComparison.OrdinalIgnoreCase))
                    return c;
            }
            return null;
        }
    }
}