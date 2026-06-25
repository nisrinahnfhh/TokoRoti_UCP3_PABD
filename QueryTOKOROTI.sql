CREATE DATABASE TOKO_ROTI;
GO

USE TOKO_ROTI;
GO

-- 1. Tabel login
CREATE TABLE login (
    loginID INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(50) NOT NULL,
    role VARCHAR(20) NOT NULL,
    CONSTRAINT CHK_Login_Role CHECK (role IN ('admin', 'kasir'))
);
GO

-- 2. Tabel adminMenu
CREATE TABLE adminMenu (
    adminID INT IDENTITY(1,1) PRIMARY KEY,
    loginID INT NOT NULL UNIQUE,
    CONSTRAINT FK_adminMenu_login
        FOREIGN KEY (loginID) REFERENCES login(loginID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

-- 3. Tabel kasirMenu
CREATE TABLE kasirMenu (
    kasirID INT IDENTITY(1,1) PRIMARY KEY,
    loginID INT NOT NULL UNIQUE,
    CONSTRAINT FK_kasirMenu_login
        FOREIGN KEY (loginID) REFERENCES login(loginID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

-- 4. Tabel pelanggan
CREATE TABLE pelanggan (
    pelangganID INT IDENTITY(1,1) PRIMARY KEY,
    nama VARCHAR(100) NOT NULL,
    telepon VARCHAR(15) NULL
);
GO

-- 5. Tabel produk
CREATE TABLE produk (
    produkID INT IDENTITY(1,1) PRIMARY KEY,
    namaProduk VARCHAR(100) NOT NULL,
    harga DECIMAL(18,2) NOT NULL,
    stok INT NOT NULL,
    CONSTRAINT CHK_Produk_Stok CHECK (stok >= 0),
    CONSTRAINT CHK_Produk_Harga CHECK (harga >= 0)
);
GO

-- 6. Tabel transaksi
CREATE TABLE transaksi (
    transaksiID INT IDENTITY(1,1) PRIMARY KEY,
    tanggal DATETIME NOT NULL DEFAULT GETDATE(),
    totalHarga DECIMAL(18,2) NOT NULL,
    kasirID INT NOT NULL,
    pelangganID INT NULL,
    CONSTRAINT CHK_Transaksi_Total CHECK (totalHarga >= 0),
    CONSTRAINT FK_Transaksi_Kasir
        FOREIGN KEY (kasirID) REFERENCES kasirMenu(kasirID)
        ON UPDATE CASCADE,
    CONSTRAINT FK_Transaksi_Pelanggan
        FOREIGN KEY (pelangganID) REFERENCES pelanggan(pelangganID)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);
GO

-- 7. Tabel detailTransaksi
CREATE TABLE detailTransaksi (
    detailID INT IDENTITY(1,1) PRIMARY KEY,
    transaksiID INT NOT NULL,
    produkID INT NOT NULL,
    jumlah INT NOT NULL,
    hargaSatuan DECIMAL(18,2) NOT NULL,
    total DECIMAL(18,2) NOT NULL,
    CONSTRAINT CHK_DetailTransaksi_Jumlah CHECK (jumlah > 0),
    CONSTRAINT CHK_DetailTransaksi_HargaSatuan CHECK (hargaSatuan >= 0),
    CONSTRAINT CHK_DetailTransaksi_Total CHECK (total >= 0),
    CONSTRAINT FK_DetailTransaksi_Transaksi
        FOREIGN KEY (transaksiID) REFERENCES transaksi(transaksiID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT FK_DetailTransaksi_Produk
        FOREIGN KEY (produkID) REFERENCES produk(produkID)
        ON UPDATE CASCADE
);
GO


/*
TESTING
*/

INSERT INTO login (username, password, role)
VALUES 
('nana', '123', 'admin'),
('apip', '123', 'kasir');

INSERT INTO adminMenu (loginID)
SELECT loginID FROM login WHERE username = 'nana';

INSERT INTO kasirMenu (loginID)
SELECT loginID FROM login WHERE username = 'apip';

INSERT INTO pelanggan (nama, telepon)
VALUES
('Budi', '08123456789'),
('Siti', '08129876543');

INSERT INTO produk (namaProduk, harga, stok)
VALUES
('Roti Coklat', 8000, 50),
('Roti Keju', 9000, 30),
('Roti Tawar', 12000, 20);
GO

SELECT * FROM login;
SELECT * FROM adminMenu;
SELECT * FROM kasirMenu;
SELECT * FROM pelanggan;
SELECT * FROM produk;
SELECT * FROM transaksi;
SELECT * FROM detailTransaksi;


---------------------------------------------------------------------------------------------------------------------------------


IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'log_aktivitas')
BEGIN
    CREATE TABLE dbo.log_aktivitas (
        logID       INT           IDENTITY(1,1) PRIMARY KEY,
        tabelTarget VARCHAR(50)   NOT NULL,
        aksi        VARCHAR(10)   NOT NULL,
        keterangan  NVARCHAR(500) NULL,
        waktu       DATETIME      DEFAULT GETDATE()
    );
    PRINT 'Tabel log_aktivitas dibuat.';
END
ELSE
    PRINT 'Tabel log_aktivitas sudah ada, skip.';
GO

-- ================================================
-- TRIGGER 1 : trg_AfterInsert_Produk
-- ================================================
IF OBJECT_ID('dbo.trg_AfterInsert_Produk', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_AfterInsert_Produk;
GO
CREATE TRIGGER dbo.trg_AfterInsert_Produk
ON  dbo.produk
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.log_aktivitas (tabelTarget, aksi, keterangan)
    SELECT
        'produk',
        'INSERT',
        'Produk baru: [ID=' + CAST(produkID AS VARCHAR(10)) + '] '
        + namaProduk
        + ' | Harga: Rp ' + CAST(harga AS VARCHAR(20))
        + ' | Stok: '     + CAST(stok  AS VARCHAR(10))
    FROM inserted;
END;
GO
PRINT 'OK: trg_AfterInsert_Produk';
GO

-- ================================================
-- TRIGGER 2 : trg_AfterUpdate_Produk
-- ================================================
IF OBJECT_ID('dbo.trg_AfterUpdate_Produk', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_AfterUpdate_Produk;
GO
CREATE TRIGGER dbo.trg_AfterUpdate_Produk
ON  dbo.produk
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE stok < 0)
    BEGIN
        RAISERROR('Stok tidak boleh kurang dari 0.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    INSERT INTO dbo.log_aktivitas (tabelTarget, aksi, keterangan)
    SELECT
        'produk',
        'UPDATE',
        'Produk diubah: [ID=' + CAST(i.produkID AS VARCHAR(10)) + '] '
        + i.namaProduk
        + ' | Stok: ' + CAST(d.stok  AS VARCHAR(10))
        + ' -> '      + CAST(i.stok  AS VARCHAR(10))
        + ' | Harga: Rp ' + CAST(d.harga AS VARCHAR(20))
        + ' -> Rp '       + CAST(i.harga AS VARCHAR(20))
    FROM inserted i
    JOIN deleted  d ON d.produkID = i.produkID;
END;
GO
PRINT 'OK: trg_AfterUpdate_Produk';
GO

-- ================================================
-- TRIGGER 3 : trg_AfterDelete_Transaksi
-- ================================================
IF OBJECT_ID('dbo.trg_AfterDelete_Transaksi', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_AfterDelete_Transaksi;
GO
CREATE TRIGGER dbo.trg_AfterDelete_Transaksi
ON  dbo.transaksi
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET    p.stok = p.stok + dt.jumlah
    FROM   dbo.produk          p
    JOIN   dbo.detailTransaksi dt ON dt.produkID    = p.produkID
    JOIN   deleted             d  ON d.transaksiID  = dt.transaksiID;

    INSERT INTO dbo.log_aktivitas (tabelTarget, aksi, keterangan)
    SELECT
        'transaksi',
        'DELETE',
        'Transaksi dihapus: [ID=' + CAST(transaksiID AS VARCHAR(10)) + ']'
        + ' | Stok produk terkait sudah dikembalikan otomatis.'
    FROM deleted;
END;
GO
PRINT 'OK: trg_AfterDelete_Transaksi';
GO

-- ================================================
-- VERIFIKASI
-- ================================================
SELECT
    t.name                   AS NamaTrigger,
    OBJECT_NAME(t.parent_id) AS DiTabel,
    t.is_disabled            AS Nonaktif,
    t.create_date            AS Dibuat
FROM sys.triggers t
WHERE t.name IN (
    'trg_AfterInsert_Produk',
    'trg_AfterUpdate_Produk',
    'trg_AfterDelete_Transaksi'
)
ORDER BY t.name;
GO