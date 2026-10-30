using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.NghiepVu;
using CongNghenetBTL.TienIch;
using CongNghenetBTL.TruyCapDuLieu;
using Microsoft.Win32;

namespace CongNghenetBTL.GiaoDien
{
    public partial class TrangChuWindow : Window
    {
        private readonly ThongKeDAL _thongKeDAL = new ThongKeDAL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly DanhMucBLL _danhMucBLL = new DanhMucBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly HoaDonBLL _hoaDonBLL = new HoaDonBLL();

        private readonly DispatcherTimer _boDemGio;

        private readonly ObservableCollection<ChiTietHoaDon> _gioHang = new ObservableCollection<ChiTietHoaDon>();

        private string _duongDanAnhChon = string.Empty;

        public TrangChuWindow()
        {
            InitializeComponent();

            dgvGioHang.ItemsSource = _gioHang;

            _boDemGio = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _boDemGio.Tick += BoDemGio_Tick;
            _boDemGio.Start();
            CapNhatDongHo();

            HienThiThongTinNguoiDung();

            dpTuNgayHoaDon.SelectedDate = DateTime.Today.AddDays(-30);
            dpDenNgayHoaDon.SelectedDate = DateTime.Today;

            TaiDuLieuTrangChu();

            this.Loaded += (s, e) =>
            {
                CapNhatTongTienBanHang();
            };
        }

        #region 1. QUẢN LÝ CỬA SỔ & ĐỒNG HỒ & MENU ĐIỀU HƯỚNG

        private void BoDemGio_Tick(object sender, EventArgs e)
        {
            CapNhatDongHo();
        }

        private void CapNhatDongHo()
        {
            DateTime bayGio = DateTime.Now;
            string thuTrongTuan = "";
            switch (bayGio.DayOfWeek)
            {
                case DayOfWeek.Monday: thuTrongTuan = "Thứ Hai"; break;
                case DayOfWeek.Tuesday: thuTrongTuan = "Thứ Ba"; break;
                case DayOfWeek.Wednesday: thuTrongTuan = "Thứ Tư"; break;
                case DayOfWeek.Thursday: thuTrongTuan = "Thứ Năm"; break;
                case DayOfWeek.Friday: thuTrongTuan = "Thứ Sáu"; break;
                case DayOfWeek.Saturday: thuTrongTuan = "Thứ Bảy"; break;
                case DayOfWeek.Sunday: thuTrongTuan = "Chủ Nhật"; break;
            }

            txtDigitalClock.Text = $"{bayGio:HH:mm:ss} - {thuTrongTuan}, {bayGio:dd/MM/yyyy}";
        }

        private void HienThiThongTinNguoiDung()
        {
            NguoiDung user = PhienDangNhap.NguoiDungHienTai;
            if (user != null)
            {
                txtGreeting.Text = $"Xin chào, {user.HoTen} 👋";
                txtUserHoTen.Text = user.HoTen;
                txtUserVaiTro.Text = user.VaiTro;

                string avatar = "NV";
                var parts = user.HoTen.Trim().Split(' ');
                if (parts.Length >= 2)
                {
                    avatar = $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();
                }
                else if (parts.Length == 1 && parts[0].Length > 0)
                {
                    avatar = parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
                }
                txtUserAvatar.Text = avatar;
            }
            else
            {
                txtGreeting.Text = "Xin chào Quản trị viên 👋";
                txtUserHoTen.Text = "Quản Trị Viên";
                txtUserVaiTro.Text = "Quản trị viên";
                txtUserAvatar.Text = "AD";
            }
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ChuyenTrangThaiPhongTo();
            }
            else if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void NutThuNho_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void NutPhongTo_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrangThaiPhongTo();
        }

