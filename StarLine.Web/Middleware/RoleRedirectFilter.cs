using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace StarLine.Web.Middleware
{
    public class RoleRedirectFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                // If current request is Home/Index, redirect based on role
                var controller = context.RouteData.Values["controller"]?.ToString();
                var action = context.RouteData.Values["action"]?.ToString();

                if (controller == "Home" && action == "Index")
                {
                    if (user.IsInRole("Admin") || user.IsInRole("Super-Admin") || user.IsInRole("HR-Manager"))
                    {
                        context.Result = new RedirectToActionResult("Index", "Admin", null);
                    }
                    else if (user.IsInRole("Employee") || user.IsInRole("Department-Head"))
                    {
                        context.Result = new RedirectToActionResult("Index", "Employee", null);
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Not needed
        }
    }
}
