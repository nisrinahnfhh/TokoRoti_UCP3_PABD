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
--VIEW--
USE TOKO_ROTI;
GO

CREATE VIEW dbo.vw_produk
AS
SELECT produkID, namaProduk, harga, stok
FROM dbo.produk;
GO

CREATE VIEW dbo.vw_produk_tersedia
AS
SELECT produkID, namaProduk, harga, stok
FROM dbo.produk
WHERE stok > 0;
GO

CREATE VIEW dbo.vw_kasir
AS
SELECT k.kasirID, l.loginID, l.username, l.role
FROM dbo.login l
INNER JOIN dbo.kasirMenu k ON l.loginID = k.loginID
WHERE l.role = 'kasir';
GO

CREATE VIEW dbo.vw_transaksi_detail
AS
SELECT 
    t.transaksiID AS NomorTransaksi,
    t.tanggal AS Tanggal,
    t.totalHarga AS TotalTransaksi,
    p.namaProduk AS NamaProduk,
    dt.jumlah AS Jumlah,
    dt.hargaSatuan AS HargaSatuan,
    dt.total AS Subtotal
FROM dbo.transaksi t
INNER JOIN dbo.detailTransaksi dt ON dt.transaksiID = t.transaksiID
INNER JOIN dbo.produk p ON p.produkID = dt.produkID;
GO


select * from vw_kasir
INSERT INTO dbo.kasirMenu (loginID)
VALUES (5);


---------------------------------------------------------------------------------------------------------------------------------
--STOREDPROCEDURE--
-- =========================================
-- PRODUK
-- =========================================
CREATE PROCEDURE dbo.sp_InsertProduk
    @namaProduk VARCHAR(100),
    @harga DECIMAL(18,2),
    @stok INT,
    @outProdukID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF LEN(RTRIM(ISNULL(@namaProduk,''))) = 0
    BEGIN
        RAISERROR('Nama produk tidak boleh kosong',16,1);
        RETURN;
    END

    IF @harga < 0 OR @stok < 0
    BEGIN
        RAISERROR('Harga atau stok tidak valid',16,1);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO dbo.produk (namaProduk, harga, stok)
        VALUES (@namaProduk, @harga, @stok);

        SET @outProdukID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
        RETURN;
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_UpdateProduk
    @produkID INT,
    @namaProduk VARCHAR(100),
    @harga DECIMAL(18,2),
    @stok INT,
    @outRows INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @produkID IS NULL OR @produkID <= 0
    BEGIN
        RAISERROR('produkID tidak valid',16,1);
        RETURN;
    END

    BEGIN TRY
        UPDATE dbo.produk
        SET namaProduk = @namaProduk,
            harga = @harga,
            stok = @stok
        WHERE produkID = @produkID;

        SET @outRows = @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
        RETURN;
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_DeleteProduk
    @produkID INT,
    @outRows INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM dbo.produk
        WHERE produkID = @produkID;
        SET @outRows = @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_SearchProduk
    @query VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF @query IS NULL SET @query = '';

    SELECT produkID, namaProduk, harga, stok
    FROM dbo.vw_produk
    WHERE namaProduk LIKE '%' + @query + '%';
END
GO

-- =========================================
-- KASIR & LOGIN
-- =========================================
CREATE PROCEDURE dbo.sp_InsertKasir
    @username VARCHAR(50),
    @password VARCHAR(50),
    @outLoginID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF LEN(RTRIM(ISNULL(@username,''))) < 3 OR LEN(RTRIM(ISNULL(@password,''))) < 3
    BEGIN
        RAISERROR('Panjang username/password minimal 3 karakter',16,1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.login WHERE username = @username)
    BEGIN
        RAISERROR('Username sudah ada',16,1);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Insert ke tabel login
        INSERT INTO dbo.login (username, password, role)
        VALUES (@username, @password, 'kasir');

        SET @outLoginID = SCOPE_IDENTITY();

        -- Insert ke tabel kasirMenu karena kasir terikat ke login
        INSERT INTO dbo.kasirMenu (loginID)
        VALUES (@outLoginID);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_UpdateKasir
    @loginID INT,
    @username VARCHAR(50),
    @password VARCHAR(50),
    @outRows INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.login
        SET username = @username,
            password = @password
        WHERE loginID = @loginID AND role = 'kasir';

        SET @outRows = @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_DeleteKasir
    @loginID INT,
    @outRows INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Karena ada ON DELETE CASCADE, menghapus dari login otomatis menghapus data di kasirMenu
        DELETE FROM dbo.login
        WHERE loginID = @loginID AND role = 'kasir';

        SET @outRows = @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_SearchKasir
    @query VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF @query IS NULL SET @query = '';

    SELECT kasirID, loginID, username, role
    FROM dbo.vw_kasir
    WHERE username LIKE '%' + @query + '%';
END
GO

CREATE PROCEDURE dbo.sp_Login_Safe
    @username VARCHAR(50),
    @password VARCHAR(50),
    @outRole VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @outRole = role
    FROM dbo.login
    WHERE username = @username AND password = @password;
END
GO

CREATE PROCEDURE dbo.sp_Login_Insecure
    @username VARCHAR(50),
    @password VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sql VARCHAR(MAX);
    SET @sql = '
        SELECT role
        FROM dbo.login
        WHERE username = ''' + ISNULL(@username,'') + '''
          AND password = ''' + ISNULL(@password,'') + '''';
    EXEC(@sql);
END
GO

-- =========================================
-- TRANSAKSI
-- =========================================
CREATE PROCEDURE dbo.sp_CreateTransaction
    @kasirID INT,
    @pelangganID INT = NULL,
    @outTransaksiID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.transaksi (tanggal, totalHarga, kasirID, pelangganID)
        VALUES (GETDATE(), 0, @kasirID, @pelangganID);

        SET @outTransaksiID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_InsertDetailTransaksi
    @transaksiID INT,
    @produkID INT,
    @jumlah INT,
    @hargaSatuan DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.detailTransaksi (transaksiID, produkID, jumlah, hargaSatuan, total)
        VALUES (@transaksiID, @produkID, @jumlah, @hargaSatuan, @jumlah * @hargaSatuan);
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_UpdateTransaksi
    @transaksiID INT,
    @tanggal DATETIME,
    @totalHarga DECIMAL(18,2),
    @kasirID INT,
    @pelangganID INT = NULL,
    @outRows INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.transaksi
        SET tanggal = @tanggal,
            totalHarga = @totalHarga,
            kasirID = @kasirID,
            pelangganID = @pelangganID
        WHERE transaksiID = @transaksiID;

        SET @outRows = @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_DeleteTransaksi
    @transaksiID INT,
    @outRows INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Karena ada ON DELETE CASCADE pada tabel detailTransaksi, 
        -- cukup delete di tabel transaksi saja
        DELETE FROM dbo.transaksi
        WHERE transaksiID = @transaksiID;

        SET @outRows = @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @Err VARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_SearchTransaksi
    @fromDate DATETIME = NULL,
    @toDate DATETIME = NULL,
    @transaksiID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @fromDate IS NULL SET @fromDate = '1900-01-01';
    IF @toDate IS NULL SET @toDate = GETDATE();

    SELECT *
    FROM dbo.vw_transaksi_detail
    WHERE (@transaksiID IS NULL OR NomorTransaksi = @transaksiID)
      AND (Tanggal >= @fromDate AND Tanggal <= @toDate);
END
GO


---------------------------------------------------------------------------------------------------------------------------------

--TRIGGER---
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