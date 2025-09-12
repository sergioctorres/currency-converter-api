using Application.Dtos.CurrencyRate;

namespace Application.Constants;

public static class CacheKeys
{
    public static class CurrencyService
    {
        public static string Latest(LatestRequest latestRequest) =>
            $"currencyservice:latest:{latestRequest.Base.ToLowerInvariant()}:{string.Join(",", (latestRequest.Symbols ?? []).Select(s => s.ToLowerInvariant()))}";

        public static string Historical(HistoricalRequest historicalRequest) =>
            $"currencyservice:history:{historicalRequest.Base!.ToLowerInvariant()}:{historicalRequest.StartDate:yyyy-MM-dd}:" +
            $"{historicalRequest.EndDate:yyyy-MM-dd}:{historicalRequest.Page}:{historicalRequest.Page}:" +
            $"{string.Join(",", (historicalRequest.Symbols ?? []).Select(s => s.ToLowerInvariant()))}";
    }
}
