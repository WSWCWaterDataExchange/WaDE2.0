namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

public class Collections
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required Link[] Links { get; set; }
    public Extent Extent { get; set; }
}