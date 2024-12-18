using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Extensions;
using WesternStatesWater.WaDE.Accessors.Mapping;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class SiteSearchHandler(IConfiguration configuration) : IRequestHandler<SiteSearchRequest, SiteSearchResponse>
{
    public async Task<SiteSearchResponse> Handle(SiteSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var dbSites = await db.SitesDim
            .AsNoTracking()
            .Include(s => s.PODSiteToPOUSitePODFact)
            .OrderBy(s => s.SiteUuid)
            .ApplyFilters(request.Filters)
            .ToListAsync();

        return new SiteSearchResponse
        {
            Sites = DtoMapper.Map<List<Site>>(dbSites)
        };
    }
}