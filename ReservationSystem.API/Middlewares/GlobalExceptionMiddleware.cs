using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ReservationSystem.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                ValidationException _ => (HttpStatusCode.BadRequest, "خطای اعتبارسنجی ورودی"),
                ApplicationException appEx => (HttpStatusCode.BadRequest, appEx.Message),
                KeyNotFoundException _ => (HttpStatusCode.NotFound, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "خطای داخلی سرور. لطفاً با پشتیبانی تماس بگیرید.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "خطای پیش‌بینی نشده");

            context.Response.StatusCode = (int)statusCode;
            var response = new
            {
                StatusCode = (int)statusCode,
                Message = message,
                Detail = exception is ValidationException ve ? ve.Data : null
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
