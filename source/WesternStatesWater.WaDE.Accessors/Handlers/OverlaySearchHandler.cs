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

        var overlays =await  db.OverlayDim
            .AsNoTracking()
            .OrderBy(o => o.OverlayUuid)
            .ApplySearchFilters(request)
            .ApplyLimit(request)
            .ProjectTo<OverlaySearchItem>(DtoMapper.Configuration)
            .ToListAsync();
        
        string lastUuid = null;
        // Only set lastUuid if more than one item was returned.
        // Requests looking up a specific record will only have count of 1 or 0.
        if (overlays.Count > 1)
        {
            // Get the last UUID of the page (not the first one on the next page).
            lastUuid = overlays.Count <= request.Limit ? null : overlays[^2].OverlayUuid;
        }
        
        return new OverlaySearchResponse
        {
            LastUuid = lastUuid,
            Overlays = overlays.Take(request.Limit).ToList()
        };
    }
}