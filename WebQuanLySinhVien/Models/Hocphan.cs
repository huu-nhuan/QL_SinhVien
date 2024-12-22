using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class Hocphan
{
    public string MaHp { get; set; } = null!;

    public string? TenHp { get; set; }

    public int SoTc { get; set; }

    public string? MaNganh { get; set; }

    public string? HocKy { get; set; }

    public virtual ICollection<Diemhp> Diemhps { get; set; } = new List<Diemhp>();

    public virtual Nganh? MaNganhNavigation { get; set; }
}
