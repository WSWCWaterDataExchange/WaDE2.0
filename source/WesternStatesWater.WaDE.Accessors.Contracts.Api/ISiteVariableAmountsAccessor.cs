using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.TimeSeries.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.TimeSeries.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface ISiteVariableAmountsAccessor
    {
        Task<SiteVariableAmounts> GetSiteVariableAmountsAsync(SiteVariableAmountsFilters filters, int startIndex,
            int recordCount);

        Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : TimeSeriesSearchRequestBase
            where TResponse : TimeSeriesSearchResponseBase;
    }
}