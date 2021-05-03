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
    public class ExcelConversion
    {
        public ExcelConversion(ManagerImport.IExcelFileConversionManager excelFileConversionManager)
        {
            ExcelFileConversionManager = excelFileConversionManager;
        }

        private ManagerImport.IExcelFileConversionManager ExcelFileConversionManager { get; set; }

        [FunctionName("ExcelConversionToJson")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var errors = new List<string>();

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string fileName = req.Query["fileName"];

            if (string.IsNullOrEmpty(container))
            {
                errors.Add("container");
            }

            if (string.IsNullOrEmpty(folder))
            {
                errors.Add("folder");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                errors.Add("fileName");
            }

            if (errors.Count > 0)
            {
                return new BadRequestObjectResult($"The following parameter(s) must be specified: {string.Join(", ", errors)}");
            }

            await ExcelFileConversionManager.ConvertExcelFileToJsonFiles(container, folder, fileName);

            return new OkObjectResult(new { status = "success" });
        }
    }
}
