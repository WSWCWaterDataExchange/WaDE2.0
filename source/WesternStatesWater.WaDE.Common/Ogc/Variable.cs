namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The variable object provides fields to describe information that only applies to the owning link.
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_1b54f97a-e1dc-4920-b8b4-e4981554138d
/// </summary>
public class Variable
{
    /// <summary>
    /// A short text label for the query
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// A short text label for the query
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// A description of the query
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// One of: position, radius, area, cube, trajectory, corridor, items, locations, instances
    /// </summary>
    public string? QueryType { get; set; }

    /// <summary>
    /// An example of valid coords query parameter values
    /// </summary>
    public string? Coords { get; set; }

    /// <summary>
    /// A list of the valid within units for radius queries
    /// </summary>
    public string[]? WithinWidth { get; set; }

    /// <summary>
    /// A list of the valid width units
    /// </summary>
    public string[]? WidthUnits { get; set; }

    /// <summary>
    /// A list of the valid height units
    /// </summary>
    public string[]? HeightUnits { get; set; }

    /// <summary>
    /// A list of output formats supported by the query, if this field exists it overrides the output formats definition supplied at a collection level.
    /// </summary>
    public string[]? OutputFormats { get; set; }

    /// <summary>
    /// Specifies the default output format for the query
    /// </summary>
    public string[]? DefaultOutputFormat { get; set; }

    /// <summary>
    /// A list of coordinate reference systems supported by the query, if this field exists it overrides the crs values defined at a collection level.
    /// </summary>
    public CrsDetail[] CrsDetails { get; set; }
}