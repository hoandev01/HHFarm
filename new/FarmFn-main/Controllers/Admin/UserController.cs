using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Farm.Models;
using mvcbasic.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Farm.Controllers.Admin
{
    public class UserController : Controller
    {
        public const string SessionKeyName = "_Name";
        private readonly MvcBasicDbContext _context;

        public UserController(MvcBasicDbContext context)
        {
            _context = context;
        }

        [Route("/admin/user", Name = "admin.user")]
        public IActionResult List()
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Redirect("/login");
            }

            var userList = _context.Users.ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UserList", userList);
            }

            var model = new UserListViewModel
            {
                UserList = userList,
                Name = sessionName
            };
            return View("~/Views/Admin/User/List.cshtml", model);
        }

        [Route("/admin/user/add", Name = "admin.user.add")]
        public IActionResult Add()
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Redirect("/login");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AddUser", new User());
            }

            return View("~/Views/Admin/User/Add.cshtml", new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/user/insert", Name = "admin.user.insert")]
        public IActionResult Insert([Bind("Username, Name, Password, Role")] User user)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Redirect("/login");
            }

            if (user != null && ModelState.IsValid)
            {
                _context.Add(user);
                _context.SaveChanges();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "User added successfully!" });
                }

                return RedirectToAction("List", "User");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AddUser", user);
            }

            return View("~/Views/Admin/User/Add.cshtml", user);
        }

        [HttpGet]
        [Route("/admin/user/edit", Name = "admin.user.edit")]
        public IActionResult Edit(int? id)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Redirect("/login");
            }

            if (id == null)
            {
                return Content("not ok");
            }

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditUser", user);
            }

            ViewBag.id = id;
            ViewBag.name = user.Name ?? "";
            ViewBag.username = user.Username ?? "";
            ViewBag.passd = user.Password ?? "";
            ViewBag.role = user.Role;

            return View("~/Views/Admin/User/Edit.cshtml", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/user/update", Name = "admin.user.update")]
        public IActionResult Update([Bind("Id, Username, Name, Password, Role")] User user)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Redirect("/login");
            }

            if (user != null && ModelState.IsValid)
            {
                var data = _context.Users.Find(user.Id);
                if (data != null)
                {
                    data.Username = user.Username;
                    data.Name = user.Name;
                    data.Password = user.Password;
                    data.Role = user.Role;

                    _context.Entry(data).State = EntityState.Modified;
                    _context.SaveChanges();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "User updated successfully!" });
                    }

                    return RedirectToAction("List", "User");
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Cập nhật thất bại!" });
            }

            return Content("<script language='javascript' type='text/javascript'>alert('indefiend Message');</script>");
        }

        [HttpGet]
        [Route("/admin/user/delete", Name = "admin.user.delete")]
        public IActionResult Delete(int id)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Redirect("/login");
            }

            var data = _context.Users.Find(id);
            if (data != null)
            {
                _context.Users.Remove(data);
                _context.SaveChanges();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "User deleted successfully!" });
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Delete failed!" });
            }

            return RedirectToAction("List", "User");
        }

        [HttpPost]
        [Route("/admin/user/search", Name = "admin.user.search")]
        public JsonResult Search(string search)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            ViewBag.nameSession = sessionName;

            if (string.IsNullOrEmpty(ViewBag.nameSession))
            {
                return Json(new { success = false, message = "Please log in!" });
            }

            string query = "SELECT * FROM Users WHERE Username LIKE @search OR Name LIKE @search";
            var lst = _context.Users.FromSqlRaw(query, new SqlParameter("@search", $"%{search}%")).ToList();
            string[] arr_role = { "", "Quản trị viên", "Nhân viên" };

            if (lst != null)
            {
                return Json(new { success = true, arr_user = lst, role = arr_role });
            }
            return Json(new { success = false, message = "No results found!" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/admin/user/profile", Name = "admin.user.profile")]
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User information not found.");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Profile", user);
            }

            return View("~/Views/Admin/User/Profile.cshtml", user);
        }

        [Route("/admin/user/editprofile", Name = "admin.user.editprofile")]
        public IActionResult EditProfile()
        {
            var username = HttpContext.Session.GetString(SessionKeyName);
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditProfile", user);
            }

            return View("~/Views/Admin/User/EditProfile.cshtml", user);
        }

        [HttpPost]
        [Route("/admin/user/editprofile", Name = "admin.user.editprofile.post")]
        public IActionResult EditProfile(User model)
        {
            var sessionUsername = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrEmpty(sessionUsername))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                if (model.Username != sessionUsername)
                {
                    return BadRequest("You do not have the authority to edit another user's information.");
                }

                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    _context.Update(user);
                    _context.SaveChanges();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "The information has been updated successfully!"});
                        }

                    TempData["Success"] = "The information has been updated successfully!";
                    return RedirectToAction("Profile");
                }
                return NotFound("User information not found.");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditProfile", model);
            }

            return View("~/Views/Admin/User/EditProfile.cshtml", model);
        }
    }
}