using Application.Dtos.Common;
using Application.Dtos.CurrencyRate;

namespace Application.Interfaces;

public interface ICurrencyRateProvider
{
    Task<LatestResult?> GetLatestCurrencyRatesAsync(LatestRequest request, CancellationToken cancellationToken = default);
    Task<ConvertResult?> ConvertCurrencyRatesAsync(ConvertRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<HistoricalResult>?> GetHistoricalCurrencyRatesAsync(HistoricalRequest request, CancellationToken cancellationToken = default);
}
