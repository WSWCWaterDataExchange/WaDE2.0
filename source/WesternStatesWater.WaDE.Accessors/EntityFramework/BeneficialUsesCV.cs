﻿using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class BeneficialUsesCV
    {
        public BeneficialUsesCV()
        {
            AggBridgeBeneficialUsesFact = new HashSet<AggBridgeBeneficialUsesFact>();
            AggregatedAmountsFact = new HashSet<AggregatedAmountsFact>();
            AllocationBridgeBeneficialUsesFact = new HashSet<AllocationBridgeBeneficialUsesFact>();
            SitesBridgeBeneficialUsesFact = new HashSet<SitesBridgeBeneficialUsesFact>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
            
        }

        
        public string Name { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string State { get; set; }

        public string SourceVocabularyURI { get; set; }

        public string UsgscategoryNameCv { get; set; }
        public string NaicscodeNameCv { get; set; }

        
        public virtual ICollection<AggBridgeBeneficialUsesFact> AggBridgeBeneficialUsesFact { get; set; }
        public virtual ICollection<AggregatedAmountsFact> AggregatedAmountsFact { get; set; }
        public virtual ICollection<AllocationBridgeBeneficialUsesFact> AllocationBridgeBeneficialUsesFact { get; set; }
        public virtual ICollection<SitesBridgeBeneficialUsesFact> SitesBridgeBeneficialUsesFact { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }


    }
}
