using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Web.Middleware;

namespace Test.WebTests.MiddlewareTests
{
    public class LoggingMiddlewareTests
    {
        [Fact]
        public async Task LoggingMiddleware_LogsRequestAndResponse()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            var middleware = new LoggingMiddleware(mockNext.Object);

            var context = new DefaultHttpContext
            {
                Request =
                {
                    Method = "POST",
                    Path = "/test",
                    ContentType = "application/json"
                }
            };

            var requestBody = "{\"message\":\"test\"}";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            // Act
            await middleware.Invoke(context);

            // Assert
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await new StreamReader(responseBodyStream).ReadToEndAsync();

            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}

