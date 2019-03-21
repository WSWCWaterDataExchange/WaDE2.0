using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class SiteVariableAmountsFact
    {
        public SiteVariableAmountsFact()
        {
            SitesBridgeBeneficialUsesFact = new HashSet<SitesBridgeBeneficialUsesFact>();
        }

        public long SiteVariableAmountId { get; set; }
        public long OrganizationId { get; set; }
        public long? AllocationId { get; set; }
        public long SiteId { get; set; }
        public long VariableSpecificId { get; set; }
        public long BeneficialUseId { get; set; }
        public long WaterSourceId { get; set; }
        public long MethodId { get; set; }
        public long TimeframeStart { get; set; }
        public long TimeframeEnd { get; set; }
        public long DataPublicationDate { get; set; }
        public string ReportYear { get; set; }
        public double Amount { get; set; }
        public double? PopulationServed { get; set; }
        public double? PowerGeneratedGwh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string IrrigationMethodCv { get; set; }
        public string CropTypeCv { get; set; }
        public string InterbasinTransferFromId { get; set; }
        public string InterbasinTransferToId { get; set; }
        public IGeometry Geometry { get; set; }

        public virtual AllocationsDim Allocation { get; set; }
        public virtual BeneficialUsesDim BeneficialUse { get; set; }
        public virtual DateDim DataPublicationDateNavigation { get; set; }
        public virtual MethodsDim Method { get; set; }
        public virtual OrganizationsDim Organization { get; set; }
        public virtual ReportYearCv ReportYearNavigation { get; set; }
        public virtual SitesDim Site { get; set; }
        public virtual DateDim TimeframeEndNavigation { get; set; }
        public virtual DateDim TimeframeStartNavigation { get; set; }
        public virtual VariablesDim VariableSpecific { get; set; }
        public virtual WaterSourcesDim WaterSource { get; set; }
        public virtual ICollection<SitesBridgeBeneficialUsesFact> SitesBridgeBeneficialUsesFact { get; set; }
    }
}
