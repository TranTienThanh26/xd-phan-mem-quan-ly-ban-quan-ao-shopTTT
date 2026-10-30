using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using CongNghenetBTL.MoHinh;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public class NguoiDungDAL
    {
        public List<NguoiDung> LayDanhSach(string tuKhoa = null)
        {
            var danhSach = new List<NguoiDung>();
            string sql = @"SELECT MaND, TenDangNhap, MatKhau, HoTen, Email, SoDienThoai, VaiTro, TrangThai, NgayTao 
                           FROM NguoiDung 
                           WHERE (1=1)";

            var thamSoList = new List<MySqlParameter>();
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                sql += " AND (TenDangNhap LIKE @TuKhoa OR HoTen LIKE @TuKhoa OR SoDienThoai LIKE @TuKhoa OR Email LIKE @TuKhoa)";
                thamSoList.Add(new MySqlParameter("@TuKhoa", MySqlDbType.VarChar) { Value = $"%{tuKhoa.Trim()}%" });
            }

            sql += " ORDER BY MaND ASC";

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSoList.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(ChuyenDoiDongSangNguoiDung(row));
            }
            return danhSach;
        }

        public bool KiemTraTenDangNhapTonTai(string tenDangNhap)
        {
            string sql = "SELECT COUNT(1) FROM NguoiDung WHERE LOWER(TenDangNhap) = LOWER(@TenDangNhap)";
            MySqlParameter[] thamSo = {
                new MySqlParameter("@TenDangNhap", MySqlDbType.VarChar, 50) { Value = tenDangNhap.Trim() }
            };

            object ketQua = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return ketQua != null && Convert.ToInt32(ketQua) > 0;
        }

        public NguoiDung LayTheoTenDangNhap(string tenDangNhap)
        {
            string sql = @"SELECT MaND, TenDangNhap, MatKhau, HoTen, Email, SoDienThoai, VaiTro, TrangThai, NgayTao 
                           FROM NguoiDung 
                           WHERE LOWER(TenDangNhap) = LOWER(@TenDangNhap)
                           LIMIT 1";
            MySqlParameter[] thamSo = {
                new MySqlParameter("@TenDangNhap", MySqlDbType.VarChar, 50) { Value = tenDangNhap.Trim() }
            };

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
            if (dt.Rows.Count > 0)
            {
                return ChuyenDoiDongSangNguoiDung(dt.Rows[0]);
            }
            return null;
        }

        public bool ThemNguoiDung(NguoiDung nd)
        {
            string sql = @"INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, Email, SoDienThoai, VaiTro, TrangThai, NgayTao)
                           VALUES (@TenDangNhap, @MatKhau, @HoTen, @Email, @SoDienThoai, @VaiTro, @TrangThai, NOW())";

            MySqlParameter[] thamSo = {
                new MySqlParameter("@TenDangNhap", MySqlDbType.VarChar, 50) { Value = nd.TenDangNhap.Trim() },
                new MySqlParameter("@MatKhau", MySqlDbType.VarChar, 256) { Value = nd.MatKhau },
                new MySqlParameter("@HoTen", MySqlDbType.VarChar, 100) { Value = nd.HoTen.Trim() },
                new MySqlParameter("@Email", MySqlDbType.VarChar, 100) { Value = (object)nd.Email ?? DBNull.Value },
                new MySqlParameter("@SoDienThoai", MySqlDbType.VarChar, 20) { Value = (object)nd.SoDienThoai ?? DBNull.Value },
                new MySqlParameter("@VaiTro", MySqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(nd.VaiTro) ? "Nhân viên" : nd.VaiTro },
                new MySqlParameter("@TrangThai", MySqlDbType.Byte) { Value = nd.TrangThai ? (byte)1 : (byte)0 }
            };

            int soDong = KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo);
            return soDong > 0;
        }

        public bool CapNhatThongTin(NguoiDung nd)
        {
            string sql = @"UPDATE NguoiDung 
                           SET HoTen = @HoTen, 
                               Email = @Email, 
                               SoDienThoai = @SoDienThoai, 
                               VaiTro = @VaiTro, 
                               TrangThai = @TrangThai 
                           WHERE MaND = @MaND";

            MySqlParameter[] thamSo = {
                new MySqlParameter("@MaND", MySqlDbType.Int32) { Value = nd.MaND },
                new MySqlParameter("@HoTen", MySqlDbType.VarChar, 100) { Value = nd.HoTen.Trim() },
                new MySqlParameter("@Email", MySqlDbType.VarChar, 100) { Value = (object)nd.Email ?? DBNull.Value },
                new MySqlParameter("@SoDienThoai", MySqlDbType.VarChar, 20) { Value = (object)nd.SoDienThoai ?? DBNull.Value },
                new MySqlParameter("@VaiTro", MySqlDbType.VarChar, 50) { Value = nd.VaiTro },
                new MySqlParameter("@TrangThai", MySqlDbType.Byte) { Value = nd.TrangThai ? (byte)1 : (byte)0 }
            };

            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool DoiTrangThai(int maND, bool trangThai)
        {
            string sql = "UPDATE NguoiDung SET TrangThai = @TrangThai WHERE MaND = @MaND";
            MySqlParameter[] thamSo = {
                new MySqlParameter("@MaND", MySqlDbType.Int32) { Value = maND },
                new MySqlParameter("@TrangThai", MySqlDbType.Byte) { Value = trangThai ? (byte)1 : (byte)0 }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool DatLaiMatKhau(int maND, string matKhauMoiHash)
        {
            string sql = "UPDATE NguoiDung SET MatKhau = @MatKhau WHERE MaND = @MaND";
            MySqlParameter[] thamSo = {
                new MySqlParameter("@MaND", MySqlDbType.Int32) { Value = maND },
                new MySqlParameter("@MatKhau", MySqlDbType.VarChar, 256) { Value = matKhauMoiHash }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool KiemTraCoHoaDon(int maND)
        {
            string sql = "SELECT COUNT(1) FROM HoaDon WHERE MaND = @MaND";
            MySqlParameter[] thamSo = {
                new MySqlParameter("@MaND", MySqlDbType.Int32) { Value = maND }
            };
            object res = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return res != null && Convert.ToInt32(res) > 0;
        }

        public bool Xoa(int maND)
        {
            string sql = "DELETE FROM NguoiDung WHERE MaND = @MaND";
            MySqlParameter[] thamSo = {
                new MySqlParameter("@MaND", MySqlDbType.Int32) { Value = maND }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        private NguoiDung ChuyenDoiDongSangNguoiDung(DataRow row)
        {
            return new NguoiDung
            {
                MaND = Convert.ToInt32(row["MaND"]),
                TenDangNhap = row["TenDangNhap"].ToString(),
                MatKhau = row["MatKhau"].ToString(),
                HoTen = row["HoTen"].ToString(),
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : string.Empty,
                SoDienThoai = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : string.Empty,
                VaiTro = row["VaiTro"].ToString(),
                TrangThai = Convert.ToBoolean(row["TrangThai"]),
                NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : DateTime.Now
            };
        }
    }
}
