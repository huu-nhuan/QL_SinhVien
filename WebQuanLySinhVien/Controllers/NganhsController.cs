using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebQuanLySinhVien.Models;

namespace WebQuanLySinhVien.Controllers
{
    public class NganhsController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public NganhsController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Nganhs
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            var quanLySinhVienContext = _context.Nganhs.Include(n => n.MaKhoaNavigation).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                quanLySinhVienContext = quanLySinhVienContext.Where(s => s.TenNganh.Contains(searchString));
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
            //var quanLySinhVienContext = _context.Nganhs.Include(n => n.MaKhoaNavigation);
            //return View(await quanLySinhVienContext.ToListAsync());
        }

        // GET: Nganhs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nganh = await _context.Nganhs
                .Include(n => n.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaNganh == id);
            if (nganh == null)
            {
                return NotFound();
            }

            return View(nganh);
        }

        // GET: Nganhs/Create
        public IActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa");
            return PartialView("_CreatePartial", new Nganh());
        }

        // POST: Nganhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNganh,TenNganh,MaKhoa")] Nganh nganh)
        {
            if(NganhExists(nganh.MaNganh))
            {
                return Json(new { success = false, message = "Mã ngành này đã tồn tại" });
            }    
            if (ModelState.IsValid)
            {

                _context.Add(nganh);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Thêm thất bại" });
            //string loi = "";
            //foreach (var state in ModelState)
            //{
            //    foreach (var error in state.Value.Errors)
            //    {
            //        // Log or inspect the error
            //        loi = loi + error.ErrorMessage + "\n" + nganh.MaKhoaNavigation;
            //    }
            //}
            //return BadRequest(loi); 
            //ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", nganh.MaKhoa);
            //return View(nganh);
        }

        // GET: Nganhs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nganh = await _context.Nganhs.FindAsync(id);
            if (nganh == null)
            {
                return NotFound();
            }
            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", nganh.MaKhoa);
            return PartialView("_EditPartial", nganh);
        }

        // POST: Nganhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNganh,TenNganh,MaKhoa")] Nganh nganh)
        {
            if (id != nganh.MaNganh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nganh);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NganhExists(nganh.MaNganh))
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

        // GET: Nganhs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nganh = await _context.Nganhs
                .Include(n => n.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaNganh == id);
            if (nganh == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", nganh);
        }

        // POST: Nganhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nganh = await _context.Nganhs.FindAsync(id);
            if (nganh == null)
            {
                return NotFound();
            }

            try
            {
                _context.Nganhs.Remove(nganh);
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

        private bool NganhExists(string id)
        {
            return _context.Nganhs.Any(e => e.MaNganh == id);
        }
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
