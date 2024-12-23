namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public abstract class FeaturesRequestBase : FormattingRequestBase
{
    public int Limit { get; set; }
}