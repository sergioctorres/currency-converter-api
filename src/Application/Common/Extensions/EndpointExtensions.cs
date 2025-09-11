using Application.Common.CustomAttributes;
using System.Reflection;

namespace Application.Common.Extensions;

public static class EndpointExtensions
{
    public static string ToEndpoint<T>(this T request)
    {
        var route = request.ToRoute();
        var query = request.ToQueryString();

        return $"{route}{query}";
    }

    public static string ToRoute<T>(this T request)
    {
        var properties = GetProperties(request, EndpointLocation.Route);

        var values = properties
            .Select(property => FormatValue(property.GetValue(request)));

        if (!values.Any()) return string.Empty;

        return $"/{string.Join("..", values!)}";
    }

    private static IEnumerable<PropertyInfo> GetProperties<T>(T request, EndpointLocation location)
    {
        return typeof(T).GetProperties()
            .Where(property => property.GetValue(request) is not null
                && property.GetCustomAttribute<EndpointLocationAttribute>()?.Location == location
            );
    }

    public static string ToQueryString<T>(this T request)
    {
        var properties = GetProperties(request, EndpointLocation.Query);

        var query = new List<string>();

        foreach (var property in properties)
            query.Add($"{GetPropertyName(property)}={FormatValue(property.GetValue(request))}");

        return query.Count > 0 ? $"?{string.Join("&", query)}" : string.Empty;
    }

    private static string? FormatValue(object? value)
    {
        return value switch
        {
            null => null,
            string text when string.IsNullOrWhiteSpace(text) => null,
            IEnumerable<string> array => string.Join(",", array),
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd"),
            _ => value.ToString()
        };
    }

    private static string GetPropertyName(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<EndpointLocationAttribute>();
        return attribute?.Name ?? property.Name.ToLower();
    }
}
