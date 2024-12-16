namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

public class Link
{
    public required string Href { get; set; }
    public required string Rel { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
}