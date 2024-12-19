using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

public class OverlaySearchResponse : SearchResponseBase
{
    public List<ReportingUnit> Overlays { get; set; }
}