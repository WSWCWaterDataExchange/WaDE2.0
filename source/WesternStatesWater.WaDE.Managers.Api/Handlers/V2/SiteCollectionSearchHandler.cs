using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;
using WesternStatesWater.WaDE.Accessors.Sites;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class SiteCollectionSearchHandler(IConfiguration configuration, ISiteAccessor siteAccessor)
    : IRequestHandler<SiteCollectionSearchRequest, SiteCollectionSearchResponse>
{
    public readonly string ServerUrl = $"{configuration["ServerUrl"]}/api/v2";

    public async Task<SiteCollectionSearchResponse> Handle(SiteCollectionSearchRequest request)
    {
        var dtoRequest = DtoMapper.Map<SiteExtentSearchRequest>(request);
        var siteExtentResponse =
            await siteAccessor.Search<SiteExtentSearchRequest, SiteExtentSearchResponse>(dtoRequest);

        var extent = DtoMapper.Map<Extent>(siteExtentResponse);
        var response = new SiteCollectionSearchResponse
        {
            Collections = new[]
            {
                new Collection
                {
                    Id = "sites",
                    Extent = extent, // add CRS when spatial provided... http://www.opengis.net/def/crs/OGC/1.3/CRS84
                    Links =
                    [
                        new Link
                        {
                            Type = "text/html",
                            Href = "http://localhost:7071/api/swagger/ui",
                            Rel = "root",
                            Title = "Landing page"
                        },
                        new Link
                        {
                            Type = "application/json",
                            Href = "http://localhost:7071/api/swagger.json",
                            Rel = "root",
                            Title = "Landing page"
                        },
                        new Link
                        {
                            Type = "application/json",
                            Rel = "self",
                            Href = $"{ServerUrl}/collections/sites",
                        },
                        new Link
                        {
                            Type = "application/geo+json",
                            Rel = "items",
                            Href = $"{ServerUrl}/collections/sites/items",
                        }
                    ],
                    ItemType = "feature",
                    Crs =
                    [
                        "http://www.opengis.net/def/crs/OGC/1.3/CRS84"
                    ],
                    StorageCrs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84"
                },
                new Collection
                {
                    Id = "waterRights",
                    Links = []
                },
                new Collection
                {
                    Id = "overlays",
                    Extent = extent, // bbox
                    Links = []
                },
                new Collection
                {
                    Id = "allocations",
                    Extent = extent, // interval could be DatePublicDate...
                    Links = []
                }
            },
            Links =
            [
                new Link
                {
                    Href = $"{ServerUrl}/api/v2/collections",
                    Rel = "self",
                    Type = "application/json",
                }
            ]
        };
        return response;
    }
}