# BÀI TẬP LỚN MÔN CÔNG NGHỆ .NET
## ĐỀ TÀI: PHẦN MỀM QUẢN LÝ BÁN QUẦN ÁO THỜI TRANG (TTT STORE)
### HỆ THỐNG FULL-STACK VỚI 2 TÁC NHÂN CHÍNH: PHÂN HỆ QUẢN TRỊ (ADMIN / POS) & CỬA HÀNG MUA SẮM TRỰC TUYẾN (KHÁCH HÀNG)

---

### 1. Giới thiệu tổng quan
- **Tên phần mềm:** TTT STORE - Quản Lý Bán Quần Áo & Cửa Hàng Thời Trang Trực Tuyến
- **Hai tác nhân chính của hệ thống:**
  1.  **Quản Trị Viên (Admin / Chủ Cửa Hàng):** Quản lý toàn bộ danh mục, quần áo, khách hàng, hóa đơn, báo cáo doanh thu và bán hàng trực tiếp tại quầy (POS).
  2.  **Khách Hàng (Customer):** Xem catalog thời trang, lọc size/màu/giá, giỏ hàng, đặt hàng trực tuyến với trừ kho tự động, tra cứu lịch sử đơn hàng, thẻ thành viên tích điểm và bảo mật SHA-256.
- **Nền tảng:** C# .NET Framework 4.7.2, Windows Presentation Foundation (WPF XAML)
- **Cơ sở dữ liệu:** MySQL Server 8.x / 9.x (`QLBanQuanAoDB`) kết nối trực tiếp qua **ADO.NET thuần** (`MySqlConnector`: `MySqlConnection`, `MySqlCommand`, `MySqlTransaction`, `MySqlDataAdapter`).
- **Bảo mật & Mã hóa:** Mã hóa một chiều mật khẩu bằng thuật toán **SHA-256** an toàn tuyệt đối; che số điện thoại bảo mật thông tin cá nhân.
- **Kiến trúc:** 3 tầng (3-Tier Architecture) chuẩn mực với toàn bộ tên file, thư mục và lớp mã nguồn bằng **Tiếng Việt** dễ đọc, dễ hiểu, logic và chuẩn mực sư phạm.

---

### 2. Cấu trúc thư mục dự án chuẩn 3 lớp (3-Tier Architecture)

