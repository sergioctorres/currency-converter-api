using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Infrastructure.Security.Configurations;

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
        });
    }
}
