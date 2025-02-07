namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The unit object provides the information to describe the units of measure of the parameter values.
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_5378d779-6a38-4607-9051-6f12c3d3107b
/// </summary>
public class Unit
{
    /// <summary>
    /// Name of the unit
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// Information to describe the symbols used to represent the unit
    /// </summary>
    public required Symbol Symbol { get; set; }
}