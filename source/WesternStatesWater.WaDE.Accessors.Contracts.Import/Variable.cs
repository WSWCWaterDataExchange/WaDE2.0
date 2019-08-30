using CsvHelper.Configuration.Attributes;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Variable
    {

        [NullValues("")]
        public string VariableSpecificCV { get; set; }

        [NullValues("")]
        public string VariableSpecificUUID { get; set; }

        [NullValues("")]
        public string VariableCV { get; set; }

        [NullValues("")]
        public string AggregationStatisticCV { get; set; }

        public double AggregationInterval { get; set; }

        [NullValues("")]
        public string AggregationIntervalUnitCV { get; set; }

        [NullValues("")]
        public string ReportYearStartMonth { get; set; }

        [NullValues("")]
        public string ReportYearTypeCV { get; set; }

        [NullValues("")]
        public string AmountUnitCV { get; set; }

        [NullValues("")]
        public string MaximumAmountUnitCV { get; set; }
    }
}
