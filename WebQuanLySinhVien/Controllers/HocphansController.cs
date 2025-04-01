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
    public class HocphansController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public HocphansController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Hocphans
        public async Task<IActionResult> Index()
        {
            var quanLySinhVienContext = _context.Hocphans.Include(h => h.MaNganhNavigation);
            return View(await quanLySinhVienContext.ToListAsync());
        }

        // GET: Hocphans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocphan = await _context.Hocphans
                .Include(h => h.MaNganhNavigation)
                .FirstOrDefaultAsync(m => m.MaHp == id);
            if (hocphan == null)
            {
                return NotFound();
            }

            return View(hocphan);
        }

        // GET: Hocphans/Create
        public IActionResult Create()
        {
            ViewBag.MaNganh = new SelectList(_context.Nganhs, "MaNganh", "MaNganh");
            return PartialView("_CreatePartial", new Hocphan());
        }

        // POST: Hocphans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHp,TenHp,SoTc,MaNganh,HocKy")] Hocphan hocphan)
        {
            if (HocphanExists(hocphan.MaHp))
            {
                return Json(new { success = false, message = "Mã học phần này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(hocphan);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Thêm thành công" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
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
                return Json(new { success = false, message = "Dữ liệu nhập vào chưa chính xác" });
            }
            return Json(new { success = false, message = "Thêm thất bại" });
        }

        // GET: Hocphans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocphan = await _context.Hocphans.FindAsync(id);
            if (hocphan == null)
            {
                return NotFound();
            }
            ViewBag.MaNganh = new SelectList(_context.Nganhs, "MaNganh", "MaNganh", hocphan.MaNganh);
            return PartialView("_EditPartial", hocphan);
        }

        // POST: Hocphans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHp,TenHp,SoTc,MaNganh,HocKy")] Hocphan hocphan)
        {
            if (id != hocphan.MaHp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocphan);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocphanExists(hocphan.MaHp))
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

        // GET: Hocphans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocphan = await _context.Hocphans
                .Include(h => h.MaNganhNavigation)
                .FirstOrDefaultAsync(m => m.MaHp == id);
            if (hocphan == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", hocphan);
        }

        // POST: Hocphans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hocphan = await _context.Hocphans.FindAsync(id);
            if (hocphan == null)
            {
                return NotFound();
            }

            try
            {
                _context.Hocphans.Remove(hocphan);
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

        private bool HocphanExists(string id)
        {
            return _context.Hocphans.Any(e => e.MaHp == id);
        }
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }

}
