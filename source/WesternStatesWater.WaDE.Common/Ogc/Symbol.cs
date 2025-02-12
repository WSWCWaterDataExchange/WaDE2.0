namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The symbol object provides the information to describe the symbols which represent the unit of a value.
/// </summary>
public class Symbol
{
    /// <summary>
    /// Symbol name
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// A text description of the symbol
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// A Unicode representation for the symbol
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// A URI to a registry entry providing more detailed information about the unit
    /// (i.e. QUDT (https://www.qudt.org/) is one example of a registry that provide links for many common units)
    /// </summary>
    public string? Type { get; set; }
}