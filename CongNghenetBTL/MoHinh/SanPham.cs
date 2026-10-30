using System;
using System.IO;

namespace CongNghenetBTL.MoHinh
{
    public class SanPham
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int? MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string KichCo { get; set; }
        public string MauSac { get; set; }
        public string ChatLieu { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string DonViTinh { get; set; } = "Cái";
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; } = true;

        public string DuongDanAnhDayDu
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(HinhAnh))
                    {
                        if (File.Exists(HinhAnh))
                            return HinhAnh;

                        string duongDanBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HinhAnh);
                        if (File.Exists(duongDanBase))
                            return duongDanBase;
                    }

                    string anhMacDinh = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HinhAnh", "mac_dinh.jpg");
                    if (File.Exists(anhMacDinh))
                        return anhMacDinh;
                }
                catch { }

                return null;
            }
        }

        public string GiaBanDinhDang => string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", GiaBan);
        public string TrangThaiHienThi => TrangThai ? "Đang kinh doanh" : "Ngừng kinh doanh";
        public string ThongTinChiTiet => $"Size: {KichCo} | Màu: {MauSac} | Vải: {ChatLieu}";
    }
}
