using WesternStatesWater.WaDE.Engines.Contracts.Attributes;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class WaterSourceSummary
{
    [FeaturePropertyName("id")]
    public required string WaterSourceUuId { get; set; }
    [FeaturePropertyName("nId")]
    public string? NativeId { get; set; }
    [FeaturePropertyName("name")]
    public string? Name { get; set; }
    [FeaturePropertyName("sourceType")]
    public required string SourceType { get; set; }
    [FeaturePropertyName("type")]
    public required string WaterType { get; set; }
}