//using System.Diagnostics;
//using Microsoft.AspNetCore.Mvc;
//using Farm.Models;
//using mvcbasic.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Data.SqlClient;
//using System.Data;

//namespace Farm.Controllers.Admin;
//public class HomeController : Controller
//{
//    public const string SessionKeyName = "_Name";
//    private readonly MvcBasicDbContext _context;

//    public HomeController(MvcBasicDbContext context){
//        _context = context;
//    }

//    [Route("/admin", Name="admin")]
//    public IActionResult Index()
//    {          
//        var sessionName = HttpContext.Session.GetString(SessionKeyName);
//        ViewBag.nameSession = sessionName;
//        return View("~/Views/Admin/Index.cshtml");
//    }


//}


using Farm.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Controllers
{
    [AdminAuthorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Name = HttpContext.Session.GetString("SessionKeyName");
            return View();
        }

        public IActionResult Introduce()
        {
            ViewBag.Name = HttpContext.Session.GetString("SessionKeyName");
            return View();
        }

        public IActionResult News()
        {
            ViewBag.Name = HttpContext.Session.GetString("SessionKeyName");
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Name = HttpContext.Session.GetString("SessionKeyName");
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewBag.Name = HttpContext.Session.GetString("SessionKeyName");
            return View();
        }
    }
}