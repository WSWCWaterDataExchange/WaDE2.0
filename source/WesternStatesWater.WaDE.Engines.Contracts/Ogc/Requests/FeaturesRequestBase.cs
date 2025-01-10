namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public abstract class FeaturesRequestBase : FormattingRequestBase
{
    public FeatureBase[] Items { get; set; }
    public string? LastUuid { get; set; }
}