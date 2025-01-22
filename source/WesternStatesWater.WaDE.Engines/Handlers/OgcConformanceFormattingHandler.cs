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
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core",
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30",
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/geojson"
            ]
        });
    }
}