using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WebQuanLySinhVien.Models;

namespace WebQuanLySinhVien.Controllers
{
    public class KhoasController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public KhoasController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Khoas
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            var quanLySinhVienContext = _context.Khoas.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                quanLySinhVienContext = quanLySinhVienContext.Where(s => s.TenKhoa.Contains(searchString));
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
            //return View(await _context.Khoas.ToListAsync());
        }

        // GET: Khoas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas
                .FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
            {
                return NotFound();
            }

            return View(khoa);
        }

        // GET: Khoas/Create
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new Khoa());
        }

        // POST: Khoas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhoa,TenKhoa")] Khoa khoa)
        {
            if(KhoaExists(khoa.MaKhoa))
            {
                return Json(new { success = false, message = "Mã khoa này đã tồn tại" });
            }    
            if (ModelState.IsValid)
            {
                _context.Add(khoa);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Thêm thất bại" });
        }

        // GET: Khoas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", khoa);
        }

        // POST: Khoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKhoa,TenKhoa")] Khoa khoa)
        {
            if (id != khoa.MaKhoa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoa);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaExists(khoa.MaKhoa))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }

        // GET: Khoas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas
                .FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", khoa);
        }

        // POST: Khoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa == null)
            {
                return NotFound();
            }
            try
            {
                _context.Khoas.Remove(khoa);
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
                //return PartialView("_DeletePartial", khoa);
        }

        private bool KhoaExists(string id)
        {
            return _context.Khoas.Any(e => e.MaKhoa == id);
        }

        // Hàm kiểm tra lỗi khóa ngoại (cho EF Core)
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
