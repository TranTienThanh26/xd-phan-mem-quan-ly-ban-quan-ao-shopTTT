using System;

namespace CongNghenetBTL.MoHinh
{
    public class NguoiDung
    {
        public int MaND { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string VaiTro { get; set; } = "Nhân viên";
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool LaQuanTriVien => string.Equals(VaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase) 
                                  || string.Equals(VaiTro, "Admin", StringComparison.OrdinalIgnoreCase);

        public override string ToString()
        {
            return $"{HoTen} ({TenDangNhap} - {VaiTro})";
        }
    }
}
