using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

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
        public long AllocationId { get; set; }
        public long SiteId { get; set; }
        public long VariableSpecificId { get; set; }
        public long WaterSourceId { get; set; }
        public long MethodId { get; set; }
        public long TimeframeStartDateId { get; set; }
        public long TimeframeEndDateId { get; set; }
        public long? DataPublicationDateId { get; set; }
        public string ReportYear { get; set; }
        public double? AllocationCropDutyAmount { get; set; }
        public double? AllocationAmount { get; set; }
        public double? AllocationMaximum { get; set; }
        public double? PopulationServed { get; set; }
        public double? PowerGeneratedGwh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string AllocationCommunityWaterSupplySystem { get; set; }
        public string Sdwisidentifier { get; set; }
        public IGeometry Geometry { get; set; }
        public long? PrimaryBeneficialUseId { get; set; }

        public virtual AllocationsDim Allocation { get; set; }
        public virtual DateDim DataPublicationDate { get; set; }
        public virtual MethodsDim Method { get; set; }
        public virtual OrganizationsDim Organization { get; set; }
        public virtual BeneficialUsesDim PrimaryBeneficialUse { get; set; }
        public virtual ReportYearCv ReportYearNavigation { get; set; }
        public virtual SitesDim Site { get; set; }
        public virtual DateDim TimeframeEndDate { get; set; }
        public virtual DateDim TimeframeStartDate { get; set; }
        public virtual VariablesDim VariableSpecific { get; set; }
        public virtual WaterSourcesDim WaterSource { get; set; }
        public virtual ICollection<AllocationBridgeBeneficialUsesFact> AllocationBridgeBeneficialUsesFact { get; set; }
    }
}
