using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Import;

namespace WaDEImportFunctions
{
    public class ImportOrganizations
    {
        public ImportOrganizations(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }

        private const int BatchCount = 25000;
        private const string FunctionName = FunctionNames.LoadOrganizations;
        private const string BatchFunctionName = FunctionName + "Batch";
        private const string CountFunctionName = "Get" + FunctionName + "Count";

        [FunctionName(FunctionName)]
        public async Task<StatusHelper> LoadData([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            return await Import.LoadData(context, FunctionName, CountFunctionName, BatchFunctionName, BatchCount, log);
        }

        [FunctionName(BatchFunctionName)]
        public async Task<StatusHelper> LoadBatch([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            return await Import.LoadBatch(context, BatchFunctionName, WaterAllocationManager.LoadOrganizations, log);
        }

        [FunctionName(CountFunctionName)]
        public async Task<int> GetCount([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            return await Import.GetCount(context, CountFunctionName, WaterAllocationManager.GetOrganizationsCount, log);
        }
    }
}
