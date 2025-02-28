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
            ViewData["MaHp"] = new SelectList(_context.Hocphans, "MaHp", "MaHp");
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv");
            return View();
        }

        // POST: Diemhps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,MaHp,DiemHp")] Diemhp diemhp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diemhp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHp"] = new SelectList(_context.Hocphans, "MaHp", "MaHp", diemhp.MaHp);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", diemhp.MaSv);
            return View(diemhp);
        }

        // GET: Diemhps/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diemhp = await _context.Diemhps.FindAsync(id);
            if (diemhp == null)
            {
                return NotFound();
            }
            ViewData["MaHp"] = new SelectList(_context.Hocphans, "MaHp", "MaHp", diemhp.MaHp);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", diemhp.MaSv);
            return View(diemhp);
        }

        // POST: Diemhps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,MaHp,DiemHp")] Diemhp diemhp)
        {
            if (id != diemhp.MaSv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diemhp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiemhpExists(diemhp.MaSv))
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
            ViewData["MaHp"] = new SelectList(_context.Hocphans, "MaHp", "MaHp", diemhp.MaHp);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", diemhp.MaSv);
            return View(diemhp);
        }

        // GET: Diemhps/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Diemhps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var diemhp = await _context.Diemhps.FindAsync(id);
            if (diemhp != null)
            {
                _context.Diemhps.Remove(diemhp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiemhpExists(string id)
        {
            return _context.Diemhps.Any(e => e.MaSv == id);
        }
    }
}
