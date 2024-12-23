using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Contracts.Api;

public interface ISiteManager
{
    public Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : FeaturesSearchRequestBase
        where TResponse : FeaturesSearchResponseBase;
}