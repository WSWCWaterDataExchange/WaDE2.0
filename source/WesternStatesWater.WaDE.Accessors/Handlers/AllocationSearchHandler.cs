using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Mapping;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class AllocationSearchHandler(IConfiguration configuration)
    : IRequestHandler<AllocationSearchRequest, AllocationSearchResponse>
{
    public async Task<AllocationSearchResponse> Handle(AllocationSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var query = db.AllocationAmountsFact
            .AsNoTracking()
            .OrderBy(alloc => alloc.AllocationUUID)
            .AsQueryable();

        if (request.AllocationUuid != null && request.AllocationUuid.Any())
        {
            query = query.Where(x => request.AllocationUuid.Contains(x.AllocationUUID));
        }

        if (request.SiteUuid != null && request.SiteUuid.Count != 0)
        {
            query = query.Where(x => x.AllocationBridgeSitesFact.Any(
                bridge => request.SiteUuid.Contains(bridge.Site.SiteUuid)));
        }

        if (!string.IsNullOrWhiteSpace(request.LastKey))
        {
            query = query.Where(x => x.AllocationUUID.CompareTo(request.LastKey) > 0);
        }

        query = query.Take(request.Limit);

        var allocations = await query
            .ProjectTo<AllocationSearchItem>(DtoMapper.Configuration)
            .ToListAsync();

        return new AllocationSearchResponse
        {
            Allocations = allocations
        };
    }
}