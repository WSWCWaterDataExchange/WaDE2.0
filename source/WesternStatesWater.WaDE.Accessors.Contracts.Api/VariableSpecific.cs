﻿namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class VariableSpecific
    {
        public long VariableSpecificId { get; set; }
        public string VariableSpecificUUID { get; set; }
        public string VariableSpecificTypeCV { get; set; }
        public string VariableCV { get; set; }
        public string AmountUnitCV { get; set; }
        public string AggregationStatisticCV { get; set; }
        public string AggregationInterval { get; set; }
        public string AggregationIntervalUnitCV { get; set; }
        public string ReportYearStartMonth { get; set; }
        public string ReportYearTypeCV { get; set; }
        public string MaximumAmountUnitCV { get; set; }
    }
}
