namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class TimeSeriesFeaturesSearchRequestBase : FeaturesSearchRequestBase
{
    public string SiteUuids { get; set; }
    public string States { get; set; }
    public string VariableTypes { get; set; }
    public string WaterSourceTypes { get; set; }
    public string PrimaryUses { get; set; }
}