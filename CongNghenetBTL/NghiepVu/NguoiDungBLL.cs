using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.TienIch;
using CongNghenetBTL.TruyCapDuLieu;

namespace CongNghenetBTL.NghiepVu
{
    public class NguoiDungBLL
    {
        private readonly NguoiDungDAL _dal = new NguoiDungDAL();

        public NguoiDung DangNhap(string tenDangNhap, string matKhau, out string thongBao)
        {
            thongBao = string.Empty;

            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                thongBao = "Vui lòng nhập tên đăng nhập!";
                return null;
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                thongBao = "Vui lòng nhập mật khẩu!";
                return null;
            }

            NguoiDung user = _dal.LayTheoTenDangNhap(tenDangNhap.Trim());
            if (user == null)
            {
                thongBao = "Tài khoản không tồn tại trên hệ thống!";
                return null;
            }

            if (!user.TrangThai)
            {
                thongBao = "Tài khoản này đã bị khóa. Vui lòng liên hệ quản lý cửa hàng!";
                return null;
            }

            if (!MaHoaMatKhau.KiemTraMatKhau(matKhau, user.MatKhau))
            {
                thongBao = "Mật khẩu không chính xác. Vui lòng thử lại!";
                return null;
            }

            thongBao = "Đăng nhập thành công!";
            return user;
        }

        public bool DangKy(NguoiDung nd, string xacNhanMatKhau, out string thongBao)
        {
            thongBao = string.Empty;

            if (nd == null)
            {
                thongBao = "Thông tin đăng ký không hợp lệ!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nd.TenDangNhap))
            {
                thongBao = "Tên đăng nhập không được để trống!";
                return false;
            }

            string username = nd.TenDangNhap.Trim();
            if (username.Length < 3 || username.Length > 30)
            {
                thongBao = "Tên đăng nhập phải có từ 3 đến 30 ký tự!";
                return false;
            }

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                thongBao = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới (_)!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nd.HoTen))
            {
                thongBao = "Họ và tên nhân viên không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nd.MatKhau))
            {
                thongBao = "Mật khẩu không được để trống!";
                return false;
            }

            if (nd.MatKhau.Length < 6)
            {
                thongBao = "Mật khẩu phải có ít nhất 6 ký tự!";
                return false;
            }

            if (!string.Equals(nd.MatKhau, xacNhanMatKhau))
            {
                thongBao = "Mật khẩu xác nhận không khớp với mật khẩu đã nhập!";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(nd.Email))
            {
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(nd.Email.Trim(), emailPattern))
                {
                    thongBao = "Địa chỉ email không đúng định dạng!";
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(nd.SoDienThoai))
            {
                string phonePattern = @"^0\d{9,10}$";
                if (!Regex.IsMatch(nd.SoDienThoai.Trim(), phonePattern))
                {
                    thongBao = "Số điện thoại phải bắt đầu bằng 0 và gồm 10 hoặc 11 chữ số!";
                    return false;
                }
            }

            if (_dal.KiemTraTenDangNhapTonTai(username))
            {
                thongBao = $"Tên đăng nhập '{username}' đã được đăng ký. Vui lòng chọn tên khác!";
                return false;
            }

            nd.TenDangNhap = username;
            nd.MatKhau = MaHoaMatKhau.BamSHA256(nd.MatKhau);
            nd.TrangThai = true;
            if (string.IsNullOrWhiteSpace(nd.VaiTro))
            {
                nd.VaiTro = "Nhân viên";
            }

            bool thanhCong = _dal.ThemNguoiDung(nd);
            if (!thanhCong)
            {
                thongBao = "Có lỗi xảy ra khi lưu vào cơ sở dữ liệu. Vui lòng thử lại!";
                return false;
            }

            thongBao = "Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay.";
            return true;
        }

        public List<NguoiDung> LayDanhSach(string tuKhoa = null)
        {
            return _dal.LayDanhSach(tuKhoa);
        }

        public bool CapNhat(NguoiDung nd, out string thongBao)
        {
            thongBao = string.Empty;
            if (nd.MaND <= 0)
            {
                thongBao = "Mã người dùng không hợp lệ!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(nd.HoTen))
            {
                thongBao = "Họ tên không được để trống!";
                return false;
            }

            if (_dal.CapNhatThongTin(nd))
            {
                thongBao = "Cập nhật thông tin tài khoản thành công!";
                return true;
            }

            thongBao = "Cập nhật tài khoản thất bại!";
            return false;
        }

        public bool DoiTrangThai(int maND, bool trangThai, out string thongBao)
        {
            thongBao = string.Empty;
            if (_dal.DoiTrangThai(maND, trangThai))
            {
                thongBao = trangThai ? "Đã mở khóa tài khoản thành công!" : "Đã khóa tài khoản thành công!";
                return true;
            }

            thongBao = "Thay đổi trạng thái thất bại!";
            return false;
        }

        public bool DatLaiMatKhau(int maND, string matKhauMoi, out string thongBao)
        {
            thongBao = string.Empty;
            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6)
            {
                thongBao = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return false;
            }

            string hash = MaHoaMatKhau.BamSHA256(matKhauMoi);
            if (_dal.DatLaiMatKhau(maND, hash))
            {
                thongBao = $"Đã đặt lại mật khẩu mới cho tài khoản thành '{matKhauMoi}'!";
                return true;
            }

            thongBao = "Đặt lại mật khẩu thất bại!";
            return false;
        }

        public bool Xoa(int maND, out string thongBao)
        {
            thongBao = string.Empty;
            if (_dal.KiemTraCoHoaDon(maND))
            {
                _dal.DoiTrangThai(maND, false);
                thongBao = "Nhân viên này đã lập hóa đơn trong quá khứ nên hệ thống đã chuyển sang 'Khóa tài khoản' để bảo lưu dữ liệu lịch sử!";
                return true;
            }

            if (_dal.Xoa(maND))
            {
                thongBao = "Đã xóa tài khoản thành công!";
                return true;
            }

            thongBao = "Xóa tài khoản thất bại!";
            return false;
        }
    }
}
