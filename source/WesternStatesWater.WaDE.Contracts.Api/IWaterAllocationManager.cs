using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IWaterAllocationManager
    {
        Task<OgcApi.CollectionsResponse> Collections();
    }
}