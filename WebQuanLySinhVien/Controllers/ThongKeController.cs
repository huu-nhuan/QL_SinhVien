using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;
using WebQuanLySinhVien.Models.ViewModels.ThongKe;

namespace WebQuanLySinhVien.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly QuanLySinhVienContext _context;
        public ThongKeController(QuanLySinhVienContext context)
        {
            _context = context;
        }
        public IActionResult Index( string manganh, string masv)
        {
            if(manganh==null)
            {
                manganh = "KTPM";
            }
            var danhSachSV_Nganh = _context.Hocphans.Where(hp => hp.MaNganh == manganh)
                                   .Join(_context.Diemhps, hp => hp.MaHp, d => d.MaHp, (hp, d) => d.MaSv)
                                   .Distinct().ToList(); // lấy danh sách sinh viên theo ngành | .Distinct để đảm bảo không bị trùng
            var danhSachIdHocPhan = _context.Hocphans.Where(hp => hp.MaNganh==manganh).OrderBy(hp => hp.MaHp).Select(hp => hp.MaHp).ToList(); // lấy danh sách hp
            var danhSachSoLuongSinhVien = new List<SoLuongSinhVien>(); //tạo list
            var danhSachIDNganh = _context.Nganhs.OrderBy(n => n.MaNganh).Select(n => n.MaNganh).ToList(); //lấy danh sach id ngành
            int[] danhSachDiem = new int[danhSachIdHocPhan.Count]; // tạo mảng chứa danh sách điểm của sv
            int temp =0; //biến tạm lưu vị trí trong mảng danhSachDiem

            // tạo danh sách gồm mã hp và số lượng sinh viên tương ứng
            foreach (var hp in danhSachIdHocPhan )
            {
                danhSachDiem[temp] = _context.Diemhps.Where(d => d.MaSv==masv && d.MaHp==hp).Select(d => d.DiemHp).FirstOrDefault() ?? 0; temp++; //tạo mảng chứa danh sách điểm của sv
                var sl = _context.Diemhps.Count(d =>d.MaHp==hp); // đếm số lượng sv
                var tenHP = _context.Hocphans.Where(h => h.MaHp == hp).Select(h => h.TenHp).FirstOrDefault(); // lấy tên hp
                int[] soLuongSVtheoDiem = new int[11]; //tạo mảng chứa số lượng sinh viên của từng điểm từ 0 - 10
                for (int i = 0; i <= 10; i++)
                {
                    soLuongSVtheoDiem[i] = _context.Diemhps.Where(d => d.MaHp == hp).Count(d => d.DiemHp == i);
                }; 
                var trentb = _context.Diemhps.Where(h => h.MaHp == hp).Count(h => h.DiemHp >= 5); // đếm số lượng điểm từ 5 trở lên
                var duoitb = _context.Diemhps.Where(h => h.MaHp == hp).Count(h => h.DiemHp < 5); // đếm số lượng điểm từ 5 trở xuống

                var slsv = new SoLuongSinhVien
                {
                    MaHP = tenHP,
                    SoLuong = sl,
                    DanhSachSoLuongTheoDiem = soLuongSVtheoDiem,
                    SoluongTrenTB = trentb,
                    SoluongDuoiTB = duoitb
                };

                danhSachSoLuongSinhVien.Add(slsv);
            }    
            var viewModel = new SL_SinhVienNganh
            {
                DanhSachIDNganh = danhSachIDNganh,
                DanhSachSVTheoNganh = danhSachSV_Nganh,
                SelectedID = manganh,
                SelectedSVID = masv,
                DanhSachDiem = danhSachDiem,
                IdNganh = manganh,
                DanhSachIdHocPhan = danhSachIdHocPhan,
                DanhSachSoLuongSinhVien = danhSachSoLuongSinhVien
            };
            return View(viewModel);
        }
    }
}
