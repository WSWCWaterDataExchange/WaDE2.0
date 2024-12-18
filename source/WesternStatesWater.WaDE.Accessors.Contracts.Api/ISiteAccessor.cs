using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public interface ISiteAccessor
{
    public Task<SiteMetadata> GetSiteMetadata();
    
    public Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : SearchRequestBase
        where TResponse : SearchResponseBase;
}