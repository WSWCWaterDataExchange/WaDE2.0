namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

public class CollectionsResponse
{
    public required Link[] Links { get; set; }
    public required Collection[] Collections { get; set; }
}