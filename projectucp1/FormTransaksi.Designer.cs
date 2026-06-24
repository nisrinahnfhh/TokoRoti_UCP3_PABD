namespace projectucp1
{
    partial class FormTransaksi
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTransaksi));
            this.cmbProduk = new System.Windows.Forms.ComboBox();
            this.vwproduktersediaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tOKO_ROTIDataSet = new projectucp1.TOKO_ROTIDataSet();
            this.txtHarga = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.btnTambah = new System.Windows.Forms.Button();
            this.dgvKeranjang = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtBayar = new System.Windows.Forms.TextBox();
            this.lblKembalian = new System.Windows.Forms.Label();
            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnCetakStruk = new System.Windows.Forms.Button();
            this.lblNamaKasir = new System.Windows.Forms.Label();
            this.dgvDetailTransaksi = new System.Windows.Forms.DataGridView();
            this.btnKembali = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.vw_produk_tersediaTableAdapter = new projectucp1.TOKO_ROTIDataSetTableAdapters.vw_produk_tersediaTableAdapter();
            this.txtStokTersedia = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.vwproduktersediaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tOKO_ROTIDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeranjang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailTransaksi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnKembali)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbProduk
            // 
            this.cmbProduk.DataSource = this.vwproduktersediaBindingSource;
            this.cmbProduk.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProduk.Location = new System.Drawing.Point(30, 39);
            this.cmbProduk.Name = "cmbProduk";
            this.cmbProduk.Size = new System.Drawing.Size(121, 37);
            this.cmbProduk.TabIndex = 0;
            this.cmbProduk.SelectedIndexChanged += new System.EventHandler(this.cmbProduk_SelectedIndexChanged);
            // 
            // vwproduktersediaBindingSource
            // 
            this.vwproduktersediaBindingSource.DataMember = "vw_produk_tersedia";
            this.vwproduktersediaBindingSource.DataSource = this.tOKO_ROTIDataSet;
            // 
            // tOKO_ROTIDataSet
            // 
            this.tOKO_ROTIDataSet.DataSetName = "TOKO_ROTIDataSet";
            this.tOKO_ROTIDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtHarga
            // 
            this.txtHarga.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.vwproduktersediaBindingSource, "harga", true));
            this.txtHarga.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHarga.Location = new System.Drawing.Point(259, 41);
            this.txtHarga.Name = "txtHarga";
            this.txtHarga.ReadOnly = true;
            this.txtHarga.Size = new System.Drawing.Size(130, 33);
            this.txtHarga.TabIndex = 1;
            this.txtHarga.TextChanged += new System.EventHandler(this.txtHarga_TextChanged);
            // 
            // txtQty
            // 
            this.txtQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQty.Location = new System.Drawing.Point(596, 41);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(75, 33);
            this.txtQty.TabIndex = 2;
            this.txtQty.TextChanged += new System.EventHandler(this.txtQty_TextChanged);
            // 
            // btnTambah
            // 
            this.btnTambah.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTambah.Location = new System.Drawing.Point(576, 312);
            this.btnTambah.Name = "btnTambah";
            this.btnTambah.Size = new System.Drawing.Size(95, 36);
            this.btnTambah.TabIndex = 3;
            this.btnTambah.Text = "Tambah";
            this.btnTambah.Click += new System.EventHandler(this.btnTambah_Click);
            // 
            // dgvKeranjang
            // 
            this.dgvKeranjang.AllowUserToOrderColumns = true;
            this.dgvKeranjang.AutoGenerateColumns = false;
            this.dgvKeranjang.ColumnHeadersHeight = 34;
            this.dgvKeranjang.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dgvKeranjang.DataSource = this.vwproduktersediaBindingSource;
            this.dgvKeranjang.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvKeranjang.Location = new System.Drawing.Point(30, 77);
            this.dgvKeranjang.Name = "dgvKeranjang";
            this.dgvKeranjang.RowHeadersWidth = 62;
            this.dgvKeranjang.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKeranjang.Size = new System.Drawing.Size(641, 220);
            this.dgvKeranjang.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "produkID";
            this.dataGridViewTextBoxColumn1.HeaderText = "produkID";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "namaProduk";
            this.dataGridViewTextBoxColumn2.HeaderText = "namaProduk";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "harga";
            this.dataGridViewTextBoxColumn3.HeaderText = "harga";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 150;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "stok";
            this.dataGridViewTextBoxColumn4.HeaderText = "stok";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(26, 312);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(569, 21);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "Total: 0";
            // 
            // txtBayar
            // 
            this.txtBayar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBayar.Location = new System.Drawing.Point(110, 343);
            this.txtBayar.Name = "txtBayar";
            this.txtBayar.Size = new System.Drawing.Size(100, 33);
            this.txtBayar.TabIndex = 6;
            this.txtBayar.TextChanged += new System.EventHandler(this.txtBayar_TextChanged);
            this.txtBayar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBayar_KeyPress);
            // 
            // lblKembalian
            // 
            this.lblKembalian.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKembalian.Location = new System.Drawing.Point(30, 377);
            this.lblKembalian.Name = "lblKembalian";
            this.lblKembalian.Size = new System.Drawing.Size(565, 23);
            this.lblKembalian.TabIndex = 7;
            this.lblKembalian.Text = "Kembalian: 0";
            // 
            // btnSimpan
            // 
            this.btnSimpan.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimpan.Location = new System.Drawing.Point(30, 403);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(75, 37);
            this.btnSimpan.TabIndex = 8;
            this.btnSimpan.Text = "Simpan";
            this.btnSimpan.Click += new System.EventHandler(this.btnSimpan_Click);
            // 
            // btnCetakStruk
            // 
            this.btnCetakStruk.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCetakStruk.Location = new System.Drawing.Point(110, 403);
            this.btnCetakStruk.Name = "btnCetakStruk";
            this.btnCetakStruk.Size = new System.Drawing.Size(107, 37);
            this.btnCetakStruk.TabIndex = 9;
            this.btnCetakStruk.Text = "Cetak Struk";
            this.btnCetakStruk.Click += new System.EventHandler(this.BtnCetakStruk_Click);
            // 
            // lblNamaKasir
            // 
            this.lblNamaKasir.AutoSize = true;
            this.lblNamaKasir.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNamaKasir.Location = new System.Drawing.Point(26, 9);
            this.lblNamaKasir.Name = "lblNamaKasir";
            this.lblNamaKasir.Size = new System.Drawing.Size(0, 29);
            this.lblNamaKasir.TabIndex = 12;
            // 
            // dgvDetailTransaksi
            // 
            this.dgvDetailTransaksi.ColumnHeadersHeight = 34;
            this.dgvDetailTransaksi.Location = new System.Drawing.Point(30, 80);
            this.dgvDetailTransaksi.Name = "dgvDetailTransaksi";
            this.dgvDetailTransaksi.ReadOnly = true;
            this.dgvDetailTransaksi.RowHeadersWidth = 62;
            this.dgvDetailTransaksi.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetailTransaksi.Size = new System.Drawing.Size(600, 200);
            this.dgvDetailTransaksi.TabIndex = 13;
            this.dgvDetailTransaksi.Visible = false;
            // 
            // btnKembali
            // 
            this.btnKembali.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKembali.Image = global::projectucp1.Properties.Resources.gambar_icon_kembali;
            this.btnKembali.Location = new System.Drawing.Point(621, 416);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(50, 35);
            this.btnKembali.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnKembali.TabIndex = 10;
            this.btnKembali.TabStop = false;
            this.btnKembali.Click += new System.EventHandler(this.BtnKembali_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(551, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 29);
            this.label1.TabIndex = 14;
            this.label1.Text = "Qty :";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(30, 349);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 29);
            this.label2.TabIndex = 15;
            this.label2.Text = "Dibayar :";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = null;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = null;
            this.bindingNavigator1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2});
            this.bindingNavigator1.Location = new System.Drawing.Point(0, 0);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(792, 38);
            this.bindingNavigator1.TabIndex = 16;
            this.bindingNavigator1.Text = "bindingNavigator1";
            this.bindingNavigator1.RefreshItems += new System.EventHandler(this.bindingNavigator1_RefreshItems);
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(54, 33);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 38);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 27);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // vw_produk_tersediaTableAdapter
            // 
            this.vw_produk_tersediaTableAdapter.ClearBeforeFill = true;
            // 
            // txtStokTersedia
            // 
            this.txtStokTersedia.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.vwproduktersediaBindingSource, "harga", true));
            this.txtStokTersedia.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStokTersedia.Location = new System.Drawing.Point(464, 41);
            this.txtStokTersedia.Name = "txtStokTersedia";
            this.txtStokTersedia.ReadOnly = true;
            this.txtStokTersedia.Size = new System.Drawing.Size(70, 33);
            this.txtStokTersedia.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(395, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 29);
            this.label3.TabIndex = 18;
            this.label3.Text = "Stok :";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(180, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 29);
            this.label4.TabIndex = 19;
            this.label4.Text = "Harga :";
            // 
            // FormTransaksi
            // 
            this.ClientSize = new System.Drawing.Size(792, 463);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtStokTersedia);
            this.Controls.Add(this.bindingNavigator1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnKembali);
            this.Controls.Add(this.btnCetakStruk);
            this.Controls.Add(this.cmbProduk);
            this.Controls.Add(this.txtHarga);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.btnTambah);
            this.Controls.Add(this.dgvKeranjang);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.txtBayar);
            this.Controls.Add(this.lblKembalian);
            this.Controls.Add(this.btnSimpan);
            this.Controls.Add(this.lblNamaKasir);
            this.Controls.Add(this.dgvDetailTransaksi);
            this.Name = "FormTransaksi";
            this.Text = "FormTransaksi";
            this.Load += new System.EventHandler(this.FormTransaksi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.vwproduktersediaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tOKO_ROTIDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeranjang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailTransaksi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnKembali)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cmbProduk;
        private System.Windows.Forms.TextBox txtHarga;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Button btnTambah;
        private System.Windows.Forms.DataGridView dgvKeranjang;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.TextBox txtBayar;
        private System.Windows.Forms.Label lblKembalian;
        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnCetakStruk;
        private System.Windows.Forms.PictureBox btnKembali;
        private System.Windows.Forms.Label lblNamaKasir;
        private System.Windows.Forms.DataGridView dgvDetailTransaksi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        
        private System.Windows.Forms.DataGridViewTextBoxColumn produkIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn namaProdukDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hargaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn stokDataGridViewTextBoxColumn;
        private TOKO_ROTIDataSet tOKO_ROTIDataSet;
        private System.Windows.Forms.BindingSource vwproduktersediaBindingSource;
        private TOKO_ROTIDataSetTableAdapters.vw_produk_tersediaTableAdapter vw_produk_tersediaTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.TextBox txtStokTersedia;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}