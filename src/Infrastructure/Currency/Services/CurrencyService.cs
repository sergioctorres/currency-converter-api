using Application.Constants;
using Application.Dtos.Common;
using Application.Dtos.CurrencyRate;
using Application.Interfaces;
using Infrastructure.Currency.Configurations;
using Microsoft.Extensions.Options;

namespace Infrastructure.Currency.Services;

public class CurrencyService(ICurrencyRateProviderFactory factory, IOptions<CurrencyProviderConfiguration> options) : ICurrencyRateProvider
{
    private readonly ICurrencyRateProvider _currencyProvider = factory.Create(options.Value.Name);

    public async Task<LatestResult?> GetLatestCurrencyRatesAsync(LatestRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _currencyProvider.GetLatestCurrencyRatesAsync(request, cancellationToken);

        if (response is null) return null;

        var filteredRates = response.Rates
            .Where(r => CurrencyRateRulesConstants.BlockedCurrencies.Contains(r.Key) is false)
            .ToDictionary(r => r.Key, r => r.Value);

        return response with { Rates = filteredRates };
    }

    public Task<ConvertResult?> ConvertCurrencyRatesAsync(ConvertRequest request, CancellationToken cancellationToken = default) =>
        _currencyProvider.ConvertCurrencyRatesAsync(request, cancellationToken);

    public async Task<PagedResult<HistoricalResult>?> GetHistoricalCurrencyRatesAsync(HistoricalRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _currencyProvider.GetHistoricalCurrencyRatesAsync(request, cancellationToken);

        if (response is null) return null;

        var filteredRecords = response.Records?
            .Select(r => new HistoricalResult(
                r.Date,
                r.Values
                    .Where(v => CurrencyRateRulesConstants.BlockedCurrencies.Contains(v.Key) is false)
                    .ToDictionary(v => v.Key, v => v.Value)
            ))
            .ToList();

        return new PagedResult<HistoricalResult>(filteredRecords!, response.TotalRecords);
    }
}
