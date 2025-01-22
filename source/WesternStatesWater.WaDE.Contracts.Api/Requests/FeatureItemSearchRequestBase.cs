namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class FeatureItemSearchRequestBase : WaterResourceSearchRequestBase
{
    public string Id { get; set; }
}