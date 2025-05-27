using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Farm.Models;
using mvcbasic.Data;

namespace ConsoleDI.Example.Controllers.Admin
{
    public class CagesController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public CagesController(MvcBasicDbContext context)
        {
            _context = context;
        }

        // GET: Cages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cage.ToListAsync());
        }

        // GET: Cages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cage = await _context.Cage
                .FirstOrDefaultAsync(m => m.id == id);
            if (cage == null)
            {
                return NotFound();
            }

            return View(cage);
        }

        // GET: Cages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,users_id,category_id,quantity,status,note,created_at,close_at")] Cage cage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cage);
        }

        // GET: Cages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cage = await _context.Cage.FindAsync(id);
            if (cage == null)
            {
                return NotFound();
            }
            return View(cage);
        }

        // POST: Cages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,users_id,category_id,quantity,status,note,created_at,close_at")] Cage cage)
        {
            if (id != cage.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CageExists(cage.id))
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
            return View(cage);
        }

        // GET: Cages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cage = await _context.Cage
                .FirstOrDefaultAsync(m => m.id == id);
            if (cage == null)
            {
                return NotFound();
            }

            return View(cage);
        }

        // POST: Cages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cage = await _context.Cage.FindAsync(id);
            if (cage != null)
            {
                _context.Cage.Remove(cage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CageExists(int id)
        {
            return _context.Cage.Any(e => e.id == id);
        }
    }
}
