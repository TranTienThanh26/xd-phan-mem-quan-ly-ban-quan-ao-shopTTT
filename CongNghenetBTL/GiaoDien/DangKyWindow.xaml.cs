using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.NghiepVu;

namespace CongNghenetBTL.GiaoDien
{
    public partial class DangKyWindow : Window
    {
        private readonly NguoiDungBLL _nghiepVu = new NguoiDungBLL();

        public string TenDangNhapMoi { get; private set; }

        public DangKyWindow()
        {
            InitializeComponent();
            txtTenDangNhap.Focus();
        }

        private void CuaSo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void NutHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NutXacNhanDangKy_Click(object sender, RoutedEventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string matKhau = txtMatKhau.Password;
            string xacNhanMatKhau = txtXacNhanMatKhau.Password;
            string email = txtEmail.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string vaiTro = (cboVaiTro.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Nhân viên";

            var nd = new NguoiDung
            {
                TenDangNhap = tenDangNhap,
                HoTen = hoTen,
                MatKhau = matKhau,
                Email = email,
                SoDienThoai = soDienThoai,
                VaiTro = vaiTro,
                TrangThai = true
            };

            bool thanhCong = _nghiepVu.DangKy(nd, xacNhanMatKhau, out string thongBao);

            if (thanhCong)
            {
                TenDangNhapMoi = tenDangNhap;
                MessageBox.Show(
                    "Đăng ký tài khoản thành công! Bạn có thể sử dụng tài khoản này để đăng nhập ngay bây giờ.",
                    "Đăng Ký Thành Công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            else
            {
                HienThongBao(thongBao, true);
            }
        }

        private void HienThongBao(string noiDung, bool laLoi)
        {
            pnlThongBao.Visibility = Visibility.Visible;
            txtThongBao.Text = noiDung;

            if (laLoi)
            {
                pnlThongBao.Background = (Brush)new BrushConverter().ConvertFrom("#FEF2F2");
                pnlThongBao.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FCA5A5");
                txtThongBao.Foreground = (Brush)new BrushConverter().ConvertFrom("#DC2626");
                iconThongBao.Text = "⚠️";
            }
            else
            {
                pnlThongBao.Background = (Brush)new BrushConverter().ConvertFrom("#ECFDF5");
                pnlThongBao.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#6EE7B7");
                txtThongBao.Foreground = (Brush)new BrushConverter().ConvertFrom("#059669");
                iconThongBao.Text = "✅";
            }
        }
    }
}
