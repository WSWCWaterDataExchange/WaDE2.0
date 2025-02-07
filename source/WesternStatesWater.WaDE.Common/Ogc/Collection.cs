namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// EDR Collection Object Structure.
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_b6db449c-4ca7-4117-9bf4-241984cef569
/// </summary>
public class Collection
{
    /// <summary>
    /// Unique identifier string for the collection, used as the value for the collection_id path parameter in all queries on the collection
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Array of link objects
    /// </summary>
    public required Link[] Links { get; set; }
    
    /// <summary>
    /// A short text label for the collection
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// A text description of the information provided by the collection
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Array of words and phrases that define the information that the collection provides
    /// </summary>
    public string[]? Keywords { get; set; }
    
    /// <summary>
    /// Array of coordinate reference system names, which define the output coordinate systems supported by the collection
    /// </summary>
    public string[]? Crs { get; set; }
    public string? ItemType { get; set; }
    public string? StorageCrs { get; set; }
    
    /// <summary>
    /// Object describing the spatio-temporal extent of the information provided by the collection
    /// </summary>
    public Extent? Extent { get; set; }
    
    /// <summary>
    /// Object providing query specific information
    /// </summary>
    public Dictionary<string, DataQuery>? DataQueries { get; set; }
    
    /// <summary>
    /// Array of data format names, which define the data formats to which information in the collection can be output
    /// </summary>
    public string[] OutputFormats { get; set; }
    
    /// <summary>
    /// Describes the data values available in the collection
    /// </summary>
    public required Dictionary<string, Parameter> ParameterNames { get; set; }
}