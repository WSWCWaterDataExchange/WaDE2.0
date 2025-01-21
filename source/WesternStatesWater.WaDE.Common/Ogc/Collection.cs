namespace WesternStatesWater.WaDE.Common.Ogc;

public class Collection
{
    public required string Id { get; set; }
    public required Link[] Links { get; set; }
    public string[]? Crs { get; set; }
    public string? ItemType { get; set; }
    public string? StorageCrs { get; set; }
    public Extent? Extent { get; set; }
}