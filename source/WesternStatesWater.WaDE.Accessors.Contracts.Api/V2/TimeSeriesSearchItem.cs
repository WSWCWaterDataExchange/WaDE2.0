namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

/// <summary>
/// A Site that has Time Series associated with it.
/// </summary>
public class TimeSeriesSearchItem : Site
{
   public TimeSeries[] TimeSeries { get; set; }
}