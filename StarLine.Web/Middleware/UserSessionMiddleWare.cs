using StarLine.Core.Common;
using StarLine.Core.Session;
using System.Security.Claims;

namespace StarLine.Web.Middleware
{
    public class UserSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserSession userSession, ILogger<UserSessionMiddleware> logger)
        {
            // Only populate session if user is authenticated
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var claims = context.User.Claims;

                // Populate UserSessionModel
                userSession.Current = new UserSessionModel
                {
                    AspNetId = claims.FirstOrDefault(c => c.Type == "AspNetUser")?.Value ?? "",
                    FirstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "",
                    LastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? "",
                    EmailAddress = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "",
                    RoleName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "",
                    ReportingManager = Convert.ToInt64(claims.FirstOrDefault(c => c.Type == "ReportingManager").Value),
                    UserId = int.TryParse(claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId) ? userId : 0,
                    UserImage = claims.FirstOrDefault(c => c.Type == "Avtar")?.Value ?? "/UserAvtars/default.png",
                };

                // Only redirect on root or Home/Index to avoid loops
                var path = context.Request.Path.Value;
                if (path == "/")
                {
                    if ((context.User.IsInRole("Admin") || context.User.IsInRole("Super-Admin") || context.User.IsInRole("HR-Manager")) && !path.Equals("/Admin/Home/Index", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Redirect("/Admin/Home/Index");
                        return;
                    }
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