        private void ChuyenTrangThaiPhongTo()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                btnMaximize.Content = "🗖";
            }
            else
            {
                WindowState = WindowState.Maximized;
                btnMaximize.Content = "🗗";
            }
        }

        private void NutDong_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác Nhận Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void NutDangXuat_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Bạn có muốn đăng xuất khỏi tài khoản hiện tại không?", "Xác Nhận Đăng Xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                _boDemGio.Stop();
                PhienDangNhap.DangXuat();

                var cuaSoDangNhap = new DangNhapWindow();
                cuaSoDangNhap.Show();
                this.Close();
            }
        }

        private void NutMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            var rdo = sender as RadioButton;
            if (rdo == null) return;

            viewHome.Visibility = Visibility.Collapsed;
            viewBanHang.Visibility = Visibility.Collapsed;
            viewSanPham.Visibility = Visibility.Collapsed;
            viewDanhMuc.Visibility = Visibility.Collapsed;
            viewKhachHang.Visibility = Visibility.Collapsed;
            viewHoaDon.Visibility = Visibility.Collapsed;
            viewThongKe.Visibility = Visibility.Collapsed;

            if (rdo == navHome)
            {
                viewHome.Visibility = Visibility.Visible;
                TaiDuLieuTrangChu();
            }
            else if (rdo == navBanHang)
            {
                viewBanHang.Visibility = Visibility.Visible;
                TaiDuLieuBanHang();
            }
            else if (rdo == navSanPham)
            {
                viewSanPham.Visibility = Visibility.Visible;
                TaiDuLieuSanPham();
            }
            else if (rdo == navDanhMuc)
            {
                viewDanhMuc.Visibility = Visibility.Visible;
                TaiDuLieuDanhMuc();
            }
            else if (rdo == navKhachHang)
            {
                viewKhachHang.Visibility = Visibility.Visible;
                TaiDuLieuKhachHang();
            }
            else if (rdo == navHoaDon)
            {
                viewHoaDon.Visibility = Visibility.Visible;
                TaiDuLieuHoaDon();
            }
            else if (rdo == navThongKe)
            {
                viewThongKe.Visibility = Visibility.Visible;
                TaiDuLieuThongKe();
            }
        }

        private void NutLoiTat_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            string tag = btn.Tag?.ToString();
            if (tag == "CuaHangKhach")
            {
                var cuaSoKhach = new KhachHangWindow();
                cuaSoKhach.Show();
            }
            else if (tag == "BanHang")
            {
                navBanHang.IsChecked = true;
            }
            else if (tag == "SanPham")
            {
                navSanPham.IsChecked = true;
            }
            else if (tag == "KhachHang")
            {
                navKhachHang.IsChecked = true;
            }
        }

        #endregion

        #region 2. MÔ ĐUN TRANG CHỦ / TỔNG QUAN (DASHBOARD)

        public void TaiDuLieuTrangChu()
        {
            try
            {
                decimal tongDoanhThu = _thongKeDAL.LayTongDoanhThu();
                int tongDonHang = _thongKeDAL.LayTongSoDonHang();
                int tongSanPham = _thongKeDAL.LayTongSoSanPhamQuanAo();
                int tongKhachHang = _thongKeDAL.LayTongSoKhachHang();

                txtTongDoanhThu.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tongDoanhThu);
                txtTongDonHang.Text = $"{tongDonHang:N0} đơn";
                txtTongSanPham.Text = $"{tongSanPham:N0} mẫu";
                txtTongKhachHang.Text = $"{tongKhachHang:N0} người";

                DataTable dtHoaDon = _thongKeDAL.LayDanhSachHoaDonGanDay(6);
                dgvHoaDon.ItemsSource = dtHoaDon.DefaultView;

                DataTable dtSanPham = _thongKeDAL.LayDanhSachQuanAoMoiNhat(6);
                dgvSanPham.ItemsSource = dtSanPham.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi tải dữ liệu từ CSDL: " + ex.Message, "Lỗi kết nối CSDL", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutLamMoi_Click(object sender, RoutedEventArgs e)
        {
            TaiDuLieuTrangChu();
            MessageBox.Show("Đã làm mới dữ liệu thống kê từ cơ sở dữ liệu QLBanQuanAoDB!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region 3. MÔ ĐUN BÁN HÀNG & THU NGÂN (POS)

        private void TaiDuLieuBanHang()
        {
            try
            {
                var listDM = _danhMucBLL.LayDanhSach();
                var dmChonTatCa = new DanhMuc { MaDanhMuc = 0, TenDanhMuc = "-- Tất cả danh mục --" };
                var dmCombo = new List<DanhMuc> { dmChonTatCa };
                dmCombo.AddRange(listDM);
                cboDanhMucBanHang.ItemsSource = dmCombo;
                cboDanhMucBanHang.SelectedIndex = 0;

                var listKH = _khachHangBLL.LayDanhSach();
                var khachMacDinh = new KhachHang { MaKH = 0, HoTen = "Khách vãng lai", SoDienThoai = "" };
                var khCombo = new List<KhachHang> { khachMacDinh };
                khCombo.AddRange(listKH);
                cboKhachHangBanHang.ItemsSource = khCombo;
                cboKhachHangBanHang.SelectedIndex = 0;

                LocSanPhamBanHang();
                CapNhatTongTienBanHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu bán hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LocSanPhamBanHang()
        {
            string tuKhoa = txtTimKiemBanHang.Text?.Trim();
            int? maDM = null;
            if (cboDanhMucBanHang.SelectedValue != null)
            {
                int val = Convert.ToInt32(cboDanhMucBanHang.SelectedValue);
                if (val > 0) maDM = val;
            }

            var ds = _sanPhamBLL.LayDanhSach(maDM, tuKhoa, chiLayDangBan: true);
            lstSanPhamBanHang.ItemsSource = ds;
        }

        private void TxtTimKiemBanHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LocSanPhamBanHang();
            }
        }

        private void CboDanhMucBanHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || viewBanHang.Visibility != Visibility.Visible) return;
            LocSanPhamBanHang();
        }

        private void NutTatCaBanHang_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemBanHang.Text = string.Empty;
            cboDanhMucBanHang.SelectedIndex = 0;
            LocSanPhamBanHang();
        }

        private void NutChonMuaSP_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var sp = btn?.Tag as SanPham;
            if (sp == null) return;

            if (sp.SoLuongTon <= 0)
            {
                MessageBox.Show($"Mặt hàng '{sp.TenSP}' hiện đã hết hàng trong kho!", "Hết Hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = _gioHang.FirstOrDefault(x => x.MaSP == sp.MaSP);
            if (item != null)
            {
                if (item.SoLuong + 1 > sp.SoLuongTon)
                {
                    MessageBox.Show($"Kho chỉ còn {sp.SoLuongTon} cái, không thể thêm quá số lượng tồn!", "Cảnh Báo Tồn Kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                item.SoLuong++;
                item.ThanhTien = item.SoLuong * item.DonGia;
                dgvGioHang.Items.Refresh();
            }
            else
            {
                _gioHang.Add(new ChiTietHoaDon
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    KichCo = sp.KichCo,
                    MauSac = sp.MauSac,
                    HinhAnh = sp.HinhAnh,
                    DonGia = sp.GiaBan,
                    SoLuong = 1,
                    ThanhTien = sp.GiaBan
                });
            }

            CapNhatTongTienBanHang();
        }

        private void NutXoaKhoiGio_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.Tag as ChiTietHoaDon;
            if (item != null)
            {
                _gioHang.Remove(item);
                CapNhatTongTienBanHang();
            }
        }

        private void NutXoaGioHang_Click(object sender, RoutedEventArgs e)
        {
            if (_gioHang.Count > 0)
            {
                var res = MessageBox.Show("Bạn có muốn xóa toàn bộ giỏ hàng hiện tại không?", "Xác Nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    _gioHang.Clear();
                    CapNhatTongTienBanHang();
                }
            }
        }

        private void TxtGiamGiaBanHang_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            CapNhatTongTienBanHang();
        }

        private void TxtTienKhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            CapNhatTienThoiLai();
        }

        private void CapNhatTongTienBanHang()
        {
            if (!IsLoaded || txtTongTienBanHang == null || txtGiamGiaHienThi == null || txtKhachPhaiTraBanHang == null || txtSoLuongMatHang == null) return;

            decimal tongTien = _gioHang.Sum(x => x.ThanhTien);
            decimal giamGia = 0;
            if (txtGiamGiaBanHang != null)
            {
                decimal.TryParse(txtGiamGiaBanHang.Text?.Trim(), out giamGia);
            }
            if (giamGia < 0) giamGia = 0;

            decimal khachPhaiTra = Math.Max(0, tongTien - giamGia);

            txtSoLuongMatHang.Text = $"({_gioHang.Sum(x => x.SoLuong)} món)";
            txtTongTienBanHang.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tongTien);
            txtGiamGiaHienThi.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", giamGia);
            txtKhachPhaiTraBanHang.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", khachPhaiTra);

            CapNhatTienThoiLai();
        }

        private void CapNhatTienThoiLai()
        {
            if (!IsLoaded || txtTienThoiLai == null || txtTienKhachDua == null) return;

            decimal khachPhaiTra = 0;
            decimal tongTien = _gioHang.Sum(x => x.ThanhTien);
            decimal giamGia = 0;
            if (txtGiamGiaBanHang != null)
            {
                decimal.TryParse(txtGiamGiaBanHang.Text?.Trim(), out giamGia);
            }
            khachPhaiTra = Math.Max(0, tongTien - giamGia);

            decimal tienKhachDua = 0;
            decimal.TryParse(txtTienKhachDua.Text?.Trim(), out tienKhachDua);

            decimal thoiLai = Math.Max(0, tienKhachDua - khachPhaiTra);
            txtTienThoiLai.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", thoiLai);
        }

        private void NutThanhToanBanHang_Click(object sender, RoutedEventArgs e)
        {
            if (_gioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng chọn ít nhất 1 mặt hàng quần áo.", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal tongTien = _gioHang.Sum(x => x.ThanhTien);
            decimal giamGia = 0;
            decimal.TryParse(txtGiamGiaBanHang.Text?.Trim(), out giamGia);
            decimal thanhTien = Math.Max(0, tongTien - giamGia);

            var hoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                TongTien = tongTien,
                GiamGia = giamGia,
                ThanhTien = thanhTien,
                PhuongThucTT = (cboPhuongThucBanHang.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt",
                GhiChu = txtGhiChuBanHang.Text?.Trim(),
                MaND = PhienDangNhap.NguoiDungHienTai?.MaND,
                TenNhanVien = PhienDangNhap.NguoiDungHienTai?.HoTen ?? "Quản trị viên"
            };

            if (cboKhachHangBanHang.SelectedValue != null)
            {
                int maKH = Convert.ToInt32(cboKhachHangBanHang.SelectedValue);
                if (maKH > 0) hoaDon.MaKH = maKH;
            }

            foreach (var item in _gioHang)
            {
                hoaDon.DanhSachChiTiet.Add(item);
            }

            int maHDMoi;
            string thongBao;
            if (_hoaDonBLL.TaoHoaDon(hoaDon, out maHDMoi, out thongBao))
            {
                MessageBox.Show($"Thanh toán hóa đơn #{maHDMoi} thành công!\nSố tiền thực thu: {thanhTien:N0} đ\nKho hàng đã được tự động cập nhật.",
                    "Thanh Toán Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);

                _gioHang.Clear();
                txtGiamGiaBanHang.Text = "0";
                txtTienKhachDua.Text = "0";
                txtGhiChuBanHang.Text = string.Empty;
                CapNhatTongTienBanHang();

                LocSanPhamBanHang();
                TaiDuLieuTrangChu();
            }
            else
            {
                MessageBox.Show(thongBao, "Lỗi Thanh Toán", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 4. MÔ ĐUN QUẢN LÝ QUẦN ÁO (SẢN PHẨM)

        private void TaiDuLieuSanPham()
        {
            try
            {
                var listDM = _danhMucBLL.LayDanhSach();
                cboDanhMucSanPham.ItemsSource = listDM;

                var dmChonTatCa = new DanhMuc { MaDanhMuc = 0, TenDanhMuc = "-- Tất cả danh mục --" };
                var dmComboLoc = new List<DanhMuc> { dmChonTatCa };
                dmComboLoc.AddRange(listDM);
                cboLocDanhMucSanPham.ItemsSource = dmComboLoc;
                cboLocDanhMucSanPham.SelectedIndex = 0;

                LocSanPham();
                LamMoiFormSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LocSanPham()
        {
            string tuKhoa = txtTimKiemSanPham.Text?.Trim();
            int? maDM = null;
            if (cboLocDanhMucSanPham.SelectedValue != null)
            {
                int val = Convert.ToInt32(cboLocDanhMucSanPham.SelectedValue);
                if (val > 0) maDM = val;
            }

            var ds = _sanPhamBLL.LayDanhSach(maDM, tuKhoa, chiLayDangBan: false);
            dgvQuanAo.ItemsSource = ds;
        }

        private void TxtTimKiemSanPham_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LocSanPham();
            }
        }

        private void NutTimKiemSanPham_Click(object sender, RoutedEventArgs e)
        {
            LocSanPham();
        }

        private void NutTatCaSanPham_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemSanPham.Text = string.Empty;
            cboLocDanhMucSanPham.SelectedIndex = 0;
            LocSanPham();
        }

        private void NutThemMoiSanPham_Click(object sender, RoutedEventArgs e)
        {
            LamMoiFormSanPham();
            txtTenSP.Focus();
        }

        private void DgvQuanAo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sp = dgvQuanAo.SelectedItem as SanPham;
            if (sp == null) return;

            txtMaSP.Text = sp.MaSP.ToString();
            txtTenSP.Text = sp.TenSP;
            if (sp.MaDanhMuc.HasValue)
            {
                cboDanhMucSanPham.SelectedValue = sp.MaDanhMuc.Value;
            }
            else
            {
                cboDanhMucSanPham.SelectedIndex = -1;
            }

            foreach (ComboBoxItem item in cboKichCoSanPham.Items)
            {
                if (item.Content.ToString().Equals(sp.KichCo, StringComparison.OrdinalIgnoreCase))
                {
                    cboKichCoSanPham.SelectedItem = item;
                    break;
                }
            }

            txtMauSacSanPham.Text = sp.MauSac;
            txtChatLieuSanPham.Text = sp.ChatLieu;
            txtGiaNhapSanPham.Text = sp.GiaNhap.ToString("0");
            txtGiaBanSanPham.Text = sp.GiaBan.ToString("0");
            txtSoLuongTonSanPham.Text = sp.SoLuongTon.ToString();
            chkTrangThaiSanPham.IsChecked = sp.TrangThai;
            txtMoTaSanPham.Text = sp.MoTa;

            _duongDanAnhChon = sp.HinhAnh ?? string.Empty;
            txtDuongDanAnhHienTai.Text = !string.IsNullOrWhiteSpace(_duongDanAnhChon) ? _duongDanAnhChon : "HinhAnh/mac_dinh.jpg";

            HienThiAnhXemTruoc(sp.DuongDanAnhDayDu);
        }

        private void HienThiAnhXemTruoc(string duongDan)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(duongDan) && File.Exists(duongDan))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(duongDan, UriKind.Absolute);
                    bitmap.EndInit();
                    imgSanPhamPreview.Source = bitmap;
                    return;
                }
            }
            catch { }

            imgSanPhamPreview.Source = null;
        }

        private void NutChonAnhSanPham_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn hình ảnh sản phẩm quần áo",
                Filter = "Tệp hình ảnh (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp|Tất cả tệp (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                _duongDanAnhChon = dlg.FileName;
                txtDuongDanAnhHienTai.Text = Path.GetFileName(_duongDanAnhChon);
                HienThiAnhXemTruoc(_duongDanAnhChon);
            }
        }

        private void NutThemSanPham_Click(object sender, RoutedEventArgs e)
        {
            var sp = LayThongTinSanPhamTuForm();
            if (sp == null) return;

            string thongBao;
            if (_sanPhamBLL.Them(sp, out thongBao))
            {
                MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                LocSanPham();
                LamMoiFormSanPham();
                TaiDuLieuTrangChu();
            }
            else
            {
                MessageBox.Show(thongBao, "Thông Báo Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutSuaSanPham_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ bảng danh sách để cập nhật!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sp = LayThongTinSanPhamTuForm();
            if (sp == null) return;
            sp.MaSP = Convert.ToInt32(txtMaSP.Text);

            string thongBao;
            if (_sanPhamBLL.CapNhat(sp, out thongBao))
            {
                MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                LocSanPham();
                TaiDuLieuTrangChu();
            }
            else
            {
                MessageBox.Show(thongBao, "Thông Báo Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutXoaSanPham_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int maSP = Convert.ToInt32(txtMaSP.Text);
            var res = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm mã #{maSP} '{txtTenSP.Text}' không?", "Xác Nhận Xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                string thongBao;
                if (_sanPhamBLL.Xoa(maSP, out thongBao))
                {
                    MessageBox.Show(thongBao, "Kết Quả", MessageBoxButton.OK, MessageBoxImage.Information);
                    LocSanPham();
                    LamMoiFormSanPham();
                    TaiDuLieuTrangChu();
                }
                else
                {
                    MessageBox.Show(thongBao, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NutLamMoiSanPham_Click(object sender, RoutedEventArgs e)
        {
            LamMoiFormSanPham();
        }

        private void LamMoiFormSanPham()
        {
            txtMaSP.Text = string.Empty;
            txtTenSP.Text = string.Empty;
            cboDanhMucSanPham.SelectedIndex = -1;
            cboKichCoSanPham.SelectedIndex = 1;
            txtMauSacSanPham.Text = string.Empty;
            txtChatLieuSanPham.Text = string.Empty;
            txtGiaNhapSanPham.Text = "0";
            txtGiaBanSanPham.Text = "0";
            txtSoLuongTonSanPham.Text = "0";
            chkTrangThaiSanPham.IsChecked = true;
            txtMoTaSanPham.Text = string.Empty;

            _duongDanAnhChon = "HinhAnh/mac_dinh.jpg";
            txtDuongDanAnhHienTai.Text = "HinhAnh/mac_dinh.jpg";
            HienThiAnhXemTruoc(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HinhAnh", "mac_dinh.jpg"));

            dgvQuanAo.SelectedItem = null;
        }

        private SanPham LayThongTinSanPhamTuForm()
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm quần áo!", "Thiếu Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenSP.Focus();
                return null;
            }

            decimal giaNhap = 0;
            decimal.TryParse(txtGiaNhapSanPham.Text?.Trim(), out giaNhap);

            decimal giaBan = 0;
            if (!decimal.TryParse(txtGiaBanSanPham.Text?.Trim(), out giaBan) || giaBan <= 0)
            {
                MessageBox.Show("Giá bán sản phẩm phải là một số lớn hơn 0 đ!", "Dữ Liệu Sai", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtGiaBanSanPham.Focus();
                return null;
            }

            int tonKho = 0;
            if (!int.TryParse(txtSoLuongTonSanPham.Text?.Trim(), out tonKho) || tonKho < 0)
            {
                MessageBox.Show("Số lượng tồn kho phải là số nguyên không âm!", "Dữ Liệu Sai", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSoLuongTonSanPham.Focus();
                return null;
            }

            int? maDM = null;
            if (cboDanhMucSanPham.SelectedValue != null)
            {
                maDM = Convert.ToInt32(cboDanhMucSanPham.SelectedValue);
            }

            string size = (cboKichCoSanPham.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "M";

            return new SanPham
            {
                TenSP = txtTenSP.Text.Trim(),
                MaDanhMuc = maDM,
                KichCo = size,
                MauSac = txtMauSacSanPham.Text?.Trim(),
                ChatLieu = txtChatLieuSanPham.Text?.Trim(),
                GiaNhap = giaNhap,
                GiaBan = giaBan,
                SoLuongTon = tonKho,
                DonViTinh = "Cái",
                HinhAnh = _duongDanAnhChon,
                MoTa = txtMoTaSanPham.Text?.Trim(),
                TrangThai = chkTrangThaiSanPham.IsChecked == true
            };
        }

        #endregion

        #region 5. MÔ ĐUN QUẢN LÝ DANH MỤC THỜI TRANG

        private void TaiDuLieuDanhMuc()
        {
            try
            {
                var ds = _danhMucBLL.LayDanhSach();
                dgvDanhMuc.ItemsSource = ds;
                LamMoiFormDanhMuc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvDanhMuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dm = dgvDanhMuc.SelectedItem as DanhMuc;
            if (dm == null) return;

            txtMaDanhMuc.Text = dm.MaDanhMuc.ToString();
            txtTenDanhMuc.Text = dm.TenDanhMuc;
            txtMoTaDanhMuc.Text = dm.MoTa;
        }

        private void NutThemDanhMuc_Click(object sender, RoutedEventArgs e)
        {
            var dm = new DanhMuc
            {
                TenDanhMuc = txtTenDanhMuc.Text?.Trim(),
                MoTa = txtMoTaDanhMuc.Text?.Trim()
            };

            string thongBao;
            if (_danhMucBLL.Them(dm, out thongBao))
            {
                MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDuLieuDanhMuc();
            }
            else
            {
                MessageBox.Show(thongBao, "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutSuaDanhMuc_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDanhMuc.Text))
            {
                MessageBox.Show("Vui lòng chọn danh mục cần sửa từ danh sách!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dm = new DanhMuc
            {
                MaDanhMuc = Convert.ToInt32(txtMaDanhMuc.Text),
                TenDanhMuc = txtTenDanhMuc.Text?.Trim(),
                MoTa = txtMoTaDanhMuc.Text?.Trim()
            };

            string thongBao;
            if (_danhMucBLL.CapNhat(dm, out thongBao))
            {
                MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDuLieuDanhMuc();
            }
            else
            {
                MessageBox.Show(thongBao, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutXoaDanhMuc_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDanhMuc.Text))
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int maDM = Convert.ToInt32(txtMaDanhMuc.Text);
            var res = MessageBox.Show($"Bạn có chắc chắn muốn xóa danh mục '{txtTenDanhMuc.Text}' không?", "Xác Nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                string thongBao;
                if (_danhMucBLL.Xoa(maDM, out thongBao))
                {
                    MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                    TaiDuLieuDanhMuc();
                }
                else
                {
                    MessageBox.Show(thongBao, "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void NutLamMoiDanhMuc_Click(object sender, RoutedEventArgs e)
        {
            LamMoiFormDanhMuc();
        }

        private void LamMoiFormDanhMuc()
        {
            txtMaDanhMuc.Text = string.Empty;
            txtTenDanhMuc.Text = string.Empty;
            txtMoTaDanhMuc.Text = string.Empty;
            dgvDanhMuc.SelectedItem = null;
        }

        #endregion

        #region 6. MÔ ĐUN QUẢN LÝ KHÁCH HÀNG

        private void TaiDuLieuKhachHang()
        {
            try
            {
                string tuKhoa = txtTimKiemKhachHang.Text?.Trim();
                var ds = _khachHangBLL.LayDanhSach(tuKhoa);
                dgvKhachHang.ItemsSource = ds;
                LamMoiFormKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu khách hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtTimKiemKhachHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TaiDuLieuKhachHang();
            }
        }

        private void NutTimKiemKhachHang_Click(object sender, RoutedEventArgs e)
        {
            TaiDuLieuKhachHang();
        }

        private void NutTatCaKhachHang_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemKhachHang.Text = string.Empty;
            TaiDuLieuKhachHang();
        }

        private void DgvKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var kh = dgvKhachHang.SelectedItem as KhachHang;
            if (kh == null) return;

            txtMaKH.Text = kh.MaKH.ToString();
            txtHoTenKH.Text = kh.HoTen;
            txtSoDienThoaiKH.Text = kh.SoDienThoai;
            txtEmailKH.Text = kh.Email;
            txtDiaChiKH.Text = kh.DiaChi;

            foreach (ComboBoxItem item in cboLoaiKhach.Items)
            {
                if (item.Content.ToString().Equals(kh.LoaiKhach, StringComparison.OrdinalIgnoreCase))
                {
                    cboLoaiKhach.SelectedItem = item;
                    break;
                }
            }
        }

        private void NutThemKhachHang_Click(object sender, RoutedEventArgs e)
        {
            var kh = LayThongTinKhachHangTuForm();
            if (kh == null) return;

            string thongBao;
            if (_khachHangBLL.Them(kh, out thongBao))
            {
                MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDuLieuKhachHang();
                TaiDuLieuTrangChu();
            }
            else
            {
                MessageBox.Show(thongBao, "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutSuaKhachHang_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn một khách hàng từ bảng để sửa!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var kh = LayThongTinKhachHangTuForm();
            if (kh == null) return;
            kh.MaKH = Convert.ToInt32(txtMaKH.Text);

            string thongBao;
            if (_khachHangBLL.CapNhat(kh, out thongBao))
            {
                MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDuLieuKhachHang();
            }
            else
            {
                MessageBox.Show(thongBao, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NutXoaKhachHang_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int maKH = Convert.ToInt32(txtMaKH.Text);
            var res = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng '{txtHoTenKH.Text}' không?", "Xác Nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                string thongBao;
                if (_khachHangBLL.Xoa(maKH, out thongBao))
                {
                    MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                    TaiDuLieuKhachHang();
                    TaiDuLieuTrangChu();
                }
                else
                {
                    MessageBox.Show(thongBao, "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void NutLamMoiKhachHang_Click(object sender, RoutedEventArgs e)
        {
            LamMoiFormKhachHang();
        }

        private void LamMoiFormKhachHang()
        {
            txtMaKH.Text = string.Empty;
            txtHoTenKH.Text = string.Empty;
            txtSoDienThoaiKH.Text = string.Empty;
            txtEmailKH.Text = string.Empty;
            txtDiaChiKH.Text = string.Empty;
            cboLoaiKhach.SelectedIndex = 0;
            dgvKhachHang.SelectedItem = null;
        }

        private KhachHang LayThongTinKhachHangTuForm()
        {
            if (string.IsNullOrWhiteSpace(txtHoTenKH.Text))
            {
                MessageBox.Show("Họ và tên khách hàng không được để trống!", "Thiếu Thông Tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtHoTenKH.Focus();
                return null;
            }

            return new KhachHang
            {
                HoTen = txtHoTenKH.Text.Trim(),
                SoDienThoai = txtSoDienThoaiKH.Text?.Trim(),
                Email = txtEmailKH.Text?.Trim(),
                DiaChi = txtDiaChiKH.Text?.Trim(),
                LoaiKhach = (cboLoaiKhach.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Thường"
            };
        }

        #endregion

        #region 7. MÔ ĐUN LỊCH SỬ HÓA ĐƠN & CHI TIẾT

        private void TaiDuLieuHoaDon()
        {
            try
            {
                DateTime? tuNgay = dpTuNgayHoaDon.SelectedDate;
                DateTime? denNgay = dpDenNgayHoaDon.SelectedDate;
                string tuKhoa = txtTimKiemHoaDon.Text?.Trim();

                var ds = _hoaDonBLL.LayDanhSach(tuNgay, denNgay, tuKhoa);
                dgvLichSuHoaDon.ItemsSource = ds;

                dgvChiTietHoaDonXem.ItemsSource = null;
                txtTieuDeChiTietHD.Text = "📦 Chi Tiết Sản Phẩm Trong Hóa Đơn (Chọn 1 dòng ở trên để xem)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtTimKiemHoaDon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TaiDuLieuHoaDon();
            }
        }

        private void NutLocHoaDon_Click(object sender, RoutedEventArgs e)
        {
            TaiDuLieuHoaDon();
        }

        private void NutTatCaHoaDon_Click(object sender, RoutedEventArgs e)
        {
            dpTuNgayHoaDon.SelectedDate = null;
            dpDenNgayHoaDon.SelectedDate = null;
            txtTimKiemHoaDon.Text = string.Empty;
            TaiDuLieuHoaDon();
        }

        private void DgvLichSuHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var hd = dgvLichSuHoaDon.SelectedItem as HoaDon;
            if (hd == null) return;

            txtTieuDeChiTietHD.Text = $"📦 Chi Tiết Hóa Đơn #{hd.MaHD} - Ngày Lập: {hd.NgayLap:dd/MM/yyyy HH:mm} - Khách Hàng: {hd.TenKhachHang}";
            var chiTiet = _hoaDonBLL.LayChiTietTheoHoaDon(hd.MaHD);
            dgvChiTietHoaDonXem.ItemsSource = chiTiet;
        }

        private void NutHuyHoaDon_Click(object sender, RoutedEventArgs e)
        {
            var hd = dgvLichSuHoaDon.SelectedItem as HoaDon;
            if (hd == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn từ bảng để hủy!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = MessageBox.Show($"Bạn có chắc chắn muốn hủy hóa đơn #{hd.MaHD} không?\nToàn bộ số lượng quần áo sẽ được hoàn trả lại vào kho!", 
                "Xác Nhận Hủy Hóa Đơn", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
            {
                string thongBao;
                if (_hoaDonBLL.HuyHoaDon(hd.MaHD, out thongBao))
                {
                    MessageBox.Show(thongBao, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                    TaiDuLieuHoaDon();
                    TaiDuLieuTrangChu();
                }
                else
                {
                    MessageBox.Show(thongBao, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region 9. MÔ ĐUN BÁO CÁO DOANH THU & KHO HÀNG

        private void TaiDuLieuThongKe()
        {
            try
            {
                decimal doanhThuNgay = _thongKeDAL.LayDoanhThuHomNay();
                decimal doanhThuThang = _thongKeDAL.LayDoanhThuThangNay();
                decimal tongDoanhThu = _thongKeDAL.LayTongDoanhThu();

                txtDoanhThuHomNay.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", doanhThuNgay);
                txtDoanhThuThangNay.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", doanhThuThang);
                txtTongDoanhThuThongKe.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tongDoanhThu);

                DataTable dtTop = _thongKeDAL.LayTopSanPhamBanChay(5);
                dgvTopBanChay.ItemsSource = dtTop.DefaultView;

                DataTable dtSapHet = _thongKeDAL.LaySanPhamSapHetHang(15);
                dgvSapHetHang.ItemsSource = dtSapHet.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo thống kê: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMoGiaoDienKhachHang_Click(object sender, RoutedEventArgs e)
        {
            var cuaSoKhach = new KhachHangWindow();
            cuaSoKhach.Show();
        }

        #endregion
    }
}
