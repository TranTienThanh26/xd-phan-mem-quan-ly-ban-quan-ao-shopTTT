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
        private readonly NguoiDungBLL _nguoiDungBLL = new NguoiDungBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private bool _dangHienMatKhau = false;

        public DangNhapWindow()
        {
            InitializeComponent();
            txtTenDangNhap.Focus();
        }

        public void DienTenDangNhapMoi(string tenDangNhap)
        {
            txtTenDangNhap.Text = tenDangNhap;
            txtMatKhau.Password = string.Empty;
            txtMatKhauHienThi.Text = string.Empty;
            txtMatKhau.Focus();
            HienThongBao("Đăng ký tài khoản người dùng thành công! Vui lòng nhập mật khẩu để đăng nhập.", false);
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
                HienThongBao("Vui lòng nhập tên đăng nhập hoặc số điện thoại!", true);
                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                HienThongBao("Vui lòng nhập mật khẩu!", true);
                if (_dangHienMatKhau) txtMatKhauHienThi.Focus(); else txtMatKhau.Focus();
                return;
            }

            // 1. Kiểm tra tài khoản Quản trị viên (Admin)
            NguoiDung admin = _nguoiDungBLL.DangNhap(tenDangNhap, matKhau, out string tbAdmin);
            if (admin != null)
            {
                PhienDangNhap.DangNhap(admin);

                var trangChu = new TrangChuWindow();
                trangChu.Show();
                this.Close();
                return;
            }

            // 2. Kiểm tra tài khoản Người dùng / Khách hàng
            KhachHang kh = _khachHangBLL.DangNhap(tenDangNhap, matKhau, out string tbKhach);
            if (kh != null)
            {
                PhienDangNhap.DangNhapKhachHang(kh);

                var cuaSoKhach = new KhachHangWindow();
                cuaSoKhach.Show();
                this.Close();
                return;
            }

            // 3. Nếu cả hai đều không chính xác
            HienThongBao("Tên đăng nhập / Số điện thoại hoặc mật khẩu không chính xác!", true);
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

        private void HyperlinkQuenMatKhau_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Để đặt lại mật khẩu:\n- Quản trị viên: Vui lòng liên hệ chủ cửa hàng.\n- Khách hàng: Mật khẩu ban đầu mặc định là 123456 hoặc liên hệ hotline cửa hàng để hỗ trợ!",
                "Thông Báo Quên Mật Khẩu",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnDangKyMoi_Click(object sender, RoutedEventArgs e)
        {
            var dangKy = new DangKyWindow();
            dangKy.Owner = this;
            if (dangKy.ShowDialog() == true && !string.IsNullOrEmpty(dangKy.SoDienThoaiMoi))
            {
                DienTenDangNhapMoi(dangKy.SoDienThoaiMoi);
            }
        }
    }
}
