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

        var query = db.SitesDim
            .AsNoTracking()
            .OrderBy(s => s.SiteUuid)
            .ApplySearchFilters(request)
            .AsQueryable();

        // Fetch the number of matched records
        var count = await query.CountAsync();

        var sites = await query
            .ApplyLimit(request)
            .ProjectTo<SiteSearchItem>(DtoMapper.Configuration)
            .ToListAsync();

        // Get the last UUID of the page (not the first one on the next page).
        var lastUuid = count <= request.Limit ? null : sites[^2].SiteUuid;

        return new SiteSearchResponse
        {
            MatchedCount = count,
            LastUuid = lastUuid,
            Sites = sites.Take(request.Limit).ToList()
        };
    }
}