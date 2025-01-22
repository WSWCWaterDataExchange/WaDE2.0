using WesternStatesWater.WaDE.Common.Ogc;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

public class DiscoveryMetadataGetResponse : MetadataLoadResponseBase
{
    public required Link[] Links { get; set; } = [];
    public string Title { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }
}