using Farm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcbasic.Data;
using System.Security.Claims;

namespace Farm.Controllers
{
    public class CartController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public CartController(MvcBasicDbContext context)
        {
            _context = context;
        }

        [Route("/cart", Name = "cart.index")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please log in to view the cart.";
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Unable to identify the user. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, redirectToLogin = true });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, redirectToLogin = true });
            }

            var product = _context.Products.Find(productId);
            if (product == null || product.Stock < quantity)
            {
                return Json(new { success = false, message = "The product does not exist or is out of stock." });
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = userId.Value,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Product added to cart." });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cartItem = _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefault(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Item does not exist in the cart." });
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else if (quantity > cartItem.Product.Stock)
            {
                return Json(new { success = false, message = "Quantity exceeds available stock." });
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Item does not exist in the cart." });
            }

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private int? GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }
    }
}