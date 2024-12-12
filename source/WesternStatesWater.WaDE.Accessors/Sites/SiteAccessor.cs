using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Sites;

public class SiteAccessor : AccessorBase, ISiteAccessor
{
    public SiteAccessor(IRequestHandlerResolver requestHandlerResolver) : base(requestHandlerResolver)
    {
    }

    public async Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : SiteSearchRequestBase
        where TResponse : SiteSearchResponseBase
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}