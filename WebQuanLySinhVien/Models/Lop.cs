using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class Lop
{
    public string MaLop { get; set; } = null!;

    public string? TenLop { get; set; }

    public string MaGv { get; set; } = null!;

    public string? NamNhapHoc { get; set; }

    public virtual GiangVien? MaGvNavigation { get; set; }

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
