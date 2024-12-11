using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Sites.Handlers;

public class SiteExtentLookupHandler(IConfiguration configuration)
    : IRequestHandler<SiteExtentSearchRequest, SiteExtentSearchResponse>
{
    public async Task<SiteExtentSearchResponse> Handle(SiteExtentSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);
        var minStartDate = await db.SiteVariableAmountsFact.MinAsync(f => (DateTime?)f.TimeframeStartNavigation.Date);
        var maxEndDate = await db.SiteVariableAmountsFact.MaxAsync(f => (DateTime?)f.TimeframeEndNavigation.Date);

        return await Task.FromResult(new SiteExtentSearchResponse
        {
            // Due to the calculating boundary box for all sites is an expensive db operation,
            // we are hardcoding an approximate United States boundary box
            // Note: If we do calculate bound box, be aware not all site geometries are valid.
            BoundaryBox = new BoundaryBox
            {
                MinX = -125,
                MaxX = 32,
                MinY = -100,
                MaxY = 49
            },
            TimeframeStart = minStartDate,
            TimeframeEnd = maxEndDate
        });
    }
}