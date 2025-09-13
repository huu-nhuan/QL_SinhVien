using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class SinhVien
{
    [Display(Name = "Mã sinh viên")]
    [Required(ErrorMessage = "Mã sinh viên bắt buộc nhập")]
    [StringLength(10, ErrorMessage = "Mã sinh viên tối đa 10 ký tự")]
    public string MaSv { get; set; } = null!;

    [Display(Name = "Mã lớp")]
    [Required(ErrorMessage = "Mã lớp bắt buộc nhập")]
    [StringLength(10, ErrorMessage = "Mã lớp tối đa 10 ký tự")]
    public string MaLop { get; set; } = null!;

    [Display(Name = "Họ và tên")]
    [StringLength(50, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string? HoTen { get; set; }

    [Display(Name = "Giới tính")]
    [Required(ErrorMessage = "Giới tính bắt buộc chọn")]
    public string GioiTinh { get; set; } = null!;

    [Display(Name = "Ngày sinh")]
    public DateOnly? NgaySinh { get; set; }

    [Display(Name = "Số điện thoại")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số.")]
    [Required(ErrorMessage = "SĐT bắt buộc nhập")]
    public string Sdt { get; set; } = null!;

    [Display(Name = "Địa chỉ")]
    [StringLength(50)]
    public string? DiaChi { get; set; }

    [Display(Name = "ID tài khoản")]
    public int? IdTk { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email bắt buộc nhập")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Display(Name = "ID Hồ sơ")]
    public int? HoSo { get; set; }

    public virtual ICollection<Diemhp> Diemhps { get; set; } = new List<Diemhp>();

    public virtual HoSoHocTap? HoSoNavigation { get; set; }
    [Display(Name = "ID tài khoản")]
    public virtual Taikhoan? IdTkNavigation { get; set; }
    [Display(Name = "Mã lớp")]
    public virtual Lop? MaLopNavigation { get; set; }
}
