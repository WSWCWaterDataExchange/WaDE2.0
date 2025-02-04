using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class VariableSpecific
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }
    [JsonPropertyName("name")]
    public string VariableSpecificCv { get; set; }
    [JsonPropertyName("wadeName")]
    public string VariableSpecificWaDEName { get; set; }
    [JsonPropertyName("variable")]
    public string VariableCv { get; set; }
    [JsonPropertyName("variableWaDEName")]
    public string VariableWaDEName { get; set; }
    [JsonPropertyName("aggStatistic")]
    public string AggregationStatistic { get; set; }
    [JsonPropertyName("aggInterval")]
    public decimal AggregationInterval { get; set; }
    [JsonPropertyName("aggIntervalUnit")]
    public string AggregationIntervalUnit { get; set; }
    [JsonPropertyName("reportYearStartMonth")]
    public string ReportYearStartMonth { get; set; }
    [JsonPropertyName("reportYearType")]
    public string ReportYearType { get; set; }
    [JsonPropertyName("amtUnit")]
    public string AmountUnit { get; set; }
    [JsonPropertyName("maxAmtUnit")]
    public string MaximumAmountUnit { get; set; }
}