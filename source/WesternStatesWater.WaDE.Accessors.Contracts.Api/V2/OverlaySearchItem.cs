using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class OverlaySearchItem
{
    public string OverlayUuid { get; set; }
    public string OverlayNativeId { get; set; }
    public string RegulatoryName { get; set; }
    public string RegulatoryDescription { get; set; }
    public string RegulatoryStatus { get; set; }
    public string OversightAgency { get; set; }
    public string RegulatoryStatute { get; set; }
    public string RegulatoryStatuteLink { get; set; }
    public string StatutoryEffectiveDate { get; set; }
    public string StatutoryEndDate { get; set; }
    public string OverlayType { get; set; }
    public string WaterSource { get; set; }
    public string AreaName { get; set; }
    public string AreaNativeId { get; set; }
    public string SiteUuids { get; set; }
    public Geometry Areas { get; set; }
}