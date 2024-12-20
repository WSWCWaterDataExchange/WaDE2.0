using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

public class SiteSearchResponse : SearchResponseBase
{
    public List<SiteSearchItem> Sites { get; set; }
}