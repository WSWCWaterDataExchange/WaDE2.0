using WesternStatesWater.WaDE.Engines.Contracts.Attributes;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class OverlayFeature : FeatureBase
{
    [FeaturePropertyName("nId")]
    public string OverlayNativeId { get; set; }
    [FeaturePropertyName("name")]
    public string RegulatoryName { get; set; }
    [FeaturePropertyName("desc")]
    public string RegulatoryDescription { get; set; }
    [FeaturePropertyName("status")]
    public string RegulatoryStatus { get; set; }
    [FeaturePropertyName("agency")]
    public string OversightAgency { get; set; }
    [FeaturePropertyName("statute")]
    public string RegulatoryStatute { get; set; }
    [FeaturePropertyName("statuteRef")]
    public string RegulatoryStatuteLink { get; set; }
    [FeaturePropertyName("statueEffDate")]
    public string StatutoryEffectiveDate { get; set; }
    [FeaturePropertyName("statueEndDate")]
    public string StatutoryEndDate { get; set; }
    [FeaturePropertyName("oType")]
    public string OverlayType { get; set; }
    [FeaturePropertyName("wSource")]
    public string WaterSource { get; set; }
    [FeaturePropertyName("areaNames")]
    public string[] AreaNames { get; set; }
    [FeaturePropertyName("areaNIds")]
    public string[] AreaNativeIds { get; set; }
    [FeaturePropertyName("sites")]
    public string[] SiteUuids { get; set; }

}