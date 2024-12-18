namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;

public class SiteSearchRequest : SearchRequestBase
{
    public SiteFilters Filters { get; set; }
}