namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// Describes a coordinate system.
/// </summary>
public class CrsDetail
{
    /// <summary>
    /// Name of the coordinate reference system, used as the value in the crs query parameter to define the required output coordinate reference system
    /// </summary>
    public required string Crs { get; set; }
    
    /// <summary>
    /// Well Known Text description of the coordinate reference system
    /// </summary>
    public required string Wkt { get; set; }
}