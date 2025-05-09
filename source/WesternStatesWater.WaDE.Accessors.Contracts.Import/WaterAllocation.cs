﻿using CsvHelper.Configuration.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class WaterAllocation
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string VariableSpecificUUID { get; set; }

        [NullValues("")]
        public string SiteUUID { get; set; }

        [NullValues("")]
        public string MethodUUID { get; set; }

        [NullValues("")]
        public string BeneficialUseCategory { get; set; }
        
        [NullValues("")]
        public string PrimaryBeneficialUseCategory { get; set; }

        public DateTime? DataPublicationDate { get; set; }

        [NullValues("")]
        public string DataPublicationDOI { get; set; }

        [NullValues("")]
        public string AllocationNativeID { get; set; }

        [NullValues("")]
        public DateTime? AllocationApplicationDate { get; set; }

        [NullValues("")]
        public DateTime? AllocationPriorityDate { get; set; }

        [NullValues("")]
        public DateTime? AllocationExpirationDate { get; set; }

        [NullValues("")]
        public string AllocationOwner { get; set; }

        [NullValues("")]
        public string AllocationBasisCV { get; set; }

        [NullValues("")]
        public string AllocationLegalStatusCV { get; set; }

        [NullValues("")]
        public string AllocationTypeCV { get; set; }

        [NullValues("")]
        [MaxLength(5)]
        public string AllocationTimeframeStart { get; set; }

        [NullValues("")]
        [MaxLength(5)]
        public string AllocationTimeframeEnd { get; set; }

        [NullValues("")]
        public string AllocationCropDutyAmount { get; set; }

        [NullValues("")]
        public string AllocationFlow_CFS { get; set; }

        [NullValues("")]
        public string AllocationVolume_AF { get; set; }

        [NullValues("")]
        public string AllocationUUID { get; set; }

        [NullValues("")]
        public string PopulationServed { get; set; }
        
        [NullValues("")]
        public string GeneratedPowerCapacityMW { get; set; }
        
        [NullValues("")]
        public string IrrigatedAcreage { get; set; }

        [NullValues("")]
        public string AllocationCommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string AllocationSDWISIdentifierCV { get; set; }

        [NullValues("")]
        public string AllocationChangeApplicationIndicator { get; set; }

        [NullValues("")]
        public string LegacyAllocationIDs { get; set; }

        [NullValues("")]
        public string CustomerTypeCV { get; set; }

        [NullValues("")]
        public string IrrigationMethodCV { get; set; }

        [NullValues("")]
        public string CropTypeCV { get; set; }

        [NullValues("")]
        public string WaterAllocationNativeURL { get; set; }

        [NullValues("")]
        public string CommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string PowerType { get; set; }

        [NullValues("")]
        public bool? ExemptOfVolumeFlowPriority { get; set; }

        [NullValues("")]
        public string OwnerClassificationCV { get; set; }
    }
}
