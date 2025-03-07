namespace WesternStatesWater.WaDE.Common.Ogc;

public class Link
{
    /// <summary>
    /// URL being referenced
    /// </summary>
    public required string Href { get; set; }
    
    /// <summary>
    /// Relation type of the URL. A list of valid relation types can be found at http://www.opengis.net/def/rel
    /// </summary>
    public required string Rel { get; set; }
    
    /// <summary>
    /// Type of information being returned by the URL
    /// </summary>
    public string? Type { get; set; }
    
    /// <summary>
    /// Attribute used to specify the language and geographical targeting of information accessed by the URL. Can be defined by using a value from either languages ISO 639-1 or countries ISO 3166-1
    /// </summary>
    public string? Hreflang { get; set; }
    
    /// <summary>
    /// A short text label to describe the URL
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// If True the URL includes templated values for mandatory Query parameters
    /// </summary>
    public bool? Templated { get; set; }
    
    /// <summary>
    /// Object providing custom information relevant to the link
    /// </summary>
    public Variable? Variables { get; set; }
}