using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Common.Exceptions;
using CustomValidationException = GovOperationsHub.Application.Common.Exceptions.ValidationException;
using System.Net;
using System.Text.Json;

namespace GovOperationsHub.WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            var response = context.Response;
            response.ContentType = "application/json; charset=utf-8";

            Result<object> result;

            switch (exception)
            {
                case CustomValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var allErrors = validationEx.Errors.SelectMany(e => e.Value).ToList();
                    result = Result<object>.Failure(allErrors, "فشل التحقق من صحة البيانات المدخلة، يرجى مراجعة الحقول.");
                    break;

                case KeyNotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    result = Result<object>.Failure(notFoundEx.Message, "العنصر المطلوب غير موجود.");
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    result = Result<object>.Failure("غير مصرح لك بالقيام بهذا الإجراء.", "خطأ في الصلاحيات.");
                    break;

                default:
                    _logger.LogError(exception, "خطأ غير معالج أثناء معالجة الطلب: {Message}", exception.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    result = Result<object>.Failure("حدث خطأ تقني داخلي في الخادم. يرجى مراجعة الدعم الفني.", "خطأ غير متوقع.");
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            await response.WriteAsync(JsonSerializer.Serialize(result, jsonOptions));
        }
    }
}
