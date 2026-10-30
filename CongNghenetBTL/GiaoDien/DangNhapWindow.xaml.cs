using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.NghiepVu;
using CongNghenetBTL.TienIch;

namespace CongNghenetBTL.GiaoDien
{
    public partial class DangNhapWindow : Window
    {
        private readonly NguoiDungBLL _nghiepVu = new NguoiDungBLL();
        private bool _dangHienMatKhau = false;

        public DangNhapWindow()
        {
            InitializeComponent();
            txtTenDangNhap.Text = "admin";
            txtMatKhau.Password = "123456";
            txtMatKhauHienThi.Text = "123456";
            txtTenDangNhap.Focus();
        }

        public void DienTenDangNhapMoi(string tenDangNhap)
        {
            txtTenDangNhap.Text = tenDangNhap;
            txtMatKhau.Focus();
            HienThongBao("Đăng ký thành công! Vui lòng nhập mật khẩu để đăng nhập.", false);
        }

        private void CuaSo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void NutThuNho_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void NutDong_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _dangHienMatKhau = !_dangHienMatKhau;

            if (_dangHienMatKhau)
            {
                txtMatKhauHienThi.Text = txtMatKhau.Password;
                txtMatKhau.Visibility = Visibility.Collapsed;
                txtMatKhauHienThi.Visibility = Visibility.Visible;
                txtMatKhauHienThi.Focus();
                txtMatKhauHienThi.CaretIndex = txtMatKhauHienThi.Text.Length;
                btnTogglePassword.Content = "🙈";
            }
            else
            {
                txtMatKhau.Password = txtMatKhauHienThi.Text;
                txtMatKhauHienThi.Visibility = Visibility.Collapsed;
                txtMatKhau.Visibility = Visibility.Visible;
                txtMatKhau.Focus();
                btnTogglePassword.Content = "👁️";
            }
        }

        private void TxtTenDangNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_dangHienMatKhau)
                    txtMatKhauHienThi.Focus();
                else
                    txtMatKhau.Focus();
            }
        }

        private void TxtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ThucHienDangNhap();
            }
        }

        private void BtnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            ThucHienDangNhap();
        }

        private void ThucHienDangNhap()
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = _dangHienMatKhau ? txtMatKhauHienThi.Text : txtMatKhau.Password;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                HienThongBao("Vui lòng nhập tên đăng nhập!", true);
                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                HienThongBao("Vui lòng nhập mật khẩu!", true);
                if (_dangHienMatKhau) txtMatKhauHienThi.Focus(); else txtMatKhau.Focus();
                return;
            }

            NguoiDung nguoiDung = _nghiepVu.DangNhap(tenDangNhap, matKhau, out string thongBao);

            if (nguoiDung != null)
            {
                PhienDangNhap.DangNhap(nguoiDung);

                var trangChu = new TrangChuWindow();
                trangChu.Show();

                this.Close();
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

        private void NutMauAdmin_Click(object sender, RoutedEventArgs e)
        {
            txtTenDangNhap.Text = "admin";
            txtMatKhau.Password = "123456";
            txtMatKhauHienThi.Text = "123456";
            pnlThongBao.Visibility = Visibility.Collapsed;
        }

        private void NutVaoThangTrangChu_Click(object sender, RoutedEventArgs e)
        {
            txtTenDangNhap.Text = "admin";
            txtMatKhau.Password = "123456";
            txtMatKhauHienThi.Text = "123456";
            ThucHienDangNhap();
        }

        private void HyperlinkQuenMatKhau_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Để đặt lại mật khẩu, vui lòng liên hệ Quản lý cửa hàng hoặc sử dụng tài khoản Admin để cấp lại mật khẩu mới!",
                "Thông Báo Quên Mật Khẩu",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnVaoMuaSamKhach_Click(object sender, RoutedEventArgs e)
        {
            var cuaSoKhach = new KhachHangWindow();
            cuaSoKhach.Show();
            this.Close();
        }
    }
}
