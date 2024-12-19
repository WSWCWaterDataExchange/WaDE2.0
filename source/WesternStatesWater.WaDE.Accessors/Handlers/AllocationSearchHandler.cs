using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;
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
            .Include(alloc => alloc.AllocationBridgeSitesFact)
            .ThenInclude(bridge => bridge.Site)
            .Include(alloc => alloc.AllocationBridgeBeneficialUsesFact)
            .ThenInclude(bridge => bridge.BeneficialUse)
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

        var dbAllocations = await query.ToListAsync();

        return new AllocationSearchResponse
        {
            Allocations = dbAllocations.Map<List<Allocation>>()
        };
    }
}