using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CongNghenetBTL.MoHinh;
using CongNghenetBTL.NghiepVu;
using CongNghenetBTL.TienIch;

namespace CongNghenetBTL.GiaoDien
{
    public partial class KhachHangWindow : Window
    {
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly DanhMucBLL _danhMucBLL = new DanhMucBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly HoaDonBLL _hoaDonBLL = new HoaDonBLL();

        private readonly ObservableCollection<ChiTietHoaDon> _gioHang = new ObservableCollection<ChiTietHoaDon>();

        private List<SanPham> _danhSachSanPhamGoc = new List<SanPham>();
        private List<DanhMuc> _danhSachDanhMuc = new List<DanhMuc>();

        private SanPham _spDangXemChiTiet = null;
        private string _sizeDangChon = string.Empty;

        private int _maDanhMucDangChon = 0;

        private bool _daKhoiTao = false;

        public KhachHangWindow()
        {
            InitializeComponent();
            _daKhoiTao = true;

            dgvGioHangKhach.ItemsSource = _gioHang;

            KhoiTaoDuLieu();
        }

        private void KhoiTaoDuLieu()
        {
            try
            {
                NapDanhSachDanhMuc();
                NapDanhSachSanPham();
                KiemTraTrangThaiDangNhapKhach();
                CapNhatTongTienGioHang();
            }
            catch (Exception ex)
            {
                HienThongBao("Lỗi khi tải dữ liệu cửa hàng: " + ex.Message, "Thông Báo Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region XỬ LÝ CỬA SỔ & THANH ĐIỀU HƯỚNG

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                if (btnMaximize != null) btnMaximize.Content = "🗖";
            }
            else
            {
                WindowState = WindowState.Maximized;
                if (btnMaximize != null) btnMaximize.Content = "🗗";
            }
        }

        private void NutDong_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnChuyenSangAdmin_Click(object sender, RoutedEventArgs e)
        {
            var dangNhap = new DangNhapWindow();
            dangNhap.Show();
            this.Close();
        }

        private void TabNav_Checked(object sender, RoutedEventArgs e)
        {
            if (pnlTabMuaSam == null || pnlTabGioHang == null || pnlTabDonHang == null || pnlTabTaiKhoan == null)
                return;

            pnlTabMuaSam.Visibility = Visibility.Collapsed;
            pnlTabGioHang.Visibility = Visibility.Collapsed;
            pnlTabDonHang.Visibility = Visibility.Collapsed;
            pnlTabTaiKhoan.Visibility = Visibility.Collapsed;

            if (tabNavMuaSam.IsChecked == true)
            {
                pnlTabMuaSam.Visibility = Visibility.Visible;
            }
            else if (tabNavGioHang.IsChecked == true)
            {
                pnlTabGioHang.Visibility = Visibility.Visible;
                CapNhatTongTienGioHang();
            }
            else if (tabNavDonHang.IsChecked == true)
            {
                pnlTabDonHang.Visibility = Visibility.Visible;
                int maKH = PhienDangNhap.KhachDaDangNhap ? PhienDangNhap.KhachHangHienTai.MaKH : 0;
                TaiLichSuDonHang(maKH);
            }
            else if (tabNavTaiKhoan.IsChecked == true)
            {
                pnlTabTaiKhoan.Visibility = Visibility.Visible;
                if (!PhienDangNhap.KhachDaDangNhap)
                {
                    HienModalDangNhapKhach(true);
                }
                else
                {
                    HienThiThongTinTaiKhoan();
                }
            }
        }

        #endregion

        #region 1. TAB CỬA HÀNG / MUA SẮM (CATALOG & CARD VIEWS)

        private void NapDanhSachDanhMuc()
        {
            _danhSachDanhMuc = _danhMucBLL.LayDanhSach();

            while (pnlDanhMucPills.Children.Count > 1)
            {
                pnlDanhMucPills.Children.RemoveAt(1);
            }

            foreach (var dm in _danhSachDanhMuc)
            {
                var pill = new RadioButton
                {
                    Content = dm.TenDanhMuc,
                    Style = (Style)FindResource("NutMenuBenTrai"),
                    Height = 34,
                    Margin = new Thickness(0, 0, 6, 0),
                    Tag = dm.MaDanhMuc
                };
                pill.Checked += FilterDanhMuc_Checked;
                pnlDanhMucPills.Children.Add(pill);
            }
        }

        private void FilterDanhMuc_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rdo && rdo.Tag != null)
            {
                _maDanhMucDangChon = Convert.ToInt32(rdo.Tag);
                ApDungBoLoc();
            }
        }

        private void NapDanhSachSanPham()
        {
            _danhSachSanPhamGoc = _sanPhamBLL.LayDanhSach(null, null, true);
            ApDungBoLoc();
        }

