using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.NghiepVu;

namespace CongNghenetBTL.GiaoDien
{
    public partial class DangKyWindow : Window
    {
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();

        public string SoDienThoaiMoi { get; private set; }

        public DangKyWindow()
        {
            InitializeComponent();
            txtHoTen.Focus();
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
            string hoTen = txtHoTen.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string matKhau = txtMatKhau.Password;
            string xacNhanMatKhau = txtXacNhanMatKhau.Password;
            string email = txtEmail.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();

            var kh = new KhachHang
            {
                HoTen = hoTen,
                SoDienThoai = soDienThoai,
                Email = email,
                DiaChi = diaChi
            };

            bool thanhCong = _khachHangBLL.DangKy(kh, matKhau, xacNhanMatKhau, out string thongBao);

            if (thanhCong)
            {
                SoDienThoaiMoi = soDienThoai;
                MessageBox.Show(
                    "Đăng ký tài khoản người dùng thành công! Bạn có thể sử dụng Số điện thoại và Mật khẩu vừa tạo để đăng nhập vào cửa hàng.",
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
