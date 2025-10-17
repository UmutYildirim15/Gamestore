using System.Net;
using FluentValidation;
using Web.Utilities;

namespace Web.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Request.EnableBuffering();

            HttpStatusCode statusCode;
            string message;

            switch (ex)
            {
                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Validation error. Please check your input.";
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                Message = $"{message} (Error Code: {context.Response.StatusCode})"
            };

            LogExceptionDetails(ex);

            var responseJson = System.Text.Json.JsonSerializer.Serialize(errorResponse);

            var originalBodyStream = context.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;
                await context.Response.WriteAsync(responseJson);
        
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }


        private static void LogExceptionDetails(Exception ex)
        {
            var exceptionDetails = $"Exception Type: {ex.GetType().Name}\n" +
                                   $"Exception Message: {ex.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}\n" +
                                   (ex.InnerException != null ? $"Inner Exception: {ex.InnerException.Message}" : string.Empty);

            LogHelper.WriteExceptionLog(exceptionDetails);
        }
    }
}
