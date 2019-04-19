namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Variable
    {
        public string VariableSpecificUUID { get; set; }
        public string VariableSpecificCV { get; set; }
        public string VariableCV { get; set; }
        public string AggregationStatisticCV { get; set; }
        public double AggregationInterval { get; set; }
        public string AggregationIntervalUnitCV { get; set; }
        public string ReportYearStartMonth { get; set; }
        public string ReportYearTypeCV { get; set; }
        public string AmountUnitCV { get; set; }
        public string MaximumAmountUnitCV { get; set; }
    }
}
