using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;

public class OverlaySearchRequest : SearchRequestBase
{
    public List<string> ReportingUnitUuids { get; set; }
    public List<string> OverlayUuids { get; set; }
    public List<string> SiteUuids { get; set; }
    public Polygon FilterBoundary { get; set; }
    public string LastKey { get; set; }
}