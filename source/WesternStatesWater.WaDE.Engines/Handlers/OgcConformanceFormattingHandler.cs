using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public class OgcConformanceFormattingHandler(IConfiguration configuration) : OgcFormattingHandlerBase(configuration), IRequestHandler<ConformanceRequest, ConformanceResponse>
{
    public Task<ConformanceResponse> Handle(ConformanceRequest request)
    {
        return Task.FromResult(new ConformanceResponse
        {
            ConformsTo =
            [
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core", // OGC Features Core
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30", // Open API 3.0
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/geojson", // GeoJSON
                "http://www.opengis.net/spec/ogcapi-edr-1/1.0/conf/core", // OGC EDR Core
                "http://www.opengis.net/spec/ogcapi-edr-1/1.0/conf/collections", // OGC EDR Collections
                "http://www.opengis.net/spec/ogcapi-edr-1/1.0/conf/areas", // OGC EDR Areas
            ]
        });
    }
}