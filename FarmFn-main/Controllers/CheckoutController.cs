using Farm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcbasic.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Farm.Controllers
{
    public class CheckoutRequest
    {
        public string PaymentMethod { get; set; }
    }

    public class CheckoutController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public CheckoutController(MvcBasicDbContext context)
        {
            _context = context;
        }

        [Route("/checkout", Name = "checkout.index")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please log in to proceed with payment.";
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

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add products before checking out.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }

        [HttpPost]
        [Route("/checkout/process", Name = "checkout.process")]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Please log in to proceed with payment." });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Unable to identify the user. Please log in again." });
            }

            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty." });
            }

            if (request.PaymentMethod == "cash")
            {
                // Tạo đơn hàng mới
                var order = new Order
                {
                    UserId = userId.Value,
                    OrderDate = DateTime.Now,
                    PaymentMethod = "Cash on delivery.",
                    Status = "Pending.",
                    TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
                    OrderDetails = cartItems.Select(item => new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Price = item.Product.Price,
                        Quantity = item.Quantity
                    }).ToList()
                };

                // Lưu đơn hàng vào cơ sở dữ liệu
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                // Lưu OrderId vào TempData để sử dụng trong action Success
                TempData["OrderId"] = order.Id;

                return Json(new { success = true, redirectUrl = Url.Action("Success", "Checkout") });
            }
            else if (request.PaymentMethod == "bank" || request.PaymentMethod == "momo")
            {
                string qrImageUrl = request.PaymentMethod == "bank"
                    //? "https://via.placeholder.com/200x200.png?text=Bank+QR+Code"
                    //: "https://via.placeholder.com/200x200.png?text=MoMo+QR+Code";
                    ? "/image/bank-qr.png"  
                    : "/image/momo-qr.png";
                return Json(new { success = true, showQR = true, qrImageUrl = qrImageUrl });
            }

            return Json(new { success = false, message = "Invalid payment method." });
        }

        [Route("/success", Name = "checkout.success")]
        public IActionResult Success()
        {
            // Lấy OrderId từ TempData
            if (!TempData.ContainsKey("OrderId"))
            {
                return RedirectToAction("Index", "Home"); // Hoặc xử lý lỗi
            }

            int orderId = (int)TempData["OrderId"];
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return RedirectToAction("Index", "Home"); // Hoặc xử lý lỗi
            }

            return View(order);
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