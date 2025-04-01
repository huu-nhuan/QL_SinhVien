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
    public class DiemhpsController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public DiemhpsController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Diemhps
        public async Task<IActionResult> Index()
        {
            var quanLySinhVienContext = _context.Diemhps.Include(d => d.MaHpNavigation).Include(d => d.MaSvNavigation);
            return View(await quanLySinhVienContext.ToListAsync());
        }

        // GET: Diemhps/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diemhp = await _context.Diemhps
                .Include(d => d.MaHpNavigation)
                .Include(d => d.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (diemhp == null)
            {
                return NotFound();
            }

            return View(diemhp);
        }

        // GET: Diemhps/Create
        public IActionResult Create()
        {
            ViewBag.MaHp = new SelectList(_context.Hocphans, "MaHp", "MaHp");
            ViewBag.MaSv = new SelectList(_context.SinhViens, "MaSv", "MaSv");
            return PartialView("_CreatePartial", new Diemhp());
        }

        // POST: Diemhps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,MaHp,DiemHp")] Diemhp diem)
        {
            if (DiemhpExists(diem.MaSv, diem.MaHp))
            {
                return Json(new { success = false, message = "Điểm này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(diem);
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

        // GET: Diemhps/Edit/5

        public async Task<IActionResult> Edit(string id1, string id2)
        {
            if (id1 == null || id2 == null)
            {
                return NotFound();
            }

            var diemhp = await _context.Diemhps.FirstOrDefaultAsync(d => d.MaSv == id1 && d.MaHp == id2);
            if (diemhp == null)
            {
                return NotFound();
            }
            ViewBag.MaHp= new SelectList(_context.Hocphans, "MaHp", "MaHp", diemhp.MaHp);
            ViewBag.MaSv = new SelectList(_context.SinhViens, "MaSv", "MaSv", diemhp.MaSv);
            return PartialView("_EditPartial", diemhp);
        }

        // POST: Diemhps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id1, string id2, [Bind("MaSv,MaHp,DiemHp")] Diemhp diem)
        {
            if (id1 != diem.MaSv || id2 != diem.MaHp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diem);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiemhpExists(diem.MaSv, diem.MaHp))
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

        // GET: Diemhps/Delete/5
        public async Task<IActionResult> Delete(string id1, string id2)
        {
            if (id1 == null || id2 == null)
            {
                return NotFound();
            }

            var diemhp = await _context.Diemhps
                .Include(d => d.MaHpNavigation)
                .Include(d => d.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id1 && m.MaHp == id2);
            if (diemhp == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", diemhp);
        }

        // POST: Diemhps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id1, string id2)
        {
            var diemhp = await _context.Diemhps.FirstOrDefaultAsync(d => d.MaSv == id1 && d.MaHp == id2);
            if (diemhp == null)
            {
                return NotFound();
            }

            try
            {
                _context.Diemhps.Remove(diemhp);
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

        private bool DiemhpExists(string masv, string mahp)
        {
            return _context.Diemhps.Any(e => e.MaSv == masv && e.MaHp == mahp);
        }
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
