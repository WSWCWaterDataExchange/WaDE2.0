namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

public class ConformanceMetadataGetResponse : MetadataLoadResponseBase
{
    public required string[] ConformsTo { get; set; }
}