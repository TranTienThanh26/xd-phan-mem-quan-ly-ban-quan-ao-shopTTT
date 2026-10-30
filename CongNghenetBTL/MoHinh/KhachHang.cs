using System;

namespace CongNghenetBTL.MoHinh
{
    public class KhachHang
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string MatKhau { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string LoaiKhach { get; set; } = "Thường";
        public int DiemTichLuy { get; set; } = 0;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public decimal TyLeGiamGia
        {
            get
            {
                if (string.Equals(LoaiKhach, "VIP", StringComparison.OrdinalIgnoreCase)) return 0.10m;
                if (string.Equals(LoaiKhach, "Thân thiết", StringComparison.OrdinalIgnoreCase)) return 0.05m;
                return 0m;
            }
        }

        public string HienThiHang
        {
            get
            {
                if (string.Equals(LoaiKhach, "VIP", StringComparison.OrdinalIgnoreCase)) return "⭐ Hạng VIP (-10%)";
                if (string.Equals(LoaiKhach, "Thân thiết", StringComparison.OrdinalIgnoreCase)) return "🎖️ Thân Thiết (-5%)";
                return "👤 Khách Hàng";
            }
        }

        public string SoDienThoaiBaoMat
        {
            get
            {
                if (string.IsNullOrEmpty(SoDienThoai) || SoDienThoai.Length < 7)
                    return SoDienThoai;
                return SoDienThoai.Substring(0, 3) + "****" + SoDienThoai.Substring(SoDienThoai.Length - 3);
            }
        }

        public override string ToString()
        {
            return $"{HoTen} ({SoDienThoai})";
        }
    }
}
