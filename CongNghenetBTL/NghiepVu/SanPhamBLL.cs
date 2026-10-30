using System;
using System.Collections.Generic;
using System.IO;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.TruyCapDuLieu;

namespace CongNghenetBTL.NghiepVu
{
    public class SanPhamBLL
    {
        private readonly SanPhamDAL _sanPhamDAL = new SanPhamDAL();

        public List<SanPham> LayDanhSach(int? maDanhMuc = null, string tuKhoa = null, bool chiLayDangBan = false)
        {
            return _sanPhamDAL.LayDanhSach(maDanhMuc, tuKhoa, chiLayDangBan);
        }

        public SanPham LayTheoMa(int maSP)
        {
            return _sanPhamDAL.LayTheoMa(maSP);
        }

        public bool Them(SanPham sp, out string thongBao)
        {
            thongBao = string.Empty;
            if (!KiemTraHopLe(sp, out thongBao))
                return false;

            sp.HinhAnh = XuLyLuuAnh(sp.HinhAnh);

            int id = _sanPhamDAL.Them(sp);
            if (id > 0)
            {
                sp.MaSP = id;
                thongBao = "Thêm sản phẩm quần áo mới thành công!";
                return true;
            }

            thongBao = "Thêm sản phẩm thất bại. Vui lòng thử lại!";
            return false;
        }

        public bool CapNhat(SanPham sp, out string thongBao)
        {
            thongBao = string.Empty;
            if (sp.MaSP <= 0)
            {
                thongBao = "Mã sản phẩm không hợp lệ!";
                return false;
            }

            if (!KiemTraHopLe(sp, out thongBao))
                return false;

            sp.HinhAnh = XuLyLuuAnh(sp.HinhAnh);

            if (_sanPhamDAL.CapNhat(sp))
            {
                thongBao = "Cập nhật sản phẩm thành công!";
                return true;
            }

            thongBao = "Cập nhật sản phẩm thất bại!";
            return false;
        }

        public bool Xoa(int maSP, out string thongBao)
        {
            thongBao = string.Empty;
            if (_sanPhamDAL.KiemTraDaCoHoaDon(maSP))
            {
                _sanPhamDAL.CapNhatTrangThai(maSP, false);
                thongBao = "Sản phẩm đã có hóa đơn giao dịch nên đã được chuyển sang trạng thái 'Ngừng kinh doanh' để bảo toàn dữ liệu kế toán!";
                return true;
            }

            if (_sanPhamDAL.Xoa(maSP))
            {
                thongBao = "Xóa sản phẩm thành công khỏi hệ thống!";
                return true;
            }

            thongBao = "Xóa sản phẩm thất bại!";
            return false;
        }

        private bool KiemTraHopLe(SanPham sp, out string thongBao)
        {
            thongBao = string.Empty;
            if (string.IsNullOrWhiteSpace(sp.TenSP))
            {
                thongBao = "Tên sản phẩm không được để trống!";
                return false;
            }
            if (sp.GiaBan < 0)
            {
                thongBao = "Giá bán không được nhỏ hơn 0 đ!";
                return false;
            }
            if (sp.GiaNhap < 0)
            {
                thongBao = "Giá nhập không được nhỏ hơn 0 đ!";
                return false;
            }
            if (sp.SoLuongTon < 0)
            {
                thongBao = "Số lượng tồn kho không được âm!";
                return false;
            }
            return true;
        }

        public string XuLyLuuAnh(string duongDanNguon)
        {
            if (string.IsNullOrWhiteSpace(duongDanNguon))
                return "HinhAnh/mac_dinh.jpg";

            try
            {
                if (duongDanNguon.StartsWith("HinhAnh/", StringComparison.OrdinalIgnoreCase) ||
                    duongDanNguon.StartsWith("HinhAnh\\", StringComparison.OrdinalIgnoreCase))
                {
                    return duongDanNguon.Replace('\\', '/');
                }

                if (File.Exists(duongDanNguon))
                {
                    string thuMucAnh = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HinhAnh");
                    if (!Directory.Exists(thuMucAnh))
                    {
                        Directory.CreateDirectory(thuMucAnh);
                    }

                    string tenTep = Path.GetFileName(duongDanNguon);
                    string tenMoi = $"{DateTime.Now:yyyyMMddHHmmss}_{tenTep}";
                    string duongDanDich = Path.Combine(thuMucAnh, tenMoi);

                    File.Copy(duongDanNguon, duongDanDich, true);
                    return $"HinhAnh/{tenMoi}";
                }
            }
            catch
            {
            }

            return duongDanNguon;
        }
    }
}
