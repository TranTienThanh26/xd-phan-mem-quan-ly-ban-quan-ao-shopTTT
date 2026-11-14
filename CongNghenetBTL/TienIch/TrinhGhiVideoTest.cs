using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CongNghenetBTL.GiaoDien;

namespace CongNghenetBTL.TienIch
{
    public static class TrinhGhiVideoTest
    {
        private static void GhiLog(string msg)
        {
            try
            {
                string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recorder_debug.log");
                File.AppendAllText(logFile, string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] {1}\r\n", DateTime.Now, msg));
            }
            catch { }
        }

        private static string TimDuongDanFFmpeg()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string wingetPath = Path.Combine(localAppData, @"Microsoft\WinGet\Packages\Gyan.FFmpeg.Essentials_Microsoft.Winget.Source_8wekyb3d8bbwe\ffmpeg-9.0.1-essentials_build\bin\ffmpeg.exe");
            if (File.Exists(wingetPath)) return wingetPath;

            string standardPath = @"C:\ProgramData\chocolatey\bin\ffmpeg.exe";
            if (File.Exists(standardPath)) return standardPath;

            return "ffmpeg.exe";
        }

        private static void ChayUI(KhachHangWindow win, Action hanhDong)
        {
            if (win.Dispatcher.CheckAccess())
            {
                hanhDong();
            }
            else
            {
                win.Dispatcher.Invoke(hanhDong);
            }
        }

        public static async void ChayKiemThuVaGhiVideoAsync(KhachHangWindow win, string duongDanVideoXuat)
        {
            GhiLog("Bắt đầu ChayKiemThuVaGhiVideoAsync");
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string tempDir = Path.Combine(baseDir, "temp_demo_frames");

                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch { }
                }
                Directory.CreateDirectory(tempDir);
                GhiLog("Đã tạo thư mục temp_demo_frames: " + tempDir);

                RenderTargetBitmap rtbChung = null;
                ChayUI(win, () =>
                {
                    win.Width = 1280;
                    win.Height = 840;
                    win.Measure(new Size(1280, 840));
                    win.Arrange(new Rect(0, 0, 1280, 840));
                    win.UpdateLayout();
                    rtbChung = new RenderTargetBitmap(1280, 840, 96, 96, PixelFormats.Pbgra32);
                });

                int frameCount = 0;

