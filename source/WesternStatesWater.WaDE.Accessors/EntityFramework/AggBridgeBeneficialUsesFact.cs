using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AggBridgeBeneficialUsesFact
    {
        public long AggBridgeId { get; set; }
        public long BeneficialUseId { get; set; }
        public long AggregatedAmountId { get; set; }

        public virtual AggregatedAmountsFact AggregatedAmount { get; set; }
        public virtual BeneficialUsesDim BeneficialUse { get; set; }
    }
}
