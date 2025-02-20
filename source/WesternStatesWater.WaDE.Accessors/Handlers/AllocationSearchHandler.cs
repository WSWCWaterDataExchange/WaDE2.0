using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Extensions;
using WesternStatesWater.WaDE.Accessors.Mapping;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class AllocationSearchHandler(IConfiguration configuration)
    : IRequestHandler<AllocationSearchRequest, AllocationSearchResponse>
{
    public async Task<AllocationSearchResponse> Handle(AllocationSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        // Performing a spatial search from AllocationAmountsFact to SitesDim causes queries to be very slow when the Geometry does not match any records.
        // To improve performance, we first query SitesDim to get the SiteIds that match the spatial search.
        List<long> siteIds = null;
        if (request.GeometrySearch?.Geometry != null && !request.GeometrySearch.Geometry.IsEmpty)
        {
            siteIds = await db.SitesDim
                .AsNoTracking()
                .Where(s =>
                    s.Geometry.Intersects(request.GeometrySearch.Geometry) ||
                    s.SitePoint.Intersects(request.GeometrySearch.Geometry))
                .ApplyLimit(request) // This must happen before OrderBy otherwise the query gets very slow and causes timeouts.
                .Select(x => x.SiteId)
                .ToListAsync();

            // If no siteIds are found, return an empty response, no need to query the rest of the data.
            if (siteIds.Count == 0)
            {
                return new AllocationSearchResponse()
                {
                    Allocations = new List<AllocationSearchItem>()
                };
            }
        }

        var allocations = await db.AllocationAmountsFact
            .AsNoTracking()
            .OrderBy(alloc => alloc.AllocationUUID)
            .ApplySearchFilters(request, siteIds)
            .ApplyLimit(request)
            .ProjectTo<AllocationSearchItem>(DtoMapper.Configuration)
            .ToListAsync();
        
        string lastUuid = null;
        // Only set lastUuid if more than one item was returned.
        // Requests looking up a specific record will only have count of 1 or 0.
        if (allocations.Count > 1)
        {
            // Get the last UUID of the page (not the first one on the next page).
            lastUuid = allocations.Count <= request.Limit ? null : allocations[^2].AllocationUuid;    
        }
        
        return new AllocationSearchResponse
        {
            LastUuid = lastUuid,
            Allocations = allocations.Take(request.Limit).ToList()
        };
    }
}