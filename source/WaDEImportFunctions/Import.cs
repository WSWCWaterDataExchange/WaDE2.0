using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common;
using WesternStatesWater.WaDE.Contracts.Import;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Newtonsoft.Json.Serialization;

namespace WaDEImportFunctions
{
    public class Import
    {
        public Import(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }

        [Function(FunctionNames.LoadWaterAllocationDataOrchestration)]
        public async Task<string> LoadWaterAllocationDataOrchestration([OrchestrationTrigger] TaskOrchestrationContext context, ILogger log)
        {
            var runId = context.GetInput<string>();

            var parallelTasks = new List<Task<StatusHelper>>
            {
                context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadOrganizations, runId),
                context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadWaterSources, runId),
                context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadMethods, runId),
                context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadRegulatoryOverlays, runId),
                context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadReportingUnits, runId),
                context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadVariables, runId)
            };

            var parallelResults = await Task.WhenAll(parallelTasks);

            foreach (var result in parallelResults)
            {
                log.LogInformation(JsonConvert.SerializeObject(result));
            }

            if (parallelResults.Any(a => !a.Status))
            {
                throw new WaDEException("Failure Loading Initial Data");
            }

            //sites have to be run after WaterSources and RegulatoryOverlays
            var sitesResult = await context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadSites, runId);
            log.LogInformation(JsonConvert.SerializeObject(sitesResult));
            if (!sitesResult.Status)
            {
                throw new WaDEException("Failure Loading Sites Data");
            }

            var results = new List<StatusHelper>
            {
                await context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadAggregatedAmounts, runId),
                await context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadWaterAllocations, runId),
                await context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadSiteSpecificAmounts, runId),
                await context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadRegulatoryReportingUnits, runId),
                await context.CallSubOrchestratorAsync<StatusHelper>(FunctionNames.LoadPODToPOUSiteRelationships, runId)
            };

            foreach (var result in results)
            {
                log.LogInformation(JsonConvert.SerializeObject(result));
            }

            if (results.Any(a => !a.Status))
            {
                throw new WaDEException("Failure Loading Fact Data");
            }
            
            return JsonConvert.SerializeObject(new { status = "success" });
        }

        internal static async Task<StatusHelper> LoadData(TaskOrchestrationContext context, string currFunctionName, string countFunctionName, string batchFunctionName, int recordCount, ILogger log)
        {
            var runId = context.GetInput<string>();
            var lineCount = await context.CallActivityAsync<long>(countFunctionName, runId);
            var processed = 0;
            while (processed < lineCount)
            {
                var status = await context.CallActivityAsync<StatusHelper>(batchFunctionName, new BatchData { RunId = runId, StartIndex = processed, Count = recordCount });
                if (!status.Status)
                {
                    return new StatusHelper { Name = currFunctionName, Status = false };
                }

                processed += recordCount;
            }
           
            return new StatusHelper { Name = currFunctionName, Status = true };
        }

        internal static async Task<StatusHelper> LoadBatch(BatchData batchData, string batchFunctionName, Func<string, int, int, Task<bool>> loadFunction, ILogger log)
        {
            var result = new StatusHelper { Name = batchFunctionName, Status = await loadFunction(batchData.RunId, batchData.StartIndex, batchData.Count) };
            log.LogInformation($"{batchFunctionName} [{batchData.StartIndex}] - {JsonConvert.SerializeObject(result)}");
            return result;
        }

        internal static async Task<int> GetCount([ActivityTrigger] string runId, string countFunctionName, Func<string, Task<int>> countFunction, ILogger log)
        {
            var count = await countFunction(runId);
            log.LogInformation($"{countFunctionName} - {count}");
            return count;
        }

        [Function(FunctionNames.LoadWaterAllocationData)]
        public async Task<object> LoadWaterAllocationData([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, [DurableClient] DurableTaskClient starter, ILogger log)
        {
            string runId = req.Query["runId"].ToString();

            log.LogInformation($"Start Loading Water Allocation Data [{runId}]");

            string instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(FunctionNames.LoadWaterAllocationDataOrchestration, runId);
            return new OkObjectResult(new { instanceId });
        }

        [Function(FunctionNames.GetLoadWaterOrchestrationStatus)]
        public async Task<HttpResponseData> GetLoadWaterOrchestrationStatus([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, [DurableClient] DurableTaskClient starter, ILogger log)
        {
            var instanceId = req.Query["instanceId"];

            var status = await starter.GetInstanceAsync(instanceId);
            object resultStatus = null;
            
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            {
                resultStatus = new { Status = 1 };
            }
            else if (status.RuntimeStatus == OrchestrationRuntimeStatus.ContinuedAsNew || status.RuntimeStatus == OrchestrationRuntimeStatus.Pending || status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                resultStatus = new { Status = 0 };
            }
            else
            {
                resultStatus = new { Status = 2 };
            }
            
            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var json = JsonConvert.SerializeObject(resultStatus, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
            await jsonResult.WriteStringAsync(json);
            return jsonResult;
        }
    }

    public class StatusHelper
    {
        public bool Status { get; set; }
        public string Name { get; set; }
    }

    public class BatchData
    {
        public string RunId { get; set; }
        public int StartIndex { get; set; }
        public int Count { get; set; }
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
        public const string LoadWaterAllocations = "LoadWaterAllocations";
        public const string LoadAggregatedAmounts = "LoadAggregatedAmounts";
        public const string LoadRegulatoryOverlays = "LoadRegulatoryOverlays";
        public const string LoadRegulatoryReportingUnits = "LoadRegulatoryReportingUnits";
        public const string LoadReportingUnits = "LoadReportingUnits";
        public const string LoadSiteSpecificAmounts = "LoadSiteSpecificAmounts";
        public const string LoadVariables = "LoadVariables";
        public const string LoadPODToPOUSiteRelationships = "PODToPODSiteRelationships";
    }
}
