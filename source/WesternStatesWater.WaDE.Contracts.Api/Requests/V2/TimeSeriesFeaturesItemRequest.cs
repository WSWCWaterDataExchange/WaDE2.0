namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class TimeSeriesFeaturesItemRequest : FeaturesSearchRequestBase
{
    public required string SiteUuid { get; set; }
    public string States { get; set; }
    public string VariableTypes { get; set; }
    public string WaterSourceTypes { get; set; }
    public string PrimaryUses { get; set; }
}