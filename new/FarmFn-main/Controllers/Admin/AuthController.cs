using Microsoft.AspNetCore.Mvc;
using Farm.Models;
using mvcbasic.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Farm.Controllers.Admin
{
    public class AuthController : Controller
    {
        public const string SessionKeyName = "_Name";
        private readonly MvcBasicDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(MvcBasicDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Đăng nhập GET
        [Route("/login", Name = "login")]
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
            var viewModel = new LoginViewModel();
            return View("~/Views/Admin/Login.cshtml", viewModel);
        }

        // Đăng nhập POST
        [HttpPost]
        [Route("/login", Name = "login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }

            var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == viewModel.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Account does not exist");
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, viewModel.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Incorrect password");
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }

            // Lưu thông tin vào session
            HttpContext.Session.SetString(SessionKeyName, user.Name);
            HttpContext.Session.SetInt32("Role", user.Role);

            // Thêm Role và NameIdentifier vào Claims   
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Thêm ID người dùng
        new Claim(ClaimTypes.Name, user.Name),
        new Claim("Role", user.Role.ToString())
    };
            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

            ViewBag.Name = user.Name;

            //if (user.Role == 1)
            //{
            //    return RedirectToAction("Index", "Admin");
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            // Phân quyền bằng if-else dựa trên giá trị Role
            if (user.Role == 1) // Admin
            {
                return RedirectToAction("Index", "Customers", new { area = "Admin" });
            }
            else if (user.Role == 2) // Employee
            {
                return RedirectToAction("Index", "Products", new { area = "Employee" });  
            }
            else if (user.Role == 0) // Customer
            {
                return RedirectToAction("Index", "News");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid role");
                HttpContext.Session.Clear();
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }
        }
        // Logout
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Auth");
        }

        // Đăng ký GET
        [HttpGet]
        [Route("/register", Name = "register")]
        public IActionResult Register()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
            var viewModel = new RegisterViewModel();
            return View("~/Views/Admin/Register.cshtml", viewModel);
        }

        // Đăng ký POST
        [HttpPost]
        [Route("/register", Name = "register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Register.cshtml", viewModel);
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == viewModel.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Register.cshtml", viewModel);
            }

            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == viewModel.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "The email already exists.");
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Register.cshtml", viewModel);
            }

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(new User(), viewModel.Password);

            var newUser = new User
            {
                Username = viewModel.Username,
                Password = hashedPassword,
                Name = viewModel.Name,
                PhoneNumber = viewModel.PhoneNumber,
                Email = viewModel.Email,
                Role = 0
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Auth");
        }
    }
}
