using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using CongNghenetBTL.MoHinh;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public class KhachHangDAL
    {
        public List<KhachHang> LayDanhSach(string tuKhoa = null)
        {
            var danhSach = new List<KhachHang>();
            string sql = "SELECT MaKH, HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy, NgayTao FROM KhachHang WHERE (1=1)";

            var thamSoList = new List<MySqlParameter>();
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                sql += " AND (HoTen LIKE @TuKhoa OR SoDienThoai LIKE @TuKhoa OR Email LIKE @TuKhoa)";
                thamSoList.Add(new MySqlParameter("@TuKhoa", MySqlDbType.VarChar) { Value = $"%{tuKhoa.Trim()}%" });
            }

            sql += " ORDER BY MaKH DESC";

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSoList.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(DocKhachHangTuRow(row));
            }
            return danhSach;
        }

        public KhachHang LayTheoMa(int maKH)
        {
            string sql = "SELECT MaKH, HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy, NgayTao FROM KhachHang WHERE MaKH = @MaKH";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = maKH }
            };

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
            if (dt.Rows.Count > 0)
            {
                return DocKhachHangTuRow(dt.Rows[0]);
            }
            return null;
        }

        public KhachHang LayTheoSoDienThoai(string soDienThoai)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai)) return null;

            string sql = "SELECT MaKH, HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy, NgayTao FROM KhachHang WHERE SoDienThoai = @SoDienThoai LIMIT 1";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@SoDienThoai", MySqlDbType.VarChar) { Value = soDienThoai.Trim() }
            };

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
            if (dt.Rows.Count > 0)
            {
                return DocKhachHangTuRow(dt.Rows[0]);
            }
            return null;
        }

        public int Them(KhachHang kh)
        {
            string sql = @"
                INSERT INTO KhachHang (HoTen, SoDienThoai, MatKhau, DiaChi, Email, LoaiKhach, DiemTichLuy)
                VALUES (@HoTen, @SoDienThoai, @MatKhau, @DiaChi, @Email, @LoaiKhach, @DiemTichLuy);
                SELECT LAST_INSERT_ID();";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@HoTen", MySqlDbType.VarChar) { Value = kh.HoTen },
                new MySqlParameter("@SoDienThoai", MySqlDbType.VarChar) { Value = (object)kh.SoDienThoai ?? DBNull.Value },
                new MySqlParameter("@MatKhau", MySqlDbType.VarChar) { Value = (object)kh.MatKhau ?? DBNull.Value },
                new MySqlParameter("@DiaChi", MySqlDbType.VarChar) { Value = (object)kh.DiaChi ?? DBNull.Value },
                new MySqlParameter("@Email", MySqlDbType.VarChar) { Value = (object)kh.Email ?? DBNull.Value },
                new MySqlParameter("@LoaiKhach", MySqlDbType.VarChar) { Value = (object)kh.LoaiKhach ?? "Thường" },
                new MySqlParameter("@DiemTichLuy", MySqlDbType.Int32) { Value = kh.DiemTichLuy }
            };

            object result = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public bool CapNhat(KhachHang kh)
        {
            string sql = @"
                UPDATE KhachHang
                SET HoTen = @HoTen,
                    SoDienThoai = @SoDienThoai,
                    DiaChi = @DiaChi,
                    Email = @Email,
                    LoaiKhach = @LoaiKhach,
                    DiemTichLuy = @DiemTichLuy
                WHERE MaKH = @MaKH";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = kh.MaKH },
                new MySqlParameter("@HoTen", MySqlDbType.VarChar) { Value = kh.HoTen },
                new MySqlParameter("@SoDienThoai", MySqlDbType.VarChar) { Value = (object)kh.SoDienThoai ?? DBNull.Value },
                new MySqlParameter("@DiaChi", MySqlDbType.VarChar) { Value = (object)kh.DiaChi ?? DBNull.Value },
                new MySqlParameter("@Email", MySqlDbType.VarChar) { Value = (object)kh.Email ?? DBNull.Value },
                new MySqlParameter("@LoaiKhach", MySqlDbType.VarChar) { Value = (object)kh.LoaiKhach ?? "Thường" },
                new MySqlParameter("@DiemTichLuy", MySqlDbType.Int32) { Value = kh.DiemTichLuy }
            };

            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool DoiMatKhau(int maKH, string matKhauMoiHash)
        {
            string sql = "UPDATE KhachHang SET MatKhau = @MatKhau WHERE MaKH = @MaKH";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = maKH },
                new MySqlParameter("@MatKhau", MySqlDbType.VarChar) { Value = matKhauMoiHash }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool CongDiemTichLuy(int maKH, int diemCong)
        {
            string sql = "UPDATE KhachHang SET DiemTichLuy = DiemTichLuy + @DiemCong WHERE MaKH = @MaKH";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = maKH },
                new MySqlParameter("@DiemCong", MySqlDbType.Int32) { Value = diemCong }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool KiemTraCoHoaDon(int maKH)
        {
            string sql = "SELECT COUNT(1) FROM HoaDon WHERE MaKH = @MaKH";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = maKH }
            };
            object res = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return res != null && Convert.ToInt32(res) > 0;
        }

        public bool Xoa(int maKH)
        {
            string sql = "DELETE FROM KhachHang WHERE MaKH = @MaKH";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = maKH }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        private KhachHang DocKhachHangTuRow(DataRow row)
        {
            return new KhachHang
            {
                MaKH = Convert.ToInt32(row["MaKH"]),
                HoTen = row["HoTen"]?.ToString(),
                SoDienThoai = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : "",
                MatKhau = row["MatKhau"] != DBNull.Value ? row["MatKhau"].ToString() : "",
                DiaChi = row["DiaChi"] != DBNull.Value ? row["DiaChi"].ToString() : "",
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : "",
                LoaiKhach = row["LoaiKhach"] != DBNull.Value ? row["LoaiKhach"].ToString() : "Thường",
                DiemTichLuy = row["DiemTichLuy"] != DBNull.Value ? Convert.ToInt32(row["DiemTichLuy"]) : 0,
                NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : DateTime.Now
            };
        }
    }
}
