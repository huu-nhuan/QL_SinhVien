using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class Hocphan
{
    [Display(Name = "Mã học phần")]
    public string MaHp { get; set; } = null!;
    [Display(Name = "Tên học phần")]
    public string? TenHp { get; set; }
    [Display(Name = "Số tín chỉ")]
    public int SoTc { get; set; }
    [Display(Name = "Mã ngành")]
    public string? MaNganh { get; set; }
    [Display(Name = "Học kỳ")]
    public string? HocKy { get; set; }

    public virtual ICollection<Diemhp> Diemhps { get; set; } = new List<Diemhp>();
    [Display(Name = "Mã ngành")]
    public virtual Nganh? MaNganhNavigation { get; set; }
}
