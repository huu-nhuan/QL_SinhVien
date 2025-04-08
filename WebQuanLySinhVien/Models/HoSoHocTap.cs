using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models;

public partial class HoSoHocTap
{
    public int Id { get; set; }

    [Display(Name = "Mã sinh viên")]
    public string MaSv { get; set; } = null!;

    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = null!;

    [Display(Name = "Ngày nhập học")]
    public DateOnly? NgayNhapHoc { get; set; }

    [Display(Name = "Ngày tốt nghiệp")]
    public DateOnly NgayTotNghiep { get; set; }

    [Display(Name = "Điểm tích lũy")]
    public decimal DiemTichLuy { get; set; }

    [Display(Name = "Số tín chỉ tích lũy")]
    public int SoTctichLuy { get; set; }

    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public DateOnly NgayCapNhat { get; set; }

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
