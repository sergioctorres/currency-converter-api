using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Infrastructure.Security.Configurations;
using Infrastructure.Currency.Configurations;

namespace WebApi.Tests.Builder;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(new TokenConfiguration
            {
                SecretKey = "SecretKeyForTest1234567890!1234567890!",
                Issuer = "WebApi.Tests",
                Audience = "Clients",
                ExpirationInMinutes = 45
            });

            services.AddSingleton(new CurrencyProviderConfiguration
            {
                Url = "https://api.frankfurter.app",
                HealthCheck = new HealthCheckSettings
                {
                    Name = "Frankfurter API",
                    Path = "latest"
                }
            });
        });
    }
}
