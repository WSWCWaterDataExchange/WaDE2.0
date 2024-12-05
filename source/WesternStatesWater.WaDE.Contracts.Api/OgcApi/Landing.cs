namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

public class Landing
{
    public required Link[] Links { get; set; } = [];
    public string Title { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }
    public Contact Contact { get; set; }
}