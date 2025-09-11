using Application.Common.CustomAttributes;

namespace Application.Dtos.CurrencyRate;

public record LatestRequest(
    [property: EndpointLocation(EndpointLocation.Query)]
    string Base,

    [property: EndpointLocation(EndpointLocation.Query)]
    string[]? Symbols = null
);