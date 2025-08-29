namespace StarLine.Web.Middleware
{
    public class ErrorHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandler> _logger;

        public ErrorHandler(RequestDelegate next, ILogger<ErrorHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                // Store error message in TempData
                if (context.RequestServices.GetService(typeof(IHttpContextAccessor)) is IHttpContextAccessor accessor)
                {
                    accessor.HttpContext.Items["ErrorMessage"] = "An unexpected error occurred!";
                }

                context.Response.Redirect(context.Request.Path); // Reload same page
            }
        }
    }
}
