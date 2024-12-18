using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Accessors;

public class SiteAccessor(IAccessorRequestHandlerResolver requestHandlerResolver, IConfiguration configuration)
    : AccessorBase(requestHandlerResolver), ISiteAccessor
{
    public async Task<SiteMetadata> GetSiteMetadata()
    {
        // Due to the calculating boundary box for all sites is an expensive db operation,
        // we are hardcoding the boundary box.
        // Note: If we do calculate bound box, be aware not all site geometries are valid.

        // View of bounding box: https://linestrings.com/bbox/#-180,18,-93,72
        return await Task.FromResult(new SiteMetadata
        {
            BoundaryBox = new BoundaryBox
            {
                Crs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                MinX = -180,
                MinY = 18,
                MaxX = -93,
                MaxY = 72
            }
        });
    }

    public async Task<TResponse> Search<TRequest, TResponse>(TRequest request) where TRequest : SearchRequestBase
        where TResponse : SearchResponseBase
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}