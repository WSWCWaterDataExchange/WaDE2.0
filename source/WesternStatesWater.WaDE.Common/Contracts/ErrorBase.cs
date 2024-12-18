namespace WesternStatesWater.WaDE.Common.Contracts;

public abstract record ErrorBase
{
    public string? PublicMessage { get; init; }
}