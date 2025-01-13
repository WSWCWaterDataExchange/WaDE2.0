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

public class AllocationSearchHandler(IConfiguration configuration)
    : IRequestHandler<AllocationSearchRequest, AllocationSearchResponse>
{
    public async Task<AllocationSearchResponse> Handle(AllocationSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var query = db.AllocationAmountsFact
            .AsNoTracking()
            .OrderBy(alloc => alloc.AllocationUUID)
            .ApplySearchFilters(request)
            .AsQueryable();
        
        // Fetch the number of matched records
        var count = await query.CountAsync();

        var allocations = await query
            .ApplyLimit(request)
            .ProjectTo<AllocationSearchItem>(DtoMapper.Configuration)
            .ToListAsync();

        return new AllocationSearchResponse
        {
            MatchedCount = count,
            LastUuid = allocations.Count <= request.Limit ? null : allocations[^1].AllocationUUID,
            Allocations = allocations.Take(request.Limit).ToList()
        };
    }
}