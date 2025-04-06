using System;
using System.Collections.Generic;

namespace WebQuanLySinhVien.Models;

public partial class HoSoHocTap
{
    public int Id { get; set; }

    public string MaSv { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public DateOnly? NgayNhapHoc { get; set; }

    public DateOnly NgayTotNghiep { get; set; }

    public decimal DiemTichLuy { get; set; }

    public int SoTctichLuy { get; set; }

    public string? GhiChu { get; set; }

    public DateOnly NgayCapNhat { get; set; }
}
