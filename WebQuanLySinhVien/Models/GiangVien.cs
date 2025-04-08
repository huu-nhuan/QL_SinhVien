using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class GiangVien
{
    [Display(Name = "Mã giảng viên")]
    [Required(ErrorMessage = "Mã giảng viên không được để trống")]
    public string MaGv { get; set; } = null!;
    [Display(Name = "Họ và tên", Prompt = "Nguyễn Văn A")]
    public string? HoTen { get; set; }
    [Display(Name = "Giới tính")]
    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    public string GioiTinh { get; set; } = null!;
    [Display(Name = "Ngày sinh")]
    public DateOnly? NgaySinh { get; set; }
    [Display(Name = "Số điện thoại")]
    public string? Sdt { get; set; }
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
    [Display(Name = "Tài khoản hệ thống")]
    public int? IdTk { get; set; }
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
    [Display(Name = "Tài khoản hệ thống")]
    public virtual Taikhoan? IdTkNavigation { get; set; }

    public virtual ICollection<Lop> Lops { get; set; } = new List<Lop>();
}
