using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Managers.Api.Handlers;

namespace WesternStatesWater.WaDE.Managers.Api;

internal partial class WaterResourceManager : ManagerBase, ManagerApi.IMetadataManager
{
    private readonly IFormattingEngine _formattingEngine;
    
    private readonly AccessorApi.IAggregatedAmountsAccessor _aggregratedAmountsAccessor;

    private readonly AccessorApi.IRegulatoryOverlayAccessor _regulatoryOverlayAccessor;

    private readonly AccessorApi.ISiteVariableAmountsAccessor _siteVariableAmountsAccessor;

    private readonly AccessorApi.IWaterAllocationAccessor _waterAllocationAccessor;

    public WaterResourceManager(
        IManagerRequestHandlerResolver requestHandlerResolver,
        IFormattingEngine formattingEngine,
        AccessorApi.IAggregatedAmountsAccessor aggregratedAmountsAccessor,
        AccessorApi.IRegulatoryOverlayAccessor regulatoryOverlayAccessor,
        AccessorApi.ISiteVariableAmountsAccessor siteVariableAmountsAccessor,
        AccessorApi.IWaterAllocationAccessor waterAllocationAccessor,
        ILogger<WaterResourceManager> logger) 
        : base(logger, requestHandlerResolver)
    {
        _formattingEngine = formattingEngine;
        _aggregratedAmountsAccessor = aggregratedAmountsAccessor;
        _regulatoryOverlayAccessor = regulatoryOverlayAccessor;
        _siteVariableAmountsAccessor = siteVariableAmountsAccessor;
        _waterAllocationAccessor = waterAllocationAccessor;
    }

    public async Task<TResponse> Load<TRequest, TResponse>(TRequest request) 
        where TRequest : MetadataLoadRequestBase 
        where TResponse : MetadataLoadResponseBase
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}