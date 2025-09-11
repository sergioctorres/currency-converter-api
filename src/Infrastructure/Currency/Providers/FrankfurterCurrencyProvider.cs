using Application.Common.Extensions;
using Application.Common.Helpers;
using Application.Dtos.Common;
using Application.Dtos.CurrencyRate;
using Application.Interfaces;
using Infrastructure.Currency.Constants;
using Infrastructure.Http;

namespace Infrastructure.Currency.Providers;

public class FrankfurterCurrencyProvider(IHttpClientFactory httpClientFactory) : HttpClientBase(httpClientFactory), ICurrencyRateProvider
{
    protected override string GetClientName() => CurrencyRateConstants.ClientNames.Api;

    public Task<LatestResult?> GetLatestCurrencyRatesAsync(LatestRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<LatestResult>(
            HttpMethod.Get,
            $"/{CurrencyRateConstants.Providers.FrankfurterPaths.Latest}{request.ToEndpoint()}",
            cancellationToken: cancellationToken);
    }

    public async Task<ConvertResult?> ConvertCurrencyRatesAsync(ConvertRequest request, CancellationToken cancellationToken = default)
    {
        var latestRequest = new LatestRequest(request.Base, [request.Target]);

        var latestResponse = await GetLatestCurrencyRatesAsync(latestRequest, cancellationToken);

        if (latestResponse is null) return null;

        var currencyRate = latestResponse.Rates[request.Target.ToUpper()];

        return new ConvertResult(request.Base.ToUpper(), request.Target.ToUpper(), request.Amount, request.Amount * currencyRate, currencyRate, latestResponse.Date);
    }

    public async Task<PagedResult<HistoricalResult>?> GetHistoricalCurrencyRatesAsync(HistoricalRequest request, CancellationToken cancellationToken = default)
    {
        var datePageRange = DateRangeHelper.GetWeekdayPageRange(request.StartDate, request.EndDate, request.Page, request.PageSize);

        if (datePageRange is null) return null;

        if (datePageRange is not null)
        {
            request = request with
            {
                StartDate = datePageRange.Value.StartDate,
                EndDate = datePageRange.Value.EndDate
            };
        }

        var response = await SendAsync<HistoricalRawResult>(
            HttpMethod.Get,
            request.ToEndpoint(),
            cancellationToken: cancellationToken);

        if (response?.Rates is null || response.Rates.Any() is false) return null;

        var allRates = response.Rates
            .Select(x => new HistoricalResult(x.Key, x.Value))
            .ToList();

        return new PagedResult<HistoricalResult>(allRates, datePageRange!.Value.TotalDays);
    }
}
