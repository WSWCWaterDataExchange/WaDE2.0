using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class SitesBridgeBeneficialUsesFact
    {
        public long SiteBridgeId { get; set; }
        public long BeneficialUseId { get; set; }
        public long SiteVariableAmountId { get; set; }

        public virtual BeneficialUsesDim BeneficialUse { get; set; }
        public virtual SiteVariableAmountsFact SiteVariableAmount { get; set; }
    }
}
