using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class BeneficialUsesDim
    {
        public BeneficialUsesDim()
        {
            AggBridgeBeneficialUsesFact = new HashSet<AggBridgeBeneficialUsesFact>();
            AggregatedAmountsFact = new HashSet<AggregatedAmountsFact>();
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
            AllocationBridgeBeneficialUsesFact = new HashSet<AllocationBridgeBeneficialUsesFact>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
            SitesBridgeBeneficialUsesFact = new HashSet<SitesBridgeBeneficialUsesFact>();
        }

        public long BeneficialUseId { get; set; }
        public string BeneficialUseUuid { get; set; }
        public string BeneficialUseCategory { get; set; }
        public string PrimaryUseCategory { get; set; }
        public string UsgscategoryNameCv { get; set; }
        public string NaicscodeNameCv { get; set; }

        public virtual Naicscode NaicscodeNameCvNavigation { get; set; }
        public virtual Usgscategory UsgscategoryNameCvNavigation { get; set; }
        public virtual ICollection<AggBridgeBeneficialUsesFact> AggBridgeBeneficialUsesFact { get; set; }
        public virtual ICollection<AggregatedAmountsFact> AggregatedAmountsFact { get; set; }
        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
        public virtual ICollection<AllocationBridgeBeneficialUsesFact> AllocationBridgeBeneficialUsesFact { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
        public virtual ICollection<SitesBridgeBeneficialUsesFact> SitesBridgeBeneficialUsesFact { get; set; }
    }
}
