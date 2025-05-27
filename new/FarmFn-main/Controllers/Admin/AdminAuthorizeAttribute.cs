using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Farm.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kiểm tra nếu chưa đăng nhập
            var sessionKeyName = context.HttpContext.Session.GetString("SessionKeyName");
            if (string.IsNullOrEmpty(sessionKeyName))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Kiểm tra nếu không phải admin (Role != 1)
            var role = context.HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}