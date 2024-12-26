using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

public class CollectionsMetadataGetResponse : MetadataLoadResponseBase
{
    public required Link[] Links { get; set; }
    public required Collection[] Collections { get; set; }
}