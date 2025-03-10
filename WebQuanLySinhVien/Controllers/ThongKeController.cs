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
        public IActionResult Index( string manganh)
        {
            if(manganh==null)
            {
                manganh = "KTPM";
            }
            var danhSachIdHocPhan = _context.Hocphans.Where(hp => hp.MaNganh==manganh).OrderBy(hp => hp.MaHp).Select(hp => hp.MaHp).ToList(); // lấy danh sách hp
            var danhSachSoLuongSinhVien = new List<SoLuongSinhVien>();
            // tạo danh sách gồm mã hp và số lượng sinh viên tương ứng
            foreach (var hp in danhSachIdHocPhan )
            {
                var sl = _context.Diemhps.Count(d =>d.MaHp==hp);

                var slsv = new SoLuongSinhVien
                {
                    MaHP = hp,
                    SoLuong = sl
                };

                danhSachSoLuongSinhVien.Add(slsv);
            }    
            var viewModel = new SL_SinhVienNganh
            {
                IdNganh = manganh,
                DanhSachIdHocPhan = danhSachIdHocPhan,
                DanhSachSoLuongSinhVien = danhSachSoLuongSinhVien
            };
            return View(viewModel);
        }
    }
}
