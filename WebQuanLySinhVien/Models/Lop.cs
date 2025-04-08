using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class Lop
{
    public string MaLop { get; set; } = null!;

    public string? TenLop { get; set; }

    public string MaGv { get; set; } = null!;
    [Display(Name = "Năm nhập học")]
    [RegularExpression(@"^(19|20)\d{2}$", ErrorMessage = "Năm phải có dạng YYYY (1900-2099)")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "Năm phải đủ 4 chữ số")]
    public string? NamNhapHoc { get; set; }

    public virtual GiangVien? MaGvNavigation { get; set; }

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
