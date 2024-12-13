using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.TimeSeries.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.TimeSeries.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.TimeSeries.Handlers;

internal class TimeSeriesExtentSearchHandler(IConfiguration configuration)
    : IRequestHandler<TimeSeriesExtentSearchRequest, TimeSeriesExtentSearchResponse>
{
    public async Task<TimeSeriesExtentSearchResponse> Handle(TimeSeriesExtentSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);
        var minStartDate = await db.SiteVariableAmountsFact.MinAsync(f => (DateTime?) f.TimeframeStartNavigation.Date);
        var maxEndDate = await db.SiteVariableAmountsFact.MaxAsync(f => (DateTime?) f.TimeframeEndNavigation.Date);

        return new TimeSeriesExtentSearchResponse
        {
            Extent = new Extent()
            {
                Temporal = new Temporal
                {
                    Interval = [[minStartDate?.ToString("o"), maxEndDate?.ToString("o")]]
                }
            }
        };
    }
}