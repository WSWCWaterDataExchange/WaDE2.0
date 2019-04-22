using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class AggregatedAmount
    {
        public string OrganizationUUID { get; set; }
        public string ReportingUnitUUID { get; set; }
        public string VariableSpecificUUID { get; set; }
        public string MethodUUID { get; set; }
        public string WaterSourceUUID { get; set; }
        public DateTime? TimeframeStart { get; set; }
        public DateTime? TimeframeEnd { get; set; }
        public DateTime? DataPublicationDate { get; set; }
        public string DataPublicationDOI { get; set; }
        public string ReportYearCV { get; set; }
        public double Amount { get; set; }
        public double? PopulationServed { get; set; }
        public double? PowerGeneratedGWh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string InterbasinTransferToID { get; set; }
        public string InterbasinTransferFromID { get; set; }
        public string BeneficialUseCategory { get; set; }
        public string PrimaryUseCategory { get; set; }
    }
}
