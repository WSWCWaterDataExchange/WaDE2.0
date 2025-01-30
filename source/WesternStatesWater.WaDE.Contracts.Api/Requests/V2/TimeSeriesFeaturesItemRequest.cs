namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class TimeSeriesFeaturesItemRequest : TimeSeriesFeaturesSearchRequestBase
{
    public string SiteUuids { get; set; }
    public string States { get; set; }
    public string VariableTypes { get; set; }
    public string WaterSourceTypes { get; set; }
    public string PrimaryUses { get; set; }
    public string DateTime { get; set; }
}