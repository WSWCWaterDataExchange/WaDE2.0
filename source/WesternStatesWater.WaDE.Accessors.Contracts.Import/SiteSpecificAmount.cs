﻿using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class SiteSpecificAmount
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string SiteUUID { get; set; }
        
        [NullValues("")]
        public string VariableSpecificUUID { get; set; }

        [NullValues("")]
        public string WaterSourceUUID { get; set; }

        [NullValues("")]
        public string MethodUUID { get; set; }

        public DateTime? TimeframeStart { get; set; }

        public DateTime? TimeframeEnd { get; set; }

        public DateTime? DataPublicationDate { get; set; }

        [NullValues("")]
        public string DataPublicationDOI { get; set; }

        [NullValues("")]
        public string ReportYearCV { get; set; }

        public double Amount { get; set; }

        public double? PopulationServed { get; set; }

        public double? PowerGeneratedGWh { get; set; }

        public double? IrrigatedAcreage { get; set; }

        [NullValues("")]
        public string IrrigationMethodCV { get; set; }

        [NullValues("")]
        public string CropTypeCV { get; set; }

        [NullValues("")]
        public string CommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string SDWISIdentifier { get; set; }

        [NullValues("")]
        public string AssociatedNativeAllocationIDs { get; set; }

        [NullValues("")]
        public string Geometry { get; set; }

        [NullValues("")]
        public string BeneficialUseCategory { get; set; }

        [NullValues("")]
        public string PrimaryUseCategory { get; set; }
    }
}