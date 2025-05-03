using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebQuanLySinhVien.Models;

namespace WebQuanLySinhVien.Controllers
{
    public class GiangViensController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public GiangViensController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: GiangViens
        public async Task<IActionResult> Index(string searchString, string searchBy, int page = 1, int pageSize = 10)
        {
            var quanLySinhVienContext = _context.GiangViens.Include(g => g.IdTkNavigation).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchBy == "ten")
                {
                    quanLySinhVienContext = quanLySinhVienContext.Where(s => s.HoTen.Contains(searchString));
                    //ViewData["searchString"] = searchString;
                    //ViewData["searchBy"] = searchBy;
                    //return View(quanLySinhVienContext);
                }
                else
                {
                    quanLySinhVienContext = quanLySinhVienContext.Where(s => s.Email.Contains(searchString));
                    //ViewData["searchString"] = searchString;
                    //ViewData["searchBy"] = searchBy;
                    //return View(quanLySinhVienContext);
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
            //var quanLySinhVienContext = _context.GiangViens.Include(g => g.IdTkNavigation);
            //return View(await quanLySinhVienContext.ToListAsync());
        }

        // GET: GiangViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giangVien = await _context.GiangViens
                .Include(g => g.IdTkNavigation)
                .FirstOrDefaultAsync(m => m.MaGv == id);
            if (giangVien == null)
            {
                return NotFound();
            }

            return View(giangVien);
        }

        // GET: GiangViens/Create
        public IActionResult Create()
        {
            ViewBag.IdTk = new SelectList(_context.Taikhoans.Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
            && !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk)), "IdTk", "IdTk");
            return PartialView("_CreatePartial", new GiangVien());
        }

        // POST: GiangViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGv,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] GiangVien giangVien)
        {
            if (GiangVienExists(giangVien.MaGv))
            {
                return Json(new { success = false, message = "Mã giảng viên này đã tồn tại" });
            }
            if (emailExist(giangVien.Email))
            {
                return Json(new { success = false, message = "Email này đã tồn tại" });
            }
            if (sdtlExist(giangVien.Sdt))
            {
                return Json(new { success = false, message = "Số điên thoại này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(giangVien);
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

                return Json(new { success = false, message = Newtonsoft.Json.JsonConvert.SerializeObject(errorList) });
            }
            return Json(new { success = false, message = "Thêm thất bại" });
        }

        // GET: GiangViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giangVien = await _context.GiangViens.FindAsync(id);
            if (giangVien == null)
            {
                return NotFound();
            }
            //ViewBag.IdTk = new SelectList(_context.Taikhoans, "IdTk", "IdTk", giangVien.IdTk);
            var taikhoans = _context.Taikhoans
            .Where(t1 => !_context.SinhViens.Select(t2 => t2.IdTk).Contains(t1.IdTk)
              && !_context.GiangViens.Select(t3 => t3.IdTk).Contains(t1.IdTk))
            .ToList(); // Chuyển thành danh sách để có thể thêm phần tử mới

            // Kiểm tra nếu sinhVien.IdTk khác null và chưa có trong danh sách, thì thêm vào
            if (giangVien.IdTk.HasValue && !taikhoans.Any(t => t.IdTk == giangVien.IdTk.Value))
            {
                taikhoans.Add(new Taikhoan { IdTk = giangVien.IdTk.Value });
            }

            // Tạo SelectList với danh sách đã sửa đổi
            ViewBag.IdTk = new SelectList(taikhoans, "IdTk", "IdTk", giangVien.IdTk);
            return PartialView("_EditPartial", giangVien);
        }

        // POST: GiangViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaGv,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] GiangVien giangVien)
        {
            if (id != giangVien.MaGv)
            {
                return NotFound();
            }
            if (emailExist(giangVien.Email, giangVien.MaGv))
            {
                return Json(new { success = false, message = "Email này đã tồn tại" });
            }
            if (sdtlExist(giangVien.Sdt, giangVien.MaGv))
            {
                return Json(new { success = false, message = "Số điện thoại này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giangVien);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (Exception ex)
                {
                    if (!GiangVienExists(giangVien.MaGv))
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

        // GET: GiangViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giangVien = await _context.GiangViens
                .Include(g => g.IdTkNavigation)
                .FirstOrDefaultAsync(m => m.MaGv == id);
            if (giangVien == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", giangVien);
        }

        // POST: GiangViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var giangVien = await _context.GiangViens.FindAsync(id);
            if (giangVien == null)
            {
                return NotFound();
            }

            try
            {
                _context.GiangViens.Remove(giangVien);
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

        private bool GiangVienExists(string id)
        {
            return _context.GiangViens.Any(e => e.MaGv == id);
        }
        private bool emailExist(string email, string? id = null)
        {
            bool existsInSinhVien;
            bool existsInGiangVien;

            if (!string.IsNullOrEmpty(id))
            {
                existsInSinhVien = _context.SinhViens.Any(e => e.Email == email);
                existsInGiangVien = _context.GiangViens.Any(e => e.Email == email && e.MaGv != id);
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
            if (string.IsNullOrEmpty(sdt))
            {
                // Nếu số điện thoại là null hoặc rỗng, thì cho phép giống nhau
                return false;
            }

            bool existsInSinhVien;
            bool existsInGiangVien;

            if (!string.IsNullOrEmpty(id))
            {
                existsInSinhVien = _context.SinhViens.Any(e => e.Sdt == sdt);
                existsInGiangVien = _context.GiangViens.Any(e => e.Sdt == sdt && e.MaGv != id);
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
