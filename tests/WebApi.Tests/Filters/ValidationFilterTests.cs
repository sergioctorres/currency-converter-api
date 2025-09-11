using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Filters;
using Microsoft.AspNetCore.Routing;
using Application.Dtos.CurrencyRate;
using Application.Validators.Currency;

namespace WebApi.Tests.Filters;

public class ValidationFilterTests
{
    private static ActionExecutingContext CreateActionExecutingContext(object arg)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor()
        );

        var actionArguments = new Dictionary<string, object?>
        {
            { "request", arg }
        };

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            actionArguments,
            controller: null!
        );
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithValidRequest_AllowsNext()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddScoped<IValidator<LatestRequest>, LatestRequestValidator>()
            .BuildServiceProvider();

        var filter = new ValidationFilter(services);
        var context = CreateActionExecutingContext(new LatestRequest("BRL"));

        var nextCalled = false;
        ActionExecutionDelegate next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null!));
        };

        // Act
        await filter.OnActionExecutionAsync(context, next);

        // Assert
        Assert.Null(context.Result);
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddScoped<IValidator<LatestRequest>, LatestRequestValidator>()
            .BuildServiceProvider();

        var filter = new ValidationFilter(services);
        var context = CreateActionExecutingContext(new LatestRequest(string.Empty));

        Task<ActionExecutedContext> next() =>
            Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null!));

        // Act
        await filter.OnActionExecutionAsync(context, next);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(context.Result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.Contains("Base currency is required.", problemDetails.Errors["Base"]);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithNoValidator_ContinuesExecution()
    {
        // Arrange
        var services = new ServiceCollection().BuildServiceProvider();

        var filter = new ValidationFilter(services);
        var context = CreateActionExecutingContext(new LatestRequest("EUR"));

        var nextCalled = false;
        Task<ActionExecutedContext> next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null!));
        }

        // Act
        await filter.OnActionExecutionAsync(context, next);

        // Assert
        Assert.Null(context.Result);
        Assert.True(nextCalled);
    }
}
