using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class Nganh
{
    [Display(Name = "Mã ngành")]
    public string MaNganh { get; set; } = null!;
    [Display(Name = "Tên ngành")]
    public string? TenNganh { get; set; }
    [Display(Name = "Mã khoa")]
    public string MaKhoa { get; set; } = null!;

    public virtual ICollection<Hocphan> Hocphans { get; set; } = new List<Hocphan>();
    [Display(Name = "Mã khoa")]
    public virtual Khoa? MaKhoaNavigation { get; set; }
}
