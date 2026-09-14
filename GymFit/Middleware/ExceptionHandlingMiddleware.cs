using System.Net;
using GymFit.Web.Services;

namespace GymFit.Web.Middleware
{

    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                // Log to custom logging service
                var additionalInfo = $"URL: {context.Request.Path}, Method: {context.Request.Method}, " +
                                   $"User: {context.User?.Identity?.Name ?? "Anonymous"}";

                await loggingService.LogErrorAsync(ex, additionalInfo);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (context.Request.Method != HttpMethods.Get ||
                context.Request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new
                {
                    statusCode = context.Response.StatusCode,
                    message = "An error occurred processing your request."
                });
            }

            context.Response.Redirect("/Home/Error");
            return Task.CompletedTask;
        }
    }
}
