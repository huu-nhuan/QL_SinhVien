namespace WebQuanLySinhVien.Models.ViewModels.ThongKe
{
    public class SL_SinhVienNganh
    {
        public string? IdNganh { get; set; }
        public IList<string>? DanhSachIdHocPhan { get; set; }
        public List<SoLuongSinhVien> DanhSachSoLuongSinhVien { get; set; }
    }
    public class SoLuongSinhVien
    {
        public string MaHP { get; set; }
        public int SoLuong { get; set; }
    }
}
