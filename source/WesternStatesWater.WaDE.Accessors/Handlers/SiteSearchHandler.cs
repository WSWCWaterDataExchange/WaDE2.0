using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Extensions;
using WesternStatesWater.WaDE.Accessors.Mapping;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class SiteSearchHandler(IConfiguration configuration) : IRequestHandler<SiteSearchRequest, SiteSearchResponse>
{
    public async Task<SiteSearchResponse> Handle(SiteSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var sites = await db.SitesDim
            .AsNoTracking()
            .OrderBy(s => s.SiteUuid)
            .ApplySearchFilters(request)
            .ApplyLimit(request)
            .ProjectTo<SiteSearchItem>(DtoMapper.Configuration)
            .ToListAsync();
        
        string lastUuid = null;
        // Only set lastUuid if more than one item was returned.
        // Requests looking up a specific record will only have count of 1 or 0.
        if (sites.Count > 1)
        {
            // Get the last UUID of the page (not the first one on the next page).
            lastUuid = sites.Count <= request.Limit ? null : sites[^2].SiteUuid;    
        }

        return new SiteSearchResponse
        {
            LastUuid = lastUuid,
            Sites = sites.Take(request.Limit).ToList()
        };
    }
}