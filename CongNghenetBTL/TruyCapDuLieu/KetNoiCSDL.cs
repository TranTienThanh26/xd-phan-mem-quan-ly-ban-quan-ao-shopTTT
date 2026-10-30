using System;
using System.Configuration;
using System.Data;
using MySqlConnector;

namespace CongNghenetBTL.TruyCapDuLieu
{
    public static class KetNoiCSDL
    {
        private static readonly string ChuoiKetNoiDuPhong = 
            @"Server=localhost;Port=3306;Database=QLBanQuanAoDB;Uid=root;Pwd=12345;Charset=utf8mb4;";

        public static string ChuoiKetNoi
        {
            get
            {
                try
                {
                    var cauHinh = ConfigurationManager.ConnectionStrings["QLBanQuanAoConn"];
                    if (cauHinh != null && !string.IsNullOrWhiteSpace(cauHinh.ConnectionString))
                    {
                        return cauHinh.ConnectionString;
                    }
                }
                catch
                {
                }
                return ChuoiKetNoiDuPhong;
            }
        }

        public static MySqlConnection LayKetNoi()
        {
            return new MySqlConnection(ChuoiKetNoi);
        }

        public static bool KiemTraKetNoi(out string loi)
        {
            loi = string.Empty;
            try
            {
                using (var kn = LayKetNoi())
                {
                    kn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                loi = ex.Message;
                return false;
            }
        }

        public static DataTable ThucThiTruyVan(string cauTruyVan, MySqlParameter[] thamSo = null)
        {
            var bangDuLieu = new DataTable();
            using (var kn = LayKetNoi())
            {
                using (var lenh = new MySqlCommand(cauTruyVan, kn))
                {
                    if (thamSo != null && thamSo.Length > 0)
                    {
                        lenh.Parameters.AddRange(thamSo);
                    }
                    using (var boDoc = new MySqlDataAdapter(lenh))
                    {
                        boDoc.Fill(bangDuLieu);
                    }
                }
            }
            return bangDuLieu;
        }

        public static int ThucThiKhongTruyVan(string cauLenh, MySqlParameter[] thamSo = null)
        {
            using (var kn = LayKetNoi())
            {
                using (var lenh = new MySqlCommand(cauLenh, kn))
                {
                    if (thamSo != null && thamSo.Length > 0)
                    {
                        lenh.Parameters.AddRange(thamSo);
                    }
                    kn.Open();
                    return lenh.ExecuteNonQuery();
                }
            }
        }

        public static object ThucThiGiaTri(string cauTruyVan, MySqlParameter[] thamSo = null)
        {
            using (var kn = LayKetNoi())
            {
                using (var lenh = new MySqlCommand(cauTruyVan, kn))
                {
                    if (thamSo != null && thamSo.Length > 0)
                    {
                        lenh.Parameters.AddRange(thamSo);
                    }
                    kn.Open();
                    return lenh.ExecuteScalar();
                }
            }
        }
    }
}
