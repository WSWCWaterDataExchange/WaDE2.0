namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class FeaturesSearchRequestBase : WaterResourceSearchRequestBase
{
    public string Bbox { get; set; }
    public string Limit { get; set; }
    public string Next { get; set; }
    public string Coords { get; set; }
}