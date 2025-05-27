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
    public class ProductsController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public ProductsController(MvcBasicDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2)  
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            return View(await _context.Products.ToListAsync());  
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Image,Description,Category")] Product product)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Stock,Image,Description,Category")] Product product)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
