using Infrastructure.Currency.Configurations;
using Infrastructure.Security.Configurations;

namespace WebApi.Configuration;

public static class ConfigureOptions
{
    public static void AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenConfiguration>(configuration.GetSection("TokenConfiguration"));
        services.Configure<CurrencyProviderConfiguration>(configuration.GetSection("CurrencyProviderConfiguration"));
    }
}
