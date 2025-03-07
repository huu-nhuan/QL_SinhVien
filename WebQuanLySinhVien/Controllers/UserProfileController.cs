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

namespace WebQuanLySinhVien.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly QuanLySinhVienContext _context;
        public UserProfileController(QuanLySinhVienContext context)
        {
            _context = context;
        }
        public  async Task<IActionResult> Profile(string selectedID)
        {
            var danhSachID =  _context.SinhViens.OrderBy(sv => sv.MaSv).Select(sv => sv.MaSv).Concat(
                              _context.GiangViens.OrderBy(gv => gv.MaGv).Select(gv => gv.MaGv)).ToList();
            if (selectedID == null)
            {
                selectedID = "None";
            }
            var sv = await _context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.MaSv==selectedID);
            var gv = await _context.GiangViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGv==selectedID);
            if (sv == null && gv == null)
            {
                var viewModel = new UserProfile
                {
                    DanhSachID = danhSachID,
                    SelectedID = selectedID
                };
                return View(viewModel);
            }
            else if (gv != null)
            {
                var viewModel = new UserProfile
                {
                    DanhSachID = danhSachID,
                    SelectedID = selectedID,
                    HoTen = gv.HoTen,
                    GioiTinh = gv.GioiTinh,
                    NgaySinh = gv.NgaySinh,
                    Sdt = gv.Sdt,
                    DiaChi = gv.DiaChi,
                    Email = gv.Email
                };
                return View(viewModel);
            }
            else
            {
                var viewModel = new UserProfile
                {
                    DanhSachID = danhSachID,
                    SelectedID = selectedID,
                    HoTen = sv.HoTen,
                    GioiTinh = sv.GioiTinh,
                    NgaySinh = sv.NgaySinh,
                    Sdt = sv.Sdt,
                    DiaChi = sv.DiaChi,
                    Email = sv.Email
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfile user)
        {
            var danhSachID = _context.SinhViens.OrderBy(sv => sv.MaSv).Select(sv => sv.MaSv).Concat(
                             _context.GiangViens.OrderBy(gv => gv.MaGv).Select(gv => gv.MaGv)).ToList();
            user.DanhSachID=danhSachID;
            var sv = await _context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.MaSv == user.SelectedID);
            var gv = await _context.GiangViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGv == user.SelectedID);
            if (sv == null && gv == null)
            {
                ViewBag.Thongbao = "lỗi";
                return View(user);
            }
            else if (gv != null)
            {
                GiangVien giangVien = new GiangVien();
                giangVien.MaGv = user.SelectedID;
                giangVien.HoTen = user.HoTen;
                giangVien.GioiTinh = user.GioiTinh;
                giangVien.NgaySinh = user.NgaySinh;
                giangVien.Sdt = user.Sdt;
                giangVien.DiaChi = user.DiaChi;
                giangVien.IdTk = gv.IdTk;
                giangVien.Email = user.Email;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(giangVien);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!GiangVienExists(giangVien.MaGv))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }    
                }
                ViewBag.Thongbao = "Cập nhật thành công";
                return View(user);
            }
            else
            {
                SinhVien sinhvien = new SinhVien();
                sinhvien.MaSv = user.SelectedID;
                sinhvien.MaLop = sv.MaLop;
                sinhvien.HoTen = user.HoTen;
                sinhvien.GioiTinh = user.GioiTinh;
                sinhvien.NgaySinh = user.NgaySinh;
                sinhvien.Sdt = user.Sdt;
                sinhvien.DiaChi = user.DiaChi;
                sinhvien.IdTk = sv.IdTk;
                sinhvien.Email = user.Email;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(sinhvien);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SinhVienExists(sinhvien.MaSv))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                ViewBag.Thongbao = "Cập nhật thành công";
                return View(user);
            }
        }
        private bool GiangVienExists(string id)
        {
            return _context.GiangViens.Any(e => e.MaGv == id);
        }
        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }
    }
}
