using System;
using System.Security.Cryptography;
using System.Text;

namespace CongNghenetBTL.TienIch
{
    public static class MaHoaMatKhau
    {
        public static string BamSHA256(string matKhauGoc)
        {
            if (string.IsNullOrEmpty(matKhauGoc))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(matKhauGoc);
                var hashBytes = sha256.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string BamMatKhau(string matKhauGoc)
        {
            return BamSHA256(matKhauGoc);
        }

        public static bool KiemTraMatKhau(string matKhauNhap, string matKhauLuuTru)
        {
            if (string.IsNullOrEmpty(matKhauNhap) || string.IsNullOrEmpty(matKhauLuuTru))
                return false;

            if (string.Equals(matKhauNhap, matKhauLuuTru, StringComparison.Ordinal))
                return true;

            var maBam = BamSHA256(matKhauNhap);
            return string.Equals(maBam, matKhauLuuTru, StringComparison.OrdinalIgnoreCase);
        }
    }
}
