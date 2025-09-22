using StarLine.Core.Common;
using StarLine.Core.Session;
using System.Security.Claims;
using System.IO; // Add this for file operations

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
            logger.LogInformation("Path: {Path}", context.Request.Path.Value);
            System.IO.File.AppendAllText("UserSessionMiddleware.log", $"[{DateTime.Now}] Path: {context.Request.Path.Value}\n");

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
                    UserId = int.TryParse(claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId) ? userId : 0,
                    UserImage = claims.FirstOrDefault(c => c.Type == "Avtar")?.Value ?? "/UserAvtars/default.png"
                };

                logger.LogInformation("Authenticated user {Email} with roles: {Roles}", userSession.Current.EmailAddress, string.Join(", ", claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));
                System.IO.File.AppendAllText("UserSessionMiddleware.log", $"[{DateTime.Now}] Authenticated user {userSession.Current.EmailAddress} with roles: {string.Join(", ", claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}\n");

                // Only redirect on root or Home/Index to avoid loops
                var path = context.Request.Path.Value;
                if (path == "/" || path.Equals("/Home/Index", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.User.IsInRole("Admin") || context.User.IsInRole("Super-Admin") || context.User.IsInRole("HR-Manager"))
                    {
                        context.Response.Redirect("/Admin/Home/Index");
                        return;
                    }
                    else if (context.User.IsInRole("Employee") || context.User.IsInRole("Department-Head"))
                    {
                        context.Response.Redirect("/Home/Index");
                        return;
                    }
                    else // 👈 Add this else block for debugging
                    {
                        context.Response.Redirect("/Account/Login"); // Force redirect to login for unhandled authenticated users
                        return;
                    }
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
