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
    public class GiangViensController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public GiangViensController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: GiangViens
        public async Task<IActionResult> Index()
        {
            var quanLySinhVienContext = _context.GiangViens.Include(g => g.IdTkNavigation);
            return View(await quanLySinhVienContext.ToListAsync());
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
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk");
            return View();
        }

        // POST: GiangViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGv,HoTen,GioiTinh,NgaySinh,Sdt,DiaChi,IdTk,Email")] GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giangVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk", giangVien.IdTk);
            return View(giangVien);
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
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk", giangVien.IdTk);
            return View(giangVien);
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giangVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiangVienExists(giangVien.MaGv))
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
            ViewData["IdTk"] = new SelectList(_context.Taikhoans, "IdTk", "IdTk", giangVien.IdTk);
            return View(giangVien);
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

            return View(giangVien);
        }

        // POST: GiangViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var giangVien = await _context.GiangViens.FindAsync(id);
            if (giangVien != null)
            {
                _context.GiangViens.Remove(giangVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiangVienExists(string id)
        {
            return _context.GiangViens.Any(e => e.MaGv == id);
        }
    }
}
