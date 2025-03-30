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
                var userRole = User.FindFirst("Role")?.Value;
                var userID = User.FindFirst("Id")?.Value;
                if (userRole == "2" || (userRole == "3"))
                {
                    var SVID = _context.SinhViens.Where(sv => sv.IdTk.ToString() == userID).Select(sv => sv.MaSv).FirstOrDefault();
                    var GVID = _context.GiangViens.Where(gv => gv.IdTk.ToString() == userID).Select(sv => sv.MaGv).FirstOrDefault();
                    selectedID = SVID ?? (GVID ?? "None");
                }
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
                var patch = _context.Taikhoans.Where(i => i.IdTk == gv.IdTk).Select(x => x.ImagePath).ToString();
                var viewModel = new UserProfile
                {
                    DanhSachID = danhSachID,
                    SelectedID = selectedID,
                    HoTen = gv.HoTen,
                    GioiTinh = gv.GioiTinh,
                    NgaySinh = gv.NgaySinh,
                    Sdt = gv.Sdt,
                    DiaChi = gv.DiaChi,
                    Email = gv.Email,
                    ImagePath = patch
                };
                return View(viewModel);
            }
            else
            {
                var patch = _context.Taikhoans.Where(i => i.IdTk == sv.IdTk).Select(x => x.ImagePath).FirstOrDefault();
                var viewModel = new UserProfile
                {
                    DanhSachID = danhSachID,
                    SelectedID = selectedID,
                    HoTen = sv.HoTen,
                    GioiTinh = sv.GioiTinh,
                    NgaySinh = sv.NgaySinh,
                    Sdt = sv.Sdt,
                    DiaChi = sv.DiaChi,
                    Email = sv.Email,
                    ImagePath = patch
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfile user, IFormFile? imageFile)
        {
            var danhSachID = _context.SinhViens.OrderBy(sv => sv.MaSv).Select(sv => sv.MaSv).Concat(
                             _context.GiangViens.OrderBy(gv => gv.MaGv).Select(gv => gv.MaGv)).ToList();
            user.DanhSachID=danhSachID;
            var sv = await _context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.MaSv == user.SelectedID);
            var gv = await _context.GiangViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGv == user.SelectedID); 
            if (sv == null && gv == null)
            {
                ViewData["Thongbao"] = "Không tìm thấy người dùng";
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

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (gv.IdTk == null)
                    {
                        ViewData["Thongbao"] = "Người dùng này chưa tạo tài khoản, không thể cập nhật ảnh đại diện";
                        return View(user);
                    }
                    SaveIMG(imageFile, gv.IdTk);
                }

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
                    user.ImagePath = _context.Taikhoans.Where(i => i.IdTk == gv.IdTk).Select(x => x.ImagePath).FirstOrDefault();
                    ViewData["thanhcong"] = "Cập nhật thành công";
                    return View(user);
                }
                user.ImagePath = _context.Taikhoans.Where(i => i.IdTk == gv.IdTk).Select(x => x.ImagePath).FirstOrDefault();
                ViewData["Thongbao"] = "có gì đó không đúng";
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

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (sv.IdTk == null)
                    {
                        ViewData["Thongbao"] = "Người dùng này chưa tạo tài khoản, không thể cập nhật ảnh đại diện";
                        return View(user);
                    }
                    SaveIMG(imageFile, sv.IdTk);
                }

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
                    user.ImagePath = _context.Taikhoans.Where(i => i.IdTk == sv.IdTk).Select(x => x.ImagePath).FirstOrDefault();
                    ViewData["thanhcong"] = "Cập nhật thành công";
                    return View(user);
                }
                
                //if (!ModelState.IsValid)
                //{
                //    var errorList = ModelState
                //        .Where(ms => ms.Value.Errors.Any())
                //        .Select(ms => new
                //        {
                //            Key = ms.Key,
                //            Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                //        })
                //        .ToList();

                //    ViewBag.Errors = Newtonsoft.Json.JsonConvert.SerializeObject(errorList);
                //}
                
                user.ImagePath = _context.Taikhoans.Where(i => i.IdTk == sv.IdTk).Select(x => x.ImagePath).FirstOrDefault();
                ViewData["Thongbao"] = "có gì đó không đúng";
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

        private void SaveIMG(IFormFile imageFile, int? tk)
        {
            var taikhoan = _context.Taikhoans.Where(t => t.IdTk == tk).FirstOrDefault();

            string fileName = Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/template/assets/img/faces", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }
            taikhoan.ImagePath = "/template/assets/img/faces/" + fileName;
            _context.Taikhoans.Update(taikhoan);
                 //user.ImagePath = "/Uploads/" + fileName; // Lưu đường dẫn vào database
                
            //return false;
        }
    }
}
