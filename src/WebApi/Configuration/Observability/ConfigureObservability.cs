using HealthChecks.UI.Client;
using Infrastructure.Currency.Configurations;
using Infrastructure.Security.Configurations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace WebApi.Configuration.Observability;

public static class ConfigureObservability
{
    public static void AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation()
                       .AddConsoleExporter();
            });

        var currencyApiProvider = configuration.GetSection("CurrencyProviderConfiguration").Get<CurrencyProviderConfiguration>()!;

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddUrlGroup(
                serviceProvider =>
                {
                    var currencyApiProvider = serviceProvider.GetRequiredService<IOptions<CurrencyProviderConfiguration>>().Value;
                    return new Uri($"{currencyApiProvider.Url}/{currencyApiProvider.HealthCheck.Path}");
                },
                name: "Currency API"
            );
    }

    public static void UseObservability(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}
