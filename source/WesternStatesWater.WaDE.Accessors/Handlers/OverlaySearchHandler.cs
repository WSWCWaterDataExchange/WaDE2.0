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

public class OverlaySearchHandler(IConfiguration configuration)
    : IRequestHandler<OverlaySearchRequest, OverlaySearchResponse>
{
    public async Task<OverlaySearchResponse> Handle(OverlaySearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var overlays =await  db.RegulatoryOverlayDim
            .AsNoTracking()
            .OrderBy(o => o.RegulatoryOverlayUuid)
            .ApplySearchFilters(request)
            .ApplyLimit(request)
            .ProjectTo<OverlaySearchItem>(DtoMapper.Configuration)
            .ToListAsync();
        
        return new OverlaySearchResponse
        {
            LastUuid = overlays.Count <= request.Limit ? null : overlays[^1].OverlayUuid,
            Overlays = overlays.Take(request.Limit).ToList()
        };
    }
}