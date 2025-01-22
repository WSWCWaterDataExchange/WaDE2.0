namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public abstract class FeaturesFormattingRequestBase : FormattingRequestBase
{
    public required FeatureBase[] Items { get; init; }
    public string? LastUuid { get; set; }
}