```
CongNghenetBTL/
├── CongNghenetBTL.sln                   # File Solution chuẩn mở bằng Visual Studio
├── CongNghenetBTL.slnx                  # File Solution thế hệ mới (VS 2022/2026)
│
└── CongNghenetBTL/                      # Thư mục Project chính
    ├── App.config                       # Cấu hình chuỗi kết nối MySQL (QLBanQuanAoDB)
    ├── App.xaml / App.xaml.cs           # Khởi động ứng dụng, tham số --trangchu / --khachhang
    ├── CongNghenetBTL.csproj            # Cấu hình dự án WPF .NET Framework 4.7.2
    ├── README_BTL.md                    # Tài liệu giải thích kiến trúc & hướng dẫn chấm điểm
    │
    ├── Database/                        # KỊCH BẢN CƠ SỞ DỮ LIỆU
    │   └── TaoCSDL_MySQL.sql            # Script MySQL chuẩn (Bảng, khóa ngoại, bảo mật, dữ liệu mẫu)
    │
    ├── GiaoDien/                        # TẦNG GIAO DIỆN (Presentation Layer - WPF XAML)
    │   ├── DinhDangChung.xaml           # Bảng màu thời trang, drop shadow, style nút, ô nhập, DataGrid
    │   ├── DangNhapWindow.xaml(.cs)     # Màn hình Đăng nhập Quản trị viên (Admin) & Cửa hàng Khách
    │   ├── TrangChuWindow.xaml(.cs)     # Màn hình Quản trị Admin (Dashboard, POS, Quần áo, Danh mục, Khách hàng, Hóa đơn, Báo cáo)
    │   └── KhachHangWindow.xaml(.cs)    # Màn hình Mua sắm Khách hàng (Catalog, Giỏ hàng, Đơn hàng, Thẻ VIP)
    │
    ├── HinhAnh/                         # THƯ MỤC HÌNH ẢNH SẢN PHẨM & BANNER THỜI TRANG THẬT
    │   ├── banner_thoi_trang.jpg        # Banner lớn siêu đẹp cho trang chủ Khách Hàng
    │   ├── ao_so_mi_oxford.jpg          # Áo sơ mi nam Oxford
    │   ├── ao_so_mi_nu.jpg              # Áo sơ mi lụa nữ Hàn Quốc
    │   ├── ao_polo_pique.jpg            # Áo thun Polo Pique
    │   ├── ao_thun_unisex.jpg           # Áo thun form rộng Unisex
    │   ├── quan_jeans_regular.jpg       # Quần jeans regular fit
    │   ├── quan_kaki_nam.jpg            # Quần kaki công sở
    │   ├── vay_dam_hoa_nhi.jpg          # Váy đầm hoa nhí dáng xòe
    │   ├── chan_vay_xep_ly.jpg          # Chân váy chữ A xếp ly
    │   ├── ao_khoac_bomber.jpg          # Áo khoác bomber kaki
    │   ├── ao_hoodie_ni_bong.jpg        # Áo hoodie nỉ bông trơn
    │   └── mac_dinh.jpg                 # Ảnh đại diện mặc định
    │
    ├── MoHinh/                          # TẦNG THỰC THỂ DỮ LIỆU (Models / Entity Layer)
    │   ├── KhachHang.cs                 # Khách hàng (SĐT bảo mật, Mật khẩu SHA-256, Hạng VIP, Điểm)
    │   ├── SanPham.cs                   # Quần áo thời trang (size, màu sắc, vải, giá bán, ảnh thật)
    │   ├── DanhMuc.cs                   # Danh mục thời trang
    │   ├── HoaDon.cs                    # Hóa đơn thanh toán
    │   ├── ChiTietHoaDon.cs             # Chi tiết từng mặt hàng trong đơn
    │   └── NguoiDung.cs                 # Tài khoản Admin / Nhân viên
    │
    ├── TruyCapDuLieu/                   # TẦNG TRUY XUẤT CSDL (Data Access Layer - DAL)
    │   ├── KetNoiCSDL.cs                # Quản lý kết nối MySqlConnection, MySqlCommand, Transaction
    │   ├── KhachHangDAL.cs              # CRUD khách hàng, đăng nhập/đổi mật khẩu, tra cứu SĐT, tích điểm
    │   ├── SanPhamDAL.cs                # CRUD sản phẩm, lọc theo danh mục, size, màu, khoảng giá
    │   ├── DanhMucDAL.cs                # CRUD danh mục thời trang
    │   ├── HoaDonDAL.cs                 # Lưu đơn hàng (Transaction trừ tồn kho), lịch sử đơn khách
    │   ├── NguoiDungDAL.cs              # Đăng nhập nhân viên, băm mật khẩu, đổi trạng thái
    │   └── ThongKeDAL.cs                # Doanh thu ngày/tháng, top bán chạy, cảnh báo kho
    │
    ├── NghiepVu/                        # TẦNG XỬ LÝ NGHIỆP VỤ (Business Logic Layer - BLL)
    │   ├── KhachHangBLL.cs              # Xác thực đăng nhập khách, đăng ký, đổi MK, bảo mật SHA-256
    │   ├── SanPhamBLL.cs                # Kiểm tra giá, tồn kho, kiểm tra logic kinh doanh
    │   ├── DanhMucBLL.cs                # Kiểm tra ràng buộc danh mục
    │   ├── HoaDonBLL.cs                 # Nghiệp vụ đặt hàng online / POS, thanh toán, hủy đơn hoàn tồn
    │   └── NguoiDungBLL.cs              # Xác thực đăng nhập nhân viên, phân quyền
    │
    └── TienIch/                         # TẦNG TIỆN ÍCH DÙNG CHUNG (Utilities)
        ├── PhienDangNhap.cs             # Quản lý phiên làm việc của Admin/NV và Khách Hàng
        └── MaHoaMatKhau.cs              # Băm mật khẩu một chiều an toàn bằng SHA-256
```

---

### 3. Chi tiết đầy đủ tính năng BÊN KHÁCH HÀNG (KhachHangWindow.xaml)

