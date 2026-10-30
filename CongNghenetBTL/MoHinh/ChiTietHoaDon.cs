using System;

namespace CongNghenetBTL.MoHinh
{
    public class ChiTietHoaDon
    {
        public int MaCTHD { get; set; }
        public int MaHD { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string KichCo { get; set; }
        public string MauSac { get; set; }
        public int SoLuong { get; set; } = 1;
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string HinhAnh { get; set; }
    }
}
