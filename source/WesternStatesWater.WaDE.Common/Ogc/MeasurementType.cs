namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The measurementType object provides basic information about how the parameter is calculated and over what time period.
/// https://docs.ogc.org/is/19-086r6/19-086r6.html#_c81181d6-fd09-454e-9c00-a3bb3b21d592
/// </summary>
public class MeasurementType
{
    /// <summary>
    /// Calculation method e.g. Mean, Sum, Max, etc.
    /// </summary>
    public required string Method { get; set; }

    /// <summary>
    /// Duration of calculation. For time durations, this follows the ISO 8601 Duration standard.
    ///
    /// A negative sign before a duration value (i.e. -PT10M) infers that the time start starts at the specified duration before the time value assigned to the parameter value.
    /// So if the measurement had a time value of 2020-04-05T14:30Z and a measurementType duration of -PT10M the value is representative of the period 2020-04-05T14:20Z/2020-04-05T14:30Z; if the measurement had a time value of 2020-04-05T14:30Z and a measurementType duration of PT10M the value is representative of the period 2020-04-05T14:30Z/2020-04-05T14:40Z
    /// </summary>
    public required string Duration { get; set; }
}