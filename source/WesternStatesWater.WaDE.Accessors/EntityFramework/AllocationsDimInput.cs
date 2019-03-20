using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AllocationsDimInput
    {
        public string AllocationNativeId { get; set; }
        public string AllocationOwner { get; set; }
        public string AllocationBasisCv { get; set; }
        public string AllocationLegalStatusCv { get; set; }
        public long? AllocationApplicationDate { get; set; }
        public long AllocationPriorityDate { get; set; }
        public long? AllocationExpirationDate { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string LegacyAllocationIds { get; set; }
    }
}
