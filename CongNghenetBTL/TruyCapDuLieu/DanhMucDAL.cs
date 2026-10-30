using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using CongNghenetBTL.MoHinh;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public class DanhMucDAL
    {
        public List<DanhMuc> LayTatCa()
        {
            var danhSach = new List<DanhMuc>();
            string sql = "SELECT MaDanhMuc, TenDanhMuc, MoTa FROM DanhMuc ORDER BY MaDanhMuc ASC";

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new DanhMuc
                {
                    MaDanhMuc = Convert.ToInt32(row["MaDanhMuc"]),
                    TenDanhMuc = row["TenDanhMuc"]?.ToString(),
                    MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : ""
                });
            }
            return danhSach;
        }

        public int Them(DanhMuc dm)
        {
            string sql = @"INSERT INTO DanhMuc (TenDanhMuc, MoTa) 
                           VALUES (@TenDanhMuc, @MoTa);
                           SELECT LAST_INSERT_ID();";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@TenDanhMuc", MySqlDbType.VarChar) { Value = dm.TenDanhMuc },
                new MySqlParameter("@MoTa", MySqlDbType.VarChar) { Value = (object)dm.MoTa ?? DBNull.Value }
            };

            object result = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public bool CapNhat(DanhMuc dm)
        {
            string sql = @"UPDATE DanhMuc 
                           SET TenDanhMuc = @TenDanhMuc, MoTa = @MoTa 
                           WHERE MaDanhMuc = @MaDanhMuc";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaDanhMuc", MySqlDbType.Int32) { Value = dm.MaDanhMuc },
                new MySqlParameter("@TenDanhMuc", MySqlDbType.VarChar) { Value = dm.TenDanhMuc },
                new MySqlParameter("@MoTa", MySqlDbType.VarChar) { Value = (object)dm.MoTa ?? DBNull.Value }
            };

            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool KiemTraCoSanPham(int maDanhMuc)
        {
            string sql = "SELECT COUNT(1) FROM SanPham WHERE MaDanhMuc = @MaDanhMuc";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaDanhMuc", MySqlDbType.Int32) { Value = maDanhMuc }
            };
            object res = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return res != null && Convert.ToInt32(res) > 0;
        }

        public bool Xoa(int maDanhMuc)
        {
            string sql = "DELETE FROM DanhMuc WHERE MaDanhMuc = @MaDanhMuc";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaDanhMuc", MySqlDbType.Int32) { Value = maDanhMuc }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }
    }
}
