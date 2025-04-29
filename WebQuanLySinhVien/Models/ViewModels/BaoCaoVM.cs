namespace WebQuanLySinhVien.Models.ViewModels
{
    public class BaoCaoVM
    {
        public required SinhVien sinhvien { get; set; }
        public required HoSoHocTap hoso { get; set; }
        public required List<Diemhp> DSdiem { get; set; }
        public required string tenlop { get; set; }
        public required string tenGV { get; set; }
        public required string makhoa { get;  set; }
    }
}
