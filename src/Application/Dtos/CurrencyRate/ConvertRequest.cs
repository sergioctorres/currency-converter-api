using Application.Common.CustomAttributes;

namespace Application.Dtos.CurrencyRate;

public record ConvertRequest(
    [property: EndpointLocation(EndpointLocation.Query, "base")]
    string Base,

    [property: EndpointLocation(EndpointLocation.Query, "symbols")]
    string Target,

    double Amount
);