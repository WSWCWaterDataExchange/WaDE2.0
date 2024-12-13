using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Overlays.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Overlays.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Overlays.Handlers;

internal class OverlayExtentSearchHandler(IConfiguration configuration)
    : IRequestHandler<OverlayExtentSearchRequest, OverlayExtentSearchResponse>
{
    public async Task<OverlayExtentSearchResponse> Handle(OverlayExtentSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);
        var result = await db.Database.SqlQueryRaw<BoundaryBox>(@"
            SELECT 
                MIN(envelope.STPointN(1).STX) AS MinX,
                MIN(envelope.STPointN(1).STY) AS MinY,
                MAX(envelope.STPointN(3).STX) AS MaxX,
                MAX(envelope.STPointN(3).STY) AS MaxY
            FROM 
                (SELECT 
                    CASE Geometry.STIsValid()
                        WHEN 1 THEN Geometry.STEnvelope()
                        ELSE null
                    END AS envelope 
                FROM Core.ReportingUnits_dim) AS subquery
            WHERE subquery.envelope IS NOT NULL;")
            .ToListAsync();

        return new OverlayExtentSearchResponse();
        // return new OverlayExtentSearchResponse
        // {
        //     Extent = new Extent
        //     {
        //         Spatial = new Spatial
        //         {
        //             Bbox = [[result.MinX, result.MinY, result.MaxX, result.MaxY]],
        //             Crs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84"
        //         }
        //     }
        // };
    }
}