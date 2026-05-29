using System.Net;
using System.Text.Json;
using VisitBookingSystem.Exceptions;

namespace VisitBookingSystem.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił nieoczekiwany błąd.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                message = exception.Message,
                type = exception.GetType().Name
            };

            context.Response.StatusCode = exception switch
            {
                BaseException be => be.StatusCode,
                _ => (int)HttpStatusCode.InternalServerError
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}