1. **Trang Chủ Cửa Hàng / Danh Mục Sản Phẩm (Catalog):**
   - **Hero Banner Khuyến Mãi:** Ảnh banner thời trang cao cấp (`banner_thoi_trang.jpg`) kết hợp hiệu ứng gradient mờ sang trọng, thông điệp ưu đãi độc quyền.
   - **Thanh Chọn Danh Mục Dạng Chip (Pills):** Tất cả sản phẩm, Áo sơ mi, Áo thun & Polo, Quần Jeans & Kaki, Váy & Đầm, Áo khoác & Hoodie, Phụ kiện...
   - **Bộ Lọc Đa Chiều & Tìm Kiếm Thông Minh:**
     * Tìm theo từ khóa (tên sản phẩm, màu sắc, size, mã SP).
     * Lọc theo Kích cỡ (S, M, L, XL, XXL, Freesize).
     * Lọc theo Khoảng giá (<300k, 300k - 500k, >500k).
     * Sắp xếp (Mới nhất, Giá tăng dần, Giá giảm dần, Tên A - Z).
   - **Card View Sản Phẩm Hiện Đại:**
     * Ảnh thật sản phẩm sắc nét, bo góc mượt mà, bóng đổ card cao cấp.
     * Tên sản phẩm, tag danh mục, kích cỡ và màu sắc có sẵn.
     * Giá niêm yết định dạng tiền tệ Việt Nam (`350.000 đ`).
     * Trạng thái tồn kho thực tế từ CSDL MySQL (xanh: còn hàng, cam: sắp hết, đỏ: hết hàng).
     * Nút "👁️ Chi tiết" và nút "🛒 Thêm Giỏ" với hiệu ứng thông báo nhanh.

2. **Modal Xem Nhanh Chi Tiết Sản Phẩm (Quick View):**
   - Phóng to ảnh sản phẩm, xem đầy đủ chất liệu vải, kích cỡ, màu sắc, mô tả chi tiết, số lượng tồn kho.
   - Bộ chọn số lượng mua `[-] [Số lượng] [+]` và nút "Thêm Vào Giỏ Hàng".

3. **Giỏ Hàng & Đặt Hàng Trực Tuyến (Cart & Checkout):**
   - Danh sách món trong giỏ: Ảnh nhỏ, Tên SP, Size, Màu, Đơn giá, tăng/giảm số lượng, Thành tiền, nút xóa món, nút xóa sạch giỏ.
   - Badge hiển thị số lượng món trên Header tự động cập nhật thời gian thực.
   - **Khung Điền Thông Tin Giao Hàng:** Họ tên người nhận, Số điện thoại, Địa chỉ nhận hàng chi tiết, Ghi chú giao hàng. (Tự động điền sẵn nếu khách đã đăng nhập).
   - **Phương Thức Thanh Toán Linh Hoạt:**
     *  Tiền mặt khi nhận hàng (COD).
     *  Chuyển khoản ngân hàng (Hiển thị ngay STK MB Bank / Techcombank và hướng dẫn).
     *  Thẻ ATM nội địa / Quét mã QR.
   - **Tự Động Tính Tiền & Giảm Giá Thành Viên:**
     * Tạm tính tiền hàng.
     * Giảm giá thành viên: **Hạng VIP tự động giảm 10%**, **Thân thiết giảm 5%**.
     * Phí giao hàng: Đơn từ 500k được **Miễn phí vận chuyển (Freeship)**.
     * Tổng thanh toán cuối cùng.
   - **Nút "Xác Nhận Đặt Hàng Ngay":**
     * Thực thi qua **MySqlTransaction**: Lưu hóa đơn vào `HoaDon`, lưu các mặt hàng vào `ChiTietHoaDon`, và **tự động trừ số lượng tồn kho** trong bảng `SanPham`!
     * Cộng điểm tích lũy cho khách hàng.
     * Tự động xóa giỏ hàng và chuyển sang xem hóa đơn vừa đặt.

4. **Đơn Hàng Của Tôi (My Orders & Tracking):**
   - Tra cứu nhanh bằng số điện thoại hoặc tự động nạp theo tài khoản khách hàng đang đăng nhập.
   - Bảng danh sách các đơn hàng đã đặt: Mã đơn `#HD...`, Ngày giờ đặt, Tổng thanh toán, Phương thức thanh toán.
   - Chọn một đơn hàng để xem ngay danh sách các món quần áo trong đơn (Tên, Size, Màu, Số lượng, Đơn giá, Thành tiền).

5. **Tài Khoản & Bảo Mật (Profile & Security):**
   - **Thẻ Thành Viên Điện Tử (Loyalty Card):** Thiết kế gradient thời trang hiển thị họ tên, huy hiệu hạng (` HẠNG VIP (-10%)` / ` THÂN THIẾT (-5%)`), số điểm tích lũy và số điện thoại đã được **che bảo mật** (ví dụ `090****567`).
   - Cập nhật thông tin nhận hàng (Họ tên, Email, Địa chỉ mặc định).
   - **Đổi Mật Khẩu Bảo Mật:** Nhập mật khẩu cũ, mật khẩu mới và xác nhận; mật khẩu mới được băm bằng chuẩn **SHA-256** an toàn trước khi lưu vào CSDL MySQL.

