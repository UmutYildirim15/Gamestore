using System.Net;
using Microsoft.AspNetCore.Http;
using Moq;
using Web.Middleware;

namespace Test.WebTests.MiddlewareTests
{
    public class ExceptionMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly ExceptionMiddleware _middleware;

        public ExceptionMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _middleware = new ExceptionMiddleware(_nextMock.Object);
        }

        [Fact]
        public async Task Invoke_Should_Return_InternalServerError_When_Exception_Thrown()
        {
            // Arrange
            var exceptionMessage = "An error occurred.";
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new Exception(exceptionMessage));

            var context = new DefaultHttpContext();

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task Invoke_Should_Not_Throw_If_Response_Already_Started()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Response.OnStarting(() => Task.CompletedTask);
            context.Response.ContentType = "application/json";

            var middleware = new ExceptionMiddleware(_ => Task.CompletedTask);

            // Act & Assert
            await middleware.Invoke(context);
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task HandleExceptionAsync_Should_Write_Error_Message_To_Response()
        {
            // Arrange
            var exceptionMessage = "Test exception";
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var middleware = new ExceptionMiddleware(_ => throw new Exception(exceptionMessage));

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.Contains("ErrorMessage", responseBody);
            Assert.Contains(exceptionMessage, responseBody);
        }
    }

    public static class HttpResponseExtensions
    {
        public static async Task<string> ReadBodyAsync(this HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(response.Body);
            var body = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
