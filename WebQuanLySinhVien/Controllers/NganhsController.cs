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
    public class NganhsController : Controller
    {
        private readonly QuanLySinhVienContext _context;

        public NganhsController(QuanLySinhVienContext context)
        {
            _context = context;
        }

        // GET: Nganhs
        public async Task<IActionResult> Index()
        {
            var quanLySinhVienContext = _context.Nganhs.Include(n => n.MaKhoaNavigation);
            return View(await quanLySinhVienContext.ToListAsync());
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
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa");
            return View();
        }

        // POST: Nganhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNganh,TenNganh,MaKhoa")] Nganh nganh)
        {
            if (ModelState.IsValid)
            {

                _context.Add(nganh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            string loi = "";
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log or inspect the error
                    loi = loi + error.ErrorMessage + "\n" + nganh.MaKhoaNavigation;
                }
            }
            return BadRequest(loi); 
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
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", nganh.MaKhoa);
            return View(nganh);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", nganh.MaKhoa);
            return View(nganh);
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

            return View(nganh);
        }

        // POST: Nganhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nganh = await _context.Nganhs.FindAsync(id);
            if (nganh != null)
            {
                _context.Nganhs.Remove(nganh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NganhExists(string id)
        {
            return _context.Nganhs.Any(e => e.MaNganh == id);
        }
    }
}
