------------------------------------------
-- TẠO DATABASE
------------------------------------------
CREATE DATABASE WarehouseDB;
GO

USE WarehouseDB;
GO

------------------------------------------
-- 1. BẢNG PHÂN QUYỀN
------------------------------------------
CREATE TABLE Roles (
    RoleID INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    UserID INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID)
);

------------------------------------------
-- 2. NHÓM SẢN PHẨM & SẢN PHẨM
------------------------------------------
CREATE TABLE Categories (
    CategoryID INT IDENTITY PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    ProductID INT IDENTITY PRIMARY KEY,
    ProductName NVARCHAR(200) NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
	Quantity INT NOT NULL,
    Unit NVARCHAR(50),
    Price DECIMAL(18, 2),
    Description NVARCHAR(500)
);

------------------------------------------
-- 3. NHÀ CUNG CẤP
------------------------------------------
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY PRIMARY KEY,
    SupplierName NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(200)
);

------------------------------------------
-- 4. DANH SÁCH KHO
------------------------------------------
CREATE TABLE Warehouses (
    WarehouseID INT IDENTITY PRIMARY KEY,
    WarehouseName NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200)
);

------------------------------------------
-- 5. PHIẾU NHẬP KHO
------------------------------------------
CREATE TABLE ImportOrders (
    ImportID INT IDENTITY PRIMARY KEY,
    ImportDate DATETIME DEFAULT GETDATE(),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    WarehouseID INT FOREIGN KEY REFERENCES Warehouses(WarehouseID),
    Note NVARCHAR(200)
);

