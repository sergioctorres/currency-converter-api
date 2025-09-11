namespace Application.Dtos.Common;

public record PagedResult<T>(
    IEnumerable<T>? Records,
    int TotalRecords
);
