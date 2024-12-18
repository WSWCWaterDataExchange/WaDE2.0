using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

public class SiteSearchResponse : SearchResponseBase
{
    public List<Site> Sites { get; set; }
}