using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

public class CollectionMetadataResponse : MetadataLoadResponseBase
{
    public Collection Collection { get; set; }
}