using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
            var (status, title, detail) = ex switch
            {
                HttpRequestException => (StatusCodes.Status502BadGateway, "External API error", ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problemDetails);
		}
    }
}
