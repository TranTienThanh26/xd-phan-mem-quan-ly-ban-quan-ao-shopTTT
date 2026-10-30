using System;
using System.Collections.Generic;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.TruyCapDuLieu;

namespace CongNghenetBTL.NghiepVu
{
    public class DanhMucBLL
    {
        private readonly DanhMucDAL _danhMucDAL = new DanhMucDAL();

        public List<DanhMuc> LayDanhSach()
        {
            return _danhMucDAL.LayTatCa();
        }

        public bool Them(DanhMuc dm, out string thongBao)
        {
            thongBao = string.Empty;
            if (string.IsNullOrWhiteSpace(dm.TenDanhMuc))
            {
                thongBao = "Tên danh mục không được để trống!";
                return false;
            }

            int id = _danhMucDAL.Them(dm);
            if (id > 0)
            {
                dm.MaDanhMuc = id;
                thongBao = "Thêm danh mục mới thành công!";
                return true;
            }

            thongBao = "Không thể thêm danh mục. Vui lòng thử lại!";
            return false;
        }

        public bool CapNhat(DanhMuc dm, out string thongBao)
        {
            thongBao = string.Empty;
            if (dm.MaDanhMuc <= 0)
            {
                thongBao = "Mã danh mục không hợp lệ!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(dm.TenDanhMuc))
            {
                thongBao = "Tên danh mục không được để trống!";
                return false;
            }

            if (_danhMucDAL.CapNhat(dm))
            {
                thongBao = "Cập nhật danh mục thành công!";
                return true;
            }

            thongBao = "Cập nhật thất bại. Vui lòng kiểm tra lại!";
            return false;
        }

        public bool Xoa(int maDanhMuc, out string thongBao)
        {
            thongBao = string.Empty;
            if (_danhMucDAL.KiemTraCoSanPham(maDanhMuc))
            {
                thongBao = "Không thể xóa danh mục này vì đang có sản phẩm quần áo thuộc danh mục!";
                return false;
            }

            if (_danhMucDAL.Xoa(maDanhMuc))
            {
                thongBao = "Xóa danh mục thành công!";
                return true;
            }

            thongBao = "Xóa danh mục thất bại!";
            return false;
        }
    }
}
