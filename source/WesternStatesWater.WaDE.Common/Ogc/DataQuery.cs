namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The EDR query object provides the metadata for the specified query type.
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_9a6620ce-6093-4b1b-8f68-2e2c04a13746
/// </summary>
public class DataQuery
{
    public required Link Link { get; set; }
}