using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class TimeSeriesFeature : SiteFeature
{
    [JsonPropertyName("timeSeries")]
    public TimeSeries[] TimeSeries { get; set; }
}