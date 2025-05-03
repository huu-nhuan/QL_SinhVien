using System.ComponentModel.DataAnnotations;

namespace WebQuanLySinhVien.Models.ViewModels
{
    public class XinVangHocViewModel
    {
        public string? EmailGiangVien { get; set; }
        [Required]
        public required string MSSV { get; set; }

        [Required]
        public required string HoTen { get; set; }

        [Required]
        public required string Lop { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayNghi { get; set; }

        [Required]
        public int TietBatDau { get; set; }

        [Required]
        public int TietKetThuc { get; set; }

        [Required]
        public required string LyDo { get; set; }
    }
}
