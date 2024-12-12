using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;

namespace WesternStatesWater.WaDE.Accessors.Sites;

public interface ISiteAccessor
{
    Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : SiteSearchRequestBase
        where TResponse : SiteSearchResponseBase;
}