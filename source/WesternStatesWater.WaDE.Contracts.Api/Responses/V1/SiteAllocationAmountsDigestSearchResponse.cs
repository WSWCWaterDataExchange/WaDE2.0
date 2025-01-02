using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

public class SiteAllocationAmountsDigestSearchResponse : WaterResourceLoadResponseBase
{
    public required IEnumerable<WaterAllocationDigest> WaterAllocationDigests { get; init; }
}