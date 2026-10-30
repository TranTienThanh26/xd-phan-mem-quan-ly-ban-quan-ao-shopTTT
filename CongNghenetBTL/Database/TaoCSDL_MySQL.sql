-- ==============================================================
-- BÀI TẬP LỚN MÔN CÔNG NGHỆ .NET
-- ĐỀ TÀI: PHẦN MỀM QUẢN LÝ BÁN QUẦN ÁO THỜI TRANG (TTT STORE)
-- CƠ SỞ DỮ LIỆU: QLBanQuanAoDB (PHIÊN BẢN CHO MYSQL / MYSQL WORKBENCH)
-- ==============================================================

CREATE DATABASE IF NOT EXISTS QLBanQuanAoDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE QLBanQuanAoDB;

-- 1. XÓA BẢNG CŨ NẾU ĐÃ TỒN TẠI (TẮT TẠM THỜI RÀNG BUỘC ĐỂ DROP AN TOÀN)
SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS ChiTietHoaDon;
DROP TABLE IF EXISTS HoaDon;
DROP TABLE IF EXISTS KhachHang;
DROP TABLE IF EXISTS SanPham;
DROP TABLE IF EXISTS DanhMuc;
DROP TABLE IF EXISTS NguoiDung;
SET FOREIGN_KEY_CHECKS = 1;

