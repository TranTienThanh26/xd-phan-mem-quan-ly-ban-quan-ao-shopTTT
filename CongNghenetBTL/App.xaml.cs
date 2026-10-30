using System;
using System.IO;
using System.Windows;
using CongNghenetBTL.GiaoDien;

namespace CongNghenetBTL
{
    public partial class App : Application
    {
        private static void GhiLog(string noiDung)
        {
            try
            {
                string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_debug.log");
                File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {noiDung}\r\n");
            }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GhiLog("=== Ứng dụng khởi động ===");

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                GhiLog($"CRITICAL AppDomain UnhandledException: {args.ExceptionObject}");
            };

            this.DispatcherUnhandledException += (sender, args) =>
            {
                GhiLog($"DispatcherUnhandledException: {args.Exception}");
                MessageBox.Show(
                    "Đã xảy ra lỗi không mong muốn:\n" + args.Exception.Message, 
                    "Lỗi Hệ Thống", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                args.Handled = true;
            };

            try
            {
                if (e.Args.Length > 0 && e.Args[0] == "--trangchu")
                {
                    GhiLog("Khởi động chế độ --trangchu");
                    var bll = new NghiepVu.NguoiDungBLL();
                    var admin = bll.DangNhap("admin", "123456", out string tb);
                    GhiLog($"Đăng nhập admin: {(admin != null ? admin.HoTen : "Thất bại: " + tb)}");
                    TienIch.PhienDangNhap.DangNhap(admin);

                    var trangChu = new TrangChuWindow();
                    this.MainWindow = trangChu;
                    trangChu.Show();
                    trangChu.Activate();
                    GhiLog("TrangChuWindow đã Show thành công!");
                }
                else if (e.Args.Length > 0 && e.Args[0] == "--khachhang")
                {
                    GhiLog("Khởi động chế độ --khachhang");
                    var cuaSoKhach = new KhachHangWindow();
                    this.MainWindow = cuaSoKhach;
                    cuaSoKhach.Show();
                    cuaSoKhach.Activate();
                    GhiLog("KhachHangWindow đã Show thành công!");
                }
                else
                {
                    GhiLog("Khởi động màn hình Đăng Nhập");
                    var cuaSoDangNhap = new DangNhapWindow();
                    this.MainWindow = cuaSoDangNhap;
                    cuaSoDangNhap.Show();
                    cuaSoDangNhap.Activate();
                    GhiLog("DangNhapWindow đã Show thành công!");
                }
            }
            catch (Exception ex)
            {
                GhiLog($"Lỗi Khởi Động: {ex}");
                MessageBox.Show(
                    "Không thể khởi động cửa sổ:\n" + ex.Message + "\n\nChi tiết:\n" + ex.StackTrace, 
                    "Lỗi Khởi Động", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
