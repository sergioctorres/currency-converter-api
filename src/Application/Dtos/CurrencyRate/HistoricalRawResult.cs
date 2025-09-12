using System.Text.Json.Serialization;

namespace Application.Dtos.CurrencyRate;

public record HistoricalRawResult(
    string Base,

    [property: JsonPropertyName("start_date")]
    DateTime StartDate,

    [property: JsonPropertyName("end_date")]
    DateTime EndDate,

    IDictionary<DateTime, IDictionary<string, double>> Rates
);
