using Farm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcbasic.Data;

namespace Farm.Controllers.Admin
{
    public class CustomersController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public CustomersController(MvcBasicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1) // Chỉ Admin (Role = 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            return View(await _context.Users.Where(u => u.Role == 0).ToListAsync());
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Username,Password,PhoneNumber,Email")] User user)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password); // Mã hóa mật khẩu
                user.Role = 0; // Đặt Role là Customer
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role != 0) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Username,Password,PhoneNumber,Email")] User user)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id != user.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);           
                    if (existingUser != null && existingUser.Role == 0)
                    {
                        existingUser.Name = user.Name;
                        existingUser.PhoneNumber = user.PhoneNumber;
                        existingUser.Email = user.Email;
                        existingUser.Username = user.Username;
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            var passwordHasher = new PasswordHasher<User>();
                            existingUser.Password = passwordHasher.HashPassword(existingUser, user.Password);
                        }
                        _context.Update(existingUser);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1) // Chỉ Admin (Role = 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null) return NotFound();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == 0);
            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1) // Chỉ Admin (Role = 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null) return NotFound();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == 0);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null && user.Role == 0)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}