using System.ComponentModel;

namespace Application.Dtos.Common;

public record PagedRequest(
    [property: DefaultValue(5)] int PageSize = 5,
    [property: DefaultValue(1)] int Page = 1
);
