﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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

        [FunctionName("Flatten")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string sourceFileName = req.Query["sourceFileName"];
            string destinationFileName = req.Query["destinationFileName"];
            string keyCol = req.Query["keyCol"];
            string valueCol = req.Query["valueCol"];

            await FlattenManager.Flatten(container, folder, sourceFileName, destinationFileName, keyCol, valueCol);

            return new OkObjectResult(new { status = "success" });
        }
    }
}