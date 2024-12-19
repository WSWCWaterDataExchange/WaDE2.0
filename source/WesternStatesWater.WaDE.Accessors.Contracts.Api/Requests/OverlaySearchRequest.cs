using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;

public class OverlaySearchRequest : SearchRequestBase
{
    public List<string> OverlayUuids { get; set; }
    public List<string> SiteUuids { get; set; }
    public string LastKey { get; set; }
}