using Application.Common.CustomAttributes;
using Application.Dtos.Common;

namespace Application.Dtos.CurrencyRate;

public record HistoricalRequest(
    [property: EndpointLocation(EndpointLocation.Route)]
    DateTime StartDate,

    [property: EndpointLocation(EndpointLocation.Query, "base")]
    string? Base = null,

    [property: EndpointLocation(EndpointLocation.Query, "symbols")]
    string[]? Symbols = null
) : PagedRequest
{
    [property: EndpointLocation(EndpointLocation.Route)]
    public DateTime EndDate { get; init; } = DateTime.UtcNow.Date;
}
