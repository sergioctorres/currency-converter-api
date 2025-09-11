using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApi.Middlewares;

namespace WebApi.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithHttpRequestException_ReturnsBadGatewayProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        static Task next(HttpContext _) => throw new HttpRequestException("API unreachable");
        var middleware = new ExceptionMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(body);

        Assert.Equal(StatusCodes.Status502BadGateway, context.Response.StatusCode);
        Assert.Equal("External API error", problem!.Title);
        Assert.Equal("API unreachable", problem.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_ReturnsInternalServerErrorProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        static Task next(HttpContext _) => throw new Exception("Something went wrong");
        var middleware = new ExceptionMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(body);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("Internal Server Error", problem!.Title);
        Assert.Equal("An unexpected error occurred.", problem.Detail);
    }
}
