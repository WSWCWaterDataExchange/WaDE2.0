namespace WesternStatesWater.WaDE.Engines.Contracts;

public class OverlayFeature : FeatureBase
{
    public string RegulatoryOverlayUuid { get; set; }
    public string RegulatoryOverlayNativeId { get; set; }
    public string RegulatoryName { get; set; }
    public string RegulatoryDescription { get; set; }
    public string RegulatoryStatusCv { get; set; }
    public string OversightAgency { get; set; }
    public string RegulatoryStatute { get; set; }
    public string RegulatoryStatuteLink { get; set; }
    public string StatutoryEffectiveDate { get; set; }
    public string StatutoryEndDate { get; set; }
    public string RegulatoryOverlayTypeCv { get; set; }
    public string WaterSourceTypeCv { get; set; }
    public string[] ReportingUnitNames { get; set; }
    public string[] ReportingUnitNativeIds { get; set; }
}