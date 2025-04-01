using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class Diemhp
{
    public string MaSv { get; set; } = null!;

    public string MaHp { get; set; } = null!;

    public int? DiemHp { get; set; }

    public virtual Hocphan? MaHpNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
