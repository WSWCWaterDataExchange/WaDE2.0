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

    public async Task<SiteSearchResponseBase> Search(SiteSearchRequestBase request)
    {
        return await ExecuteAsync<SiteSearchRequestBase, SiteSearchResponseBase>(request);
    }
}