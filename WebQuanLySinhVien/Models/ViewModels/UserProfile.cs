namespace WebQuanLySinhVien.Models.ViewModels
{
    public class UserProfile
    {
        public List<UserInfo>? DanhSachUser { get; set; }
        public IList<string>? DanhSachID { get; set; }
        public string? SelectedID { get; set; }
        public string? HoTen { get; set; }
        public string GioiTinh { get; set; } = null!;
        public DateOnly? NgaySinh { get; set; }
        public string Sdt { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string Email { get; set; } = null!;
        public string? ImagePath { get; set; }
    }

    public class UserInfo
    {
        public string ID { get; set; }
        public string Ten { get; set; }
    }
}
