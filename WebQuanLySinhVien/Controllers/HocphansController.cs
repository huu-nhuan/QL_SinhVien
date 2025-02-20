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
            ViewData["MaNganh"] = new SelectList(_context.Nganhs, "MaNganh", "MaNganh");
            return View();
        }

        // POST: Hocphans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHp,TenHp,SoTc,MaNganh,HocKy")] Hocphan hocphan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocphan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNganh"] = new SelectList(_context.Nganhs, "MaNganh", "MaNganh", hocphan.MaNganh);
            return View(hocphan);
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
            ViewData["MaNganh"] = new SelectList(_context.Nganhs, "MaNganh", "MaNganh", hocphan.MaNganh);
            return View(hocphan);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNganh"] = new SelectList(_context.Nganhs, "MaNganh", "MaNganh", hocphan.MaNganh);
            return View(hocphan);
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

            return View(hocphan);
        }

        // POST: Hocphans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hocphan = await _context.Hocphans.FindAsync(id);
            if (hocphan != null)
            {
                _context.Hocphans.Remove(hocphan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocphanExists(string id)
        {
            return _context.Hocphans.Any(e => e.MaHp == id);
        }
    }
}
