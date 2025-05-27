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
    public class NewsController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public NewsController(MvcBasicDbContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Customer (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            return View(await _context.News.ToListAsync());
        }

        // GET: News/Details/5
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

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,image,title,subtit,content,created_at")] News news)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: News/Edit/5
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

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,image,title,subtit,content,created_at")] News news)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id != news.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.id))
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
            return View(news);
        }

        // GET: News/Delete/5
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

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.id == id);
        }
    }
}
