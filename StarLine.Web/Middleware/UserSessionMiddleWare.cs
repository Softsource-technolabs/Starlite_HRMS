using StarLine.Core.Common;
using StarLine.Core.Session;
using System.IO;

namespace StarLine.Web.Middleware
{
    public class UserSessionMiddleWare
    {
        private RequestDelegate _next;
        public UserSessionMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserSession userSession)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var claims = context.User.Claims;
                var path = context.Request.Path.Value;
                userSession.Current = new UserSessionModel
                {
                    AspNetId = claims.FirstOrDefault(c => c.Type == "AspNetUser")?.Value ?? "",
                    FirstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "",
                    LastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? "",
                    EmailAddress = claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? "",
                    RoleName = claims.FirstOrDefault(c => c.Type == "RoleName")?.Value ?? "",
                    UserId = int.TryParse(claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId) ? userId : 0
                };

                if (path == "/" || path.Equals("/Home/Index", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.User.IsInRole("Admin") || context.User.IsInRole("Super-Admin") || context.User.IsInRole("HR-Manager"))
                    {
                        context.Response.Redirect("/Admin/Index");
                        return;
                    }
                    else if (context.User.IsInRole("Employee") || context.User.IsInRole("Department-Head"))
                    {
                        context.Response.Redirect("/Employee/Index");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
