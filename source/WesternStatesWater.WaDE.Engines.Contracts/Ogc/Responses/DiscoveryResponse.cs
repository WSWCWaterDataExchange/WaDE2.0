namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

public class DiscoveryResponse : FormattingResponseBase
{
    public required Link[] Links { get; set; } = [];
    public string Title { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }
}