using System;
using System.Collections.Generic;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.TruyCapDuLieu;

namespace CongNghenetBTL.NghiepVu
{
    public class HoaDonBLL
    {
        private readonly HoaDonDAL _hoaDonDAL = new HoaDonDAL();

        public bool TaoHoaDon(HoaDon hd, out int maHDMoi, out string thongBao)
        {
            maHDMoi = 0;
            thongBao = string.Empty;

            if (hd == null || hd.DanhSachChiTiet == null || hd.DanhSachChiTiet.Count == 0)
            {
                thongBao = "Giỏ hàng đang trống! Vui lòng chọn ít nhất một mặt hàng quần áo.";
                return false;
            }

            foreach (var ct in hd.DanhSachChiTiet)
            {
                if (ct.SoLuong <= 0)
                {
                    thongBao = $"Sản phẩm '{ct.TenSP}' có số lượng không hợp lệ (phải > 0)!";
                    return false;
                }
            }

            if (hd.GiamGia < 0)
            {
                thongBao = "Số tiền giảm giá không được âm!";
                return false;
            }

            if (hd.ThanhTien < 0)
            {
                thongBao = "Số tiền thanh toán không hợp lệ!";
                return false;
            }

            string loi;
            if (_hoaDonDAL.TaoHoaDon(hd, out maHDMoi, out loi))
            {
                thongBao = $"Thanh toán thành công! Mã hóa đơn: #{maHDMoi}";
                return true;
            }

            thongBao = $"Lỗi khi lưu hóa đơn: {loi}";
            return false;
        }

        public List<HoaDon> LayDanhSach(DateTime? tuNgay = null, DateTime? denNgay = null, string tuKhoa = null)
        {
            return _hoaDonDAL.LayDanhSach(tuNgay, denNgay, tuKhoa);
        }

        public List<HoaDon> LayLichSuTheoKhachHang(int maKH)
        {
            if (maKH <= 0) return new List<HoaDon>();
            return _hoaDonDAL.LayDanhSachTheoKhachHang(maKH);
        }

        public List<ChiTietHoaDon> LayChiTietTheoHoaDon(int maHD)
        {
            return _hoaDonDAL.LayChiTietTheoHoaDon(maHD);
        }

        public bool HuyHoaDon(int maHD, out string thongBao)
        {
            thongBao = string.Empty;
            string loi;
            if (_hoaDonDAL.XoaHoaDon(maHD, out loi))
            {
                thongBao = $"Đã hủy hóa đơn #{maHD} và hoàn lại số lượng tồn kho thành công!";
                return true;
            }

            thongBao = $"Hủy hóa đơn thất bại: {loi}";
            return false;
        }
    }
}
