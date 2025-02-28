using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;


namespace WebQuanLySinhVien.Controllers
{
    public class AdminTableListController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public AdminTableListController(QuanLySinhVienContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? pageKhoa, int? pageNganh, int? pageHP, int? pageTK, int? pageGV, int? pageLop, int? pageSV, int? pageDHP)
        {
            int pageSize = 5;
            int pageTKNumber = (pageTK ?? 1);
            int pageKhoaNumber = (pageKhoa ?? 1);
            int pageNganhNumber = (pageNganh ?? 1);
            int pageHPNumber = (pageHP ?? 1);
            int pageGVNumber = (pageGV ?? 1);
            int pageLopNumber = (pageLop ?? 1);
            int pageSVNumber = (pageSV ?? 1);
            int pageDHPNumber = (pageDHP ?? 1);
            // Lấy dữ liệu từ bảng 
            var danhSachTaiKhoan = _context.Taikhoans.OrderBy(tk => tk.IdTk).ToPagedList(pageTKNumber, pageSize);
            var danhSachKhoa = _context.Khoas.OrderBy(k => k.MaKhoa).ToPagedList(pageKhoaNumber, pageSize);
            var danhSachNganh = _context.Nganhs.OrderBy(n => n.MaNganh).ToPagedList(pageNganhNumber, pageSize);
            var danhSachHocPhan = _context.Hocphans.OrderBy(hp => hp.MaHp).ToPagedList(pageHPNumber, pageSize);
            var danhSachGiangVien = _context.GiangViens.OrderBy(gv => gv.MaGv).ToPagedList(pageGVNumber, pageSize);
            var danhSachLop = _context.Lops.OrderBy(l => l.MaLop).ToPagedList(pageLopNumber, pageSize);
            var danhSachSinhVien = _context.SinhViens.OrderBy(sv => sv.MaSv).ToPagedList(pageSVNumber, pageSize);
            var danhSachDiemHocPhan = _context.Diemhps.OrderBy(d => d.MaSv).ToPagedList(pageDHPNumber, pageSize);

            // Tạo ViewModel và gán dữ liệu
            var viewModel = new AdminTableList
            {
                DanhSachTaiKhoan = danhSachTaiKhoan,
                DanhSachKhoa = danhSachKhoa,
                DanhSachNganh = danhSachNganh,
                DanhSachHocPhan = danhSachHocPhan,
                DanhSachGiangVien = danhSachGiangVien,
                DanhSachLop = danhSachLop,
                DanhSachSinhVien = danhSachSinhVien,
                DanhSachDiemHocPhan = danhSachDiemHocPhan
            };
            return View(viewModel);
        }
    }
}
