namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class ReportingArea
{
    public string ReportingUnitUuid { get; set; }
    public string ReportingUnitNativeId { get; set; }
    public string ReportingUnitName { get; set; }
    public string ReportingUnitTypeCv { get; set; }
    public string ReportingUnitTypeWaDEName { get; set; }
    public string State { get; set; }
    public string EpsgCodeCv { get; set; }
}