namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

public class Collections
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required Link[] Links { get; set; }
    public Extent Extent { get; set; }
}

public class Collection
{
    public required string Id { get; set; }
    public required Link[] Links { get; set; }
    public long? StorageCrsCoordinateEpoch { get; set; }
    public string[]? Crs { get; set; }
    public string? ItemType { get; set; }
    private string? StorageCrs { get; set; }
    public Extent? Extent { get; set; }
}

public class Site
{
    public required string Type { get; set; } // not sure if this is rtight
    // public Feature geometry { get; set; }
}

public class Extent
{
    public Spatial? Spatial { get; set; }
    public Temporal? Temporal { get; set; }
}

public class Spatial
{
    public string? Crs { get; set; }
    public double[][] Bbox { get; set; }
}

public class Temporal
{
    public string[][] Interval { get; set; }
    public string Trs { get; set; }
}