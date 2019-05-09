using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AllocationAmountsFact
    {
        public AllocationAmountsFact()
        {
            AllocationBridgeBeneficialUsesFact = new HashSet<AllocationBridgeBeneficialUsesFact>();
        }

        public long AllocationAmountId { get; set; }
        public long OrganizationId { get; set; }
        public long VariableSpecificId { get; set; }
        public long? SiteId { get; set; }
        public long WaterSourceId { get; set; }
        public long MethodId { get; set; }
        public long? PrimaryBeneficialUseId { get; set; }
        public long DataPublicationDateId { get; set; }
        public string DataPublicationDoi { get; set; }
        public string AllocationNativeId { get; set; }
        public long? AllocationApplicationDate { get; set; }
        public long AllocationPriorityDate { get; set; }
        public long? AllocationExpirationDate { get; set; }
        public string AllocationOwner { get; set; }
        public string AllocationBasisCv { get; set; }
        public string AllocationLegalStatusCv { get; set; }
        public string AllocationTypeCv { get; set; }
        public long? AllocationTimeframeStart { get; set; }
        public long? AllocationTimeframeEnd { get; set; }
        public double? AllocationCropDutyAmount { get; set; }
        public double? AllocationAmount { get; set; }
        public double? AllocationMaximum { get; set; }
        public long? PopulationServed { get; set; }
        public double? PowerGeneratedGwh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string AllocationCommunityWaterSupplySystem { get; set; }
        public string AllocationSdwisidentifier { get; set; }
        public string AllocationAssociatedWithdrawalSiteIds { get; set; }
        public string AllocationAssociatedConsumptiveUseSiteIds { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string LegacyAllocationIds { get; set; }
        public string WaterAllocationNativeUrl { get; set; }

        public virtual DateDim AllocationApplicationDateNavigation { get; set; }
        public virtual WaterAllocationBasis AllocationBasisCvNavigation { get; set; }
        public virtual DateDim AllocationExpirationDateNavigation { get; set; }
        public virtual LegalStatus AllocationLegalStatusCvNavigation { get; set; }
        public virtual DateDim AllocationPriorityDateNavigation { get; set; }
        public virtual WaterAllocationType AllocationTypeCvNavigation { get; set; }
        public virtual DateDim DataPublicationDate { get; set; }
        public virtual MethodsDim Method { get; set; }
        public virtual OrganizationsDim Organization { get; set; }
        public virtual BeneficialUsesDim PrimaryBeneficialUse { get; set; }
        public virtual SitesDim Site { get; set; }
        public virtual VariablesDim VariableSpecific { get; set; }
        public virtual WaterSourcesDim WaterSource { get; set; }
        public virtual ICollection<AllocationBridgeBeneficialUsesFact> AllocationBridgeBeneficialUsesFact { get; set; }
    }
}
