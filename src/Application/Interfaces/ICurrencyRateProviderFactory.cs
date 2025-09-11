namespace Application.Interfaces;

public interface ICurrencyRateProviderFactory
{
    ICurrencyRateProvider Create(string providerName);
}
