using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class Taikhoan
{
    public int IdTk { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int VaiTro { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<GiangVien> GiangViens { get; set; } = new List<GiangVien>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
