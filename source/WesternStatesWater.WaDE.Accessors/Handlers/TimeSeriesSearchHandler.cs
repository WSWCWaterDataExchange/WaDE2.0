using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Mapping;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class TimeSeriesSearchHandler(IConfiguration configuration)
    : IRequestHandler<TimeSeriesSearchRequest, TimeSeriesSearchResponse>
{
    public async Task<TimeSeriesSearchResponse> Handle(TimeSeriesSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var query = db.SiteVariableAmountsFact
            .AsNoTracking()
            .OrderBy(ts => ts.SiteVariableAmountId)
            .AsQueryable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(x =>
                (request.StartDate >= x.TimeframeStartNavigation.Date && request.StartDate <= x.TimeframeEndNavigation.Date) ||
                request.StartDate <= x.TimeframeStartNavigation.Date);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(x =>
                (request.EndDate >= x.TimeframeStartNavigation.Date && request.EndDate <= x.TimeframeEndNavigation.Date) ||
                x.TimeframeEndNavigation.Date <= request.EndDate );
        }

        if (request.SiteUuids != null && request.SiteUuids.Count != 0)
        {
            query = query.Where(x => request.SiteUuids.Contains(x.Site.SiteUuid));
        }

        if (request.LastKey.HasValue)
        {
            query = query.Where(x => x.SiteVariableAmountId > request.LastKey);
        }

        query = query.Take(request.Limit);

        var timeSeries = await query
            .ProjectTo<TimeSeriesSearchItem>(DtoMapper.Configuration)
            .ToListAsync();

        return new TimeSeriesSearchResponse
        {
            TimeSeries = timeSeries
        };
    }
}