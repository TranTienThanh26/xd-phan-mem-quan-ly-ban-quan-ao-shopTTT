using System;
using System.Collections.Generic;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.TienIch;
using CongNghenetBTL.TruyCapDuLieu;

namespace CongNghenetBTL.NghiepVu
{
    public class KhachHangBLL
    {
        private readonly KhachHangDAL _khachHangDAL = new KhachHangDAL();

        public List<KhachHang> LayDanhSach(string tuKhoa = null)
        {
            return _khachHangDAL.LayDanhSach(tuKhoa);
        }

        public KhachHang LayTheoMa(int maKH)
        {
            if (maKH <= 0) return null;
            return _khachHangDAL.LayTheoMa(maKH);
        }

        public KhachHang LayTheoSoDienThoai(string soDienThoai)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai)) return null;
            return _khachHangDAL.LayTheoSoDienThoai(soDienThoai.Trim());
        }

        public KhachHang DangNhap(string soDienThoai, string matKhau, out string thongBao)
        {
            thongBao = string.Empty;

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                thongBao = "Vui lòng nhập số điện thoại!";
                return null;
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                thongBao = "Vui lòng nhập mật khẩu!";
                return null;
            }

            KhachHang kh = _khachHangDAL.LayTheoSoDienThoai(soDienThoai.Trim());
            if (kh == null)
            {
                thongBao = "Số điện thoại này chưa được đăng ký trong hệ thống!";
                return null;
            }

            if (string.IsNullOrEmpty(kh.MatKhau))
            {
                if (matKhau == "123456")
                {
                    kh.MatKhau = MaHoaMatKhau.BamMatKhau("123456");
                    _khachHangDAL.DoiMatKhau(kh.MaKH, kh.MatKhau);
                    thongBao = "Đăng nhập thành công!";
                    return kh;
                }
                else
                {
                    thongBao = "Tài khoản của bạn chưa kích hoạt mật khẩu. Mật khẩu khởi tạo là 123456!";
                    return null;
                }
            }

            if (!MaHoaMatKhau.KiemTraMatKhau(matKhau, kh.MatKhau))
            {
                thongBao = "Mật khẩu không chính xác. Vui lòng kiểm tra lại!";
                return null;
            }

            thongBao = "Đăng nhập thành công!";
            return kh;
        }

        public bool DangKy(KhachHang kh, string matKhau, string xacNhanMatKhau, out string thongBao)
        {
            thongBao = string.Empty;

            if (string.IsNullOrWhiteSpace(kh.HoTen))
            {
                thongBao = "Họ tên khách hàng không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(kh.SoDienThoai))
            {
                thongBao = "Số điện thoại không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length < 4)
            {
                thongBao = "Mật khẩu phải có độ dài từ 4 ký tự trở lên!";
                return false;
            }

            if (matKhau != xacNhanMatKhau)
            {
                thongBao = "Mật khẩu xác nhận không khớp!";
                return false;
            }

            var khCu = _khachHangDAL.LayTheoSoDienThoai(kh.SoDienThoai.Trim());
            if (khCu != null)
            {
                thongBao = "Số điện thoại này đã được sử dụng! Vui lòng dùng số khác hoặc đăng nhập.";
                return false;
            }

            kh.MatKhau = MaHoaMatKhau.BamMatKhau(matKhau);
            kh.LoaiKhach = "Thường";
            kh.DiemTichLuy = 10;

            int id = _khachHangDAL.Them(kh);
            if (id > 0)
            {
                kh.MaKH = id;
                thongBao = "Đăng ký tài khoản khách hàng thành công! Bạn nhận được 10 điểm thưởng.";
                return true;
            }

            thongBao = "Đăng ký thất bại. Vui lòng thử lại sau!";
            return false;
        }

        public bool DoiMatKhau(int maKH, string matKhauCu, string matKhauMoi, string xacNhanMoi, out string thongBao)
        {
            thongBao = string.Empty;

            if (maKH <= 0)
            {
                thongBao = "Tài khoản không hợp lệ!";
                return false;
            }

            var kh = _khachHangDAL.LayTheoMa(maKH);
            if (kh == null)
            {
                thongBao = "Không tìm thấy thông tin tài khoản!";
                return false;
            }

            if (!string.IsNullOrEmpty(kh.MatKhau) && !MaHoaMatKhau.KiemTraMatKhau(matKhauCu, kh.MatKhau))
            {
                thongBao = "Mật khẩu hiện tại không chính xác!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 4)
            {
                thongBao = "Mật khẩu mới phải có từ 4 ký tự trở lên!";
                return false;
            }

            if (matKhauMoi != xacNhanMoi)
            {
                thongBao = "Xác nhận mật khẩu mới không trùng khớp!";
                return false;
            }

            string hashMoi = MaHoaMatKhau.BamMatKhau(matKhauMoi);
            if (_khachHangDAL.DoiMatKhau(maKH, hashMoi))
            {
                kh.MatKhau = hashMoi;
                thongBao = "Đổi mật khẩu thành công!";
                return true;
            }

            thongBao = "Đổi mật khẩu thất bại!";
            return false;
        }

        public bool Them(KhachHang kh, out string thongBao)
        {
            thongBao = string.Empty;
            if (string.IsNullOrWhiteSpace(kh.HoTen))
            {
                thongBao = "Họ tên khách hàng không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(kh.MatKhau))
            {
                kh.MatKhau = MaHoaMatKhau.BamMatKhau("123456");
            }

            int id = _khachHangDAL.Them(kh);
            if (id > 0)
            {
                kh.MaKH = id;
                thongBao = "Thêm khách hàng mới thành công!";
                return true;
            }

            thongBao = "Thêm khách hàng thất bại!";
            return false;
        }

        public bool CapNhat(KhachHang kh, out string thongBao)
        {
            thongBao = string.Empty;
            if (kh.MaKH <= 0)
            {
                thongBao = "Mã khách hàng không hợp lệ!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(kh.HoTen))
            {
                thongBao = "Họ tên khách hàng không được để trống!";
                return false;
            }

            if (_khachHangDAL.CapNhat(kh))
            {
                thongBao = "Cập nhật thông tin khách hàng thành công!";
                return true;
            }

            thongBao = "Cập nhật khách hàng thất bại!";
            return false;
        }

        public bool Xoa(int maKH, out string thongBao)
        {
            thongBao = string.Empty;
            if (_khachHangDAL.KiemTraCoHoaDon(maKH))
            {
                thongBao = "Không thể xóa khách hàng này vì đã có lịch sử hóa đơn mua hàng!";
                return false;
            }

            if (_khachHangDAL.Xoa(maKH))
            {
                thongBao = "Xóa khách hàng thành công!";
                return true;
            }

            thongBao = "Xóa khách hàng thất bại!";
            return false;
        }
    }
}