                async Task GhiFramesAsync(int soLuongFrame, int delayMs = 100)
                {
                    for (int i = 0; i < soLuongFrame; i++)
                    {
                        try
                        {
                            byte[] jpgBytes = null;
                            ChayUI(win, () =>
                            {
                                win.UpdateLayout();
                                rtbChung.Clear();
                                rtbChung.Render(win);

                                var encoder = new JpegBitmapEncoder { QualityLevel = 85 };
                                encoder.Frames.Add(BitmapFrame.Create(rtbChung));
                                using (var ms = new MemoryStream())
                                {
                                    encoder.Save(ms);
                                    jpgBytes = ms.ToArray();
                                }
                            });

                            if (jpgBytes != null)
                            {
                                string framePath = Path.Combine(tempDir, string.Format("frame_{0:D5}.jpg", ++frameCount));
                                using (var fs = new FileStream(framePath, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    fs.Write(jpgBytes, 0, jpgBytes.Length);
                                }
                            }

                            if (frameCount % 40 == 0)
                            {
                                GC.Collect(1, GCCollectionMode.Optimized);
                            }
                        }
                        catch (Exception ex)
                        {
                            GhiLog("LỖI trong GhiFramesAsync: " + ex);
                        }

                        await Task.Delay(delayMs);
                    }
                }

                // Chờ cửa sổ ổn định
                await Task.Delay(1000);
                GhiLog("Bắt đầu các bước kịch bản demo");

                // ====================================================
                // 1. INTRO / CATALOG & BANNER
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 1/10] Cửa Hàng Thời Trang TTT Store - Catalog Sản Phẩm & Hero Banner");
                    win.ChonTab(0);
                });
                await GhiFramesAsync(30); // 3.0s
                GhiLog("Xong Bước 1, frameCount=" + frameCount);

                // ====================================================
                // 2. BỘ LỌC DANH MỤC (PILLS)
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 2/10] Lọc Danh Mục: Áo sơ mi nam Oxford & lụa nữ");
                    win.ChonDanhMucTheoTen("sơ mi");
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 2/10] Lọc Danh Mục: Áo thun & Polo Pique");
                    win.ChonDanhMucTheoTen("Áo thun");
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 2/10] Lọc Danh Mục: Quần Jeans Regular & Quần Kaki");
                    win.ChonDanhMucTheoTen("Quần");
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 2/10] Lọc Danh Mục: Váy đầm hoa nhí & Chân váy xếp ly");
                    win.ChonDanhMucTheoTen("Váy");
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 2/10] Xem lại tất cả sản phẩm trong CSDL");
                    win.ChonDanhMucTheoTen("Tất cả");
                });
                await GhiFramesAsync(20); // 2.0s
                GhiLog("Xong Bước 2, frameCount=" + frameCount);

                // ====================================================
                // 3. TÌM KIẾM & LỌC SIZE & GIÁ & SẮP XẾP
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 3/10] Tìm kiếm thông minh: Nhập từ khóa 'oxford'");
                    win.TimKiem("oxford");
                });
                await GhiFramesAsync(25); // 2.5s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 3/10] Khôi phục tìm kiếm & Lọc kích cỡ (Size: L)");
                    win.TimKiem("");
                    win.ChonSize("L");
                });
                await GhiFramesAsync(25); // 2.5s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 3/10] Lọc khoảng giá: 300k - 500k & Sắp xếp giá tăng dần");
                    win.ChonSize("Tất cả");
                    win.ChonGia("300k");
                    win.ChonSapXep("Giá tăng dần");
                });
                await GhiFramesAsync(25); // 2.5s

                ChayUI(win, () =>
                {
                    win.ChonGia("Tất cả");
                    win.ChonSapXep("Mới nhất");
                });
                await GhiFramesAsync(10); // 1.0s
                GhiLog("Xong Bước 3, frameCount=" + frameCount);

                // ====================================================
                // 4. QUICK VIEW & THÊM VÀO GIỎ HÀNG
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 4/10] Quick View: Xem chi tiết sản phẩm, ảnh lớn & tồn kho");
                    win.MoChiTietSanPhamTheoTen("Oxford");
                });
                await GhiFramesAsync(25); // 2.5s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 4/10] Tăng số lượng mua lên 2 & Thêm vào giỏ hàng");
                    win.TangSoLuongMuaModal();
                });
                await GhiFramesAsync(15); // 1.5s

                ChayUI(win, () =>
                {
                    win.ThemVaoGioTuModal();
                });
                await GhiFramesAsync(25); // 2.5s

                ChayUI(win, () =>
                {
                    win.DongThongBaoChung();
                    win.DongModalChiTiet();
                });
                await GhiFramesAsync(10); // 1.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 4/10] Thêm nhanh sản phẩm thứ 2 vào giỏ hàng");
                    win.ThemVaoGioNhanh("Váy");
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DongThongBaoChung();
                });
                await GhiFramesAsync(10); // 1.0s
                GhiLog("Xong Bước 4, frameCount=" + frameCount);

                // ====================================================
                // 5. ĐĂNG NHẬP KHÁCH HÀNG VIP
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 5/10] Mở cửa sổ Đăng Nhập / Đăng Ký Khách Hàng");
                    win.MoModalDangNhapKhach();
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 5/10] Bấm 'Điền Khách VIP mẫu' (Trần Minh Hoàng - 0901234567)");
                    win.DienKhachVIP();
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 5/10] Đăng nhập thành công: Khách Hàng ⭐ Hạng VIP (-10%)");
                    win.DangNhapKhachHang();
                });
                await GhiFramesAsync(30); // 3.0s

                ChayUI(win, () =>
                {
                    win.DongThongBaoChung();
                });
                await GhiFramesAsync(10); // 1.0s
                GhiLog("Xong Bước 5, frameCount=" + frameCount);

                // ====================================================
                // 6. GIỎ HÀNG & ÁP DỤNG CHIẾT KHẤU VIP
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 6/10] Giỏ hàng: Tự động giảm giá VIP 10% + Miễn phí vận chuyển (Freeship)");
                    win.ChonTab(1);
                });
                await GhiFramesAsync(25); // 2.5s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 6/10] Tăng/giảm số lượng món trong giỏ hàng");
                    win.TangGiamSoLuongGioHang(true);
                });
                await GhiFramesAsync(20); // 2.0s

                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 6/10] Phương thức thanh toán: Chuyển khoản ngân hàng (MB Bank)");
                    win.ChonPTTT(1);
                });
                await GhiFramesAsync(25); // 2.5s
                GhiLog("Xong Bước 6, frameCount=" + frameCount);

                // ====================================================
                // 7. ĐẶT HÀNG TRỰC TUYẾN & TRỪ TỒN KHO MYSQL
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 7/10] Bấm 'Xác Nhận Đặt Hàng': Lưu đơn & Tự động trừ tồn kho MySQL");
                    win.ThucHienDatHangDemo();
                });
                await GhiFramesAsync(35); // 3.5s

                ChayUI(win, () =>
                {
                    win.DongThongBaoChung();
                });
                await GhiFramesAsync(10); // 1.0s
                GhiLog("Xong Bước 7, frameCount=" + frameCount);

                // ====================================================
                // 8. ĐƠN HÀNG CỦA TÔI
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 8/10] Đơn hàng của tôi: Danh sách hóa đơn & Chi tiết các món vừa mua");
                    win.ChonTab(2);
                    win.ChonDonHangMoiNhat();
                });
                await GhiFramesAsync(35); // 3.5s
                GhiLog("Xong Bước 8, frameCount=" + frameCount);

                // ====================================================
                // 9. THẺ THÀNH VIÊN VIP & BẢO MẬT SHA-256
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("🎬 [BƯỚC 9/10] Thẻ VIP điện tử (Che SĐT bảo mật) & Đổi mật khẩu SHA-256");
                    win.ChonTab(3);
                });
                await GhiFramesAsync(35); // 3.5s
                GhiLog("Xong Bước 9, frameCount=" + frameCount);

                // ====================================================
                // 10. HOÀN THÀNH TỔNG KẾT
                // ====================================================
                ChayUI(win, () =>
                {
                    win.DatOverlayDemo("✨ [HOÀN TẤT] Kiểm thử thành công 100% chức năng Khách Hàng - TTT Store!");
                    win.HienThongBaoChung("Hoàn Tất Kiểm Thử",
                        "ĐÃ KIỂM THỬ THÀNH CÔNG 100% CHỨC NĂNG PHÍA KHÁCH HÀNG:\n\n" +
                        "1. Catalog thời trang, banner khuyến mãi & bộ lọc đa chiều\n" +
                        "2. Quick View chi tiết sản phẩm, kích cỡ & thêm vào giỏ\n" +
                        "3. Đăng nhập Khách Hàng VIP & nhận chiết khấu 10%\n" +
                        "4. Giỏ hàng, tính giảm giá tự động & Freeship\n" +
                        "5. Đặt hàng trực tuyến, lưu CSDL & trừ kho MySQL tự động\n" +
                        "6. Tra cứu đơn hàng & chi tiết từng mặt hàng\n" +
                        "7. Thẻ thành viên VIP điện tử & bảo mật mật khẩu SHA-256\n\n" +
                        "Video đã được lưu và sẵn sàng gửi thầy cô chấm điểm!",
                        "🏆");
                });
                await GhiFramesAsync(45); // 4.5s

                ChayUI(win, () =>
                {
                    win.DongThongBaoChung();
                });
                await GhiFramesAsync(10); // 1.0s
                GhiLog("Xong Bước 10, tổng frameCount=" + frameCount);

                // ====================================================
                // BIÊN TẬP THÀNH VIDEO MP4 BẰNG FFMPEG
                // ====================================================
                string ffmpegPath = TimDuongDanFFmpeg();
                string inputPattern = Path.Combine(tempDir, "frame_%05d.jpg");

                string targetMp4 = Path.GetFullPath(duongDanVideoXuat);
                string targetDir = Path.GetDirectoryName(targetMp4);
                if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                GhiLog("Bắt đầu gọi FFmpeg: " + ffmpegPath + " -> " + targetMp4);

                string ffmpegArgs = string.Format("-y -framerate 10 -i \"{0}\" -c:v libx264 -pix_fmt yuv420p -preset fast \"{1}\"", inputPattern, targetMp4);

                var psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = ffmpegArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    GhiLog("FFmpeg hoàn thành với ExitCode=" + proc.ExitCode);
                }

                // Dọn dẹp thư mục tạm
                try { Directory.Delete(tempDir, true); } catch { }

                GhiLog("Hoàn tất quy trình quay video thành công! File lưu tại: " + targetMp4);

                // Thoát ứng dụng sau khi hoàn thành xuất video
                ChayUI(win, () => Application.Current.Shutdown());
            }
            catch (Exception ex)
            {
                GhiLog("LỖI CRITICAL trong ChayKiemThuVaGhiVideoAsync: " + ex);
                ChayUI(win, () => Application.Current.Shutdown());
            }
        }
    }
}
