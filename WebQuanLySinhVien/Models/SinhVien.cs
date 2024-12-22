using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class SinhVien
{
    public string MaSv { get; set; } = null!;

    public string MaLop { get; set; } = null!;

    public string? HoTen { get; set; }

    public string GioiTinh { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string Sdt { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? IdTk { get; set; }

    public string Email { get; set; } = null!;

    public virtual ICollection<Diemhp> Diemhps { get; set; } = new List<Diemhp>();

    public virtual Taikhoan? IdTkNavigation { get; set; }

    public virtual Lop MaLopNavigation { get; set; } = null!;
}
