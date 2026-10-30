using System;
using CongNghenetBTL.MoHinh;

namespace CongNghenetBTL.TienIch
{
    public static class PhienDangNhap
    {
        public static NguoiDung NguoiDungHienTai { get; private set; }
        public static bool DaDangNhap => NguoiDungHienTai != null;
        public static bool LaQuanTriVien => NguoiDungHienTai != null && NguoiDungHienTai.LaQuanTriVien;

        public static void DangNhap(NguoiDung nd)
        {
            NguoiDungHienTai = nd;
        }

        public static void DangXuat()
        {
            NguoiDungHienTai = null;
        }

        public static KhachHang KhachHangHienTai { get; private set; }
        public static bool KhachDaDangNhap => KhachHangHienTai != null;

        public static void DangNhapKhachHang(KhachHang kh)
        {
            KhachHangHienTai = kh;
        }

        public static void DangXuatKhachHang()
        {
            KhachHangHienTai = null;
        }
    }
}
