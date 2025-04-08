using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class Diemhp
{
    [Display(Name = "Mã sinh viên")]
    [Required(ErrorMessage = "Mã sinh viên không được để trống")]
    [StringLength(10, ErrorMessage = "Mã sinh viên không quá 10 ký tự")]
    public string MaSv { get; set; } = null!;

    [Display(Name = "Mã học phần")]
    [Required(ErrorMessage = "Mã học phần không được để trống")]
    [StringLength(10, ErrorMessage = "Mã học phần không quá 10 ký tự")]
    public string MaHp { get; set; } = null!;

    [Display(Name = "Điểm học phần")]
    [Range(0, 10, ErrorMessage = "Điểm học phần phải từ 0 đến 10")]
    public decimal? DiemHp { get; set; }

    public virtual Hocphan? MaHpNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
