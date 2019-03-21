using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AllocationsDim
    {
        public AllocationsDim()
        {
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
        }

        public long AllocationId { get; set; }
        public string AllocationUuid { get; set; }
        public string AllocationNativeId { get; set; }
        public string AllocationOwner { get; set; }
        public string AllocationBasisCv { get; set; }
        public string AllocationLegalStatusCv { get; set; }
        public string WaterRightTypeCv { get; set; }
        public long? AllocationApplicationDate { get; set; }
        public long AllocationPriorityDate { get; set; }
        public long? AllocationExpirationDate { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string LegacyAllocationIds { get; set; }

        public virtual DateDim AllocationApplicationDateNavigation { get; set; }
        public virtual DateDim AllocationExpirationDateNavigation { get; set; }
        public virtual DateDim AllocationPriorityDateNavigation { get; set; }
        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
    }
}
