using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class Nganh
{
    public string MaNganh { get; set; } = null!;

    public string? TenNganh { get; set; }

    public string MaKhoa { get; set; } = null!;

    public virtual ICollection<Hocphan> Hocphans { get; set; } = new List<Hocphan>();

    public virtual Khoa MaKhoaNavigation { get; set; } = null!;
}
