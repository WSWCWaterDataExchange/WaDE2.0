﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AllocationAmountsFact
    {
        public AllocationAmountsFact()
        {
            AllocationBridgeBeneficialUsesFact = new HashSet<AllocationBridgeBeneficialUsesFact>();
            AllocationBridgeSitesFact = new HashSet<AllocationBridgeSitesFact>();
        }

        public long AllocationAmountId { get; set; }
        public long OrganizationId { get; set; }
        public long VariableSpecificId { get; set; }
        public long MethodId { get; set; }
        public string PrimaryUseCategoryCV { get; set; }
        public long DataPublicationDateId { get; set; }
        public string DataPublicationDoi { get; set; }
        public string AllocationNativeId { get; set; }
        public long? AllocationApplicationDateID { get; set; }
        public long? AllocationPriorityDateID { get; set; }
        public long? AllocationExpirationDateID { get; set; }
        public string AllocationOwner { get; set; }
        public string AllocationBasisCv { get; set; }
        public string AllocationLegalStatusCv { get; set; }
        public string AllocationTypeCv { get; set; }
        [MaxLength(5)]
        public string AllocationTimeframeStart { get; set; }
        [MaxLength(5)]
        public string AllocationTimeframeEnd { get; set; }
        public double? AllocationCropDutyAmount { get; set; }
        public double? AllocationFlow_CFS { get; set; }
        public double? AllocationVolume_AF { get; set; }
        public long? PopulationServed { get; set; }
        public double? GeneratedPowerCapacityMW { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string AllocationCommunityWaterSupplySystem { get; set; }
        public string SdwisidentifierCV { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string LegacyAllocationIds { get; set; }
        public string WaterAllocationNativeUrl { get; set; }
        public string CropTypeCV { get; set; }
        public string IrrigationMethodCV { get; set; }
        public string CustomerTypeCV { get; set; }
        public string CommunityWaterSupplySystem { get; set; }
        public bool? ExemptOfVolumeFlowPriority { get; set; }
        public string PowerType { get; set; }
        public string OwnerClassificationCV { get; set; }
        public string AllocationUUID { get; set; }
        public Guid? ConservationApplicationFundingOrganizationId { get; set; }

        public virtual DateDim AllocationApplicationDateNavigation { get; set; }
        public virtual WaterAllocationBasis AllocationBasisCvNavigation { get; set; }
        public virtual DateDim AllocationExpirationDateNavigation { get; set; }
        public virtual LegalStatus AllocationLegalStatusCvNavigation { get; set; }
        public virtual DateDim AllocationPriorityDateNavigation { get; set; }
        public virtual WaterAllocationType AllocationTypeCvNavigation { get; set; }
        public virtual DateDim DataPublicationDate { get; set; }
        public virtual MethodsDim Method { get; set; }
        public virtual OrganizationsDim Organization { get; set; }
        public virtual VariablesDim VariableSpecific { get; set; }
        public virtual CropType CropType { get; set; }
        public virtual CustomerType CustomerType { get; set; }
        public virtual SDWISIdentifier SDWISIdentifier { get; set; }
        public virtual IrrigationMethod IrrigationMethod { get; set; }
        public virtual PowerType PowerTypeCV { get; set; }
        public virtual OwnerClassificationCv OwnerClassification { get; set; }
        public virtual ICollection<AllocationBridgeBeneficialUsesFact> AllocationBridgeBeneficialUsesFact { get; set; }
        public virtual ICollection<AllocationBridgeSitesFact> AllocationBridgeSitesFact { get; set; }
    }
}
