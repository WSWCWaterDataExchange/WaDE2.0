namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public class BoundaryBox
{
    public required string Crs { get; set; }
    public double MinX { get; set; }
    public double MinY { get; set; }
    public double MaxX { get; set; }
    public double MaxY { get; set; }
    
}