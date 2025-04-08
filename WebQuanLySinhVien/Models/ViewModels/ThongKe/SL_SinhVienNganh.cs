namespace WebQuanLySinhVien.Models.ViewModels.ThongKe
{
    public class SL_SinhVienNganh
    {
        public IList<string>? DanhSachIDNganh { get; set; }
        public IList<string>? DanhSachSVTheoNganh { get; set; }
        public string? SelectedID { get; set; }
        public string? SelectedSVID { get; set; }
        public string? IdNganh { get; set; }
        public decimal[] DanhSachDiem { get; set; }
        public IList<string>? DanhSachIdHocPhan { get; set; }
        public List<SoLuongSinhVien> DanhSachSoLuongSinhVien { get; set; }
    }
    public class SoLuongSinhVien
    {
        public string MaHP { get; set; }
        public int SoLuong { get; set; }
        public int[] DanhSachSoLuongTheoDiem { get; set; }
        public int SoluongTrenTB {  get; set; }
        public int SoluongDuoiTB { get; set; }
    }
}
