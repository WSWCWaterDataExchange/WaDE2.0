namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// Information about the data parameters supported by the collection
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_75b53201-11ab-44cd-98eb-b998f32d706e
/// </summary>
public class Parameter
{
    /// <summary>
    /// Always 'Parameter'
    /// </summary>
    public static string Type => "Parameter";

    /// <summary>
    /// Parameter Id
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// A short text label for the parameter
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// A description of the parameter
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The data type of the parameter values [integer, float, string]
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// A formal definition of the parameter
    /// </summary>
    public required ObservedProperty ObservedProperty { get; set; }

    /// <summary>
    /// Information on the spatio-temporal extent of the parameter values (if different from other parameters in the collection)
    /// </summary>
    public Unit? Unit { get; set; }

    /// <summary>
    /// Information on how the value was derived
    /// </summary>
    public MeasurementType? MeasurementType { get; set; }
}