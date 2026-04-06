-- =============================================
-- Tạo Database: qlbh_h
-- =============================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'qlbh_h')
BEGIN
    CREATE DATABASE qlbh_h;
END
GO

USE qlbh_h;
GO

-- =============================================
-- Bảng: NguoiDung
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NguoiDung' AND xtype='U')
BEGIN
    CREATE TABLE NguoiDung (
        ID       INT IDENTITY(1,1) PRIMARY KEY,
        TaiKhoan NVARCHAR(50)  NOT NULL,
        MatKhau  NVARCHAR(150) NOT NULL,
        HoTen    NVARCHAR(250) NULL
    );
END
GO

-- =============================================
-- Bảng: KhachHang
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KhachHang' AND xtype='U')
BEGIN
    CREATE TABLE KhachHang (
        ID      INT IDENTITY(1,1) PRIMARY KEY,
        Ten     NVARCHAR(250) NULL,
        SDT     NVARCHAR(20)  NULL,
        DiaChi  NVARCHAR(500) NULL,
        Email   NVARCHAR(100) NULL
    );
END
GO

-- =============================================
-- Bảng: SanPham
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SanPham' AND xtype='U')
BEGIN
    CREATE TABLE SanPham (
        ID      INT IDENTITY(1,1) PRIMARY KEY,
        MaSP    NVARCHAR(100) NULL,
        Ten     NVARCHAR(250) NULL,
        Gia     DECIMAL(18,0) NULL,
        Dvt     NVARCHAR(50)  NULL,
        SoLuong INT           NULL DEFAULT 0
    );
END
GO

-- =============================================
-- Bảng: MaHang
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MaHang' AND xtype='U')
BEGIN
    CREATE TABLE MaHang (
        ID   INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(250) NOT NULL
    );
END
GO

-- =============================================
-- Bảng: DonHang
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DonHang' AND xtype='U')
BEGIN
    CREATE TABLE DonHang (
        ID       INT IDENTITY(1,1) PRIMARY KEY,
        ID_KH    INT            NOT NULL,
        Ten_KH   NVARCHAR(250)  NULL,
        Loai     INT            NULL,   -- 1: Nhap hang, 2: Xuat hang
        TongTien DECIMAL(18,0)  NULL,
        Date     DATETIME       NULL,
        Status   BIT            NULL DEFAULT 0
    );
END
GO

-- =============================================
-- Bảng: ChiTietDonHang
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChiTietDonHang' AND xtype='U')
BEGIN
    CREATE TABLE ChiTietDonHang (
        ID           INT IDENTITY(1,1) PRIMARY KEY,
        ID_DH        INT            NOT NULL,
        ID_SP        INT            NOT NULL,
        Ten_SP       NVARCHAR(250)  NULL,
        SoLuong      INT            NOT NULL DEFAULT 0,
        Gia          DECIMAL(18,0)  NULL,
        ThanhTien    DECIMAL(18,0)  NULL,
        Updated_Time DATETIME       NULL,
        SL_Ton_Dau   INT            NULL,
        SL_Ton_Cuoi  INT            NULL,
        Dvt          NVARCHAR(50)   NULL
    );
END
GO

-- =============================================
-- Seed data: Tài khoản admin mặc định
-- =============================================
IF NOT EXISTS (SELECT 1 FROM NguoiDung WHERE TaiKhoan = 'admin')
BEGIN
    INSERT INTO NguoiDung (TaiKhoan, MatKhau, HoTen)
    VALUES ('admin', 'admin', N'Quản trị viên');
END
GO

-- =============================================
-- Seed data: Khách hàng mẫu
-- =============================================
IF NOT EXISTS (SELECT 1 FROM KhachHang)
BEGIN
    INSERT INTO KhachHang (Ten, SDT, DiaChi, Email) VALUES
    (N'Nguyễn Văn A', '0901234567', N'123 Lê Lợi, TP.HCM', 'nguyenvana@email.com'),
    (N'Trần Thị B',   '0912345678', N'456 Nguyễn Huệ, Hà Nội', 'tranthib@email.com'),
    (N'Công ty TNHH XYZ', '0283456789', N'789 Điện Biên Phủ, Đà Nẵng', 'xyz@company.com');
END
GO

-- =============================================
-- Seed data: Sản phẩm mẫu
-- =============================================
IF NOT EXISTS (SELECT 1 FROM SanPham)
BEGIN
    INSERT INTO SanPham (MaSP, Ten, Gia, Dvt, SoLuong) VALUES
    ('SP001', N'Laptop Dell Inspiron 15', 15000000, N'Cái', 0),
    ('SP002', N'Chuột không dây Logitech', 350000,  N'Cái', 0),
    ('SP003', N'Bàn phím cơ Keychron K2', 1800000, N'Cái', 0),
    ('SP004', N'Màn hình LG 24 inch',     4500000, N'Cái', 0),
    ('SP005', N'Tai nghe Sony WH-1000XM4',7500000, N'Cái', 0);
END
GO

PRINT N'Database qlbh_h đã được tạo thành công!';
GO
