using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AggregatedAmountsFact
    {
        public AggregatedAmountsFact()
        {
            AggBridgeBeneficialUsesFact = new HashSet<AggBridgeBeneficialUsesFact>();
        }

        public long AggregatedAmountId { get; set; }
        public long OrganizationId { get; set; }
        public long ReportingUnitId { get; set; }
        public long VariableSpecificId { get; set; }
        public long BeneficialUseId { get; set; }
        public long WaterSourceId { get; set; }
        public long MethodId { get; set; }
        public long? TimeframeStartId { get; set; }
        public long? TimeframeEndId { get; set; }
        public long? DataPublicationDate { get; set; }
        public string DataPublicationDOI { get; set; }
        public string ReportYearCV { get; set; }
        public double Amount { get; set; }
        public double? PopulationServed { get; set; }
        public double? PowerGeneratedGwh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string InterbasinTransferToId { get; set; }
        public string InterbasinTransferFromId { get; set; }

        public virtual BeneficialUsesDim BeneficialUse { get; set; }
        public virtual DateDim DataPublicationDateNavigation { get; set; }
        public virtual MethodsDim Method { get; set; }
        public virtual OrganizationsDim Organization { get; set; }
        public virtual ReportYearCv ReportYearNavigation { get; set; }
        public virtual ReportingUnitsDim ReportingUnit { get; set; }
        public virtual DateDim TimeframeEnd { get; set; }
        public virtual DateDim TimeframeStart { get; set; }
        public virtual VariablesDim VariableSpecific { get; set; }
        public virtual WaterSourcesDim WaterSource { get; set; }
        public virtual ICollection<AggBridgeBeneficialUsesFact> AggBridgeBeneficialUsesFact { get; set; }
    }
}
