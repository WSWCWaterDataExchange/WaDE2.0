namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc;

public class Link
{
    public required string Href { get; set; }
    public required string Rel { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
}