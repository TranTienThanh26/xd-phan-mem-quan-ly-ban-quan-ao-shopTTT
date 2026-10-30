using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using CongNghenetBTL.MoHinh;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public class HoaDonDAL
    {
        public bool TaoHoaDon(HoaDon hd, out int maHDMoi, out string loi)
        {
            maHDMoi = 0;
            loi = string.Empty;

            using (var kn = KetNoiCSDL.LayKetNoi())
            {
                kn.Open();
                using (var trans = kn.BeginTransaction())
                {
                    try
                    {
                        string sqlHD = @"
                            INSERT INTO HoaDon (MaKH, MaND, NgayLap, TongTien, GiamGia, ThanhTien, PhuongThucTT, GhiChu)
                            VALUES (@MaKH, @MaND, @NgayLap, @TongTien, @GiamGia, @ThanhTien, @PhuongThucTT, @GhiChu);
                            SELECT LAST_INSERT_ID();";

                        using (var cmdHD = new MySqlCommand(sqlHD, kn, trans))
                        {
                            cmdHD.Parameters.Add(new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = (object)hd.MaKH ?? DBNull.Value });
                            cmdHD.Parameters.Add(new MySqlParameter("@MaND", MySqlDbType.Int32) { Value = (object)hd.MaND ?? DBNull.Value });
                            cmdHD.Parameters.Add(new MySqlParameter("@NgayLap", MySqlDbType.DateTime) { Value = hd.NgayLap });
                            cmdHD.Parameters.Add(new MySqlParameter("@TongTien", MySqlDbType.Decimal) { Value = hd.TongTien });
                            cmdHD.Parameters.Add(new MySqlParameter("@GiamGia", MySqlDbType.Decimal) { Value = hd.GiamGia });
                            cmdHD.Parameters.Add(new MySqlParameter("@ThanhTien", MySqlDbType.Decimal) { Value = hd.ThanhTien });
                            cmdHD.Parameters.Add(new MySqlParameter("@PhuongThucTT", MySqlDbType.VarChar) { Value = (object)hd.PhuongThucTT ?? "Tiền mặt" });
                            cmdHD.Parameters.Add(new MySqlParameter("@GhiChu", MySqlDbType.VarChar) { Value = (object)hd.GhiChu ?? DBNull.Value });

                            object res = cmdHD.ExecuteScalar();
                            maHDMoi = Convert.ToInt32(res);
                        }

                        string sqlCT = @"
                            INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien)
                            VALUES (@MaHD, @MaSP, @SoLuong, @DonGia, @ThanhTien);";

                        string sqlTruKho = @"
                            UPDATE SanPham
                            SET SoLuongTon = CASE WHEN SoLuongTon >= @SoLuong THEN SoLuongTon - @SoLuong ELSE 0 END
                            WHERE MaSP = @MaSP;";

                        foreach (var ct in hd.DanhSachChiTiet)
                        {
                            using (var cmdCT = new MySqlCommand(sqlCT, kn, trans))
                            {
                                cmdCT.Parameters.Add(new MySqlParameter("@MaHD", MySqlDbType.Int32) { Value = maHDMoi });
                                cmdCT.Parameters.Add(new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = ct.MaSP });
                                cmdCT.Parameters.Add(new MySqlParameter("@SoLuong", MySqlDbType.Int32) { Value = ct.SoLuong });
                                cmdCT.Parameters.Add(new MySqlParameter("@DonGia", MySqlDbType.Decimal) { Value = ct.DonGia });
                                cmdCT.Parameters.Add(new MySqlParameter("@ThanhTien", MySqlDbType.Decimal) { Value = ct.ThanhTien });
                                cmdCT.ExecuteNonQuery();
                            }

                            using (var cmdKho = new MySqlCommand(sqlTruKho, kn, trans))
                            {
                                cmdKho.Parameters.Add(new MySqlParameter("@SoLuong", MySqlDbType.Int32) { Value = ct.SoLuong });
                                cmdKho.Parameters.Add(new MySqlParameter("@MaSP", MySqlDbType.Int32) { Value = ct.MaSP });
                                cmdKho.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        loi = ex.Message;
                        return false;
                    }
                }
            }
        }

        public List<HoaDon> LayDanhSach(DateTime? tuNgay = null, DateTime? denNgay = null, string tuKhoa = null)
        {
            var danhSach = new List<HoaDon>();
            string sql = @"
                SELECT hd.MaHD, hd.MaKH, IFNULL(kh.HoTen, 'Khách vãng lai') AS TenKhachHang, kh.SoDienThoai AS SoDienThoaiKH,
                       hd.MaND, IFNULL(nd.HoTen, 'Hệ thống') AS TenNhanVien,
                       hd.NgayLap, hd.TongTien, hd.GiamGia, hd.ThanhTien, hd.PhuongThucTT, hd.GhiChu
                FROM HoaDon hd
                LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                LEFT JOIN NguoiDung nd ON hd.MaND = nd.MaND
                WHERE (1=1)";

            var thamSoList = new List<MySqlParameter>();

            if (tuNgay.HasValue)
            {
                sql += " AND hd.NgayLap >= @TuNgay";
                thamSoList.Add(new MySqlParameter("@TuNgay", MySqlDbType.DateTime) { Value = tuNgay.Value.Date });
            }

            if (denNgay.HasValue)
            {
                sql += " AND hd.NgayLap < @DenNgay";
                thamSoList.Add(new MySqlParameter("@DenNgay", MySqlDbType.DateTime) { Value = denNgay.Value.Date.AddDays(1) });
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                sql += " AND (CAST(hd.MaHD AS CHAR) = @TuKhoaExact OR kh.HoTen LIKE @TuKhoa OR kh.SoDienThoai LIKE @TuKhoa OR nd.HoTen LIKE @TuKhoa)";
                thamSoList.Add(new MySqlParameter("@TuKhoaExact", MySqlDbType.VarChar) { Value = tuKhoa.Trim() });
                thamSoList.Add(new MySqlParameter("@TuKhoa", MySqlDbType.VarChar) { Value = $"%{tuKhoa.Trim()}%" });
            }

            sql += " ORDER BY hd.MaHD DESC";

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSoList.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new HoaDon
                {
                    MaHD = Convert.ToInt32(row["MaHD"]),
                    MaKH = row["MaKH"] != DBNull.Value ? Convert.ToInt32(row["MaKH"]) : (int?)null,
                    TenKhachHang = row["TenKhachHang"]?.ToString(),
                    SoDienThoaiKH = row["SoDienThoaiKH"] != DBNull.Value ? row["SoDienThoaiKH"].ToString() : "",
                    MaND = row["MaND"] != DBNull.Value ? Convert.ToInt32(row["MaND"]) : (int?)null,
                    TenNhanVien = row["TenNhanVien"]?.ToString(),
                    NgayLap = Convert.ToDateTime(row["NgayLap"]),
                    TongTien = Convert.ToDecimal(row["TongTien"]),
                    GiamGia = Convert.ToDecimal(row["GiamGia"]),
                    ThanhTien = Convert.ToDecimal(row["ThanhTien"]),
                    PhuongThucTT = row["PhuongThucTT"]?.ToString(),
                    GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : ""
                });
            }

            return danhSach;
        }

        public List<HoaDon> LayDanhSachTheoKhachHang(int maKH)
        {
            var danhSach = new List<HoaDon>();
            string sql = @"
                SELECT hd.MaHD, hd.MaKH, IFNULL(kh.HoTen, 'Khách vãng lai') AS TenKhachHang, kh.SoDienThoai AS SoDienThoaiKH,
                       hd.MaND, IFNULL(nd.HoTen, 'Hệ thống') AS TenNhanVien,
                       hd.NgayLap, hd.TongTien, hd.GiamGia, hd.ThanhTien, hd.PhuongThucTT, hd.GhiChu
                FROM HoaDon hd
                LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                LEFT JOIN NguoiDung nd ON hd.MaND = nd.MaND
                WHERE hd.MaKH = @MaKH
                ORDER BY hd.MaHD DESC";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaKH", MySqlDbType.Int32) { Value = maKH }
            };

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new HoaDon
                {
                    MaHD = Convert.ToInt32(row["MaHD"]),
                    MaKH = Convert.ToInt32(row["MaKH"]),
                    TenKhachHang = row["TenKhachHang"]?.ToString(),
                    SoDienThoaiKH = row["SoDienThoaiKH"] != DBNull.Value ? row["SoDienThoaiKH"].ToString() : "",
                    MaND = row["MaND"] != DBNull.Value ? Convert.ToInt32(row["MaND"]) : (int?)null,
                    TenNhanVien = row["TenNhanVien"]?.ToString(),
                    NgayLap = Convert.ToDateTime(row["NgayLap"]),
                    TongTien = Convert.ToDecimal(row["TongTien"]),
                    GiamGia = Convert.ToDecimal(row["GiamGia"]),
                    ThanhTien = Convert.ToDecimal(row["ThanhTien"]),
                    PhuongThucTT = row["PhuongThucTT"]?.ToString(),
                    GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : ""
                });
            }

            return danhSach;
        }

        public List<ChiTietHoaDon> LayChiTietTheoHoaDon(int maHD)
        {
            var danhSach = new List<ChiTietHoaDon>();
            string sql = @"
                SELECT ct.MaCTHD, ct.MaHD, ct.MaSP, sp.TenSP, sp.KichCo, sp.MauSac, sp.HinhAnh,
                       ct.SoLuong, ct.DonGia, ct.ThanhTien
                FROM ChiTietHoaDon ct
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                WHERE ct.MaHD = @MaHD
                ORDER BY ct.MaCTHD ASC";

            MySqlParameter[] thamSo = new MySqlParameter[]
            {
                new MySqlParameter("@MaHD", MySqlDbType.Int32) { Value = maHD }
            };

            DataTable dt = KetNoiCSDL.ThucThiTruyVan(sql, thamSo);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new ChiTietHoaDon
                {
                    MaCTHD = Convert.ToInt32(row["MaCTHD"]),
                    MaHD = Convert.ToInt32(row["MaHD"]),
                    MaSP = Convert.ToInt32(row["MaSP"]),
                    TenSP = row["TenSP"]?.ToString(),
                    KichCo = row["KichCo"] != DBNull.Value ? row["KichCo"].ToString() : "",
                    MauSac = row["MauSac"] != DBNull.Value ? row["MauSac"].ToString() : "",
                    HinhAnh = row["HinhAnh"] != DBNull.Value ? row["HinhAnh"].ToString() : "",
                    SoLuong = Convert.ToInt32(row["SoLuong"]),
                    DonGia = Convert.ToDecimal(row["DonGia"]),
                    ThanhTien = Convert.ToDecimal(row["ThanhTien"])
                });
            }

            return danhSach;
        }

        public bool XoaHoaDon(int maHD, out string loi)
        {
            loi = string.Empty;
            using (var kn = KetNoiCSDL.LayKetNoi())
            {
                kn.Open();
                using (var trans = kn.BeginTransaction())
                {
                    try
                    {
                        string sqlHoanKho = @"
                            UPDATE SanPham sp
                            INNER JOIN ChiTietHoaDon ct ON sp.MaSP = ct.MaSP
                            SET sp.SoLuongTon = sp.SoLuongTon + ct.SoLuong
                            WHERE ct.MaHD = @MaHD";

                        using (var cmdKho = new MySqlCommand(sqlHoanKho, kn, trans))
                        {
                            cmdKho.Parameters.Add(new MySqlParameter("@MaHD", MySqlDbType.Int32) { Value = maHD });
                            cmdKho.ExecuteNonQuery();
                        }

                        string sqlXoa = "DELETE FROM HoaDon WHERE MaHD = @MaHD";
                        using (var cmdXoa = new MySqlCommand(sqlXoa, kn, trans))
                        {
                            cmdXoa.Parameters.Add(new MySqlParameter("@MaHD", MySqlDbType.Int32) { Value = maHD });
                            cmdXoa.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        loi = ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}
