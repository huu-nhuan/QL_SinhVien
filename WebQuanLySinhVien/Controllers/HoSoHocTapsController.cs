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
    public class HoSoHocTapsController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public HoSoHocTapsController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: HoSoHocTaps
        public async Task<IActionResult> Index()
        {
            return View(await _context.HoSoHocTaps.ToListAsync());
        }

        // GET: HoSoHocTaps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoHocTap = await _context.HoSoHocTaps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hoSoHocTap == null)
            {
                return NotFound();
            }

            return View(hoSoHocTap);
        }

        // GET: HoSoHocTaps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HoSoHocTaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaSv,TrangThai,NgayNhapHoc,NgayTotNghiep,DiemTichLuy,SoTctichLuy,GhiChu,NgayCapNhat")] HoSoHocTap hoSoHocTap)
        {
            if (HoSoHocTapExists(hoSoHocTap.Id))
            {
                return Json(new { success = false, message = "Hồ sơ này đã tồn tại" });
            }
            var diem = _context.Diemhps.Where(m => m.MaSv == hoSoHocTap.MaSv).Sum(s => s.DiemHp ?? 0);
            hoSoHocTap.DiemTichLuy = (decimal)diem; // gán điểm tích lũy
            int tc = 0;
            var listHp = _context.Diemhps.Where(m => m.MaSv==hoSoHocTap.MaSv).Select(h => h.MaHp).ToList();
            foreach (var hp in listHp)
            {
                if (hp != null)
                {
                    tc += _context.Hocphans.Where(h => h.MaHp == hp).Sum(t => t.SoTc);
                }
            }
            hoSoHocTap.SoTctichLuy = tc; // gán số tín chỉ tích lũy
            hoSoHocTap.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now); // gán ngày hiện tại
            // Tìm giá trị lớn nhất của id hiện có
            var maxId = _context.HoSoHocTaps.Any() ? _context.HoSoHocTaps.Max(h => h.Id) : 0;

            // Tăng id lên 1 hoặc bắt đầu từ 1 nếu danh sách rỗng
            hoSoHocTap.Id = maxId + 1;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(hoSoHocTap);
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

        // GET: HoSoHocTaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoHocTap = await _context.HoSoHocTaps.FindAsync(id);
            if (hoSoHocTap == null)
            {
                return NotFound();
            }

            return PartialView("_EditPartial", hoSoHocTap);
        }

        // POST: HoSoHocTaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaSv,TrangThai,NgayNhapHoc,NgayTotNghiep,DiemTichLuy,SoTctichLuy,GhiChu,NgayCapNhat")] HoSoHocTap hoSoHocTap)
        {
            var hs = await _context.HoSoHocTaps
           .AsNoTracking()
           .FirstOrDefaultAsync(h => h.Id == id);

            if (id != hoSoHocTap.Id || hs == null)
            {
                return NotFound();
            }
            
            if (hs.TrangThai == "Thôi học" && hoSoHocTap.TrangThai == "Tốt nghiệp")
            {
                return Json(new
                {
                    success = false,
                    message = "Không thể chuyển trạng thái từ thôi học sang tốt nghiệp!"
                }); 
            }
            if (hs.TrangThai == "Đang học" && hoSoHocTap.TrangThai == "Tốt nghiệp")
            {
                if (hoSoHocTap.DiemTichLuy <= 50 || hoSoHocTap.SoTctichLuy <= 10)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Điểm tích lũy >= 50 và số tín chỉ tích lũy >= 10 mới đủ điều kiện tốt nghiệp!"
                    });
                }
            }

            var diem = _context.Diemhps.Where(m => m.MaSv == hoSoHocTap.MaSv).Sum(s => s.DiemHp ?? 0);
            hoSoHocTap.DiemTichLuy = (decimal)diem; // gán điểm tích lũy
            int tc = 0;
            var listHp = _context.Diemhps.Where(m => m.MaSv == hoSoHocTap.MaSv).Select(h => h.MaHp).ToList();
            foreach (var hp in listHp)
            {
                if (hp != null)
                {
                    tc += _context.Hocphans.Where(h => h.MaHp == hp).Sum(t => t.SoTc);
                }
            }
            hoSoHocTap.SoTctichLuy = tc; // gán số tín chỉ tích lũy
            hoSoHocTap.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now); // gán ngày hiện tại

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoSoHocTap);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (Exception ex)
                {
                    if (!HoSoHocTapExists(hoSoHocTap.Id))
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

        // GET: HoSoHocTaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoHocTap = await _context.HoSoHocTaps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hoSoHocTap == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", hoSoHocTap);
        }

        // POST: HoSoHocTaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoSoHocTap = await _context.HoSoHocTaps.FindAsync(id);
            if (hoSoHocTap == null)
            {
                return NotFound();
            }

            try
            {
                _context.HoSoHocTaps.Remove(hoSoHocTap);
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

        public async Task<IActionResult> DSHoSo(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dshs = await _context.HoSoHocTaps
                .Where(m => m.MaSv == id).ToListAsync();
            if (dshs == null)
            {
                return NotFound();
            }
            ViewData["hoten"] = _context.SinhViens.Where(s => s.MaSv == id).Select(s => s.HoTen).FirstOrDefault();
            ViewData["idhs"] = _context.SinhViens.Where(s => s.MaSv == id).Select(s => s.HoSo).FirstOrDefault();
            ViewData["masv"] = id;
            return View(dshs);
        }

        public IActionResult AddHs(string id)
        {
            ViewData["MaSV"] = id;
            return PartialView("_CreatePartial", new HoSoHocTap());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHs([Bind("Id,MaSv,TrangThai,NgayNhapHoc,NgayTotNghiep,DiemTichLuy,SoTctichLuy,GhiChu,NgayCapNhat")] HoSoHocTap hoSoHocTap)
        {
            if (HoSoHocTapExists(hoSoHocTap.Id))
            {
                return Json(new { success = false, message = "Hồ sơ này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(hoSoHocTap);
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
        [HttpPost]
        public async Task<IActionResult> SuDungHS(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hs = await _context.HoSoHocTaps
                .Where(m => m.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (hs == null)
            {
                return NotFound();
            }
            var sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaSv == hs.MaSv);
            if (sv == null)
            {
                return NotFound();
            }
            sv.HoSo = hs.Id;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sv);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Lỗi cơ sở dữ liệu: {ex.Message}" });   
                }
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }

        private bool IsForeignKeyViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 547 || sqlEx.Message.Contains("FOREIGN KEY"));
        }
        private bool HoSoHocTapExists(int id)
        {
            return _context.HoSoHocTaps.Any(e => e.Id == id);
        }
    }
}
