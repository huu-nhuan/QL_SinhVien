using X.PagedList;

namespace WebQuanLySinhVien.Models.ViewModels
{
    public class AdminTableList
    {
        public IPagedList<Taikhoan>? DanhSachTaiKhoan { get; set; }
        public IPagedList<Khoa>? DanhSachKhoa { get; set; }
        public IPagedList<Nganh>? DanhSachNganh { get; set; }
        public IPagedList<Hocphan>? DanhSachHocPhan { get; set; }
        public IPagedList<GiangVien>? DanhSachGiangVien { get; set; }
        public IPagedList<Lop>? DanhSachLop { get; set; }
        public IPagedList<SinhVien>? DanhSachSinhVien { get; set; }
        public IPagedList<Diemhp>? DanhSachDiemHocPhan{ get; set; }
    }
}
