using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Allocations.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Allocations.Responses;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Allocations.Handlers;

internal class AllocationExtentSearchHandler : IRequestHandler<AllocationExtentSearchRequest, AllocationExtentSearchResponse>
{
    public Task<AllocationExtentSearchResponse> Handle(AllocationExtentSearchRequest request)
    {
        throw new System.NotImplementedException();
    }
}