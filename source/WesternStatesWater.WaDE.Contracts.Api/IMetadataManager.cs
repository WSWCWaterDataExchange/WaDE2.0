using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Contracts.Api;

public interface IMetadataManager
{
    Task<TResponse> Load<TRequest, TResponse>(TRequest request)
        where TRequest : MetadataLoadRequestBase
        where TResponse : MetadataLoadResponseBase;
}