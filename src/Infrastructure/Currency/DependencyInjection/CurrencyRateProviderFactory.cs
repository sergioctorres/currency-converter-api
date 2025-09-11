using Application.Interfaces;
using Infrastructure.Currency.Constants;
using Infrastructure.Currency.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Currency.DependencyInjection;

public class CurrencyRateProviderFactory(IServiceProvider services) : ICurrencyRateProviderFactory
{
    public ICurrencyRateProvider Create(string providerName)
    {
        return providerName switch
        {
            CurrencyRateConstants.Providers.FrankfurterApi => services.GetRequiredService<FrankfurterCurrencyProvider>(),
            _ => throw new NotSupportedException($"Provider {providerName} not supported")
        };
    }
}
