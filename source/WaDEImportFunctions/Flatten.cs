using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;

namespace WaDEImportFunctions
{
    public class Flatten
    {
        public Flatten(ManagerImport.IFlattenManager flattenManager)
        {
            FlattenManager = flattenManager;
        }

        private ManagerImport.IFlattenManager FlattenManager { get; set; }

        [FunctionName("CoordinateProjection")]
        public async Task<IActionResult> RunProjection([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var errors = new List<string>();

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string sourceFileName = req.Query["sourceFileName"];
            string destinationFileName = req.Query["destinationFileName"];
            string xValueCol = req.Query["xValueCol"];
            string yValueCol = req.Query["yValueCol"];

            if (string.IsNullOrEmpty(container))
            {
                errors.Add("container");
            }

            if (string.IsNullOrEmpty(folder))
            {
                errors.Add("folder");
            }

            if (string.IsNullOrEmpty(sourceFileName))
            {
                errors.Add("sourceFileName");
            }

            if (string.IsNullOrEmpty(destinationFileName))
            {
                errors.Add("destinationFileName");
            }

            if (string.IsNullOrEmpty(xValueCol))
            {
                errors.Add("xValueCol");
            }

            if (string.IsNullOrEmpty(yValueCol))
            {
                errors.Add("yValueCol");
            }

            if (errors.Count > 0)
            {
                return new BadRequestObjectResult($"The following parameter(s) must be specified: {string.Join(", ", errors)}");
            }

            await FlattenManager.CoordinateProjection(container, folder, sourceFileName, destinationFileName, xValueCol, yValueCol);

            return new OkObjectResult(new { status = "success" });
        }

        [FunctionName("Flatten")]
        public async Task<IActionResult> RunFlatten([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var errors = new List<string>();

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string sourceFileName = req.Query["sourceFileName"];
            string destinationFileName = req.Query["destinationFileName"];
            string keyCol = req.Query["keyCol"];
            string valueCol = req.Query["valueCol"];

            if (string.IsNullOrEmpty(container))
            {
                errors.Add("container");
            }

            if (string.IsNullOrEmpty(folder))
            {
                errors.Add("folder");
            }

            if (string.IsNullOrEmpty(sourceFileName))
            {
                errors.Add("sourceFileName");
            }

            if (string.IsNullOrEmpty(destinationFileName))
            {
                errors.Add("destinationFileName");
            }

            if (string.IsNullOrEmpty(keyCol))
            {
                errors.Add("keyCol");
            }

            if (string.IsNullOrEmpty(valueCol))
            {
                errors.Add("valueCol");
            }

            if (errors.Count > 0)
            {
                return new BadRequestObjectResult($"The following parameter(s) must be specified: {string.Join(", ", errors)}");
            }

            await FlattenManager.Flatten(container, folder, sourceFileName, destinationFileName, keyCol, valueCol);

            return new OkObjectResult(new { status = "success" });
        }
    }
}
