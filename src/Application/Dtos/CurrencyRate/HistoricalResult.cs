namespace Application.Dtos.CurrencyRate;

public record HistoricalResult(
    DateTime Date,
    IDictionary<string, double> Values
);
