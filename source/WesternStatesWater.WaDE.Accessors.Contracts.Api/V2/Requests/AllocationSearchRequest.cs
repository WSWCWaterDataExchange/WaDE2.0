using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class AllocationSearchRequest : SearchRequestBase
{
    public List<string> AllocationUuid { get; set; }

    public List<string> SiteUuid { get; set; }

    public string LastKey { get; set; }
    
    public SpatialSearchCriteria GeometrySearch { get; set; }
    
    public List<string> States { get; set; }
    
    public List<string> WaterSourceTypes { get; set; }
    
    public List<string> BeneficialUses { get; set; }
}