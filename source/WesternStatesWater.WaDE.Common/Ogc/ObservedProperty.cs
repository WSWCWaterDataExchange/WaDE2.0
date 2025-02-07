namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The observedProperty object provides the metadata for the specified query type.
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_7e053ab4-5cde-4a5c-a8be-acc6495f9eb5
/// </summary>
public class ObservedProperty
{
    /// <summary>
    /// URI linking to an external registry which contains the definitive definition of the observed property
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// A short text label for the property
    /// </summary>
    /// <returns></returns>
    public required string Label { get; set; }
    
    /// <summary>
    /// A description of the observed property
    /// </summary>
    public string? Description { get; set; }
}