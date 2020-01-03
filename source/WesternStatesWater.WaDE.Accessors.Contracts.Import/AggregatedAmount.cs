using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class AggregatedAmount
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string ReportingUnitUUID { get; set; }

        [NullValues("")]
        public string VariableSpecificUUID { get; set; }

        [NullValues("")]
        public string BeneficialUseCategory { get; set; }

        [NullValues("")]
        public string PrimaryUseCategory { get; set; }

        [NullValues("")]
        public string MethodUUID { get; set; }

        [NullValues("")]
        public string WaterSourceUUID { get; set; }

        [NullValues("")]
        public DateTime? TimeframeStart { get; set; }

        [NullValues("")]
        public DateTime? TimeframeEnd { get; set; }

        [NullValues("")]
        public DateTime? DataPublicationDate { get; set; }

        [NullValues("")]
        public string DataPublicationDOI { get; set; }

        [NullValues("")]
        public string ReportYearCV { get; set; }

        [NullValues("")]
        public string Amount { get; set; }

        [NullValues("")]
        public string PopulationServed { get; set; }

        [NullValues("")]
        public string PowerGeneratedGWh { get; set; }
        
        [NullValues("")]
        public string IrrigatedAcreage { get; set; }

        [NullValues("")]
        public string InterbasinTransferToID { get; set; }

        [NullValues("")]
        public string InterbasinTransferFromID { get; set; }

        [NullValues("")]
        public string CustomerTypeCV { get; set; }

        [NullValues("")]
        public string AllocationCropDutyAmount { get; set; }

        [NullValues("")]
        public string IrrigationMethodCV { get; set; }

        [NullValues("")]
        public string CropTypeCV { get; set; }

        [NullValues("")]
        public string CommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string SDWISIdentifierCV { get; set; }

        [NullValues("")]
        public string PowerType { get; set; }
    }
}
