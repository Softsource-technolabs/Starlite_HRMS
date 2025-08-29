using StarLine.Core.Common;
using StarLine.Core.Session;

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

                userSession.Current = new UserSessionModel
                {
                    AspNetId = claims.FirstOrDefault(c => c.Type == "AspNetUser")?.Value ?? "",
                    FirstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "",
                    LastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? "",
                    EmailAddress = claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? "",
                    RoleName = claims.FirstOrDefault(c => c.Type == "RoleName")?.Value ?? "",
                    UserId = int.TryParse(claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId) ? userId : 0
                };

                if (context.User.Identity.IsAuthenticated && context.Request.Path == "/")
                {
                    var roleName = context.User.FindFirst("RoleName")?.Value;

                    if (userSession.Current.RoleName.ToLower() == "admin" || userSession.Current.RoleName.ToLower() == "super admin")
                    {
                        context.Response.Redirect("/admin/home");
                        return;
                    }
                    context.Response.Redirect("/Home/Index");
                    return;
                }
            }
            await _next(context);
        }
    }
}
