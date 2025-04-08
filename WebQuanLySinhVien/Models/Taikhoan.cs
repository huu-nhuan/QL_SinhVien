using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class Taikhoan
{
    [Display(Name = "ID Tài khoản")]
    public int IdTk { get; set; }

    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = null!;

    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = null!;

    [Display(Name = "Vai trò")]
    public int VaiTro { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<GiangVien> GiangViens { get; set; } = new List<GiangVien>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
