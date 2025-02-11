namespace WesternStatesWater.WaDE.Engines.Contracts;

public class WaterSourceSummary
{
    public required string WaterSourceUuid { get; set; }
    public string? WaterSourceNativeId { get; set; }
    public string? WaterSourceName { get; set; }
    public required string WaterSourceTypeCv { get; set; }
    public required string WaterQualityIndicatorCv { get; set; }
}