using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Import;

namespace WaDEImportFunctions
{
    public class WaterAllocationImport
    {
        public WaterAllocationImport(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }

        [FunctionName(FunctionNames.LoadWaterAllocationDataOrchestration)]
        public async Task<IActionResult> LoadWaterAllocationDataOrchestration([OrchestrationTrigger] DurableOrchestrationContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();

            log.LogInformation($"Load Water Allocation Data Orchestration [{runId}]");

            var parallelTasks = new List<Task<StatusHelper>>
            {
                context.CallActivityAsync<StatusHelper>(FunctionNames.LoadOrganizations, runId)
                ,context.CallActivityAsync<StatusHelper>(FunctionNames.LoadSites, runId)
                ,context.CallActivityAsync<StatusHelper>(FunctionNames.LoadWaterSources, runId)
                ,context.CallActivityAsync<StatusHelper>(FunctionNames.LoadMethods, runId)
                ,context.CallActivityAsync<StatusHelper>(FunctionNames.LoadRegulatoryOverlays, runId)
                ,context.CallActivityAsync<StatusHelper>(FunctionNames.LoadReportingUnits, runId)
                ,context.CallActivityAsync<StatusHelper>(FunctionNames.LoadVariables, runId)
            };

            var parallelResults = await Task.WhenAll(parallelTasks);

            foreach (var result in parallelResults)
            {
                log.LogInformation(JsonConvert.SerializeObject(result));
            }

            if (parallelResults.Any(a => !a.Status))
            {
                throw new Exception("Failure Loading Initial Data");
            }

            var results = new List<StatusHelper>
            {
                await context.CallActivityAsync<StatusHelper>(FunctionNames.LoadAggregatedAmounts, runId)
                ,await context.CallActivityAsync<StatusHelper>(FunctionNames.LoadWaterAllocation, runId)
                ,await context.CallActivityAsync<StatusHelper>(FunctionNames.LoadSiteSpecificAmounts, runId)
            };

            foreach (var result in results)
            {
                log.LogInformation(JsonConvert.SerializeObject(result));
            }

            if (results.Any(a => !a.Status))
            {
                throw new Exception("Failure Loading Fact Data");
            }

            return new OkObjectResult(new { status = "success" });
        }

        [FunctionName(FunctionNames.LoadOrganizations)]
        public async Task<StatusHelper> LoadOrganizations([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadOrganizations, Status = await WaterAllocationManager.LoadOrganizations(runId) };
            log.LogInformation(JsonConvert.SerializeObject(result));
            return result;
        }

        [FunctionName(FunctionNames.LoadAggregatedAmounts)]
        public async Task<StatusHelper> LoadAggregatedAmounts([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadAggregatedAmounts, Status = await WaterAllocationManager.LoadAggregatedAmounts(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadRegulatoryOverlays)]
        public async Task<StatusHelper> LoadRegulatoryOverlays([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadRegulatoryOverlays, Status = await WaterAllocationManager.LoadRegulatoryOverlays(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadReportingUnits)]
        public async Task<StatusHelper> LoadReportingUnits([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadReportingUnits, Status = await WaterAllocationManager.LoadReportingUnits(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadSites)]
        public async Task<StatusHelper> LoadSites([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadSites, Status = await WaterAllocationManager.LoadSites(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadSiteSpecificAmounts)]
        public async Task<StatusHelper> LoadSiteSpecificAmounts([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadSiteSpecificAmounts, Status = await WaterAllocationManager.LoadSiteSpecificAmounts(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadVariables)]
        public async Task<StatusHelper> LoadVariables([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadVariables, Status = await WaterAllocationManager.LoadVariables(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadWaterSources)]
        public async Task<StatusHelper> LoadWaterSources([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadWaterSources, Status = await WaterAllocationManager.LoadWaterSources(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadMethods)]
        public async Task<StatusHelper> LoadMethods([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadMethods, Status = await WaterAllocationManager.LoadMethods(runId) };

            log.LogInformation(JsonConvert.SerializeObject(result));

            return result;
        }

        [FunctionName(FunctionNames.LoadWaterAllocation)]
        public async Task<StatusHelper> LoadWaterAllocation([ActivityTrigger] DurableActivityContextBase context, ILogger log)
        {
            var runId = context.GetInput<string>();
            var result = new StatusHelper { Name = FunctionNames.LoadWaterAllocation, Status = await WaterAllocationManager.LoadWaterAllocations(runId) };
            log.LogInformation(JsonConvert.SerializeObject(result));
            return result;
        }

        [FunctionName(FunctionNames.LoadWaterAllocationData)]
        public async Task<object> LoadWaterAllocationData([HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest req, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log)
        {
            string runId = req.Query["runId"].ToString();

            log.LogInformation($"Start Loading Water Allocation Data [{runId}]");

            string instanceId = await starter.StartNewAsync(FunctionNames.LoadWaterAllocationDataOrchestration, runId);
            return new OkObjectResult(new { instanceId });
        }

        [FunctionName(FunctionNames.GetLoadWaterOrchestrationStatus)]
        public async Task<IActionResult> GetLoadWaterOrchestrationStatus([HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest req, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log)
        {
            var instanceId = req.Query["instanceId"];
            var status = await starter.GetStatusAsync(instanceId);
            dynamic resultStatus = new System.Dynamic.ExpandoObject();
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            {
                resultStatus.Status = 1;
            }
            else if (status.RuntimeStatus == OrchestrationRuntimeStatus.ContinuedAsNew || status.RuntimeStatus == OrchestrationRuntimeStatus.Pending || status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                resultStatus.Status = 0;
            }
            else
            {
                resultStatus.Status = 2;
            }
            return new OkObjectResult(resultStatus);
        }

        public class StatusHelper
        {
            public bool Status { get; set; }
            public string Name { get; set; }
        }
    }

    internal static class FunctionNames
    {
        public const string LoadWaterAllocationData = "LoadWaterAllocationData";
        public const string LoadWaterAllocationDataOrchestration = "LoadWaterAllocationDataOrchestration";
        public const string GetLoadWaterOrchestrationStatus = "GetLoadWaterOrchestrationStatus";
        public const string LoadOrganizations = "LoadOrganizations";
        public const string LoadSites = "LoadSites";
        public const string LoadWaterSources = "LoadWaterSources";
        public const string LoadMethods = "LoadMethods";
        public const string LoadWaterAllocation = "LoadWaterAllocation";
        public const string LoadAggregatedAmounts = "LoadAggregatedAmounts";
        public const string LoadRegulatoryOverlays = "LoadRegulatoryOverlays";
        public const string LoadReportingUnits = "LoadReportingUnits";
        public const string LoadSiteSpecificAmounts = "LoadSiteSpecificAmounts";
        public const string LoadVariables = "LoadVariables";
    }
}
