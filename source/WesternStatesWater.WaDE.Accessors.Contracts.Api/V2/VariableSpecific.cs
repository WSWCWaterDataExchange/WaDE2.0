namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class VariableSpecific
{
    public string Uuid { get; set; }
    public string VariableSpecificCv { get; set; }
    public string VariableSpecificWaDEName { get; set; }
    public string VariableCv { get; set; }
    public string VariableWaDEName { get; set; }
    public string AggregationStatistic { get; set; }
    public decimal AggregationInterval { get; set; }
    public string AggregationIntervalUnit { get; set; }
    public string ReportYearStartMonth { get; set; }
    public string ReportYearType { get; set; }
    public string AmountUnit { get; set; }
    public string MaximumAmountUnit { get; set; }
}