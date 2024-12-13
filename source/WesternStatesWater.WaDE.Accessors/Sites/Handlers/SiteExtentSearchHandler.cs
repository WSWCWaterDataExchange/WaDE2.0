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

internal class SiteExtentSearchHandler(IConfiguration configuration)
    : IRequestHandler<SiteExtentSearchRequest, SiteExtentSearchResponse>
{
    public async Task<SiteExtentSearchResponse> Handle(SiteExtentSearchRequest request)
    {
        return await Task.FromResult(new SiteExtentSearchResponse
        {
            // Due to the calculating boundary box for all sites is an expensive db operation,
            // we are hardcoding an approximate United States boundary box
            // Note: If we do calculate bound box, be aware not all site geometries are valid.
            Extent = new Extent()
            {
                Spatial = new Spatial()
                {
                    Bbox = [[-125, -100, 32, 49]],
                    Crs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                }
            }
        });
    }
}