        private void BtnNapLaiDuLieu_Click(object sender, RoutedEventArgs e)
        {
            NapDanhSachSanPham();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_daKhoiTao) return;
            ApDungBoLoc();
        }

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            if (!_daKhoiTao) return;
            ApDungBoLoc();
        }

        private void TxtTimKiemSanPham_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApDungBoLoc();
            }
        }

        private void BtnXemTatCa_Click(object sender, RoutedEventArgs e)
        {
            if (!_daKhoiTao) return;
            txtTimKiemSanPham.Text = string.Empty;
            cboLocSize.SelectedIndex = 0;
            cboLocGia.SelectedIndex = 0;
            cboSapXep.SelectedIndex = 0;
            btnDMTatCa.IsChecked = true;
            _maDanhMucDangChon = 0;
            ApDungBoLoc();
        }

        private void ApDungBoLoc()
        {
            if (!_daKhoiTao || _danhSachSanPhamGoc == null || wrapDanhSachSanPham == null || txtKetQuaTimKiem == null) return;

            var ds = _danhSachSanPhamGoc.AsEnumerable();

            if (_maDanhMucDangChon > 0)
            {
                ds = ds.Where(sp => sp.MaDanhMuc == _maDanhMucDangChon);
            }

            string tuKhoa = txtTimKiemSanPham.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                ds = ds.Where(sp =>
                    (!string.IsNullOrEmpty(sp.TenSP) && sp.TenSP.ToLower().Contains(tuKhoa)) ||
                    (!string.IsNullOrEmpty(sp.MauSac) && sp.MauSac.ToLower().Contains(tuKhoa)) ||
                    (!string.IsNullOrEmpty(sp.KichCo) && sp.KichCo.ToLower().Contains(tuKhoa)) ||
                    (!string.IsNullOrEmpty(sp.ChatLieu) && sp.ChatLieu.ToLower().Contains(tuKhoa)) ||
                    sp.MaSP.ToString() == tuKhoa);
            }

            string sizeChon = (cboLocSize?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(sizeChon) && sizeChon != "Tất cả")
            {
                ds = ds.Where(sp => !string.IsNullOrEmpty(sp.KichCo) && sp.KichCo.IndexOf(sizeChon, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            string giaChon = (cboLocGia?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(giaChon) && giaChon != "Tất cả mức giá")
            {
                if (giaChon == "Dưới 300.000 đ")
                    ds = ds.Where(sp => sp.GiaBan < 300000);
                else if (giaChon == "300k - 500k")
                    ds = ds.Where(sp => sp.GiaBan >= 300000 && sp.GiaBan <= 500000);
                else if (giaChon == "Trên 500.000 đ")
                    ds = ds.Where(sp => sp.GiaBan > 500000);
            }

            string sapXep = (cboSapXep?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (sapXep == "Giá tăng dần")
                ds = ds.OrderBy(sp => sp.GiaBan);
            else if (sapXep == "Giá giảm dần")
                ds = ds.OrderByDescending(sp => sp.GiaBan);
            else if (sapXep == "Tên A - Z")
                ds = ds.OrderBy(sp => sp.TenSP);
            else
                ds = ds.OrderByDescending(sp => sp.MaSP);

            var ketQua = ds.ToList();
            HienThiDanhSachSanPham(ketQua);
        }

        private void HienThiDanhSachSanPham(List<SanPham> danhSach)
        {
            if (!_daKhoiTao || wrapDanhSachSanPham == null || txtKetQuaTimKiem == null) return;

            wrapDanhSachSanPham.Children.Clear();

            txtKetQuaTimKiem.Text = $"Hiển thị {danhSach.Count} mẫu quần áo thời trang cao cấp:";

            if (danhSach.Count == 0)
            {
                pnlKhongCoSanPham.Visibility = Visibility.Visible;
                return;
            }

            pnlKhongCoSanPham.Visibility = Visibility.Collapsed;

            foreach (var sp in danhSach)
            {
                var card = TaoCardSanPham(sp);
                wrapDanhSachSanPham.Children.Add(card);
            }
        }

        private Border TaoCardSanPham(SanPham sp)
        {
            var card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                BorderBrush = (Brush)FindResource("MauVienNheBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(7),
                Padding = new Thickness(12),
                Effect = (System.Windows.Media.Effects.Effect)FindResource("BongTheCard"),
                Cursor = Cursors.Hand
            };

            // Hieu ung hover vien doi mau noi bat
            card.MouseEnter += (s, e) => { card.BorderBrush = (Brush)FindResource("MauChinhBrush"); };
            card.MouseLeave += (s, e) => { card.BorderBrush = (Brush)FindResource("MauVienNheBrush"); };

            // Bam vao bat ky dau tren the ao de hien chi tiet
            card.MouseLeftButtonUp += (s, e) =>
            {
                if (e.OriginalSource is DependencyObject dep)
                {
                    while (dep != null && dep != card)
                    {
                        if (dep is Button) return;
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                }
                HienModalChiTiet(sp);
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(175) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var borderAnh = new Border
            {
                CornerRadius = new CornerRadius(8),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1F5F9")),
                ClipToBounds = true,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(borderAnh, 0);

            var img = new Image
            {
                Stretch = Stretch.UniformToFill
            };
            try
            {
                string path = sp.DuongDanAnhDayDu;
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    img.Source = bitmap;
                }
                else
                {
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/HinhAnh/mac_dinh.jpg", UriKind.Absolute));
                }
            }
            catch
            {
                img.Source = new BitmapImage(new Uri("pack://application:,,,/HinhAnh/mac_dinh.jpg", UriKind.Absolute));
            }
            borderAnh.Child = img;
            grid.Children.Add(borderAnh);

            var tagDM = new Border
            {
                Background = (Brush)FindResource("MauChinhNhatBrush"),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2, 6, 2),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 6)
            };
            Grid.SetRow(tagDM, 1);
            tagDM.Child = new TextBlock
            {
                Text = sp.TenDanhMuc ?? "Thời Trang",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("MauChinhBrush")
            };
            grid.Children.Add(tagDM);

            var txtTen = new TextBlock
            {
                Text = sp.TenSP,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("MauChuChinhBrush"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 4)
            };
            Grid.SetRow(txtTen, 2);
            grid.Children.Add(txtTen);

            var txtChiTiet = new TextBlock
            {
                Text = $"Size: {sp.KichCo} | {sp.MauSac}",
                FontSize = 11,
                Foreground = (Brush)FindResource("MauChuPhuBrush"),
                Margin = new Thickness(0, 0, 0, 6)
            };
            Grid.SetRow(txtChiTiet, 3);
            grid.Children.Add(txtChiTiet);

            var pnlGia = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            Grid.SetRow(pnlGia, 4);

            var txtGia = new TextBlock
            {
                Text = sp.GiaBanDinhDang,
                FontSize = 16,
                FontWeight = FontWeights.Black,
                Foreground = (Brush)FindResource("MauThoiTrangBrush"),
                VerticalAlignment = VerticalAlignment.Center
            };
            pnlGia.Children.Add(txtGia);

            var txtTon = new TextBlock
            {
                Text = sp.SoLuongTon > 0 ? $"Còn: {sp.SoLuongTon}" : "Hết hàng",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = sp.SoLuongTon > 10 ? (Brush)FindResource("MauThanhCongBrush") : (Brush)FindResource("MauCanhBaoBrush"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            pnlGia.Children.Add(txtTon);
            grid.Children.Add(pnlGia);

            var pnlNut = new Grid();
            Grid.SetRow(pnlNut, 5);
            pnlNut.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            pnlNut.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.3, GridUnitType.Star) });

            var btnThemGio = new Button
            {
                Content = "🛒 Thêm Giỏ",
                Style = (Style)FindResource("NutPhu"),
                Height = 34,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 4, 0),
                Tag = sp
            };
            btnThemGio.Click += (s, ev) => HienModalChiTiet(sp);
            Grid.SetColumn(btnThemGio, 0);
            pnlNut.Children.Add(btnThemGio);

            var btnMua = new Button
            {
                Content = "⚡ Mua Ngay",
                Style = (Style)FindResource("NutChinh"),
                Background = (Brush)FindResource("MauThoiTrangBrush"),
                BorderBrush = (Brush)FindResource("MauThoiTrangBrush"),
                Height = 34,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(4, 0, 0, 0),
                Tag = sp
            };
            btnMua.Click += (s, ev) => HienModalChiTiet(sp);
            Grid.SetColumn(btnMua, 1);
            pnlNut.Children.Add(btnMua);

            grid.Children.Add(pnlNut);
            card.Child = grid;
            return card;
        }

        #endregion

                #region 2. MODAL CHI TIẾT SẢN PHẨM & CHỌN SIZE, SỐ LƯỢNG

        private void HienModalChiTiet(SanPham sp)
        {
            if (sp == null) return;
            _spDangXemChiTiet = sp;

            txtModalTenSP.Text = sp.TenSP;
            txtModalDanhMuc.Text = sp.TenDanhMuc ?? "Thời Trang";
            txtModalGiaBan.Text = sp.GiaBanDinhDang;
            txtModalSizeMau.Text = $"Màu sắc: {sp.MauSac}";
            txtModalChatLieu.Text = $"Chất liệu vải: {sp.ChatLieu}";
            txtModalTonKho.Text = sp.SoLuongTon > 0 ? $"Số lượng có sẵn trong kho: {sp.SoLuongTon} cái" : "Tạm hết hàng trong kho";
            txtModalGhiChuTonKho.Text = sp.SoLuongTon > 0 ? $"(Còn {sp.SoLuongTon} cái)" : "(Hết hàng)";
            txtModalMoTa.Text = !string.IsNullOrEmpty(sp.MoTa) ? sp.MoTa : "Sản phẩm thời trang cao cấp, đường may tỉ mỉ, tôn dáng và bền đẹp.";
            txtModalSoLuongMua.Text = "1";

            NapDanhSachSizeModal(sp);

            btnModalThemVaoGio.IsEnabled = sp.SoLuongTon > 0;
            btnModalThemVaoGio.Content = sp.SoLuongTon > 0 ? "🛒 Thêm Vào Giỏ Hàng" : "❌ Tạm Hết Hàng";

            try
            {
                string path = sp.DuongDanAnhDayDu;
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    imgModalAnhSP.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                }
                else
                {
                    imgModalAnhSP.Source = new BitmapImage(new Uri("pack://application:,,,/HinhAnh/mac_dinh.jpg", UriKind.Absolute));
                }
            }
            catch
            {
                imgModalAnhSP.Source = new BitmapImage(new Uri("pack://application:,,,/HinhAnh/mac_dinh.jpg", UriKind.Absolute));
            }

            modalChiTietSP.Visibility = Visibility.Visible;
        }

        private void NapDanhSachSizeModal(SanPham sp)
        {
            if (pnlModalChonSize == null) return;
            pnlModalChonSize.Children.Clear();

            var dsSize = LayDanhSachSizeChoSanPham(sp);
            _sizeDangChon = !string.IsNullOrEmpty(sp.KichCo) && dsSize.Contains(sp.KichCo) ? sp.KichCo : dsSize.FirstOrDefault() ?? "Freesize";
            if (txtModalSizeHienTai != null) txtModalSizeHienTai.Text = _sizeDangChon;

            foreach (var sz in dsSize)
            {
                var btnSize = new Button
                {
                    Content = sz,
                    Tag = sz,
                    Height = 32,
                    MinWidth = 46,
                    Margin = new Thickness(0, 0, 8, 6),
                    FontSize = 12,
                    Cursor = Cursors.Hand
                };

                CapNhatGiaoDienNutSize(btnSize, sz == _sizeDangChon);

                btnSize.Click += (s, e) =>
                {
                    _sizeDangChon = sz;
                    if (txtModalSizeHienTai != null) txtModalSizeHienTai.Text = _sizeDangChon;
                    foreach (var child in pnlModalChonSize.Children)
                    {
                        if (child is Button b && b.Tag is string tagSz)
                        {
                            CapNhatGiaoDienNutSize(b, tagSz == _sizeDangChon);
                        }
                    }
                };

                pnlModalChonSize.Children.Add(btnSize);
            }
        }

        private void CapNhatGiaoDienNutSize(Button btn, bool isSelected)
        {
            if (isSelected)
            {
                btn.Background = (Brush)FindResource("MauChinhBrush");
                btn.Foreground = Brushes.White;
                btn.BorderBrush = (Brush)FindResource("MauChinhBrush");
                btn.FontWeight = FontWeights.Bold;
            }
            else
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1F5F9"));
                btn.Foreground = (Brush)FindResource("MauChuChinhBrush");
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
                btn.FontWeight = FontWeights.SemiBold;
            }
        }

        private List<string> LayDanhSachSizeChoSanPham(SanPham sp)
        {
            var result = new List<string>();
            string dm = sp.TenDanhMuc?.ToLower() ?? string.Empty;
            string ten = sp.TenSP?.ToLower() ?? string.Empty;
            string kc = sp.KichCo?.Trim() ?? string.Empty;

            if (kc.Equals("Freesize", StringComparison.OrdinalIgnoreCase))
            {
                result.Add("Freesize");
            }
            else if (kc.Contains("cm") || dm.Contains("phụ kiện") || ten.Contains("thắt lưng") || ten.Contains("túi"))
            {
                result.Add(!string.IsNullOrEmpty(kc) ? kc : "Tiêu chuẩn");
            }
            else if (dm.Contains("quần") || ten.Contains("quần") || int.TryParse(kc, out _))
            {
                result.AddRange(new[] { "28", "29", "30", "31", "32", "33", "34" });
            }
            else
            {
                result.AddRange(new[] { "S", "M", "L", "XL", "XXL" });
            }

            if (!string.IsNullOrEmpty(kc) && !result.Contains(kc))
            {
                result.Add(kc);
            }

            return result;
        }

        private void BtnDongModalSP_Click(object sender, RoutedEventArgs e)
        {
            modalChiTietSP.Visibility = Visibility.Collapsed;
        }

        private void ModalSP_BackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == modalChiTietSP)
            {
                modalChiTietSP.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnModalGiamSL_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtModalSoLuongMua.Text, out int sl) && sl > 1)
            {
                txtModalSoLuongMua.Text = (sl - 1).ToString();
            }
        }

        private void BtnModalTangSL_Click(object sender, RoutedEventArgs e)
        {
            if (_spDangXemChiTiet == null) return;
            if (int.TryParse(txtModalSoLuongMua.Text, out int sl))
            {
                if (sl < _spDangXemChiTiet.SoLuongTon)
                {
                    txtModalSoLuongMua.Text = (sl + 1).ToString();
                }
                else
                {
                    HienThongBao($"Kho chỉ còn {_spDangXemChiTiet.SoLuongTon} sản phẩm!", "Thông Báo Kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnModalThemVaoGio_Click(object sender, RoutedEventArgs e)
        {
            if (_spDangXemChiTiet == null) return;
            if (int.TryParse(txtModalSoLuongMua.Text, out int sl) && sl > 0)
            {
                string sizeChon = !string.IsNullOrEmpty(_sizeDangChon) ? _sizeDangChon : _spDangXemChiTiet.KichCo;
                if (ThemVaoGioHang(_spDangXemChiTiet, sl, sizeChon, hienThongBao: true))
                {
                    modalChiTietSP.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnModalMuaNgay_Click(object sender, RoutedEventArgs e)
        {
            if (_spDangXemChiTiet == null) return;
            if (int.TryParse(txtModalSoLuongMua.Text, out int sl) && sl > 0)
            {
                string sizeChon = !string.IsNullOrEmpty(_sizeDangChon) ? _sizeDangChon : _spDangXemChiTiet.KichCo;
                if (ThemVaoGioHang(_spDangXemChiTiet, sl, sizeChon, hienThongBao: false))
                {
                    modalChiTietSP.Visibility = Visibility.Collapsed;
                    // Chuyển thẳng sang Tab Giỏ Hàng & Thanh Toán ngay lập tức
                    tabNavGioHang.IsChecked = true;
                }
            }
        }

        #endregion

        #region 3. TAB GIỎ HÀNG & THANH TOÁN (CART & CHECKOUT)

        private bool ThemVaoGioHang(SanPham sp, int soLuong, string kichCoChon = null, bool hienThongBao = true)
        {
            if (sp == null) return false;

            if (string.IsNullOrWhiteSpace(kichCoChon))
            {
                kichCoChon = !string.IsNullOrWhiteSpace(sp.KichCo) ? sp.KichCo : "Tiêu chuẩn";
            }

            if (sp.SoLuongTon <= 0)
            {
                HienThongBao($"Mẫu '{sp.TenSP}' tạm thời đã hết hàng trong kho!", "Hết Hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            int tongSoLuongSpTrongGio = _gioHang.Where(x => x.MaSP == sp.MaSP).Sum(x => x.SoLuong);
            if (tongSoLuongSpTrongGio + soLuong > sp.SoLuongTon)
            {
                HienThongBao($"Bạn đã có {tongSoLuongSpTrongGio} sản phẩm này trong giỏ. Số lượng kho chỉ còn {sp.SoLuongTon} cái!", "Cảnh Báo Tồn Kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var item = _gioHang.FirstOrDefault(x => x.MaSP == sp.MaSP && x.KichCo == kichCoChon);
            if (item != null)
            {
                item.SoLuong += soLuong;
                item.ThanhTien = item.SoLuong * item.DonGia;
                dgvGioHangKhach.Items.Refresh();
            }
            else
            {
                _gioHang.Add(new ChiTietHoaDon
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    KichCo = kichCoChon,
                    MauSac = sp.MauSac,
                    DonGia = sp.GiaBan,
                    SoLuong = soLuong,
                    ThanhTien = sp.GiaBan * soLuong,
                    HinhAnh = sp.HinhAnh
                });
            }

            CapNhatBadgeGioHang();
            CapNhatTongTienGioHang();

            if (hienThongBao)
            {
                HienThongBao($"Đã thêm {soLuong} x '{sp.TenSP}' (Size: {kichCoChon}) vào giỏ hàng thành công!", "Giỏ Hàng TTT Store", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return true;
        }

        private void CapNhatBadgeGioHang()
        {
            int tongSL = _gioHang.Sum(x => x.SoLuong);
            txtSoLuongBadge.Text = tongSL.ToString();
            txtTongMonTrongGio.Text = $" ({tongSL} món)";
        }

        private void CapNhatTongTienGioHang()
        {
            decimal tamTinh = _gioHang.Sum(x => x.ThanhTien);
            txtTamTinhTien.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tamTinh);

            decimal tyLeGiam = 0m;
            if (PhienDangNhap.KhachDaDangNhap && PhienDangNhap.KhachHangHienTai != null)
            {
                tyLeGiam = PhienDangNhap.KhachHangHienTai.TyLeGiamGia;
                lblGiamGiaThanhVien.Text = $"Giảm giá thành viên ({PhienDangNhap.KhachHangHienTai.LoaiKhach}):";
            }
            else
            {
                lblGiamGiaThanhVien.Text = "Giảm giá thành viên:";
            }

            decimal tienGiam = tamTinh * tyLeGiam;
            txtTienGiamGia.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "-{0:N0} đ", tienGiam);

            decimal phiShip = (tamTinh >= 500000 || tamTinh == 0) ? 0 : 30000;
            txtPhiShip.Text = phiShip == 0 ? "0 đ (Freeship)" : string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", phiShip);

            decimal tongThanhToan = tamTinh - tienGiam + phiShip;
            if (tongThanhToan < 0) tongThanhToan = 0;
            txtTongThanhToan.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tongThanhToan);
        }

        private void BtnTangSoLuong_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int maSP)
            {
                var item = _gioHang.FirstOrDefault(x => x.MaSP == maSP);
                var sp = _danhSachSanPhamGoc.FirstOrDefault(x => x.MaSP == maSP);
                if (item != null && sp != null)
                {
                    if (item.SoLuong < sp.SoLuongTon)
                    {
                        item.SoLuong++;
                        item.ThanhTien = item.SoLuong * item.DonGia;
                        dgvGioHangKhach.Items.Refresh();
                        CapNhatBadgeGioHang();
                        CapNhatTongTienGioHang();
                    }
                    else
                    {
                        HienThongBao($"Kho chỉ còn {sp.SoLuongTon} sản phẩm!", "Tồn Kho Tối Đa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void BtnGiamSoLuong_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int maSP)
            {
                var item = _gioHang.FirstOrDefault(x => x.MaSP == maSP);
                if (item != null)
                {
                    if (item.SoLuong > 1)
                    {
                        item.SoLuong--;
                        item.ThanhTien = item.SoLuong * item.DonGia;
                        dgvGioHangKhach.Items.Refresh();
                    }
                    else
                    {
                        _gioHang.Remove(item);
                    }
                    CapNhatBadgeGioHang();
                    CapNhatTongTienGioHang();
                }
            }
        }

        private void BtnXoaMonKhoiGio_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int maSP)
            {
                var item = _gioHang.FirstOrDefault(x => x.MaSP == maSP);
                if (item != null)
                {
                    _gioHang.Remove(item);
                    CapNhatBadgeGioHang();
                    CapNhatTongTienGioHang();
                }
            }
        }

        private void BtnXoaHetGioHang_Click(object sender, RoutedEventArgs e)
        {
            if (_gioHang.Count == 0) return;
            if (HienThongBao("Bạn có chắc chắn muốn xóa toàn bộ sản phẩm trong giỏ hàng?", "Xóa Giỏ Hàng", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _gioHang.Clear();
                CapNhatBadgeGioHang();
                CapNhatTongTienGioHang();
            }
        }

        private void BtnTiepTucMua_Click(object sender, RoutedEventArgs e)
        {
            tabNavMuaSam.IsChecked = true;
        }

        private void RadPTTT_Checked(object sender, RoutedEventArgs e)
        {
            if (pnlThongTinCK == null) return;
            pnlThongTinCK.Visibility = radChuyenKhoan.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnXacNhanDatHang_Click(object sender, RoutedEventArgs e)
        {
            if (_gioHang.Count == 0)
            {
                HienThongBao("Giỏ hàng của bạn đang trống! Vui lòng chọn ít nhất 1 sản phẩm trước khi đặt hàng.", "Giỏ Hàng Trống", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string hoTen = txtKhachDatHoTen.Text.Trim();
            string soDienThoai = txtKhachDatSoDienThoai.Text.Trim();
            string diaChi = txtKhachDatDiaChi.Text.Trim();
            string ghiChu = txtKhachDatGhiChu.Text.Trim();

            if (string.IsNullOrEmpty(hoTen))
            {
                HienThongBao("Vui lòng nhập họ và tên người nhận hàng!", "Thiếu Thông Tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtKhachDatHoTen.Focus();
                return;
            }

            if (string.IsNullOrEmpty(soDienThoai))
            {
                HienThongBao("Vui lòng nhập số điện thoại nhận hàng!", "Thiếu Thông Tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtKhachDatSoDienThoai.Focus();
                return;
            }

            if (string.IsNullOrEmpty(diaChi))
            {
                HienThongBao("Vui lòng nhập địa chỉ giao hàng chi tiết!", "Thiếu Thông Tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtKhachDatDiaChi.Focus();
                return;
            }

            string pttt = "Tiền mặt (COD)";
            if (radChuyenKhoan.IsChecked == true) pttt = "Chuyển khoản";
            else if (radThe.IsChecked == true) pttt = "Thẻ ATM / QR";

            int maKH = 0;
            if (PhienDangNhap.KhachDaDangNhap)
            {
                maKH = PhienDangNhap.KhachHangHienTai.MaKH;
            }
            else
            {
                var khCu = _khachHangBLL.LayTheoSoDienThoai(soDienThoai);
                if (khCu != null)
                {
                    maKH = khCu.MaKH;
                }
                else
                {
                    var khMoi = new KhachHang
                    {
                        HoTen = hoTen,
                        SoDienThoai = soDienThoai,
                        DiaChi = diaChi,
                        LoaiKhach = "Thường",
                        DiemTichLuy = 10,
                        MatKhau = MaHoaMatKhau.BamMatKhau("123456")
                    };
                    if (_khachHangBLL.Them(khMoi, out _))
                    {
                        maKH = khMoi.MaKH;
                    }
                }
            }

            decimal tamTinh = _gioHang.Sum(x => x.ThanhTien);
            decimal tyLeGiam = (PhienDangNhap.KhachDaDangNhap && PhienDangNhap.KhachHangHienTai != null) ? PhienDangNhap.KhachHangHienTai.TyLeGiamGia : 0m;
            decimal giamGia = tamTinh * tyLeGiam;
            decimal phiShip = (tamTinh >= 500000) ? 0 : 30000;
            decimal thanhTien = tamTinh - giamGia + phiShip;

            var hoaDon = new HoaDon
            {
                MaKH = maKH > 0 ? maKH : (int?)null,
                MaND = null,
                NgayLap = DateTime.Now,
                TongTien = tamTinh,
                GiamGia = giamGia,
                ThanhTien = thanhTien,
                PhuongThucTT = pttt,
                GhiChu = $"[Khách Đặt Online] Đ/c: {diaChi} | Ghi chú: {ghiChu}",
                DanhSachChiTiet = _gioHang.ToList()
            };

            if (_hoaDonBLL.TaoHoaDon(hoaDon, out int maHDMoi, out string thongBao))
            {
                HienThongBao(
                    $"🎉 ĐẶT HÀNG THÀNH CÔNG!\n\n" +
                    $"• Mã đơn hàng: #HD{maHDMoi}\n" +
                    $"• Người nhận: {hoTen} ({soDienThoai})\n" +
                    $"• Địa chỉ: {diaChi}\n" +
                    $"• Tổng thanh toán: {thanhTien:N0} đ ({pttt})\n\n" +
                    $"Cảm ơn bạn đã mua sắm tại TTT Store! Đơn hàng đã được liên kết với hệ thống quản trị để chuẩn bị giao.",
                    "Đặt Hàng Thành Công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _gioHang.Clear();
                CapNhatBadgeGioHang();
                CapNhatTongTienGioHang();

                NapDanhSachSanPham();

                tabNavDonHang.IsChecked = true;
                if (dpTraCuuTuNgay != null) dpTraCuuTuNgay.SelectedDate = DateTime.Today;
                if (dpTraCuuDenNgay != null) dpTraCuuDenNgay.SelectedDate = DateTime.Today;
                TaiLichSuDonHang(maKH, DateTime.Today, DateTime.Today);
            }
            else
            {
                HienThongBao("Đặt hàng thất bại: " + thongBao, "Lỗi Đặt Hàng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 4. TAB ĐƠN HÀNG CỦA TÔI (MY ORDERS)

        private void BtnTraCuuDon_Click(object sender, RoutedEventArgs e)
        {
            int maKH = PhienDangNhap.KhachDaDangNhap ? PhienDangNhap.KhachHangHienTai.MaKH : 0;
            DateTime? tuNgay = dpTraCuuTuNgay?.SelectedDate;
            DateTime? denNgay = dpTraCuuDenNgay?.SelectedDate;
            TaiLichSuDonHang(maKH, tuNgay, denNgay);
        }

        private void BtnHomNayDon_Click(object sender, RoutedEventArgs e)
        {
            if (dpTraCuuTuNgay != null) dpTraCuuTuNgay.SelectedDate = DateTime.Today;
            if (dpTraCuuDenNgay != null) dpTraCuuDenNgay.SelectedDate = DateTime.Today;
            int maKH = PhienDangNhap.KhachDaDangNhap ? PhienDangNhap.KhachHangHienTai.MaKH : 0;
            TaiLichSuDonHang(maKH, DateTime.Today, DateTime.Today);
        }

        private void BtnTatCaDon_Click(object sender, RoutedEventArgs e)
        {
            if (dpTraCuuTuNgay != null) dpTraCuuTuNgay.SelectedDate = null;
            if (dpTraCuuDenNgay != null) dpTraCuuDenNgay.SelectedDate = null;
            int maKH = PhienDangNhap.KhachDaDangNhap ? PhienDangNhap.KhachHangHienTai.MaKH : 0;
            TaiLichSuDonHang(maKH, null, null);
        }

        private void TaiLichSuDonHang(int maKH, DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            List<HoaDon> dsDon;
            if (maKH > 0)
            {
                dsDon = _hoaDonBLL.LayLichSuTheoKhachHang(maKH, tuNgay, denNgay);
            }
            else
            {
                dsDon = _hoaDonBLL.LayDanhSach(tuNgay, denNgay);
            }

            dgvLichSuDonKhach.ItemsSource = dsDon;
            dgvChiTietDonKhach.ItemsSource = null;
            txtTieuDeChiTietDon.Text = "Chi Tiết Các Món Trong Đơn (Chọn 1 đơn bên trái)";

            if (dsDon != null && dsDon.Count > 0)
            {
                dgvLichSuDonKhach.SelectedIndex = 0;
            }
        }

        private void DgvLichSuDonKhach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvLichSuDonKhach.SelectedItem is HoaDon hd)
            {
                txtTieuDeChiTietDon.Text = $"Chi Tiết Đơn Hàng #HD{hd.MaHD} - Ngày {hd.NgayLap:dd/MM/yyyy HH:mm}";
                var dsChiTiet = _hoaDonBLL.LayChiTietTheoHoaDon(hd.MaHD);
                dgvChiTietDonKhach.ItemsSource = dsChiTiet;
            }
        }

        #endregion

        #region 5. TAB TÀI KHOẢN & BẢO MẬT (PROFILE & SECURITY)

        private void KiemTraTrangThaiDangNhapKhach()
        {
            if (PhienDangNhap.KhachDaDangNhap && PhienDangNhap.KhachHangHienTai != null)
            {
                var kh = PhienDangNhap.KhachHangHienTai;
                pnlKhachDaDangNhap.Visibility = Visibility.Visible;
                btnMoDangNhapKhach.Visibility = Visibility.Collapsed;

                txtTenKhachHeader.Text = kh.HoTen;
                txtHangKhachHeader.Text = $"{kh.LoaiKhach} | {kh.DiemTichLuy} Điểm";

                txtKhachDatHoTen.Text = kh.HoTen;
                txtKhachDatSoDienThoai.Text = kh.SoDienThoai;
                txtKhachDatDiaChi.Text = kh.DiaChi;

                HienThiThongTinTaiKhoan();
            }
            else
            {
                pnlKhachDaDangNhap.Visibility = Visibility.Collapsed;
                btnMoDangNhapKhach.Visibility = Visibility.Visible;
            }
        }

        private void HienThiThongTinTaiKhoan()
        {
            if (!PhienDangNhap.KhachDaDangNhap) return;
            var kh = PhienDangNhap.KhachHangHienTai;

            txtCardHoTen.Text = kh.HoTen;
            txtCardSoDienThoai.Text = $"SĐT Bảo Mật: {kh.SoDienThoaiBaoMat}";
            txtCardHangThanhVien.Text = kh.HienThiHang;
            txtCardDiem.Text = $"Điểm Tích Lũy: {kh.DiemTichLuy} Điểm";

            txtProfileHoTen.Text = kh.HoTen;
            txtProfileSoDienThoai.Text = kh.SoDienThoai;
            txtProfileEmail.Text = kh.Email;
            txtProfileDiaChi.Text = kh.DiaChi;
        }

        private void BtnLuuProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!PhienDangNhap.KhachDaDangNhap) return;
            var kh = PhienDangNhap.KhachHangHienTai;

            string hoTen = txtProfileHoTen.Text.Trim();
            string email = txtProfileEmail.Text.Trim();
            string diaChi = txtProfileDiaChi.Text.Trim();

            if (string.IsNullOrEmpty(hoTen))
            {
                HienThongBao("Họ tên không được để trống!", "Thiếu Thông Tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            kh.HoTen = hoTen;
            kh.Email = email;
            kh.DiaChi = diaChi;

            if (_khachHangBLL.CapNhat(kh, out string tb))
            {
                HienThongBao("Cập nhật thông tin tài khoản thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                KiemTraTrangThaiDangNhapKhach();
            }
            else
            {
                HienThongBao("Lỗi khi cập nhật: " + tb, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDoiMatKhau_Click(object sender, RoutedEventArgs e)
        {
            if (!PhienDangNhap.KhachDaDangNhap) return;
            var kh = PhienDangNhap.KhachHangHienTai;

            string mkCu = txtDoiMKCu.Password;
            string mkMoi = txtDoiMKMoi.Password;
            string xacNhan = txtDoiMKXacNhan.Password;

            if (_khachHangBLL.DoiMatKhau(kh.MaKH, mkCu, mkMoi, xacNhan, out string tb))
            {
                HienThongBao("Đổi mật khẩu bảo mật thành công! Mật khẩu mới đã được băm SHA-256 an toàn.", "Bảo Mật Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                txtDoiMKCu.Password = string.Empty;
                txtDoiMKMoi.Password = string.Empty;
                txtDoiMKXacNhan.Password = string.Empty;
            }
            else
            {
                HienThongBao("Đổi mật khẩu thất bại:\n" + tb, "Lỗi Đổi Mật Khẩu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region 6. MODAL ĐĂNG NHẬP / ĐĂNG KÝ KHÁCH HÀNG (AUTH)

        private void BtnMoDangNhapKhach_Click(object sender, RoutedEventArgs e)
        {
            var dangNhap = new DangNhapWindow();
            Application.Current.MainWindow = dangNhap;
            dangNhap.Show();
            this.Close();
        }

        private void BtnMoDangNhapChinh_Click(object sender, RoutedEventArgs e)
        {
            var dangNhap = new DangNhapWindow();
            Application.Current.MainWindow = dangNhap;
            dangNhap.Show();
            this.Close();
        }

        private void HienModalDangNhapKhach(bool laDangNhap)
        {
            modalAuthKhachHang.Visibility = Visibility.Visible;
            pnlAuthThongBao.Visibility = Visibility.Collapsed;

            if (laDangNhap)
            {
                radTabAuthDangNhap.IsChecked = true;
                pnlAuthDangNhapForm.Visibility = Visibility.Visible;
                pnlAuthDangKyForm.Visibility = Visibility.Collapsed;
                txtAuthDNTaiKhoan.Focus();
            }
            else
            {
                radTabAuthDangKy.IsChecked = true;
                pnlAuthDangNhapForm.Visibility = Visibility.Collapsed;
                pnlAuthDangKyForm.Visibility = Visibility.Visible;
                txtAuthDKHoTen.Focus();
            }
        }

        private void BtnDongModalAuth_Click(object sender, RoutedEventArgs e)
        {
            modalAuthKhachHang.Visibility = Visibility.Collapsed;
        }

        private void ModalAuth_BackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == modalAuthKhachHang)
            {
                modalAuthKhachHang.Visibility = Visibility.Collapsed;
            }
        }

        private void RadAuthTab_Checked(object sender, RoutedEventArgs e)
        {
            if (pnlAuthDangNhapForm == null || pnlAuthDangKyForm == null) return;
            pnlAuthThongBao.Visibility = Visibility.Collapsed;

            if (radTabAuthDangNhap.IsChecked == true)
            {
                pnlAuthDangNhapForm.Visibility = Visibility.Visible;
                pnlAuthDangKyForm.Visibility = Visibility.Collapsed;
            }
            else
            {
                pnlAuthDangNhapForm.Visibility = Visibility.Collapsed;
                pnlAuthDangKyForm.Visibility = Visibility.Visible;
            }
        }

        private void BtnDienKhachVIPMau_Click(object sender, RoutedEventArgs e)
        {
            txtAuthDNTaiKhoan.Text = "0901234567";
            txtAuthDNMatKhau.Password = "123456";
            pnlAuthThongBao.Visibility = Visibility.Collapsed;
        }

        private void TxtAuthDNMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnThucHienDangNhapKhach_Click(sender, e);
            }
        }

        private void BtnThucHienDangNhapKhach_Click(object sender, RoutedEventArgs e)
        {
            string sdt = txtAuthDNTaiKhoan.Text.Trim();
            string mk = txtAuthDNMatKhau.Password;

            var kh = _khachHangBLL.DangNhap(sdt, mk, out string thongBao);
            if (kh != null)
            {
                PhienDangNhap.DangNhapKhachHang(kh);
                modalAuthKhachHang.Visibility = Visibility.Collapsed;

                KiemTraTrangThaiDangNhapKhach();
                CapNhatTongTienGioHang();

                HienThongBao($"Chào mừng quý khách {kh.HoTen} đã đăng nhập thành công!\nBạn thuộc {kh.HienThiHang}.", "Đăng Nhập Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                pnlAuthThongBao.Visibility = Visibility.Visible;
                pnlAuthThongBao.Background = (Brush)new BrushConverter().ConvertFrom("#FEF2F2");
                txtAuthThongBao.Foreground = (Brush)new BrushConverter().ConvertFrom("#DC2626");
                txtAuthThongBao.Text = "⚠️ " + thongBao;
            }
        }

        private void BtnThucHienDangKyKhach_Click(object sender, RoutedEventArgs e)
        {
            string hoTen = txtAuthDKHoTen.Text.Trim();
            string sdt = txtAuthDKSoDienThoai.Text.Trim();
            string mk = txtAuthDKMatKhau.Password;
            string xacNhan = txtAuthDKXacNhanMK.Password;
            string diaChi = txtAuthDKDiaChi.Text.Trim();

            var kh = new KhachHang
            {
                HoTen = hoTen,
                SoDienThoai = sdt,
                DiaChi = diaChi
            };

            if (_khachHangBLL.DangKy(kh, mk, xacNhan, out string thongBao))
            {
                PhienDangNhap.DangNhapKhachHang(kh);
                modalAuthKhachHang.Visibility = Visibility.Collapsed;

                KiemTraTrangThaiDangNhapKhach();
                CapNhatTongTienGioHang();

                HienThongBao("🎉 " + thongBao, "Đăng Ký Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                pnlAuthThongBao.Visibility = Visibility.Visible;
                pnlAuthThongBao.Background = (Brush)new BrushConverter().ConvertFrom("#FEF2F2");
                txtAuthThongBao.Foreground = (Brush)new BrushConverter().ConvertFrom("#DC2626");
                txtAuthThongBao.Text = "⚠️ " + thongBao;
            }
        }

        private void BtnDangXuatKhach_Click(object sender, RoutedEventArgs e)
        {
            PhienDangNhap.DangXuatKhachHang();
            PhienDangNhap.DangXuat();

            var dangNhap = new DangNhapWindow();
            Application.Current.MainWindow = dangNhap;
            dangNhap.Show();
            this.Close();
        }

        #endregion
    
        #region PHUONG THUC HO TRO DEMO & KIEM THU TU DONG BEN KHACH HANG

        private MessageBoxResult HienThongBao(string noiDung, string tieuDe = "Thông Báo", MessageBoxButton nut = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            string bieuTuong = "🎉";
            if (icon == MessageBoxImage.Error || icon == MessageBoxImage.Warning) bieuTuong = "⚠️";
            HienThongBaoChung(tieuDe, noiDung, bieuTuong);
            return MessageBoxResult.Yes;
        }

        public void BtnDongThongBaoChung_Click(object sender, RoutedEventArgs e)
        {
            DongThongBaoChung();
        }

        public void ModalThongBao_BackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == modalThongBaoChung)
            {
                DongThongBaoChung();
            }
        }

        public void HienThongBaoChung(string tieuDe, string noiDung, string bieuTuong = "🎉")
        {
            if (txtThongBaoTieuDe != null) txtThongBaoTieuDe.Text = tieuDe;
            if (txtThongBaoNoiDung != null) txtThongBaoNoiDung.Text = noiDung;
            if (txtThongBaoBieuTuong != null) txtThongBaoBieuTuong.Text = bieuTuong;
            if (modalThongBaoChung != null) modalThongBaoChung.Visibility = Visibility.Visible;
        }

        public void DongThongBaoChung()
        {
            if (modalThongBaoChung != null) modalThongBaoChung.Visibility = Visibility.Collapsed;
        }

        public void DongModalChiTiet()
        {
            if (modalChiTietSP != null) modalChiTietSP.Visibility = Visibility.Collapsed;
        }

        public void DatOverlayDemo(string buocHienTai)
        {
            if (pnlDemoOverlay != null && txtDemoStep != null)
            {
                txtDemoStep.Text = buocHienTai;
                pnlDemoOverlay.Visibility = Visibility.Visible;
            }
        }

        public void AnOverlayDemo()
        {
            if (pnlDemoOverlay != null) pnlDemoOverlay.Visibility = Visibility.Collapsed;
        }

        public void ChonTab(int index)
        {
            if (index == 0) tabNavMuaSam.IsChecked = true;
            else if (index == 1) tabNavGioHang.IsChecked = true;
            else if (index == 2) tabNavDonHang.IsChecked = true;
            else if (index == 3) tabNavTaiKhoan.IsChecked = true;
            TabNav_Checked(null, null);
        }

        public void ChonDanhMucTheoTen(string tuKhoa)
        {
            foreach (var child in pnlDanhMucPills.Children)
            {
                if (child is RadioButton rdo && rdo.Content != null && rdo.Content.ToString().IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    rdo.IsChecked = true;
                    FilterDanhMuc_Checked(rdo, null);
                    break;
                }
            }
        }

        public void TimKiem(string tuKhoa)
        {
            txtTimKiemSanPham.Text = tuKhoa;
            ApDungBoLoc();
        }

        public void ChonSize(string size)
        {
            for (int i = 0; i < cboLocSize.Items.Count; i++)
            {
                if (cboLocSize.Items[i] is ComboBoxItem item && item.Content != null && item.Content.ToString().IndexOf(size, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    cboLocSize.SelectedIndex = i;
                    break;
                }
            }
            ApDungBoLoc();
        }

        public void ChonGia(string gia)
        {
            for (int i = 0; i < cboLocGia.Items.Count; i++)
            {
                if (cboLocGia.Items[i] is ComboBoxItem item && item.Content != null && item.Content.ToString().IndexOf(gia, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    cboLocGia.SelectedIndex = i;
                    break;
                }
            }
            ApDungBoLoc();
        }

        public void ChonSapXep(string sx)
        {
            for (int i = 0; i < cboSapXep.Items.Count; i++)
            {
                if (cboSapXep.Items[i] is ComboBoxItem item && item.Content != null && item.Content.ToString().IndexOf(sx, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    cboSapXep.SelectedIndex = i;
                    break;
                }
            }
            ApDungBoLoc();
        }

        public void MoChiTietSanPhamTheoTen(string tenSP)
        {
            var sp = _danhSachSanPhamGoc.FirstOrDefault(x => x.TenSP.IndexOf(tenSP, StringComparison.OrdinalIgnoreCase) >= 0);
            if (sp != null)
            {
                HienModalChiTiet(sp);
            }
        }

        public void TangSoLuongMuaModal()
        {
            BtnModalTangSL_Click(null, null);
        }

        public void ThemVaoGioTuModal()
        {
            BtnModalThemVaoGio_Click(null, null);
        }

        public void ThemVaoGioNhanh(string tuKhoaSP)
        {
            var sp = _danhSachSanPhamGoc.FirstOrDefault(x => x.TenSP.IndexOf(tuKhoaSP, StringComparison.OrdinalIgnoreCase) >= 0);
            if (sp != null)
            {
                ThemVaoGioHang(sp, 1);
            }
        }

        public void MoModalDangNhapKhach()
        {
            HienModalDangNhapKhach(true);
        }

        public void DienKhachVIP()
        {
            BtnDienKhachVIPMau_Click(null, null);
        }

        public void DangNhapKhachHang()
        {
            BtnThucHienDangNhapKhach_Click(null, null);
        }

        public void TangGiamSoLuongGioHang(bool tang)
        {
            if (_gioHang.Count > 0)
            {
                var item = _gioHang[0];
                if (tang)
                {
                    item.SoLuong++;
                    item.ThanhTien = item.DonGia * item.SoLuong;
                }
                else if (item.SoLuong > 1)
                {
                    item.SoLuong--;
                    item.ThanhTien = item.DonGia * item.SoLuong;
                }
                dgvGioHangKhach.Items.Refresh();
                CapNhatBadgeGioHang();
                CapNhatTongTienGioHang();
            }
        }

        public void ChonPTTT(int pttt)
        {
            if (pttt == 0)
            {
                radCOD.IsChecked = true;
                pnlThongTinCK.Visibility = Visibility.Collapsed;
            }
            else if (pttt == 1)
            {
                radChuyenKhoan.IsChecked = true;
                pnlThongTinCK.Visibility = Visibility.Visible;
            }
            else
            {
                radThe.IsChecked = true;
                pnlThongTinCK.Visibility = Visibility.Collapsed;
            }
        }

        public void ThucHienDatHangDemo()
        {
            BtnXacNhanDatHang_Click(null, null);
        }

        public void ChonDonHangMoiNhat()
        {
            if (dgvLichSuDonKhach.Items.Count > 0)
            {
                dgvLichSuDonKhach.SelectedIndex = 0;
            }
        }

        #endregion
}
}
