using Microsoft.AspNetCore.Http;

namespace WesternStatesWater.WaDE.Common.Context;

public abstract class RequestContext : ContextBase
{
    public required string Scheme { get; init; }
    public required string Host { get; init; }
    public required string PathBase { get; init; }
    public required string Path { get; init; }
    public QueryString QueryString { get; init; }

    public override string ToString()
    {
        return $"{Scheme}://{Host}{PathBase}{Path}{QueryString}";
    }
}