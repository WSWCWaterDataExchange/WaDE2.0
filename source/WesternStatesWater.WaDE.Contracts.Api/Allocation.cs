using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class Allocation
    {
        public string NativeAllocationID { get; set; }
        public string AllocationOwner { get; set; }
        public DateTime AllocationApplicationDate { get; set; }
        public DateTime WaterAllocationPriorityDate { get; set; }
        public string AllocationLegalStatusCodeCV { get; set; }
        public DateTime? AllocationExpirationDate { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string LegacyAllocationIDs { get; set; }
        public double? AllocationAcreage { get; set; }
        public string AllocationBasisCV { get; set; }
    }
}
