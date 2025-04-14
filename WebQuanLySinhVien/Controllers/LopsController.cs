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
    public class LopsController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public LopsController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Lops
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            var quanLySinhVienContext = _context.Lops.Include(l => l.MaGvNavigation).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                quanLySinhVienContext = quanLySinhVienContext.Where(s => s.TenLop.Contains(searchString));
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
            ViewData["searchString"] = searchString;

            return View(items);
            //var quanLySinhVienContext = _context.Lops.Include(l => l.MaGvNavigation);
            //return View(await quanLySinhVienContext.ToListAsync());
        }

        // GET: Lops/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lop = await _context.Lops
                .Include(l => l.MaGvNavigation)
                .FirstOrDefaultAsync(m => m.MaLop == id);
            if (lop == null)
            {
                return NotFound();
            }

            return View(lop);
        }

        // GET: Lops/Create
        public IActionResult Create()
        {
            ViewBag.MaGv = new SelectList(_context.GiangViens, "MaGv", "MaGv");
            return PartialView("_CreatePartial", new Lop());
        }

        // POST: Lops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLop,TenLop,MaGv,NamNhapHoc")] Lop lop)
        {
            if(LopExists(lop.MaLop))
            {
                return Json(new { success = false, message = "Mã lớp này đã tồn tại" });
            }    
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(lop);
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

        // GET: Lops/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lop = await _context.Lops.FindAsync(id);
            if (lop == null)
            {
                return NotFound();
            }
            ViewBag.MaGv = new SelectList(_context.GiangViens, "MaGv", "MaGv", lop.MaGv);
            return PartialView("_EditPartial", lop);
        }

        // POST: Lops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaLop,TenLop,MaGv,NamNhapHoc")] Lop lop)
        {
            if (id != lop.MaLop)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lop);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopExists(lop.MaLop))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }

        // GET: Lops/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lop = await _context.Lops
                .Include(l => l.MaGvNavigation)
                .FirstOrDefaultAsync(m => m.MaLop == id);
            if (lop == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", lop);
        }

        // POST: Lops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lop = await _context.Lops.FindAsync(id);
            if (lop == null)
            {
                return NotFound();
            }
            try
            {
                _context.Lops.Remove(lop);
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

        public async Task<IActionResult> DSThanhVien(string searchString, string searchBy, string malop, int page = 1, int pageSize = 10)
        {
            var sinhviens = _context.SinhViens.Include(n => n.MaLopNavigation).Include(n => n.IdTkNavigation).AsQueryable();

            if (!String.IsNullOrEmpty(malop))
            {
                sinhviens = sinhviens.Where(s => s.MaLop.Contains(malop));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchBy == "ten")
                    sinhviens = sinhviens.Where(s => s.HoTen.Contains(searchString));
                else
                    sinhviens = sinhviens.Where(s => s.Email.Contains(searchString));
            }
            // Tính toán phân trang
            var totalItems = await sinhviens.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var gv = _context.Lops.Where(l => l.MaLop == malop).Select(l => l.MaGvNavigation).FirstOrDefault();

            var items = await sinhviens
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewData["TenLop"] = _context.Lops.Where(k => k.MaLop == malop).Select(k => k.TenLop).FirstOrDefault();
            ViewData["MaLop"] = malop;
            ViewData["searchBy"] = searchBy;
            ViewData["searchString"] = searchString;
            ViewData["GiangVien"] = gv;

            return View(items);
            //var quanLySinhVienContext = _context.Nganhs.Include(n => n.MaKhoaNavigation);
            //return View(await quanLySinhVienContext.ToListAsync());
        }

        private bool LopExists(string id)
        {
            return _context.Lops.Any(e => e.MaLop == id);
        }

        // Hàm kiểm tra lỗi khóa ngoại (cho EF Core)
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
