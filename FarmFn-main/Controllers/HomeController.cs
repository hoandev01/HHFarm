using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Farm.Models;
using mvcbasic.Data;
using System.Linq; 

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MvcBasicDbContext _context;

    public HomeController(ILogger<HomeController> logger, MvcBasicDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Route("/", Name="default")]
    public IActionResult Index()
    {
        var model = new HomeViewModel
        {
            NewsList = _context.News.OrderByDescending(n => n.created_at).ToList()
        };
        return View(model);
    }

    [Route("/privacy", Name="privacy")]
    public IActionResult Privacy()
    {
        return View("~/Views/Home/Privacy.cshtml");
    }

    [Route("/news", Name = "news")]
    public IActionResult News()
    {
        var listNews = _context.News.ToList();
        return View("~/Views/Home/News.cshtml", listNews);  
    }

    [Route("/introduce", Name="introduce")]
    public IActionResult Introduce()
    {
        return View("~/Views/Home/Introduce.cshtml");
    }

    [Route("/contact", Name="contact")]
    public IActionResult Contact()
    {
        return View("~/Views/Home/Contact.cshtml");
    }

    [HttpGet]
    [Route("/blog")]
    public IActionResult NewsDetail(int? id)
    {
        if (!id.HasValue)
        {
            return NotFound(); // Trả về 404 nếu id không hợp lệ
        }

        var news = _context.News.FirstOrDefault(x => x.id == id);
        if (news == null)
        {
            return NotFound(); // Trả về 404 nếu không tìm thấy bài viết
        }

        var model = new NewsDetailViewModel
        {
            News = news,
            RelatedNews = _context.News.Where(x => x.id != id).Take(3).ToList() // Lấy 3 bài viết liên quan
        };

        return View("~/Views/Home/Detail/News.cshtml", model);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
