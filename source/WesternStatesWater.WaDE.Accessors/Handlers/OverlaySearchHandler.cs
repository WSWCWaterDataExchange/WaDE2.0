using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Mapping;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class OverlaySearchHandler(IConfiguration configuration)
    : IRequestHandler<OverlaySearchRequest, OverlaySearchResponse>
{
    public async Task<OverlaySearchResponse> Handle(OverlaySearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var query = db.RegulatoryOverlayDim
            .AsNoTracking()
            .OrderBy(o => o.RegulatoryOverlayUuid)
            .AsQueryable();
        
        if (request.OverlayUuids != null && request.OverlayUuids.Count != 0)
        {
            query = query.Where(o => request.OverlayUuids.Contains(o.RegulatoryOverlayUuid));
        }

        if (request.SiteUuids != null && request.SiteUuids.Count != 0)
        {
            query = query.Where(o =>
                o.RegulatoryOverlayBridgeSitesFact.Any(sf =>
                    request.SiteUuids.Contains(sf.Site.SiteUuid)));
        }

        if (request.FilterBoundary != null && !request.FilterBoundary.IsEmpty)
        {
            query = query.Where(o => o.RegulatoryReportingUnitsFact.Any(fact => fact.ReportingUnit.Geometry.Intersects(request.FilterBoundary)));
        }

        if (!string.IsNullOrWhiteSpace(request.LastKey))
        {
            query = query.Where(o => o.RegulatoryOverlayUuid.CompareTo(request.LastKey) > 0);
        }

        query = query.Take(request.Limit);

        var overlays = await query
            .ProjectTo<OverlaySearchItem>(DtoMapper.Configuration)
            .ToListAsync();

        return new OverlaySearchResponse
        {
            Overlays = overlays
        };
    }
}