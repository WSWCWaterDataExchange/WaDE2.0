using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class OverlaySearchItem
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
    public string[] States { get; set; }
    public Geometry Areas { get; set; }
}