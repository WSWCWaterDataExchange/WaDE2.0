using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Import;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace WaDEImportFunctions
{
    public class ImportVariables
    {
        private readonly IWaterAllocationManager _waterAllocationManager;
        
        private readonly ILogger<ImportVariables> _logger;
        
        public ImportVariables(
            IWaterAllocationManager waterAllocationManager,
            ILogger<ImportVariables> logger
        )
        {
            _waterAllocationManager = waterAllocationManager;
            _logger = logger;
        }

        private const int BatchCount = 25000;
        private const string FunctionName = FunctionNames.LoadVariables;
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
            return await Import.LoadBatch(batchData, BatchFunctionName, _waterAllocationManager.LoadVariables, _logger);
        }

        [Function(CountFunctionName)]
        public async Task<int> GetCount([ActivityTrigger] string runId)
        {
            return await Import.GetCount(runId, CountFunctionName, _waterAllocationManager.GetVariablesCount, _logger);
        }
    }
}
