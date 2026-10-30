using System;
using System.Collections.Generic;

namespace CongNghenetBTL.MoHinh
{
    public class HoaDon
    {
        public int MaHD { get; set; }
        public int? MaKH { get; set; }
        public string TenKhachHang { get; set; } = "Khách vãng lai";
        public string SoDienThoaiKH { get; set; }
        public int? MaND { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string PhuongThucTT { get; set; } = "Tiền mặt";
        public string GhiChu { get; set; }

        public List<ChiTietHoaDon> DanhSachChiTiet { get; set; } = new List<ChiTietHoaDon>();
    }
}
