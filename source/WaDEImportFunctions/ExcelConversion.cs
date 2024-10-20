using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string fileName = req.Query["fileName"];

            await ExcelFileConversionManager.ConvertExcelFileToJsonFiles(container, folder, fileName);

            return new OkObjectResult(new { status = "success" });
        }
    }
}
