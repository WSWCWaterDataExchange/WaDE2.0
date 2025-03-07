using WesternStatesWater.WaDE.Common.Ogc;

namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

public class CollectionsResponse : FormattingResponseBase
{
    public required Link[] Links { get; set; }
    public required Collection[] Collections { get; set; }
}