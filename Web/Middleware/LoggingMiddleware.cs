using System.Text;
using Web.Utilities;

namespace Web.Middleware
{
    public class LoggingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task Invoke(HttpContext context)
        {
            
            var startTime = DateTime.UtcNow;
            var request = context.Request;

            var ipAddress = context.Connection.RemoteIpAddress?.ToString() == "::1" ? "127.0.0.1" : context.Connection.RemoteIpAddress?.ToString();

            var targetUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            var requestContent = await ReadRequestBodyAsync(request);

            var requestLog = $"IP: {ipAddress}, Method: {request.Method}, URL: {targetUrl}, Content: {requestContent}";
            LogHelper.WriteRequestLog(requestLog);

            var originalResponseBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var elapsedTime = DateTime.UtcNow - startTime;
            var responseLog = $"StatusCode: {context.Response.StatusCode}, Response: {responseText}, ElapsedTime: {elapsedTime.TotalMilliseconds} ms";

            LogHelper.WriteRequestLog(responseLog);
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Position = 0;
            var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return string.IsNullOrWhiteSpace(content) ? "No Content" : content;
        }
    }
}
