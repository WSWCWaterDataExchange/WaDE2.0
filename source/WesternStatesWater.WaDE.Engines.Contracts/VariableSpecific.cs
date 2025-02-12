namespace WesternStatesWater.WaDE.Engines.Contracts;

public class VariableSpecific
{
    public string VariableSpecificUuid { get; set; }
    public string VariableSpecificCv { get; set; }
    public string VariableSpecificWaDEName { get; set; }
    public string VariableCv { get; set; }
    public string VariableWaDEName { get; set; }
    public string AggregationStatisticCv { get; set; }
    public decimal AggregationInterval { get; set; }
    public string AggregationIntervalUnitCv { get; set; }
    public string ReportYearStartMonth { get; set; }
    public string ReportYearTypeCv { get; set; }
    public string AmountUnitCv { get; set; }
    public string MaximumAmountUnitCv { get; set; }
}