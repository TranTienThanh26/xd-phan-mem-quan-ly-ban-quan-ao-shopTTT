

SET NAMES 'utf8mb4';
SET CHARACTER SET utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;


DROP DATABASE IF EXISTS QLBanQuanAoDB;
CREATE DATABASE QLBanQuanAoDB 
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE QLBanQuanAoDB;

-- Xóa các bảng cũ nếu đã tồn tại để tránh xung đột
DROP TABLE IF EXISTS ChiTietHoaDon;
DROP TABLE IF EXISTS HoaDon;
DROP TABLE IF EXISTS KhachHang;
DROP TABLE IF EXISTS SanPham;
DROP TABLE IF EXISTS DanhMuc;
DROP TABLE IF EXISTS NguoiDung;

CREATE TABLE NguoiDung (
    MaND INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Mã định danh người dùng nội bộ',
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE COMMENT 'Tên đăng nhập hệ thống (duy nhất)',
    MatKhau VARCHAR(256) NOT NULL COMMENT 'Mật khẩu bảo mật mã hóa băm SHA-256',
    HoTen VARCHAR(100) NOT NULL COMMENT 'Họ và tên nhân viên / quản trị viên',
    Email VARCHAR(100) NULL COMMENT 'Địa chỉ thư điện tử',
    SoDienThoai VARCHAR(20) NULL COMMENT 'Số điện thoại liên lạc',
    VaiTro VARCHAR(50) NOT NULL DEFAULT 'Nhân viên' COMMENT 'Vai trò: Quản trị viên, Nhân viên thu ngân',
    TrangThai TINYINT(1) NOT NULL DEFAULT 1 COMMENT 'Trạng thái: 1 = Đang hoạt động, 0 = Đã khóa',
    NgayTao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm cấp tài khoản'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng quản lý tài khoản nhân sự nội bộ';


CREATE TABLE DanhMuc (
    MaDanhMuc INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Mã định danh danh mục thời trang',
    TenDanhMuc VARCHAR(100) NOT NULL COMMENT 'Tên nhóm hàng: Áo sơ mi, Áo thun, Quần jeans, Váy...',
    MoTa VARCHAR(255) NULL COMMENT 'Mô tả chi tiết đặc trưng của nhóm hàng'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng phân loại danh mục sản phẩm';


CREATE TABLE SanPham (
    MaSP INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Mã sản phẩm quần áo',
    TenSP VARCHAR(150) NOT NULL COMMENT 'Tên sản phẩm thời trang',
    MaDanhMuc INT NULL COMMENT 'Khóa ngoại liên kết bảng DanhMuc',
    KichCo VARCHAR(20) NULL COMMENT 'Kích cỡ: S, M, L, XL, XXL, Freesize, 30, 31, 32',
    MauSac VARCHAR(50) NULL COMMENT 'Màu sắc: Trắng, Đen, Xanh Navy, Be, Xám...',
    ChatLieu VARCHAR(100) NULL COMMENT 'Chất liệu vải: Cotton Oxford, Lụa, Denim, Kaki, Voan...',
    GiaNhap DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Giá vốn nhập hàng từ xưởng/nhà cung cấp',
    GiaBan DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Giá bán niêm yết lẻ cho khách hàng',
    SoLuongTon INT NOT NULL DEFAULT 0 COMMENT 'Số lượng sản phẩm thực tế còn trong kho',
    DonViTinh VARCHAR(30) NOT NULL DEFAULT 'Cái' COMMENT 'Đơn vị tính: Cái, Chiếc, Bộ',
    HinhAnh VARCHAR(255) NULL COMMENT 'Đường dẫn tệp hình ảnh sản phẩm thực tế',
    MoTa VARCHAR(500) NULL COMMENT 'Mô tả kiểu dáng, form dáng và hướng dẫn giặt ủi',
    TrangThai TINYINT(1) NOT NULL DEFAULT 1 COMMENT '1 = Đang kinh doanh, 0 = Ngừng kinh doanh',
    CONSTRAINT FK_SanPham_DanhMuc 
        FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc) 
        ON DELETE SET NULL 
        ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng thông tin sản phẩm quần áo thời trang';

-- 2.4 BẢNG KHACHHANG (Thông tin hồ sơ khách hàng & Thẻ thành viên VIP)
CREATE TABLE KhachHang (
    MaKH INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Mã định danh khách hàng',
    HoTen VARCHAR(100) NOT NULL COMMENT 'Họ và tên khách hàng',
    SoDienThoai VARCHAR(20) NOT NULL UNIQUE COMMENT 'Số điện thoại đăng nhập & tích điểm (Duy nhất)',
    MatKhau VARCHAR(256) NULL COMMENT 'Mật khẩu băm SHA-256 (nếu tạo tài khoản mua online)',
    DiaChi VARCHAR(200) NULL COMMENT 'Địa chỉ giao hàng nhận sản phẩm',
    Email VARCHAR(100) NULL COMMENT 'Địa chỉ email nhận hóa đơn điện tử',
    LoaiKhach VARCHAR(50) NOT NULL DEFAULT 'Thường' COMMENT 'Phân hạng khách hàng: Thường, Thân thiết, VIP',
    DiemTichLuy INT NOT NULL DEFAULT 0 COMMENT 'Điểm tích lũy mua sắm (100 điểm = VIP giảm 10%)',
    NgayTao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm đăng ký tài khoản khách hàng'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng lưu trữ thông tin khách hàng và thẻ VIP';

-- 2.5 BẢNG HOADON (Đơn đặt hàng trực tuyến & Hóa đơn bán lẻ tại quầy POS)
CREATE TABLE HoaDon (
    MaHD INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Mã số hóa đơn',
    MaKH INT NULL COMMENT 'Khóa ngoại liên kết KhachHang (NULL nếu khách vãng lai)',
    MaND INT NULL COMMENT 'Khóa ngoại liên kết NguoiDung (Nhân viên lập hóa đơn)',
    NgayLap DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm xuất hóa đơn/đặt hàng',
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Tổng tiền hàng trước giảm giá',
    GiamGia DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Tiền chiết khấu (Giảm giá VIP, Mã khuyến mãi)',
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Tổng tiền thực tế phải thanh toán',
    PhuongThucTT VARCHAR(50) NOT NULL DEFAULT 'Tiền mặt' COMMENT 'Phương thức: Tiền mặt, Chuyển khoản, Thẻ, COD',
    GhiChu VARCHAR(255) NULL COMMENT 'Ghi chú đơn hàng (Địa chỉ ship, lưu ý đóng gói)',
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Đã hoàn thành' COMMENT 'Trạng thái: Đã hoàn thành, Chờ xử lý, Đã hủy',
    CONSTRAINT FK_HoaDon_KhachHang 
        FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH) 
        ON DELETE SET NULL 
        ON UPDATE CASCADE,
    CONSTRAINT FK_HoaDon_NguoiDung 
        FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND) 
        ON DELETE SET NULL 
        ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng lưu thông tin tổng quan các hóa đơn bán hàng';

-- 2.6 BẢNG CHITIETHOADON (Các dòng sản phẩm trong từng hóa đơn)
CREATE TABLE ChiTietHoaDon (
    MaCTHD INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Mã định danh dòng chi tiết hóa đơn',
    MaHD INT NOT NULL COMMENT 'Khóa ngoại liên kết bảng HoaDon',
    MaSP INT NOT NULL COMMENT 'Khóa ngoại liên kết bảng SanPham',
    SoLuong INT NOT NULL DEFAULT 1 COMMENT 'Số lượng sản phẩm mua',
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Đơn giá bán tại thời điểm xuất hóa đơn',
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0.00 COMMENT 'Thành tiền = SoLuong * DonGia',
    CONSTRAINT FK_ChiTiet_HoaDon 
        FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE,
    CONSTRAINT FK_ChiTiet_SanPham 
        FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP) 
        ON DELETE RESTRICT 
        ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng lưu chi tiết các món hàng trong hóa đơn';

SET FOREIGN_KEY_CHECKS = 1;

-- -----------------------------------------------------------------------------
-- 3. TẠO CÁC CHỈ MỤC TỐI ƯU HÓA TRUY VẤN (INDEXES)
-- -----------------------------------------------------------------------------
CREATE INDEX IDX_SanPham_DanhMuc ON SanPham(MaDanhMuc);
CREATE INDEX IDX_SanPham_TrangThai ON SanPham(TrangThai);
CREATE INDEX IDX_SanPham_GiaBan ON SanPham(GiaBan);
CREATE INDEX IDX_SanPham_TimKiem ON SanPham(TenSP, KichCo, MauSac);

CREATE INDEX IDX_KhachHang_SDT ON KhachHang(SoDienThoai);
CREATE INDEX IDX_KhachHang_LoaiKhach ON KhachHang(LoaiKhach);

CREATE INDEX IDX_NguoiDung_TenDangNhap ON NguoiDung(TenDangNhap);

CREATE INDEX IDX_HoaDon_NgayLap ON HoaDon(NgayLap);
CREATE INDEX IDX_HoaDon_KhachHang ON HoaDon(MaKH);
CREATE INDEX IDX_HoaDon_NguoiDung ON HoaDon(MaND);
CREATE INDEX IDX_HoaDon_TrangThai ON HoaDon(TrangThai);

CREATE INDEX IDX_CTHD_HoaDon ON ChiTietHoaDon(MaHD);
CREATE INDEX IDX_CTHD_SanPham ON ChiTietHoaDon(MaSP);

-- -----------------------------------------------------------------------------
-- 4. TẠO CÁC KHUNG NHÌN BÁO CÁO (VIEWS)
-- -----------------------------------------------------------------------------

-- 4.1 View thông tin sản phẩm kèm tên danh mục và tình trạng kho
CREATE OR REPLACE VIEW v_SanPhamChiTiet AS
SELECT 
    sp.MaSP,
    sp.TenSP,
    sp.MaDanhMuc,
    IFNULL(dm.TenDanhMuc, 'Chưa phân loại') AS TenDanhMuc,
    sp.KichCo,
    sp.MauSac,
    sp.ChatLieu,
    sp.GiaNhap,
    sp.GiaBan,
    sp.SoLuongTon,
    CASE 
        WHEN sp.SoLuongTon <= 0 THEN 'Hết hàng'
        WHEN sp.SoLuongTon < 10 THEN 'Sắp hết hàng'
        ELSE 'Còn hàng'
    END AS TinhTrangKho,
    sp.DonViTinh,
    sp.HinhAnh,
    sp.MoTa,
    sp.TrangThai
FROM SanPham sp
LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc;

-- 4.2 View chi tiết hóa đơn bán hàng đầy đủ tên khách và nhân viên
CREATE OR REPLACE VIEW v_HoaDonDayDu AS
SELECT 
    hd.MaHD,
    hd.NgayLap,
    hd.MaKH,
    IFNULL(kh.HoTen, 'Khách vãng lai') AS TenKhachHang,
    IFNULL(kh.SoDienThoai, '-') AS SoDienThoaiKhach,
    IFNULL(kh.LoaiKhach, 'Thường') AS HangThanhVien,
    hd.MaND,
    IFNULL(nd.HoTen, 'Hệ thống Online') AS NhanVienLap,
    hd.TongTien,
    hd.GiamGia,
    hd.ThanhTien,
    hd.PhuongThucTT,
    hd.TrangThai,
    hd.GhiChu
FROM HoaDon hd
LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
LEFT JOIN NguoiDung nd ON hd.MaND = nd.MaND;

-- 4.3 View thống kê doanh thu theo từng ngày
CREATE OR REPLACE VIEW v_DoanhThuTheoNgay AS
SELECT 
    DATE(NgayLap) AS Ngay,
    COUNT(MaHD) AS SoLuongDon,
    SUM(TongTien) AS TongTienHang,
    SUM(GiamGia) AS TongTienGiam,
    SUM(ThanhTien) AS DoanhThuThucTe
FROM HoaDon
WHERE TrangThai = 'Đã hoàn thành'
GROUP BY DATE(NgayLap)
ORDER BY Ngay DESC;

-- 4.4 View top 10 sản phẩm thời trang bán chạy nhất
CREATE OR REPLACE VIEW v_TopSanPhamBanChay AS
SELECT 
    sp.MaSP,
    sp.TenSP,
    IFNULL(dm.TenDanhMuc, 'Khác') AS TenDanhMuc,
    sp.GiaBan,
    SUM(ct.SoLuong) AS TongSoLuongDaBan,
    SUM(ct.ThanhTien) AS TongDoanhThu
FROM ChiTietHoaDon ct
JOIN SanPham sp ON ct.MaSP = sp.MaSP
LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
JOIN HoaDon hd ON ct.MaHD = hd.MaHD
WHERE hd.TrangThai = 'Đã hoàn thành'
GROUP BY sp.MaSP, sp.TenSP, dm.TenDanhMuc, sp.GiaBan
ORDER BY TongSoLuongDaBan DESC
LIMIT 10;

-- -----------------------------------------------------------------------------
-- 5. TẠO CÁC TRIGGER TỰ ĐỘNG HÓA NGHIỆP VỤ (TRIGGERS)
-- -----------------------------------------------------------------------------
DELIMITER $$

-- 5.1 Trigger tự động tính Thành tiền của Chi tiết hóa đơn
DROP TRIGGER IF EXISTS trg_ChiTietHoaDon_BeforeInsert$$
CREATE TRIGGER trg_ChiTietHoaDon_BeforeInsert
BEFORE INSERT ON ChiTietHoaDon
FOR EACH ROW
BEGIN
    SET NEW.ThanhTien = NEW.SoLuong * NEW.DonGia;
END$$

DROP TRIGGER IF EXISTS trg_ChiTietHoaDon_BeforeUpdate$$
CREATE TRIGGER trg_ChiTietHoaDon_BeforeUpdate
BEFORE UPDATE ON ChiTietHoaDon
FOR EACH ROW
BEGIN
    SET NEW.ThanhTien = NEW.SoLuong * NEW.DonGia;
END$$

-- 5.2 Trigger tự động nâng hạng khách hàng VIP khi điểm tích lũy >= 100
DROP TRIGGER IF EXISTS trg_KhachHang_NangHangVIP$$
CREATE TRIGGER trg_KhachHang_NangHangVIP
BEFORE UPDATE ON KhachHang
FOR EACH ROW
BEGIN
    IF NEW.DiemTichLuy >= 100 AND NEW.LoaiKhach != 'VIP' THEN
        SET NEW.LoaiKhach = 'VIP';
    ELSEIF NEW.DiemTichLuy >= 30 AND NEW.DiemTichLuy < 100 AND NEW.LoaiKhach = 'Thường' THEN
        SET NEW.LoaiKhach = 'Thân thiết';
    END IF;
END$$

DELIMITER ;

-- -----------------------------------------------------------------------------
-- 6. TẠO CÁC THỦ TỤC LƯU TRỮ CHUYÊN NGHIỆP (STORED PROCEDURES)
-- -----------------------------------------------------------------------------
DELIMITER $$

-- 6.1 Thủ tục đăng ký tài khoản khách hàng mới (an toàn, kiểm tra trùng lặp)
DROP PROCEDURE IF EXISTS sp_DangKyKhachHang$$
CREATE PROCEDURE sp_DangKyKhachHang(
    IN p_HoTen VARCHAR(100),
    IN p_SoDienThoai VARCHAR(20),
    IN p_MatKhauSHA256 VARCHAR(256),
    IN p_DiaChi VARCHAR(200),
    IN p_Email VARCHAR(100),
    OUT p_MaKHMoi INT,
    OUT p_ThanhCong BOOLEAN,
    OUT p_ThongBao VARCHAR(255)
)
BEGIN
    DECLARE v_TonTai INT DEFAULT 0;

    -- Kiểm tra số điện thoại đã tồn tại chưa
    SELECT COUNT(*) INTO v_TonTai FROM KhachHang WHERE SoDienThoai = TRIM(p_SoDienThoai);

    IF v_TonTai > 0 THEN
        SET p_MaKHMoi = 0;
        SET p_ThanhCong = FALSE;
        SET p_ThongBao = 'Số điện thoại này đã được đăng ký trong hệ thống!';
    ELSE
        INSERT INTO KhachHang (HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy)
        VALUES (p_HoTen, TRIM(p_SoDienThoai), p_MatKhauSHA256, p_DiaChi, p_Email, 'Thường', 0);

        SET p_MaKHMoi = LAST_INSERT_ID();
        SET p_ThanhCong = TRUE;
        SET p_ThongBao = 'Đăng ký tài khoản khách hàng thành công!';
    END IF;
END$$

-- 6.2 Thủ tục hủy hóa đơn và tự động hoàn trả tồn kho sản phẩm trong Transaction
DROP PROCEDURE IF EXISTS sp_HuyHoaDon$$
CREATE PROCEDURE sp_HuyHoaDon(
    IN p_MaHD INT,
    OUT p_ThanhCong BOOLEAN,
    OUT p_ThongBao VARCHAR(255)
)
BEGIN
    DECLARE v_TrangThai VARCHAR(50);
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_ThanhCong = FALSE;
        SET p_ThongBao = 'Lỗi hệ thống trong quá trình hủy đơn hàng!';
    END;

    START TRANSACTION;

    SELECT TrangThai INTO v_TrangThai FROM HoaDon WHERE MaHD = p_MaHD FOR UPDATE;

    IF v_TrangThai IS NULL THEN
        SET p_ThanhCong = FALSE;
        SET p_ThongBao = 'Không tìm thấy hóa đơn cần hủy!';
        ROLLBACK;
    ELSEIF v_TrangThai = 'Đã hủy' THEN
        SET p_ThanhCong = FALSE;
        SET p_ThongBao = 'Hóa đơn này trước đó đã được hủy rồi!';
        ROLLBACK;
    ELSE
        -- 1. Hoàn trả số lượng tồn kho cho các sản phẩm trong hóa đơn
        UPDATE SanPham sp
        JOIN ChiTietHoaDon ct ON sp.MaSP = ct.MaSP
        SET sp.SoLuongTon = sp.SoLuongTon + ct.SoLuong
        WHERE ct.MaHD = p_MaHD;

        -- 2. Đánh dấu trạng thái hóa đơn là Đã hủy
        UPDATE HoaDon
        SET TrangThai = 'Đã hủy',
            GhiChu = CONCAT(IFNULL(GhiChu, ''), ' [Hủy lúc: ', NOW(), ']')
        WHERE MaHD = p_MaHD;

        COMMIT;
        SET p_ThanhCong = TRUE;
        SET p_ThongBao = 'Hủy hóa đơn và hoàn trả tồn kho thành công!';
    END IF;
END$$

-- 6.3 Thủ tục báo cáo tổng hợp doanh thu theo khoảng ngày
DROP PROCEDURE IF EXISTS sp_BaoCaoDoanhThu$$
CREATE PROCEDURE sp_BaoCaoDoanhThu(
    IN p_TuNgay DATE,
    IN p_DenNgay DATE
)
BEGIN
    SELECT 
        COUNT(MaHD) AS TongSoDonHang,
        IFNULL(SUM(TongTien), 0) AS TongTienGoc,
        IFNULL(SUM(GiamGia), 0) AS TongTienChietKhau,
        IFNULL(SUM(ThanhTien), 0) AS DoanhThuThucNhan,
        IFNULL(AVG(ThanhTien), 0) AS GiaTriDonTrungBinh
    FROM HoaDon
    WHERE TrangThai = 'Đã hoàn thành'
      AND DATE(NgayLap) BETWEEN p_TuNgay AND p_DenNgay;
END$$

DELIMITER ;

-- =============================================================================
-- 7. NẠP DỮ LIỆU KHỞI TẠO MẪU (SEED DATA - PHONG PHÚ & THỰC TẾ)
-- =============================================================================

-- 7.1 DỮ LIỆU TÀI KHOẢN NỘI BỘ (ADMIN & NHÂN VIÊN)
-- Mật khẩu mặc định là: 123456 (đã được băm bằng thuật toán SHA-256)
-- Chuỗi SHA-256 của '123456': 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, Email, SoDienThoai, VaiTro, TrangThai) VALUES
('admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Quản Trị Viên (Admin Shop TTT)', 'admin@tttstore.vn', '0988888888', 'Quản trị viên', 1),
('thungan01', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Trần Thu Ngân', 'thungan01@tttstore.vn', '0911223344', 'Nhân viên', 1),
('nv_kho', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Lê Văn Kho', 'kho@tttstore.vn', '0933445566', 'Nhân viên', 1);

-- 7.2 DỮ LIỆU DANH MỤC THỜI TRANG
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES
('Áo sơ mi nam & nữ', 'Sơ mi công sở, sơ mi Oxford, sơ mi lụa Hàn Quốc cao cấp'),
('Áo thun & Polo', 'Áo thun trơn, thun Unisex, áo Polo thể thao dạo phố thoáng mát'),
('Quần Jeans & Kaki', 'Quần jeans suông, jeans regular, quần kaki công sở thanh lịch'),
('Váy & Đầm thiết kế', 'Váy hoa nhí, đầm dạ hội, chân váy xếp ly chữ A phong cách trẻ trung'),
('Áo khoác & Hoodie', 'Áo khoác bomber, áo gió 2 lớp, hoodie nỉ ấm áp phong cách streetwear'),
('Phụ kiện thời trang', 'Thắt lưng da bò, mũ nón lưỡi trai, túi canvas thời trang');

-- 7.3 DỮ LIỆU SẢN PHẨM QUẦN ÁO THỜI TRANG (KHỚP VỚI ẢNH THẬT TRONG DỰ ÁN)
INSERT INTO SanPham (TenSP, MaDanhMuc, KichCo, MauSac, ChatLieu, GiaNhap, GiaBan, SoLuongTon, DonViTinh, HinhAnh, MoTa, TrangThai) VALUES
('Áo Sơ Mi Oxford Nam Dài Tay', 1, 'L', 'Trắng tinh khôi', 'Cotton Oxford cao cấp', 180000.00, 350000.00, 45, 'Cái', 'HinhAnh/ao_so_mi_oxford.jpg', 'Dáng regular fit, chuẩn phong cách thanh lịch công sở', 1),
('Áo Sơ Mi Lụa Hàn Quốc Nữ', 1, 'M', 'Hồng pastel', 'Lụa tơ tằm mềm mại', 190000.00, 380000.00, 30, 'Cái', 'HinhAnh/ao_so_mi_nu.jpg', 'Cổ thắt nơ nữ tính, mặc thoáng mát cả ngày', 1),
('Áo Thun Polo Pique Phối Bo Cổ', 2, 'XL', 'Xanh Navy', 'CVC Pique thấm hút', 130000.00, 280000.00, 60, 'Cái', 'HinhAnh/ao_polo_pique.jpg', 'Form dáng thể thao trẻ trung, bền màu', 1),
('Áo Thun Form Rộng Unisex', 2, 'Freesize', 'Đen cá tính', 'Cotton 100% 2 chiều', 90000.00, 199000.00, 80, 'Cái', 'HinhAnh/ao_thun_unisex.jpg', 'Họa tiết in nổi bật, phong cách streetwear năng động', 1),
('Quần Jeans Nam Dáng Suông Regular', 3, '31', 'Xanh đậm vintage', 'Denim co giãn nhẹ', 250000.00, 490000.00, 35, 'Cái', 'HinhAnh/quan_jeans_regular.jpg', 'Chất vải denim bền bỉ, khóa kéo YKK chắc chắn', 1),
('Quần Kaki Ống Đứng Nam', 3, '32', 'Màu be (Beige)', 'Kaki cotton mềm mịn', 220000.00, 420000.00, 40, 'Cái', 'HinhAnh/quan_kaki_nam.jpg', 'Thích hợp đi làm công sở hoặc dự tiệc nhẹ nhàng', 1),
('Váy Đầm Hoa Nhí Dáng Xòe Nữ', 4, 'S', 'Vàng hoa nhí', 'Voan tơ 2 lớp bồng bềnh', 210000.00, 450000.00, 25, 'Cái', 'HinhAnh/vay_dam_hoa_nhi.jpg', 'Thiết kế tay bồng tiểu thư, tôn dáng thanh lịch', 1),
('Chân Váy Chữ A Xếp Ly Nữ', 4, 'M', 'Xám ghi', 'Tuyết mưa cao cấp', 120000.00, 260000.00, 35, 'Cái', 'HinhAnh/chan_vay_xep_ly.jpg', 'Dễ phối cùng áo sơ mi hoặc áo thun thời trang', 1),
('Áo Khoác Bomber Kaki 2 Lớp', 5, 'L', 'Xanh rêu', 'Kaki gió chống bụi', 310000.00, 590000.00, 20, 'Cái', 'HinhAnh/ao_khoac_bomber.jpg', 'Lớp lót dù giữ ấm tốt, có túi tiện lợi bên trong', 1),
('Áo Hoodie Nỉ Bông Trơn Unisex', 5, 'Freesize', 'Nâu Mocha', 'Nỉ bông dày dặn ấm áp', 190000.00, 390000.00, 50, 'Cái', 'HinhAnh/ao_hoodie_ni_bong.jpg', 'Mũ 2 lớp giữ form, ấm áp mùa đông', 1),
('Áo Thun Cotton HIDDLE™ Cổ Tròn', 2, 'L', 'Trắng chữ xanh', 'Cotton 100% 2 chiều', 130000.00, 250000.00, 45, 'Cái', 'HinhAnh/ao_thun_hiddle.jpg', 'Áo thun cộc tay in chữ HIDDLE™ trẻ trung, thoáng mát năng động', 1),
('Áo Thun Basic Trắng Trơn Unisex', 2, 'M', 'Trắng tinh khôi', 'Cotton Compact cao cấp', 95000.00, 190000.00, 60, 'Cái', 'HinhAnh/ao_thun_basic_trang.jpg', 'Áo phông trơn form rộng unisex, phong cách tối giản dễ phối đồ', 1);

-- 7.4 DỮ LIỆU KHÁCH HÀNG (MẬT KHẨU BĂM SHA-256 LÀ '123456')
INSERT INTO KhachHang (HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy) VALUES
('Trần Minh Hoàng', '0901234567', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Số 18 Hoàng Quốc Việt, Cầu Giấy, Hà Nội', 'hoang.tm@gmail.com', 'VIP', 150),
('Nguyễn Thị Mai Linh', '0912348899', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Số 45 Kim Mã, Ba Đình, Hà Nội', 'mailinh.nguyen@gmail.com', 'Thân thiết', 60),
('Lê Đức Thắng', '0987654321', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Số 88 Xã Đàn, Đống Đa, Hà Nội', 'thang.ld@gmail.com', 'Thường', 15),
('Phạm Thu Trang', '0934567890', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Số 102 Nguyễn Trãi, Thanh Xuân, Hà Nội', 'trang.pt@gmail.com', 'Thường', 0),
('Vũ Đình Nam', '0977889900', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Số 12 Lê Duẩn, Hoàn Kiếm, Hà Nội', 'nam.vd@gmail.com', 'VIP', 220);

-- 7.5 DỮ LIỆU HÓA ĐƠN BÁN HÀNG VÀ CHI TIẾT HÓA ĐƠN MẪU

-- Hóa đơn 1: Khách VIP mua online (Được giảm giá VIP 50k)
INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu, TrangThai)
VALUES (1, NULL, DATE_SUB(NOW(), INTERVAL 3 DAY), 840000.00, 50000.00, 790000.00, 'Chuyển khoản', 'Giao hàng giờ hành chính', 'Đã hoàn thành');
SET @MaHD1 = LAST_INSERT_ID();

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(@MaHD1, 1, 1, 350000.00, 350000.00), -- Áo Sơ Mi Oxford Nam
(@MaHD1, 5, 1, 490000.00, 490000.00); -- Quần Jeans Nam Dáng Suông

-- Hóa đơn 2: Mua trực tiếp tại quầy POS (Thu ngân 1 lập cho khách vãng lai)
INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu, TrangThai)
VALUES (NULL, 2, DATE_SUB(NOW(), INTERVAL 2 DAY), 579000.00, 0.00, 579000.00, 'Tiền mặt', 'Bán lẻ tại quầy Shop TTT', 'Đã hoàn thành');
SET @MaHD2 = LAST_INSERT_ID();

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(@MaHD2, 3, 1, 280000.00, 280000.00), -- Áo Polo Pique
(@MaHD2, 4, 1, 199000.00, 199000.00), -- Áo Thun Unisex
(@MaHD2, 11, 1, 100000.00, 100000.00); -- Phụ kiện

-- Hóa đơn 3: Khách thân thiết mua online hôm nay
INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu, TrangThai)
VALUES (2, NULL, NOW(), 710000.00, 30000.00, 680000.00, 'Chuyển khoản', 'Khách thân thiết tích điểm', 'Đã hoàn thành');
SET @MaHD3 = LAST_INSERT_ID();

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(@MaHD3, 2, 1, 380000.00, 380000.00), -- Áo Sơ Mi Lụa Hàn Quốc Nữ
(@MaHD3, 8, 1, 260000.00, 260000.00), -- Chân Váy Chữ A Xếp Ly
(@MaHD3, 12, 1, 70000.00, 70000.00);  -- Túi Vải Canvas

-- Hóa đơn 4: Khách VIP mua tại quầy hôm nay
INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu, TrangThai)
VALUES (5, 2, NOW(), 980000.00, 98000.00, 882000.00, 'Chuyển khoản', 'Khách VIP giảm trực tiếp 10%', 'Đã hoàn thành');
SET @MaHD4 = LAST_INSERT_ID();

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(@MaHD4, 9, 1, 590000.00, 590000.00), -- Áo Khoác Bomber Kaki
(@MaHD4, 10, 1, 390000.00, 390000.00); -- Áo Hoodie Nỉ Bông

-- -----------------------------------------------------------------------------
-- 8. TRUY VẤN KIỂM TRA TÍNH TOÀN VẸN CỦA DỮ LIỆU
-- -----------------------------------------------------------------------------
SELECT '=== DANH MỤC THỜI TRANG ===' AS ThongBao;
SELECT MaDanhMuc, TenDanhMuc, MoTa FROM DanhMuc;

SELECT '=== SẢN PHẨM QUẦN ÁO THỜI TRANG ===' AS ThongBao;
SELECT MaSP, TenSP, TenDanhMuc, GiaBan, SoLuongTon, TinhTrangKho FROM v_SanPhamChiTiet;

SELECT '=== TÀI KHOẢN QUẢN TRỊ NỘI BỘ ===' AS ThongBao;
SELECT MaND, TenDangNhap, HoTen, VaiTro, TrangThai FROM NguoiDung;

SELECT '=== DANH SÁCH KHÁCH HÀNG & THẺ VIP ===' AS ThongBao;
SELECT MaKH, HoTen, SoDienThoai, LoaiKhach, DiemTichLuy FROM KhachHang;

SELECT '=== DOANH THU THEO NGÀY ===' AS ThongBao;
SELECT * FROM v_DoanhThuTheoNgay;

SELECT '=== CƠ SỞ DỮ LIỆU QLBANQUANAODB ĐÃ ĐƯỢC TẠO THÀNH CÔNG ===' AS KetQua;