6. **Đăng Nhập & Đăng Ký Khách Hàng (Customer Auth Modal):**
   - Đăng nhập bằng Số điện thoại + Mật khẩu (bảo mật SHA-256).
   - Có nút **Điền Khách VIP mẫu** để thầy cô chấm điểm kiểm tra nhanh tính năng giảm giá VIP.
   - Khách mới có thể tự Đăng ký thành viên, nhận ngay 10 điểm thưởng chào mừng!

---

### 4. Liên kết dữ liệu 2 chiều với Bên Admin (Full-Stack Data Linkage)

Hệ thống hoạt động trên cùng một CSDL MySQL `QLBanQuanAoDB`:
1. **Khách đặt hàng ➜ Admin nhận đơn ngay:**
   - Khi khách bấm đặt hàng trên `KhachHangWindow`, đơn hàng lập tức xuất hiện trong bảng "Lịch sử hóa đơn" của Admin.
   - Doanh thu của đơn hàng cộng ngay vào Dashboard Thống kê của Admin.
2. **Trừ kho tự động tức thì (Real-time Inventory):**
   - Kho hàng của bảng `SanPham` được trừ ngay qua `MySqlTransaction`.
   - Cả Admin và Khách hàng tiếp theo đều thấy số lượng tồn kho cập nhật chính xác, không bao giờ bị bán quá tồn kho.
3. **Admin cập nhật sản phẩm / giá ➜ Khách thấy ngay:**
   - Khi Admin thêm mẫu quần áo mới, sửa giá bán hoặc đổi ảnh, khách hàng chỉ cần bấm "Làm Mới" là thấy ngay sản phẩm mới cập nhật.
4. **Khách đăng ký mới ➜ Admin có trong danh bạ:**
   - Khách hàng đăng ký trên `KhachHangWindow` lập tức xuất hiện trong phân hệ "Quản lý khách hàng" của Admin để chăm sóc khách hàng.

---

### 5. Tài khoản kiểm tra nhanh hệ thống

#### Phân hệ Quản Trị Viên (Admin):
| Tên đăng nhập | Mật khẩu | Họ và tên | Quyền hạn |
|---|---|---|---|
| `admin` | `123456` | Quản Trị Viên (Chủ Cửa Hàng) | Quản trị viên (Toàn quyền quản lý kho, sản phẩm, khách hàng, đơn hàng, POS) |

#### Phân hệ Khách Hàng Mua Sắm Trực Tuyến:
| Số điện thoại | Mật khẩu | Họ và tên | Hạng thành viên | Đặc quyền |
|---|---|---|---|---|
| `0901234567` | `123456` | Trần Minh Hoàng | **VIP** | **Giảm 10% mọi đơn hàng**, 150 điểm tích lũy |
| `0912348899` | `123456` | Nguyễn Thị Mai Linh | **Thân thiết** | **Giảm 5% mọi đơn hàng**, 50 điểm tích lũy |
| `0987654321` | `123456` | Lê Đức Thắng | Thường | Tích điểm đổi quà |

*(Trên màn hình Đăng nhập và Modal Khách hàng đều có sẵn các nút bấm tự động điền tài khoản để thầy cô chấm điểm trong 1 giây).*

---

### 6. Hướng dẫn chạy và trải nghiệm

1. **Khởi động MySQL:** Đảm bảo dịch vụ MySQL đang chạy tại cổng `3306` (XAMPP hoặc MySQL Server).
2. **Mở dự án:** Mở file `CongNghenetBTL.sln` trong Visual Studio.
3. **Chạy ứng dụng (F5):**
   - Màn hình Đăng Nhập mở ra.
   - Bấm nút **" VÀO CỬA HÀNG MUA SẮM (KHÁCH HÀNG)"** để trải nghiệm giao diện khách hàng.
   - Hoặc bấm **" ĐĂNG NHẬP QUẢN TRỊ VIÊN (ADMIN)"** để vào Trang chủ Admin.
   - Ở cả 2 giao diện đều có nút chuyển qua lại mượt mà để kiểm tra liên kết dữ liệu!
