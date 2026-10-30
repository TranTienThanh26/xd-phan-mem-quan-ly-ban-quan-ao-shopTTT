using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using CongNghenetBTL.MoHinh;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public class SanPhamDAL
    {
        public List<SanPham> LayDanhSach(int? maDanhMuc = null, string tuKhoa = null, bool chiLayDangBan = false)
        {
            var danhSach = new List<SanPham>();
            string sql = @"
                SELECT sp.MaSP, sp.TenSP, sp.MaDanhMuc, IFNULL(dm.TenDanhMuc, 'Chưa phân loại') AS TenDanhMuc,
                       sp.KichCo, sp.MauSac, sp.ChatLieu, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon, 
                       sp.DonViTinh, sp.HinhAnh, sp.MoTa, sp.TrangThai
                FROM SanPham sp
                LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
                WHERE (1=1)";

            var thamSoList = new List<MySqlParameter>();

            if (chiLayDangBan)
            {
                sql += " AND sp.TrangThai = 1";
            }

            if (maDanhMuc.HasValue && maDanhMuc.Value > 0)
            {
                sql += " AND sp.MaDanhMuc = @MaDanhMuc";
                thamSoList.Add(new MySqlParameter("@MaDanhMuc", MySqlDbType.Int32) { Value = maDanhMuc.Value });
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                sql += " AND (sp.TenSP LIKE @TuKhoa OR sp.MauSac LIKE @TuKhoa OR sp.KichCo LIKE @TuKhoa OR CAST(sp.MaSP AS CHAR) = @MaExact)";
                thamSoList.Add(new MySqlParameter("@TuKhoa", MySqlDbType.VarChar) { Value = $"%{tuKhoa.Trim()}%" });
                thamSoList.Add(new MySqlParameter("@MaExact", MySqlDbType.VarChar) { Value = tuKhoa.Trim() });
            }

            sql += " ORDER BY sp.MaSP DESC";

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSoList.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(DocSanPhamTuRow(row));
            }

            return danhSach;
        }

        public SanPham LayTheoMa(int maSP)
        {
            string sql = @"
                SELECT sp.MaSP, sp.TenSP, sp.MaDanhMuc, IFNULL(dm.TenDanhMuc, 'Chưa phân loại') AS TenDanhMuc,
                       sp.KichCo, sp.MauSac, sp.ChatLieu, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon, 
                       sp.DonViTinh, sp.HinhAnh, sp.MoTa, sp.TrangThai
                FROM SanPham sp
                LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
                WHERE sp.MaSP = @MaSP";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = maSP }
            };

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
            if (dt.Rows.Count > 0)
            {
                return DocSanPhamTuRow(dt.Rows[0]);
            }
            return null;
        }

        public int Them(SanPham sp)
        {
            string sql = @"
                INSERT INTO SanPham (TenSP, MaDanhMuc, KichCo, MauSac, ChatLieu, GiaNhap, GiaBan, SoLuongTon, DonViTinh, HinhAnh, MoTa, TrangThai)
                VALUES (@TenSP, @MaDanhMuc, @KichCo, @MauSac, @ChatLieu, @GiaNhap, @GiaBan, @SoLuongTon, @DonViTinh, @HinhAnh, @MoTa, @TrangThai);
                SELECT LAST_INSERT_ID();";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@TenSP", MySqlDbType.VarChar) { Value = sp.TenSP },
                new MySqlParameter("@MaDanhMuc", MySqlDbType.Int32) { Value = (object)sp.MaDanhMuc ?? DBNull.Value },
                new MySqlParameter("@KichCo", MySqlDbType.VarChar) { Value = (object)sp.KichCo ?? DBNull.Value },
                new MySqlParameter("@MauSac", MySqlDbType.VarChar) { Value = (object)sp.MauSac ?? DBNull.Value },
                new MySqlParameter("@ChatLieu", MySqlDbType.VarChar) { Value = (object)sp.ChatLieu ?? DBNull.Value },
                new MySqlParameter("@GiaNhap", MySqlDbType.Decimal) { Value = sp.GiaNhap },
                new MySqlParameter("@GiaBan", MySqlDbType.Decimal) { Value = sp.GiaBan },
                new MySqlParameter("@SoLuongTon", MySqlDbType.Int32) { Value = sp.SoLuongTon },
                new MySqlParameter("@DonViTinh", MySqlDbType.VarChar) { Value = (object)sp.DonViTinh ?? "Cái" },
                new MySqlParameter("@HinhAnh", MySqlDbType.VarChar) { Value = (object)sp.HinhAnh ?? DBNull.Value },
                new MySqlParameter("@MoTa", MySqlDbType.VarChar) { Value = (object)sp.MoTa ?? DBNull.Value },
                new MySqlParameter("@TrangThai", MySqlDbType.Byte) { Value = sp.TrangThai ? (byte)1 : (byte)0 }
            };

            object result = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public bool CapNhat(SanPham sp)
        {
            string sql = @"
                UPDATE SanPham
                SET TenSP = @TenSP,
                    MaDanhMuc = @MaDanhMuc,
                    KichCo = @KichCo,
                    MauSac = @MauSac,
                    ChatLieu = @ChatLieu,
                    GiaNhap = @GiaNhap,
                    GiaBan = @GiaBan,
                    SoLuongTon = @SoLuongTon,
                    DonViTinh = @DonViTinh,
                    HinhAnh = @HinhAnh,
                    MoTa = @MoTa,
                    TrangThai = @TrangThai
                WHERE MaSP = @MaSP";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = sp.MaSP },
                new MySqlParameter("@TenSP", MySqlDbType.VarChar) { Value = sp.TenSP },
                new MySqlParameter("@MaDanhMuc", MySqlDbType.Int32) { Value = (object)sp.MaDanhMuc ?? DBNull.Value },
                new MySqlParameter("@KichCo", MySqlDbType.VarChar) { Value = (object)sp.KichCo ?? DBNull.Value },
                new MySqlParameter("@MauSac", MySqlDbType.VarChar) { Value = (object)sp.MauSac ?? DBNull.Value },
                new MySqlParameter("@ChatLieu", MySqlDbType.VarChar) { Value = (object)sp.ChatLieu ?? DBNull.Value },
                new MySqlParameter("@GiaNhap", MySqlDbType.Decimal) { Value = sp.GiaNhap },
                new MySqlParameter("@GiaBan", MySqlDbType.Decimal) { Value = sp.GiaBan },
                new MySqlParameter("@SoLuongTon", MySqlDbType.Int32) { Value = sp.SoLuongTon },
                new MySqlParameter("@DonViTinh", MySqlDbType.VarChar) { Value = (object)sp.DonViTinh ?? "Cái" },
                new MySqlParameter("@HinhAnh", MySqlDbType.VarChar) { Value = (object)sp.HinhAnh ?? DBNull.Value },
                new MySqlParameter("@MoTa", MySqlDbType.VarChar) { Value = (object)sp.MoTa ?? DBNull.Value },
                new MySqlParameter("@TrangThai", MySqlDbType.Byte) { Value = sp.TrangThai ? (byte)1 : (byte)0 }
            };

            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool KiemTraDaCoHoaDon(int maSP)
        {
            string sql = "SELECT COUNT(1) FROM ChiTietHoaDon WHERE MaSP = @MaSP";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = maSP }
            };
            object res = KetNoiCSDL.ThucThiGiaTri(sql, thamSo);
            return res != null && Convert.ToInt32(res) > 0;
        }

        public bool Xoa(int maSP)
        {
            string sql = "DELETE FROM SanPham WHERE MaSP = @MaSP";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = maSP }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        public bool CapNhatTrangThai(int maSP, bool trangThai)
        {
            string sql = "UPDATE SanPham SET TrangThai = @TrangThai WHERE MaSP = @MaSP";
            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = maSP },
                new MySqlParameter("@TrangThai", MySqlDbType.Byte) { Value = trangThai ? (byte)1 : (byte)0 }
            };
            return KetNoiCSDL.ThucThiKhongTruyVan(sql, thamSo) > 0;
        }

        private SanPham DocSanPhamTuRow(DataRow row)
        {
            return new SanPham
            {
                MaSP = Convert.ToInt32(row["MaSP"]),
                TenSP = row["TenSP"]?.ToString(),
                MaDanhMuc = row["MaDanhMuc"] != DBNull.Value ? Convert.ToInt32(row["MaDanhMuc"]) : (int?)null,
                TenDanhMuc = row["TenDanhMuc"]?.ToString(),
                KichCo = row["KichCo"] != DBNull.Value ? row["KichCo"].ToString() : "",
                MauSac = row["MauSac"] != DBNull.Value ? row["MauSac"].ToString() : "",
                ChatLieu = row["ChatLieu"] != DBNull.Value ? row["ChatLieu"].ToString() : "",
                GiaNhap = row["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(row["GiaNhap"]) : 0,
                GiaBan = row["GiaBan"] != DBNull.Value ? Convert.ToDecimal(row["GiaBan"]) : 0,
                SoLuongTon = row["SoLuongTon"] != DBNull.Value ? Convert.ToInt32(row["SoLuongTon"]) : 0,
                DonViTinh = row["DonViTinh"] != DBNull.Value ? row["DonViTinh"].ToString() : "Cái",
                HinhAnh = row["HinhAnh"] != DBNull.Value ? row["HinhAnh"].ToString() : "",
                MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : "",
                TrangThai = row["TrangThai"] != DBNull.Value && Convert.ToBoolean(row["TrangThai"])
            };
        }
    }
}
