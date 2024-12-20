using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public interface ISiteAccessor
{
    public Task<SiteMetadata> GetSiteMetadata();
    
    public Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : SearchRequestBase
        where TResponse : SearchResponseBase;
}