CREATE TABLE ImportOrderDetails (
    ImportDetailID INT IDENTITY PRIMARY KEY,
    ImportID INT FOREIGN KEY REFERENCES ImportOrders(ImportID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL
);

------------------------------------------
-- 6. PHIẾU XUẤT KHO
------------------------------------------
CREATE TABLE ExportOrders (
    ExportID INT IDENTITY PRIMARY KEY,
    ExportDate DATETIME DEFAULT GETDATE(),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    WarehouseID INT FOREIGN KEY REFERENCES Warehouses(WarehouseID),
    Note NVARCHAR(200)
);

CREATE TABLE ExportOrderDetails (
    ExportDetailID INT IDENTITY PRIMARY KEY,
    ExportID INT FOREIGN KEY REFERENCES ExportOrders(ExportID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL
);

USE WarehouseDB;
GO

------------------------------------------
-- 1. BẢNG PHÂN QUYỀN VÀ NGƯỜI DÙNG
------------------------------------------
-- Bảng Roles
INSERT INTO Roles (RoleName)
VALUES
(N'Admin'),
(N'Quản lý kho'),
(N'Nhân viên');
GO

-- Bảng Users (Giả sử PasswordHash là 'hashed_password' cho đơn giản)
INSERT INTO Users (Username, PasswordHash, FullName, RoleID)
VALUES
('admin', 'hashed_password_admin', N'Nguyễn Văn A', 1),
('manager', 'hashed_password_manager', N'Trần Thị B', 2),
('staff', 'hashed_password_staff', N'Lê Văn C', 3);
GO

------------------------------------------
-- 2. NHÓM SẢN PHẨM & SẢN PHẨM
------------------------------------------
-- Bảng Categories
INSERT INTO Categories (CategoryName)
VALUES
(N'Điện tử'),
(N'Văn phòng phẩm'),
(N'Hàng tiêu dùng');
GO

-- Bảng Products
INSERT INTO Products (ProductName, CategoryID, Unit, Price, Description)
VALUES
(N'Bàn phím cơ XYZ', 1, N'Cái', 1500000, N'Bàn phím cơ full-size'),
(N'Chuột quang A1', 1, N'Cái', 350000, N'Chuột quang không dây'),
(N'Giấy A4 Double A (Ram 500 tờ)', 2, N'Ram', 75000, N'Định lượng 70gsm'),
(N'Bút bi Thiên Long (Hộp 20 cái)', 2, N'Hộp', 40000, N'Mực xanh'),
(N'Nước lau sàn Sunlight', 3, N'Chai', 90000, N'Hương chanh 1L');
GO

------------------------------------------
-- 3. NHÀ CUNG CẤP
------------------------------------------
INSERT INTO Suppliers (SupplierName, Phone, Address)
VALUES
(N'Công ty TNHH Điện tử ABC', '0901234567', N'123 Đường X, Q.1, TP.HCM'),
(N'Tổng kho VPP Toàn Phát', '0987654321', N'456 Đường Y, Q.Ba Đình, Hà Nội'),
(N'Nhà phân phối Hàng Tiêu Dùng Z', '0123456789', N'789 Đường Z, Q.Hải Châu, Đà Nẵng');
GO

------------------------------------------
-- 4. DANH SÁCH KHO
------------------------------------------
INSERT INTO Warehouses (WarehouseName, Address)
VALUES
(N'Kho Tổng Hà Nội', N'Khu CN Thăng Long, Hà Nội'),
(N'Kho Miền Nam', N'Khu CN Sóng Thần, Bình Dương');
GO

------------------------------------------
-- 5. PHIẾU NHẬP KHO
------------------------------------------
-- Phiếu nhập 1 (Nhập hàng Điện tử vào Kho Hà Nội)
INSERT INTO ImportOrders (ImportDate, UserID, SupplierID, WarehouseID, Note)
VALUES
(GETDATE() - 2, 2, 1, 1, N'Nhập lô hàng bàn phím, chuột');
GO

-- Lấy ImportID vừa tạo (Giả sử là 1)
DECLARE @ImportID1 INT = SCOPE_IDENTITY();
INSERT INTO ImportOrderDetails (ImportID, ProductID, Quantity, Price)
VALUES
(@ImportID1, 1, 50, 1400000), -- ProductID 1: Bàn phím cơ
(@ImportID1, 2, 100, 300000); -- ProductID 2: Chuột quang
GO

-- Phiếu nhập 2 (Nhập VPP vào Kho Miền Nam)
INSERT INTO ImportOrders (ImportDate, UserID, SupplierID, WarehouseID, Note)
VALUES
(GETDATE() - 1, 3, 2, 2, N'Nhập giấy và bút');
GO

-- Lấy ImportID vừa tạo (Giả sử là 2)
DECLARE @ImportID2 INT = SCOPE_IDENTITY();
INSERT INTO ImportOrderDetails (ImportID, ProductID, Quantity, Price)
VALUES
(@ImportID2, 3, 200, 70000), -- ProductID 3: Giấy A4
(@ImportID2, 4, 500, 35000); -- ProductID 4: Bút bi
GO

------------------------------------------
-- 6. PHIẾU XUẤT KHO
------------------------------------------
-- Phiếu xuất 1 (Xuất VPP từ Kho Miền Nam)
INSERT INTO ExportOrders (ExportDate, UserID, WarehouseID, Note)
VALUES
(GETDATE(), 3, 2, N'Xuất VPP cho phòng Kế toán');
GO

-- Lấy ExportID vừa tạo (Giả sử là 1)
DECLARE @ExportID1 INT = SCOPE_IDENTITY();
INSERT INTO ExportOrderDetails (ExportID, ProductID, Quantity)
VALUES
(@ExportID1, 3, 10), -- ProductID 3: Giấy A4
(@ExportID1, 4, 50); -- ProductID 4: Bút bi
GO

-- Phiếu xuất 2 (Xuất Điện tử từ Kho Hà Nội)
INSERT INTO ExportOrders (ExportDate, UserID, WarehouseID, Note)
VALUES
(GETDATE(), 3, 1, N'Xuất 1 bàn phím cho phòng IT');
GO

-- Lấy ExportID vừa tạo (Giả sử là 2)
DECLARE @ExportID2 INT = SCOPE_IDENTITY();
INSERT INTO ExportOrderDetails (ExportID, ProductID, Quantity)
VALUES
(@ExportID2, 1, 1); -- ProductID 1: Bàn phím cơ
GO
