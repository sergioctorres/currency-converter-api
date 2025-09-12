namespace Application.Dtos.CurrencyRate;

public record LatestResult(string Base, DateTime Date, IDictionary<string, double> Rates);