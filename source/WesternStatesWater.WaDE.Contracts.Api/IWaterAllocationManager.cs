using System;
using System.Collections.Generic;
using System.Text;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IWaterAllocationManager
    {
        List<AllocationAmounts> GetSiteAllocationAmounts(string variableSpecificCV, string siteUuid);
    }

    public class AllocationAmounts
    {
        public Organization Organization { get; set; }
        public Allocation Allocation { get; set; }
        public Site Site { get; set; }
        public VariableSpecific VariableSpecific { get; set; }
        public List<BeneficialUse> BeneficialUses { get; set; }
        public BeneficialUse PrimaryBeneficialUse { get; set; }
        public WaterSource WaterSource { get; set; }
        public Method Method { get; set; }
        public DateTime TimeframeStart { get; set; }
        public DateTime TimeframeEnd { get; set; }
        public DateTime DataPublicationDate { get; set; }
        public string ReportYear { get; set; }
        public double? AllocationCropDutyAmount { get; set; }
        public double? AllocationAmount { get; set; }
        public double? AllocationMaximum { get; set; }
        public double? PopulationServed { get; set; }
        public double? PowerGeneratedGWh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string AllocationCommunityWaterSupplySystem { get; set; }
        public string SDWISIdentifier { get; set; }
        //public blah InterbasinTransferToID { get; set; }
        //public blah InterbasinTransferFromID { get; set; }
        //public blah Geometry { get; set; }
    }

    public class Organization
    {
        public string OrganizationName { get; set; }

        public string OrganizationPurview { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationPhoneNumber { get; set; }
        public string OrganizationContactName { get; set; }
        public string OrganizationContactEmail { get; set; }
        public string OrganizationState { get; set; }
    }

    public class Allocation
    {
        public string NativeAllocationID { get; set; }
        public string AllocationOwner { get; set; }
        public DateTime AllocationApplicationDate { get; set; }
        public DateTime WaterAllocationPriorityDate { get; set; }
        public string AllocationLegalStatusCodeCV { get; set; }
        public DateTime AllocationExpirationDate { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string LegacyAllocationIDs { get; set; }
        public double? AllocationAcreage { get; set; }
        public string AllocationBasisCV { get; set; }
    }

    public class Site
    {
        public string SiteCode { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        //public string Geometry { get; set; }
        public string VerticalDatumEPSGCodeCV { get; set; }
        public string CoordinateMethod { get; set; }
        public string GNISCodeCV { get; set; }
        public string State { get; set; }
        public double? AllocationAcreage { get; set; }
        public string AllocationBasisCV { get; set; }
        public NHDMetadata NHDMetadata { get; set; }
        public Organization Organization { get; set; }
    }

    public class NHDMetadata
    {
        public string NHDNetworkStatusCV { get; set; }
        public string NHDProductCV { get; set; }
        public DateTime NHDUpdateDate { get; set; }
        public string NHDReachCode { get; set; }
        public string NHDMeasureNumber { get; set; }
    }

    public class VariableSpecific
    {
        public string VariableSpecificTypeCV { get; set; }
        public string VariableTypeDescription { get; set; }
        public string VariableCV { get; set; }
        public string UnitOfMeasureCV { get; set; }
        public string AggregationStatisticCV { get; set; }
        public string AggregationInterval { get; set; }
        public string AggregationIntervalUnitCV { get; set; }
        public string ReportYearStartMonth { get; set; }
        public string ReportYearTypeCV { get; set; }
        public string MaximumAmountUnitCV { get; set; }
    }

    public class BeneficialUse
    {
        public string BeneficialUseUUID { get; set; }
        public string BeneficialUseCategory { get; set; }
        public string USGSCategoryNameCV { get; set; }
        public string NAICSCodeNameCV { get; set; }
    }

    public class WaterSource
    {
        public string WaterSourceName { get; set; }
        public string WaterSourceCode { get; set; }
        public string WaterSourceTypeCV { get; set; }
        public string FreshSalineIndicatorCV { get; set; }
        //public string Geometry { get; set; }
        public Organization Organization { get; set; }
    }

    public class Method
    {
        public string MethodUUID { get; set; }
        public string MethodName { get; set; }
        public string MethodDescription { get; set; }
        public string MethodNEMILink { get; set; }
        public string ApplicableResourceType { get; set; }
        public string MethodTypeCV { get; set; }
        public string DataCoverageValue { get; set; }
        public string DataQualityValue { get; set; }
        public string DataConfidenceValue { get; set; }
    }
}
