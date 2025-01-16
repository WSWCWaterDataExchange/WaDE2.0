using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class AllocationSearchItem
{
    public long AllocationAmountId { get; set; }
    public string AllocationNativeID { get; set; }
    public string AllocationOwner { get; set; }
    public DateTime? AllocationApplicationDate { get; set; }
    public DateTime? AllocationPriorityDate { get; set; }
    public string AllocationLegalStatusCodeCV { get; set; }
    public DateTime? AllocationExpirationDate { get; set; }
    public string AllocationChangeApplicationIndicator { get; set; }
    public string LegacyAllocationIDs { get; set; }
    public double? AllocationAcreage { get; set; }
    public string AllocationBasisCV { get; set; }
    public string TimeframeStart { get; set; }
    public string TimeframeEnd { get; set; }
    public DateTime? DataPublicationDate { get; set; }
    public double? AllocationCropDutyAmount { get; set; }
    public double? AllocationFlow_CFS { get; set; }
    public double? AllocationVolume_AF { get; set; }
    public string AllocationUUID { get; set; }
    public long? PopulationServed { get; set; }
    public double? GeneratedPowerCapacityMW { get; set; }
    public string AllocationCommunityWaterSupplySystem { get; set; }
    public string AllocationSDWISIdentifier { get; set; }
    public string Method { get; set; }
    public string MethodUUID { get; set; }
    public string VariableSpecificTypeCV { get; set; }
    public List<string> States { get; set; }
    public List<string> SitesUUIDs { get; set; }
    public List<string> BeneficialUses { get; set; }
    public string Organization { get; set; }
    public bool ExemptOfVolumeFlowPriority { get; set; }
}