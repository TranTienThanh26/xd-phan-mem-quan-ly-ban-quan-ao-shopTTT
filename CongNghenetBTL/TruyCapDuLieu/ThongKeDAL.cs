using System;
using System.Data;
using MySqlConnector;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public class ThongKeDAL
    {
        public decimal LayTongDoanhThu()
        {
            string sql = "SELECT IFNULL(SUM(ThanhTien), 0) FROM HoaDon";
            object res = KetNoiCSDL.ThucThiGiaTri(sql);
            return res != null && res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }

        public decimal LayDoanhThuHomNay()
        {
            string sql = "SELECT IFNULL(SUM(ThanhTien), 0) FROM HoaDon WHERE DATE(NgayLap) = CURDATE()";
            object res = KetNoiCSDL.ThucThiGiaTri(sql);
            return res != null && res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }

        public decimal LayDoanhThuThangNay()
        {
            string sql = "SELECT IFNULL(SUM(ThanhTien), 0) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(CURDATE()) AND YEAR(NgayLap) = YEAR(CURDATE())";
            object res = KetNoiCSDL.ThucThiGiaTri(sql);
            return res != null && res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }

        public int LayTongSoDonHang()
        {
            string sql = "SELECT COUNT(1) FROM HoaDon";
            object res = KetNoiCSDL.ThucThiGiaTri(sql);
            return res != null && res != DBNull.Value ? Convert.ToInt32(res) : 0;
        }

        public int LayTongSoSanPhamQuanAo()
        {
            string sql = "SELECT COUNT(1) FROM SanPham WHERE TrangThai = 1";
            object res = KetNoiCSDL.ThucThiGiaTri(sql);
            return res != null && res != DBNull.Value ? Convert.ToInt32(res) : 0;
        }

        public int LayTongSoKhachHang()
        {
            string sql = "SELECT COUNT(1) FROM KhachHang";
            object res = KetNoiCSDL.ThucThiGiaTri(sql);
            return res != null && res != DBNull.Value ? Convert.ToInt32(res) : 0;
        }

        public DataTable LayDanhSachHoaDonGanDay(int top = 6)
        {
            string sql = $@"
                SELECT 
                    h.MaHD, 
                    IFNULL(k.HoTen, 'Khách vãng lai') AS TenKhachHang, 
                    IFNULL(u.HoTen, 'Hệ thống') AS NhanVienLap,
                    h.NgayLap, 
                    h.ThanhTien, 
                    h.PhuongThucTT
                FROM HoaDon h
                LEFT JOIN KhachHang k ON h.MaKH = k.MaKH
                LEFT JOIN NguoiDung u ON h.MaND = u.MaND
                ORDER BY h.NgayLap DESC
                LIMIT {top}";

            return KetNoiCSDL.ThucThiTruyVan(sql);
        }

        public DataTable LayDanhSachQuanAoMoiNhat(int top = 6)
        {
            string sql = $@"
                SELECT 
                    s.MaSP,
                    s.TenSP,
                    IFNULL(d.TenDanhMuc, 'Chưa phân loại') AS TenDanhMuc,
                    s.KichCo,
                    s.MauSac,
                    s.GiaBan,
                    s.SoLuongTon,
                    s.DonViTinh
                FROM SanPham s
                LEFT JOIN DanhMuc d ON s.MaDanhMuc = d.MaDanhMuc
                WHERE s.TrangThai = 1
                ORDER BY s.MaSP DESC
                LIMIT {top}";

            return KetNoiCSDL.ThucThiTruyVan(sql);
        }

        public DataTable LayTopSanPhamBanChay(int top = 5)
        {
            string sql = $@"
                SELECT 
                    sp.MaSP,
                    sp.TenSP,
                    sp.KichCo,
                    sp.MauSac,
                    sp.HinhAnh,
                    IFNULL(SUM(ct.SoLuong), 0) AS DaBan,
                    IFNULL(SUM(ct.ThanhTien), 0) AS DoanhThu
                FROM ChiTietHoaDon ct
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                GROUP BY sp.MaSP, sp.TenSP, sp.KichCo, sp.MauSac, sp.HinhAnh
                ORDER BY DaBan DESC
                LIMIT {top}";

            return KetNoiCSDL.ThucThiTruyVan(sql);
        }

        public DataTable LaySanPhamSapHetHang(int nguong = 15)
        {
            string sql = @"
                SELECT sp.MaSP, sp.TenSP, IFNULL(dm.TenDanhMuc, 'Chưa phân loại') AS TenDanhMuc,
                       sp.KichCo, sp.MauSac, sp.SoLuongTon, sp.GiaBan, sp.DonViTinh
                FROM SanPham sp
                LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
                WHERE sp.TrangThai = 1 AND sp.SoLuongTon <= @Nguong
                ORDER BY sp.SoLuongTon ASC";

            MySqlParameter[] thamSo = {
                new MySqlParameter("@Nguong", MySqlDbType.Int32) { Value = nguong }
            };

            return KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
        }

        public DataTable LayDoanhThuTheoPhuongThucTT()
        {
            string sql = @"
                SELECT PhuongThucTT, COUNT(1) AS SoDon, SUM(ThanhTien) AS DoanhThu
                FROM HoaDon
                GROUP BY PhuongThucTT";

            return KetNoiCSDL.ThucThiTruyVan(sql);
        }
    }
}
