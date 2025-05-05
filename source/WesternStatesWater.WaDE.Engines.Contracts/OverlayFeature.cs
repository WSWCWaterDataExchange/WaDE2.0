namespace WesternStatesWater.WaDE.Engines.Contracts;

public class OverlayFeature : FeatureBase
{
    public string OverlayUuid { get; set; }
    public string OverlayNativeId { get; set; }
    public string OverlayName { get; set; }
    public string OverlayDescription { get; set; }
    public string OverlayStatusCv { get; set; }
    public string OversightAgency { get; set; }
    public string Statute { get; set; }
    public string StatuteLink { get; set; }
    public string StatutoryEffectiveDate { get; set; }
    public string StatutoryEndDate { get; set; }
    public string OverlayTypeCv { get; set; }
    public string WaterSourceTypeCv { get; set; }
    public ReportingArea[] ReportingAreas { get; set; }
}