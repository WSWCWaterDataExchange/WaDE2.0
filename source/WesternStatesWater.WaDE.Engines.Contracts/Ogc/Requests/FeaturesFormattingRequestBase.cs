namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public abstract class FeaturesFormattingRequestBase : FormattingRequestBase
{
    public required string CollectionId { get; init; }
    public required FeatureBase[] Items { get; init; }
    public string? LastUuid { get; set; }
}