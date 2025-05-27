using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Admin Management";
            return View();
        }
    }
}