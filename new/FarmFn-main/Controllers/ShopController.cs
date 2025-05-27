using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farm.Models;
using mvcbasic.Data;

namespace Farm.Controllers
{
    public class ShopController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public ShopController(MvcBasicDbContext context)
        {
            _context = context;
        }

        [Route("/shop", Name = "shop.index")]
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [Route("/shop/{id}", Name = "shop.detail")]
        public IActionResult Detail(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}