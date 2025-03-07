namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

public class ConformanceResponse : FormattingResponseBase
{
    public required string[] ConformsTo { get; set; }
}