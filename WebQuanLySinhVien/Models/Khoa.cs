using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class Khoa
{
    public string MaKhoa { get; set; } = null!;

    public string TenKhoa { get; set; } = null!;

    public virtual ICollection<Nganh> Nganhs { get; set; } = new List<Nganh>();
}
