using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class Khoa
{
    [Display(Name = "Mã khoa")]
    public string MaKhoa { get; set; } = null!;
    [Display(Name = "Ten khoa")]
    public string TenKhoa { get; set; } = null!;

    public virtual ICollection<Nganh> Nganhs { get; set; } = new List<Nganh>();
}
