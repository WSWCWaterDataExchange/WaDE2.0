using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Import;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace WaDEImportFunctions
{
    public class ImportSiteSpecificAmounts
    {
        private readonly IWaterAllocationManager _waterAllocationManager;
        
        private readonly ILogger<ImportSiteSpecificAmounts> _logger;

        public ImportSiteSpecificAmounts(
            IWaterAllocationManager waterAllocationManager,
            ILogger<ImportSiteSpecificAmounts> logger
        )
        {
            _waterAllocationManager = waterAllocationManager;
            _logger = logger;
        }

        private const int BatchCount = 25000;
        private const string FunctionName = FunctionNames.LoadSiteSpecificAmounts;
        private const string BatchFunctionName = FunctionName + "Batch";
        private const string CountFunctionName = "Get" + FunctionName + "Count";

        [Function(FunctionName)]
        public async Task<StatusHelper> LoadData([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            return await Import.LoadData(context, FunctionName, CountFunctionName, BatchFunctionName, BatchCount, _logger);
        }

        [Function(BatchFunctionName)]
        public async Task<StatusHelper> LoadBatch([ActivityTrigger] BatchData batchData)
        {
            return await Import.LoadBatch(batchData, BatchFunctionName, _waterAllocationManager.LoadSiteSpecificAmounts, _logger);
        }

        [Function(CountFunctionName)]
        public async Task<int> GetCount([ActivityTrigger] string runId)
        {
            return await Import.GetCount(runId, CountFunctionName, _waterAllocationManager.GetSiteSpecificAmountsCount, _logger);
        }
    }
}
