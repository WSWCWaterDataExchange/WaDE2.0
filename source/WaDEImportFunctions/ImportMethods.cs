using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Import;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace WaDEImportFunctions
{
    public class ImportMethods
    {
        public ImportMethods(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }

        private const int BatchCount = 25000;
        private const string FunctionName = FunctionNames.LoadMethods;
        private const string BatchFunctionName = FunctionName + "Batch";
        private const string CountFunctionName = "Get" + FunctionName + "Count";

        [Function(FunctionName)]
        public async Task<StatusHelper> LoadData([OrchestrationTrigger] TaskOrchestrationContext context, ILogger log)
        {
            return await Import.LoadData(context, FunctionName, CountFunctionName, BatchFunctionName, BatchCount, log);
        }

        [Function(BatchFunctionName)]
        public async Task<StatusHelper> LoadBatch([ActivityTrigger] BatchData batchData, ILogger log)
        {
            return await Import.LoadBatch(batchData, BatchFunctionName, WaterAllocationManager.LoadMethods, log);
        }

        [Function(CountFunctionName)]
        public async Task<int> GetCount([ActivityTrigger] string runId, ILogger log)
        {
            return await Import.GetCount(runId, CountFunctionName, WaterAllocationManager.GetMethodsCount, log);
        }
    }
}
