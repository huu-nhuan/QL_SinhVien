using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebQuanLySinhVien.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebQuanLySinhVien.Controllers
{
    public class SinhViensController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public SinhViensController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: SinhViens
        public async Task<IActionResult> Index(string searchString, string searchBy, int page = 1, int pageSize = 10)
        {
            var role = User.FindFirst("Role")?.Value;
            var maSo = User.FindFirst("MaSo")?.Value;
            var quanLySinhVienContext = _context.SinhViens.Include(s => s.IdTkNavigation).Include(s => s.MaLopNavigation).AsQueryable();

            if (role == "2" && !string.IsNullOrEmpty(maSo))
            {
                quanLySinhVienContext = quanLySinhVienContext
                    .Where(s => s.MaLopNavigation.MaGv == maSo);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                if(searchBy == "ten")
                {
                    quanLySinhVienContext =  quanLySinhVienContext.Where(s => s.HoTen.Contains(searchString));
                }
                else
                {
                    quanLySinhVienContext =  quanLySinhVienContext.Where(s => s.Email.Contains(searchString));
                }
            }
            // Tính toán phân trang
            var totalItems = await quanLySinhVienContext.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await quanLySinhVienContext
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewData["searchBy"] = searchBy;
            ViewData["searchString"] = searchString;

            return View(items);
        }

        // GET: SinhViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.IdTkNavigation)
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // GET: SinhViens/Create
        [Authorize(Policy = "GiangVienOrAdmin")]
        public IActionResult Create()
        {
            //var tk = _context.Taikhoans;
            //var tk = _context.Taikhoans.Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
            //&& !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk)).Select(i => i.IdTk);
            ViewBag.IdTk = new SelectList(_context.Taikhoans.Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
            && !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk)), "IdTk", "IdTk");
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "MaLop");
            return PartialView("_CreatePartial", new SinhVien());
        }

        // POST: SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "GiangVienOrAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,MaLop,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] SinhVien sinhVien)
        {
            if (SinhVienExist(sinhVien.MaSv))
            {
                return Json(new { success = false, message = "Mã sinh viên này đã tồn tại" });
            }
            if (emailExist(sinhVien.Email))
            {
                return Json(new { success = false, message = "Email này đã tồn tại" });
            }
            if (sdtlExist(sinhVien.Sdt))
            {
                return Json(new { success = false, message = "Số điên thoại này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(sinhVien);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Thêm thành công" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return Json(new { success = false, message = $"Error: {ex.Message}" });
                }

            }
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Key = ms.Key,
                        Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                ViewBag.Errors = Newtonsoft.Json.JsonConvert.SerializeObject(errorList);
                return Json(new { success = false, message = Newtonsoft.Json.JsonConvert.SerializeObject(errorList) });
            }
            return Json(new { success = false, message = "Thêm thất bại" });
        }

        // GET: SinhViens/Edit/5
        [Authorize(Policy = "GiangVienOrAdmin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }
            //var tk = _context.Taikhoans.Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
            //&& !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk));
            //ViewBag.IdTk = new SelectList(_context.Taikhoans.Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
            //&& !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk)), "IdTk", "IdTk", sinhVien.IdTk);
            var taikhoans = _context.Taikhoans
            .Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
              && !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk))
            .ToList(); // Chuyển thành danh sách để có thể thêm phần tử mới

            // Kiểm tra nếu sinhVien.IdTk khác null và chưa có trong danh sách, thì thêm vào
            if (sinhVien.IdTk.HasValue && !taikhoans.Any(t => t.IdTk == sinhVien.IdTk.Value))
            {
                taikhoans.Add(new Taikhoan { IdTk = sinhVien.IdTk.Value });
            }

            // Tạo SelectList với danh sách đã sửa đổi
            ViewBag.IdTk = new SelectList(taikhoans, "IdTk", "IdTk", sinhVien.IdTk);

            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return PartialView("_EditPartial", sinhVien);
        }

        // POST: SinhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "GiangVienOrAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,MaLop,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] SinhVien sinhVien)
        {
            if (id != sinhVien.MaSv)
            {
                return NotFound();
            }
            if(emailExist(sinhVien.Email, sinhVien.MaSv))
            {
                return Json(new { success = false, message = "Email này đã tồn tại" });
            }
            if (sdtlExist(sinhVien.Sdt, sinhVien.MaSv))
            {
                return Json(new { success = false, message = "Số điện thoại này đã tồn tại" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (Exception ex)
                {
                    if (!SinhVienExist(sinhVien.MaSv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Lỗi cơ sở dữ liệu: {ex.Message}" });
                    }
                }
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }

        // GET: SinhViens/Delete/5

        [Authorize(Policy = "GiangVienOrAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.IdTkNavigation)
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", sinhVien);
        }

        // POST: SinhViens/Delete/5
        [Authorize(Policy = "GiangVienOrAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            try
            {
                var hs = _context.HoSoHocTaps.Where(h => h.MaSv == sinhVien.MaSv);
                if (hs.Any())
                {
                    foreach (var h in hs)
                    {
                        _context.Remove(h);
                    }
                }
                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                return Json(new { success = false, message = "Không thể xóa được vì dữ liệu đang được sử dụng ở bảng khác" });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = false, message = $"Lỗi cơ sở dữ liệu: {ex.Message}" });
            }
        }
        
        [Authorize(Policy = "GiangVienOrAdmin")]
        public async Task<IActionResult> QuanLy(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.IdTkNavigation)
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }
        public async Task<IActionResult> DSHocPhan(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dshp = await _context.Diemhps
                .Include(s => s.MaSvNavigation)
                .Include(s => s.MaHpNavigation)
                .Where(m => m.MaSv == id).ToListAsync();
            if (dshp == null)
            {
                return NotFound();
            }
            ViewData["hoten"] = _context.SinhViens.Where(s => s.MaSv == id).Select(s => s.HoTen).FirstOrDefault();
            ViewData["masv"] = id;
            return View(dshp);
        }

        private bool SinhVienExist(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }

        private bool emailExist(string email, string? id = null)
        {
            bool existsInSinhVien;
            bool existsInGiangVien;

            if (!string.IsNullOrEmpty(id))
            {
                existsInSinhVien = _context.SinhViens.Any(e => e.Email == email && e.MaSv != id);
                existsInGiangVien = _context.GiangViens.Any(e => e.Email == email);
            }
            else
            {
                existsInSinhVien = _context.SinhViens.Any(e => e.Email == email);
                existsInGiangVien = _context.GiangViens.Any(e => e.Email == email);
            }

            return existsInSinhVien || existsInGiangVien;

        }
        private bool sdtlExist(string sdt, string? id = null)
        {
            bool existsInSinhVien;
            bool existsInGiangVien;

            if (!string.IsNullOrEmpty(id))
            {
                existsInSinhVien = _context.SinhViens.Any(e => e.Sdt == sdt && e.MaSv != id);
                existsInGiangVien = _context.GiangViens.Any(e => e.Sdt == sdt);
            }
            else
            {
                existsInSinhVien = _context.SinhViens.Any(e => e.Sdt == sdt);
                existsInGiangVien = _context.GiangViens.Any(e => e.Sdt == sdt);
            }

            return existsInSinhVien || existsInGiangVien;
        }
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