-- 2. TẠO BẢNG TÀI KHOẢN NGƯỜI DÙNG
CREATE TABLE NguoiDung (
    MaND INT AUTO_INCREMENT PRIMARY KEY,
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(256) NOT NULL,
    HoTen VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NULL,
    SoDienThoai VARCHAR(20) NULL,
    VaiTro VARCHAR(50) NOT NULL DEFAULT 'Nhân viên',
    TrangThai TINYINT(1) NOT NULL DEFAULT 1,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. TẠO BẢNG DANH MỤC THỜI TRANG
CREATE TABLE DanhMuc (
    MaDanhMuc INT AUTO_INCREMENT PRIMARY KEY,
    TenDanhMuc VARCHAR(100) NOT NULL,
    MoTa VARCHAR(255) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4. TẠO BẢNG SẢN PHẨM QUẦN ÁO
CREATE TABLE SanPham (
    MaSP INT AUTO_INCREMENT PRIMARY KEY,
    TenSP VARCHAR(150) NOT NULL,
    MaDanhMuc INT NULL,
    KichCo VARCHAR(20) NULL,
    MauSac VARCHAR(50) NULL,
    ChatLieu VARCHAR(100) NULL,
    GiaNhap DECIMAL(18,2) DEFAULT 0,
    GiaBan DECIMAL(18,2) NOT NULL DEFAULT 0,
    SoLuongTon INT NOT NULL DEFAULT 0,
    DonViTinh VARCHAR(30) DEFAULT 'Cái',
    HinhAnh VARCHAR(255) NULL,
    MoTa VARCHAR(500) NULL,
    TrangThai TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5. TẠO BẢNG KHÁCH HÀNG (CÓ MẬT KHẨU MÃ HÓA BẢO MẬT & ĐIỂM TÍCH LŨY)
CREATE TABLE KhachHang (
    MaKH INT AUTO_INCREMENT PRIMARY KEY,
    HoTen VARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(20) NOT NULL UNIQUE,
    MatKhau VARCHAR(256) NULL,
    DiaChi VARCHAR(200) NULL,
    Email VARCHAR(100) NULL,
    LoaiKhach VARCHAR(50) DEFAULT 'Thường',
    DiemTichLuy INT DEFAULT 0,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 6. TẠO BẢNG HÓA ĐƠN BÁN HÀNG
CREATE TABLE HoaDon (
    MaHD INT AUTO_INCREMENT PRIMARY KEY,
    MaKH INT NULL,
    MaND INT NULL,
    NgayLap DATETIME DEFAULT CURRENT_TIMESTAMP,
    TongTien DECIMAL(18,2) DEFAULT 0,
    GiamGia DECIMAL(18,2) DEFAULT 0,
    ThanhTien DECIMAL(18,2) DEFAULT 0,
    PhuongThucTT VARCHAR(50) DEFAULT 'Tiền mặt',
    GhiChu VARCHAR(255) NULL,
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH) ON DELETE SET NULL,
    CONSTRAINT FK_HoaDon_NguoiDung FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 7. TẠO BẢNG CHI TIẾT HÓA ĐƠN
CREATE TABLE ChiTietHoaDon (
    MaCTHD INT AUTO_INCREMENT PRIMARY KEY,
    MaHD INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL DEFAULT 1,
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_ChiTiet_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTiet_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==============================================================
-- DỮ LIỆU KHỞI TẠO MẪU CHO MYSQL
-- ==============================================================

-- 1. Tài khoản quản trị viên mẫu (Admin)
INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, Email, SoDienThoai, VaiTro, TrangThai)
VALUES 
('admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Quản Trị Viên (Chủ Cửa Hàng)', 'admin@tttstore.vn', '0988888888', 'Quản trị viên', 1);

-- 2. Danh mục thời trang mẫu
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES 
('Áo sơ mi nam & nữ', 'Sơ mi công sở, sơ mi Oxford, sơ mi kiểu thời trang cao cấp'),
('Áo thun & Polo', 'Áo thun trơn, áo thun in hình, Polo thể thao thoáng mát'),
('Quần Jeans & Kaki', 'Quần jeans ống đứng, jeans suông, quần kaki công sở'),
('Váy & Đầm thiết kế', 'Váy đầm dạo phố, đầm hoa nhí, chân váy chữ A trẻ trung'),
('Áo khoác & Hoodie', 'Áo khoác gió bomber, áo dạ, hoodie nỉ ấm áp mùa đông'),
('Phụ kiện thời trang', 'Thắt lưng da, nón lưỡi trai, túi vải canvas');

-- 3. Sản phẩm quần áo mẫu với đầy đủ kích cỡ, màu sắc, chất liệu và hình ảnh
INSERT INTO SanPham (TenSP, MaDanhMuc, KichCo, MauSac, ChatLieu, GiaNhap, GiaBan, SoLuongTon, DonViTinh, HinhAnh, MoTa) VALUES
('Áo Sơ Mi Oxford Nam Dài Tay', 1, 'L', 'Trắng tinh khôi', 'Cotton Oxford cao cấp', 180000, 350000, 45, 'Cái', 'HinhAnh/ao_so_mi_oxford.jpg', 'Dáng regular fit, chuẩn phong cách thanh lịch'),
('Áo Sơ Mi Lụa Hàn Quốc Nữ', 1, 'M', 'Hồng pastel', 'Lụa tơ tằm mềm mại', 190000, 380000, 30, 'Cái', 'HinhAnh/ao_so_mi_nu.jpg', 'Cổ thắt nơ nữ tính, mặc thoáng mát cả ngày'),
('Áo Thun Polo Pique Phối Bo Cổ', 2, 'XL', 'Xanh Navy', 'CVC Pique thấm hút', 130000, 280000, 60, 'Cái', 'HinhAnh/ao_polo_pique.jpg', 'Form dáng thể thao trẻ trung, bền màu'),
('Áo Thun Form Rộng Unisex', 2, 'Freesize', 'Đen cá tính', 'Cotton 100% 2 chiều', 90000, 199000, 80, 'Cái', 'HinhAnh/ao_thun_unisex.jpg', 'Họa tiết in nổi bật, phong cách streetwear năng động'),
('Quần Jeans Nam Dáng Suông Regular', 3, '31', 'Xanh đậm vintage', 'Denim co giãn nhẹ', 250000, 490000, 35, 'Cái', 'HinhAnh/quan_jeans_regular.jpg', 'Chất vải denim bền bỉ, khóa kéo YKK chắc chắn'),
('Quần Kaki Ống Đứng Nam', 3, '32', 'Màu be (Beige)', 'Kaki cotton mềm mịn', 220000, 420000, 40, 'Cái', 'HinhAnh/quan_kaki_nam.jpg', 'Thích hợp đi làm công sở hoặc dự tiệc nhẹ nhàng'),
('Váy Đầm Hoa Nhí Dáng Xòe Nữ', 4, 'S', 'Vàng hoa nhí', 'Voan tơ 2 lớp bồng bềnh', 210000, 450000, 25, 'Cái', 'HinhAnh/vay_dam_hoa_nhi.jpg', 'Thiết kế tay bồng tiểu thư, tôn dáng thanh lịch'),
('Chân Váy Chữ A Xếp Ly Nữ', 4, 'M', 'Xám ghi', 'Tuyết mưa cao cấp', 120000, 260000, 35, 'Cái', 'HinhAnh/chan_vay_xep_ly.jpg', 'Dễ phối cùng áo sơ mi hoặc áo thun thời trang'),
('Áo Khoác Bomber Kaki 2 Lớp', 5, 'L', 'Xanh rêu', 'Kaki gió chống bụi', 310000, 590000, 20, 'Cái', 'HinhAnh/ao_khoac_bomber.jpg', 'Lớp lót dù giữ ấm tốt, có túi tiện lợi bên trong'),
('Áo Hoodie Nỉ Bông Trơn Unisex', 5, 'Freesize', 'Nâu Mocha', 'Nỉ bông dày dặn ấm áp', 190000, 390000, 50, 'Cái', 'HinhAnh/ao_hoodie_ni_bong.jpg', 'Mũ 2 lớp giữ form, ấm áp mùa đông');

-- 4. Khách hàng mẫu (Mật khẩu mặc định là '123456' đã được băm mã hóa SHA-256 an toàn)
INSERT INTO KhachHang (HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy) VALUES
('Trần Minh Hoàng', '0901234567', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Quận Cầu Giấy, Hà Nội', 'hoang.tm@gmail.com', 'VIP', 150),
('Nguyễn Thị Mai Linh', '0912348899', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Quận Ba Đình, Hà Nội', 'mailinh.nguyen@gmail.com', 'Thân thiết', 50),
('Lê Đức Thắng', '0987654321', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Quận Đống Đa, Hà Nội', 'thang.ld@gmail.com', 'Thường', 0);

-- 5. Hóa đơn và chi tiết hóa đơn mẫu
INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu)
VALUES (1, 1, DATE_SUB(NOW(), INTERVAL 1 DAY), 840000, 50000, 790000, 'Chuyển khoản', 'Khách VIP giảm giá 50k');

SET @MaHD1 = LAST_INSERT_ID();
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(@MaHD1, 1, 1, 350000, 350000),
(@MaHD1, 5, 1, 490000, 490000);

INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu)
VALUES (2, 1, NOW(), 730000, 0, 730000, 'Tiền mặt', 'Mua trực tiếp tại shop');

SET @MaHD2 = LAST_INSERT_ID();
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(@MaHD2, 3, 1, 280000, 280000),
(@MaHD2, 7, 1, 450000, 450000);
