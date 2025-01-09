using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class AllocationSearchRequest : SearchRequestBase
{
    public List<string> AllocationUuid { get; set; }

    public List<string> SiteUuid { get; set; }

    public string LastKey { get; set; }
    
    public Geometry FilterBoundary { get; set; }
}