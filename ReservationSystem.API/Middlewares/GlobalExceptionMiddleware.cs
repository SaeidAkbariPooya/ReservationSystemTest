using FluentValidation;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            object response;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new
                    {
                        StatusCode = (int)statusCode,
                        Message = "خطای اعتبارسنجی ورودی",
                        Errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    };
                    break;

                case ApplicationException appEx:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new
                    {
                        StatusCode = (int)statusCode,
                        Message = appEx.Message
                    };
                    break;

                case KeyNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    response = new
                    {
                        StatusCode = (int)statusCode,
                        Message = exception.Message
                    };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    _logger.LogError(exception, "خطای پیش‌بینی نشده");
                    response = new
                    {
                        StatusCode = (int)statusCode,
                        Message = "خطای داخلی سرور. لطفاً با پشتیبانی تماس بگیرید."
                    };
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
