using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;

namespace WebQuanLySinhVien.Controllers
{
    public class AdminTableListController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public AdminTableListController(QuanLySinhVienContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy dữ liệu từ bảng Khoa và Ngành
            var danhSachKhoa = _context.Khoas.ToList();
            var danhSachNganh = _context.Nganhs.ToList();
            var danhSachHocPhan = _context.Hocphans.ToList();

            // Tạo ViewModel và gán dữ liệu
            var viewModel = new AdminTableList
            {
                DanhSachKhoa = danhSachKhoa,
                DanhSachNganh = danhSachNganh,
                DanhSachHocPhan= danhSachHocPhan
            };
            return View(viewModel);
        }
    }
}
