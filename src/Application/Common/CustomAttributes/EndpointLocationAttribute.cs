namespace Application.Common.CustomAttributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EndpointLocationAttribute(EndpointLocation location, string? name = null) : Attribute
{
    public EndpointLocation Location { get; private set; } = location;
    public string? Name { get; private set; } = name;
}

public enum EndpointLocation
{
    Route,
    Query
}