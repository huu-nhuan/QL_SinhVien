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
    public class SinhViensController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public SinhViensController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: SinhViens
        public async Task<IActionResult> Index(string searchString, string searchBy)
        {
            var quanLySinhVienContext = _context.SinhViens.Include(s => s.IdTkNavigation).Include(s => s.MaLopNavigation);

            if(!String.IsNullOrEmpty(searchString))
            {
                if(searchBy == "ten")
                {
                    var sv = await quanLySinhVienContext.Where(s => s.HoTen.Contains(searchString)).ToListAsync();
                    ViewData["searchString"] = searchString;
                    ViewData["searchBy"] = searchBy;
                    return View(sv);
                }
                else
                {
                    var sv = await quanLySinhVienContext.Where(s => s.GioiTinh.Contains(searchString)).ToListAsync();
                    ViewData["searchString"] = searchString;
                    ViewData["searchBy"] = searchBy;
                    return View(sv);
                }
                
            }    
            return View(await quanLySinhVienContext.ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,MaLop,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] SinhVien sinhVien)
        {
            if (SinhVienExist(sinhVien.MaSv))
            {
                return Json(new { success = false, message = "Mã sinh viên này đã tồn tại" });
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,MaLop,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] SinhVien sinhVien)
        {
            if (id != sinhVien.MaSv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExist(sinhVien.MaSv))
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

        // GET: SinhViens/Delete/5
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
        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
    }
}
