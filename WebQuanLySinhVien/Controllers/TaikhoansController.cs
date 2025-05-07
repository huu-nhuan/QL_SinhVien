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
    public class TaikhoansController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public TaikhoansController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Taikhoans
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            var quanLySinhVienContext = _context.Taikhoans.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                quanLySinhVienContext = quanLySinhVienContext.Where(s => s.TenDangNhap.Contains(searchString));
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
            //return View(await _context.Taikhoans.ToListAsync());
        }

        // GET: Taikhoans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoan = await _context.Taikhoans
                .FirstOrDefaultAsync(m => m.IdTk.ToString() == id);
            if (taikhoan == null)
            {
                return NotFound();
            }

            return View(taikhoan);
        }

        // GET: Taikhoans/Create
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new Taikhoan());
        }

        // POST: Taikhoans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTk,TenDangNhap,MatKhau,VaiTro")] Taikhoan taikhoan)
        {
            if (TaikhoanExists(taikhoan.TenDangNhap))
            {
                return Json(new { success = false, message = "Tên đăng nhập này đã tồn tại" });
            }
            taikhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(taikhoan.MatKhau); // mã hóa mk
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(taikhoan);
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

        // GET: Taikhoans/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }

            var taikhoan = await _context.Taikhoans.FindAsync(id);
            if (taikhoan == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", taikhoan);
        }

        // POST: Taikhoans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTk,TenDangNhap,MatKhau,VaiTro")] Taikhoan taikhoan)
        {
            if (id != taikhoan.IdTk)
            {
                return NotFound();
            }
            if (TaikhoanExists(taikhoan.TenDangNhap, id))
            {
                return Json(new { success = false, message = "Tên đăng nhập này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    taikhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(taikhoan.MatKhau);
                    _context.Update(taikhoan);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (Exception ex)
                {
                    if (!IdExists(taikhoan.IdTk))
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

        // GET: Taikhoans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoan = await _context.Taikhoans
                .FirstOrDefaultAsync(m => m.IdTk.ToString() == id);
            if (taikhoan == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", taikhoan);
        }

        // POST: Taikhoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taikhoan = await _context.Taikhoans.FindAsync(id);
            if (taikhoan == null)
            {
                return NotFound();
            }
            try
            {
                _context.Taikhoans.Remove(taikhoan);
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

        private bool TaikhoanExists(string tdn, int id = 0)
        {
            if(id>0)
            {
                return _context.Taikhoans.Any(e => e.TenDangNhap == tdn && e.IdTk != id);
            }    
            return _context.Taikhoans.Any(e => e.TenDangNhap == tdn);
        }

        private bool IdExists(int id)
        {
            return _context.Taikhoans.Any(e => e.IdTk == id);
        }
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
