using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farm.Models;  
using mvcbasic.Data;
using Microsoft.AspNetCore.Identity;

namespace Farm.Controllers.Admin
{
    public class EmployeesController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public EmployeesController(MvcBasicDbContext context)
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
            return View(await _context.Employees.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Name,Position,PhoneNumber,Username,Password")] Employee employee)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<Employee>();
                employee.Password = passwordHasher.HashPassword(employee, employee.Password); // Mã hóa mật khẩu
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null) return NotFound();
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Position,PhoneNumber,Username,Password")] Employee employee)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id != employee.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees.FindAsync(id);
                    if (existingEmployee != null)
                    {
                        existingEmployee.Name = employee.Name;
                        existingEmployee.Position = employee.Position;
                        existingEmployee.PhoneNumber = employee.PhoneNumber;
                        existingEmployee.Username = employee.Username;
                        if (!string.IsNullOrEmpty(employee.Password))
                        {
                            var passwordHasher = new PasswordHasher<Employee>();
                            existingEmployee.Password = passwordHasher.HashPassword(existingEmployee, employee.Password);
                        }
                        _context.Update(existingEmployee);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1) // Chỉ Admin (Role = 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null) return NotFound();
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1) // Chỉ Admin (Role = 1)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (id == null) return NotFound();
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
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
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



    }
}