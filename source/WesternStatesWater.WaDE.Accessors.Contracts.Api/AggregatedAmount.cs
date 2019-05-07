using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class AggregatedAmount
    {
        public string Variable { get; set; }
        public string VariableSpecificTypeCV { get; set; }
        public string MethodUUID { get; set; }
        public string ReportYear { get; set; }
        public DateTime? TimeframeStart { get; set; }
        public DateTime? TimeframeEnd { get; set; }
        public string WaterSourceUUID { get; set; }
        public string ReportingUnitUUID { get; set; }
        public double Amount { get; set; }
        public long? PopulationServed { get; set; }
        public double? PowerGeneratedGWh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public DateTime? DataPublicationDate { get; set; }
    }
}
