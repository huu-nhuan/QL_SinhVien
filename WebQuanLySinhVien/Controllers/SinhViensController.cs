using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index()
        {
            var quanLySinhVienContext = _context.SinhViens.Include(s => s.IdTkNavigation).Include(s => s.MaLopNavigation);
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
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk");
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop");
            return View();
        }

        // POST: SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,MaLop,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk", sinhVien.IdTk);
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return View(sinhVien);
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
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk", sinhVien.IdTk);
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return View(sinhVien);
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk", sinhVien.IdTk);
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return View(sinhVien);
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

            return View(sinhVien);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }
    }
}
