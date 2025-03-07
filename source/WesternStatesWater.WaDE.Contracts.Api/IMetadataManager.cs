using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Contracts.Api;

public interface IMetadataManager
{
    Task<TResponse> Load<TRequest, TResponse>(TRequest request)
        where TRequest : MetadataLoadRequestBase
        where TResponse : MetadataLoadResponseBase;
}